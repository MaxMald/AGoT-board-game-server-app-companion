using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Rules;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  public class GameStateService
  {
    public RoomState CreateRoom(string roomId, ushort maxPlayers)
    {
      return new RoomState
      {
        RoomId = roomId,
        MaxPlayers = (byte)Math.Max(GameRules.MinPlayers, Math.Min(maxPlayers, GameRules.MaxPlayers)),
        Players = new Dictionary<string, PlayerState>(),
        Round = new RoundState(),
        Wilding = new WildingState(),
        Influence = new InfluenceState(),
        IsGameStarted = false,
        IsGameFinished = false,
        Winner = null
      };
    }

    public Result StartGame(RoomState room)
    {
      Result result = GameStateRules.CanStartGame(room);
      if (!result.Success)
        return result;

      room.IsGameStarted = true;
      room.IsGameFinished = false;
      room.Winner = null;
      room.Round.RoundNumber = 1;
      room.Round.CurrentPhase = GamePhaseType.Planning;
      room.Wilding.Strength = GameRules.WildingStartingStrength;

      // Vassals Initialization
      for (byte i = 0; i < (byte)HouseType.Count; ++i)
      {
        HouseType houseType = (HouseType)i;
        if (houseType == HouseType.Undefined || houseType == HouseType.Targaryen)
          continue; // Skip undefined type. Targaryen cannot be a vassal house

        if (room.Players.Values.Any(p => p.HouseState.Type == houseType))
          continue; // Skip if the house is already taken by a player

        if (!room.Vassals.ContainsKey(houseType))
        {
          Result vassalResult = AddVassalHouse(room, houseType);
          if (!vassalResult.Success)
            return vassalResult;
        }
      }

      List<HouseType> playerHouses = room.Players.Values.Select(p => p.HouseState.Type).ToList();
      List<HouseType> vassalHouses = room.Vassals.Keys.ToList();

      InfluenceTracksInitializer.Initialize(room.Influence, playerHouses, vassalHouses);

      return Result.SUCCESS();
    }

    public Result AddNewPlayer(RoomState room, string playerId, string playerName)
    {
      Result result = GameStateRules.CanAddNewPlayer(room, playerId, playerName);
      if (!result.Success)
        return result;

      PlayerState playerState = new()
      {
        PlayerId = playerId,
        PlayerName = playerName,
        HouseState = HouseStateFactory.CreateUndefined()
      };

      room.Players.Add(playerId, playerState);

      return Result.SUCCESS();
    }

    public Result RemovePlayer(RoomState room, string playerId)
    {
      Result result = GameStateRules.CanRemovePlayer(room, playerId);
      if (!result.Success)
        return result;

      room.Players.Remove(playerId);
      return Result.SUCCESS();
    }

    public Result ModifyPlayerHouse(RoomState room, string playerId, HouseType newHouse)
    {
      Result result = GameStateRules.CanModifyPlayerHouse(room, playerId, newHouse);
      if (!result.Success)
        return result;

      PlayerState player = room.Players[playerId];
      switch (newHouse)
      {
        case HouseType.Undefined:
          player.HouseState = HouseStateFactory.CreateUndefined();
          break;
        case HouseType.Stark:
          player.HouseState = HouseStateFactory.CreateStark();
          break;
        case HouseType.Greyjoy:
          player.HouseState = HouseStateFactory.CreateGreyjoy();
          break;
        case HouseType.Lannister:
          player.HouseState = HouseStateFactory.CreateLannister();
          break;
        case HouseType.Martell:
          player.HouseState = HouseStateFactory.CreateMartell();
          break;
        case HouseType.Tyrell:
          player.HouseState = HouseStateFactory.CreateTyrell();
          break;
        case HouseType.Baratheon:
          player.HouseState = HouseStateFactory.CreateBaratheon();
          break;
        case HouseType.Arryn:
          player.HouseState = HouseStateFactory.CreateArryn();
          break;
        case HouseType.Targaryen:
          player.HouseState = HouseStateFactory.CreateTargaryen();
          break;
        default:
          return Result.FAILURE($"Not implemented factory method for house: {newHouse}");
      }

      return Result.SUCCESS();
    }

    public Result AddVassalHouse(RoomState room, HouseType vassalHouse)
    {
      Result result = GameStateRules.CanAddVassalHouse(room, vassalHouse);
      if (!result.Success)
        return result;

      VassalState vassalState;
      switch (vassalHouse)
      {
        case HouseType.Stark:
          vassalState = VassalStateFactory.CreateStark();
          break;
        case HouseType.Greyjoy:
          vassalState = VassalStateFactory.CreateGreyjoy();
          break;
        case HouseType.Lannister:
          vassalState = VassalStateFactory.CreateLannister();
          break;
        case HouseType.Martell:
          vassalState = VassalStateFactory.CreateMartell();
          break;
        case HouseType.Tyrell:
          vassalState = VassalStateFactory.CreateTyrell();
          break;
        case HouseType.Baratheon:
          vassalState = VassalStateFactory.CreateBaratheon();
          break;
        case HouseType.Arryn:
          vassalState = VassalStateFactory.CreateArryn();
          break;
        default:
          return Result.FAILURE($"Invalid vassal house: {vassalHouse}");
      }

      room.Vassals.Add(vassalHouse, vassalState);
      return Result.SUCCESS();
    }

    public Result ModifyPlayerPowerTokens(RoomState room, string playerId, short delta)
    {
      if (!room.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = room.Players[playerId];
      if (player.HouseState.IsDefeated)
        return Result.FAILURE($"Player with ID {playerId} is defeated and cannot modify power tokens.");

      short newPowerTokens = Math.Clamp((short)(player.HouseState.PowerTokens + delta), (short)0, (short)255);
      player.HouseState.PowerTokens = (byte)newPowerTokens;

      return Result.SUCCESS();
    }

    public Result UpdatePlayerPowerTokens(RoomState room, string playerId, byte newPowerTokens)
    {
      if (!room.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = room.Players[playerId];
      if (player.HouseState.IsDefeated)
        return Result.FAILURE($"Player with ID {playerId} is defeated and cannot modify power tokens.");

      player.HouseState.PowerTokens = newPowerTokens;
      return Result.SUCCESS();
    }

    public Result UpdatePlayerSupplyLevel(RoomState room, string playerId, byte newSupplyLevel)
    {
      if (!room.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = room.Players[playerId];
      if (player.HouseState.IsDefeated)
        return Result.FAILURE($"Player with ID {playerId} is defeated and cannot modify supply level.");

      player.HouseState.SupplyLevel = newSupplyLevel;

      return Result.SUCCESS();
    }

    public Result UpdatePlayerVictoryPoint(RoomState room, string playerId, byte newVictoryPoints)
    {
      if (!room.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = room.Players[playerId];
      if (player.HouseState.IsDefeated)
        return Result.FAILURE($"Player with ID {playerId} is defeated and cannot modify victory points.");

      player.HouseState.VictoryPoints = newVictoryPoints;

      CheckWinCondition(room);

      return Result.SUCCESS();
    }

    public Result TransferPowerTokens(
      RoomState room,
      string fromPlayerId,
      string toPlayerId,
      byte amount
      )
    {
      if (!room.Players.ContainsKey(fromPlayerId))
        return Result.FAILURE($"Player with ID {fromPlayerId} does not exist in the room.");

      if (!room.Players.ContainsKey(toPlayerId))
        return Result.FAILURE($"Player with ID {toPlayerId} does not exist in the room.");

      PlayerState fromPlayer = room.Players[fromPlayerId];
      PlayerState toPlayer = room.Players[toPlayerId];

      if (fromPlayer.HouseState.IsDefeated)
        return Result.FAILURE($"Player with ID {fromPlayerId} is defeated and cannot transfer power tokens.");

      if (toPlayer.HouseState.IsDefeated)
        return Result.FAILURE($"Player with ID {toPlayerId} is defeated and cannot receive power tokens.");

      if (fromPlayer.HouseState.PowerTokens < amount)
        return Result.FAILURE($"Player with ID {fromPlayerId} does not have enough power tokens to transfer.");

      fromPlayer.HouseState.PowerTokens -= amount;
      toPlayer.HouseState.PowerTokens += amount;

      return Result.SUCCESS();
    }

    public void CheckWinCondition(RoomState room)
    {
      foreach (var player in room.Players)
      {
        if (player.Value.HouseState.VictoryPoints >= GameRules.NumVictoryPointsToWin)
        {
          room.Winner = player.Value.HouseState.Type;
          room.IsGameFinished = true;
          return;
        }
      }
    }
  }
}
