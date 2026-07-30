using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  public class RoomStateService
  {
    public RoomState CreateRoom(
      string roomId,
      string hosterPlayerId,
      string hosterPlayerName
      )
    {
      if (string.IsNullOrEmpty(hosterPlayerId) || string.IsNullOrWhiteSpace(hosterPlayerId))
        throw new ArgumentException("Hoster player ID cannot be null or empty.", nameof(hosterPlayerId));

      if (string.IsNullOrEmpty(roomId) || string.IsNullOrWhiteSpace(roomId))
        throw new ArgumentException("Room ID cannot be null or empty.", nameof(roomId));

      RoomState room = new()
      {
        RoomId = roomId,
        HosterPlayerId = hosterPlayerId,
        MaxPlayers = GameConstants.MaxPlayers,
        RoomStatus = RoomStatus.PreparingGame
      };

      room.PlayersDescriptors.Add(
        new PlayerDescriptor()
        {
          Name = hosterPlayerName,
          PlayerId = hosterPlayerId,
          HouseType = HouseType.Undefined
        }
      );

      return room;
    }

    public Result AddNewPlayerDescriptor(RoomState room, string playerId, string playerName)
    {
      if (room.RoomStatus != RoomStatus.PreparingGame)
        return Result.FAILURE("Cannot add new players after the game has started.");

      if (room.PlayersDescriptors.Count >= room.MaxPlayers)
        return Result.FAILURE($"Cannot add new player. Maximum number of players ({room.MaxPlayers}) reached.");

      if (room.PlayersDescriptors.Any(pd => pd.PlayerId == playerId))
        return Result.FAILURE($"Player ID '{playerId}' is already in use.");

      playerName = playerName.Trim();
      Result result = Helpers.IsValidPlayerName(playerName);
      if (!result.Success)
        return result;

      PlayerDescriptor playerDescriptor = new()
      {
        PlayerId = playerId,
        Name = playerName,
        HouseType = HouseType.Undefined
      };

      room.PlayersDescriptors.Add(playerDescriptor);
      return Result.SUCCESS();
    }

    public Result RemovePlayerDescriptor(RoomState room, string playerId)
    {
      if (room.RoomStatus != RoomStatus.PreparingGame)
        return Result.FAILURE("Cannot remove players after the game has started.");

      if (room.HosterPlayerId == playerId)
        return Result.FAILURE("Cannot remove the hoster player from the room.");

      if (!room.PlayersDescriptors.Any(pd => pd.PlayerId == playerId))
        return Result.FAILURE($"Player ID '{playerId}' does not exist.");

      room.PlayersDescriptors.RemoveAll(pd => pd.PlayerId == playerId);

      return Result.SUCCESS();
    }

    public Result ModifyPlayersDecriptorHouse(RoomState room, string playerId, HouseType newHouse)
    {
      if (room.RoomStatus != RoomStatus.PreparingGame)
        return Result.FAILURE("Cannot modify player house after the game has started.");

      if (!room.PlayersDescriptors.Any(pd => pd.PlayerId == playerId))
        return Result.FAILURE($"Player ID '{playerId}' does not exist.");

      if (newHouse != HouseType.Undefined) // if player is selecting a valid house, check if it's already taken by another player
      {
        if (room.PlayersDescriptors.Any(pd => pd.HouseType == newHouse))
          return Result.FAILURE($"House '{newHouse}' is already selected by another player.");
      }

      PlayerDescriptor playerDescriptor = room.PlayersDescriptors.First(pd => pd.PlayerId == playerId);
      playerDescriptor.HouseType = newHouse;

      return Result.SUCCESS();
    }

    public Result ModifyMaxNumberOfPlayers(RoomState room, byte newMaxPlayers)
    {
      if (room.RoomStatus != RoomStatus.PreparingGame)
        return Result.FAILURE("Cannot modify max number of players after the game has started.");

      if (newMaxPlayers < GameConstants.MinPlayers || newMaxPlayers > GameConstants.MaxPlayers)
        return Result.FAILURE($"Max number of players must be between {GameConstants.MinPlayers} and {GameConstants.MaxPlayers}.");

      room.MaxPlayers = newMaxPlayers;
      return Result.SUCCESS();
    }

    public Result StartGame(RoomState room)
    {
      if (room.RoomStatus != RoomStatus.PreparingGame)
        return Result.FAILURE("Game has already started.");

      if (room.PlayersDescriptors.Count < GameConstants.MinPlayers)
        return Result.FAILURE($"Not enough players to start the game. Minimum required is {GameConstants.MinPlayers}.");

      if (room.PlayersDescriptors.Count > room.MaxPlayers)
        return Result.FAILURE($"Too many players to start the game. Current Number of players: {room.PlayersDescriptors.Count}. Maximum allowed is {room.MaxPlayers}.");

      foreach (PlayerDescriptor playerDescriptor in room.PlayersDescriptors)
      {
        if (playerDescriptor.HouseType == HouseType.Undefined)
          return Result.FAILURE($"Player '{playerDescriptor.Name}' has not selected a house.");
      }

      try
      { 
        room.Winner = null;
        room.Round.RoundNumber = 1;
        room.Wilding.Strength = GameConstants.WildingStartingStrength;

        AssertPlayerDescriptorsAreValidForCreation(room.PlayersDescriptors);
        CreatePlayerHouses(room);
        CreateVassalHouses(room);

        List<HouseState> allHouses = GetAllHouses(room);
        InfluenceTracksService.Initialize(allHouses);

        room.RoomStatus = RoomStatus.InProgress;
        room.Round.CurrentPhase = RoundPhaseType.Setup;
      }
      catch (Exception ex)
      {
        room.RoomStatus = RoomStatus.PreparingGame;
        room.Players.Clear();
        room.Vassals.Clear();

        return Result.FAILURE($"An error occurred while starting the game: {ex.Message}");
      }

      return Result.SUCCESS();
    }

    public Result MoveToRoundPhase(RoomState room, RoundPhaseType newPhase)
    {
      // TODO Round Transitions

      room.Round.CurrentPhase = newPhase;
      return Result.SUCCESS();
    }

    public Result ModifyPlayerPowerTokens(RoomState room, string playerId, short delta)
    {
      if (room.RoomStatus != RoomStatus.InProgress)
        return Result.FAILURE("Cannot modify power tokens if game is not in progress.");

      if (!room.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      HouseState house = room.Players[playerId].HouseState;
      short power = house.PowerTokens;
      short newPower = (short)Math.Max(0, power + delta);
      byte newPowerByte = (byte)Math.Min(newPower, byte.MaxValue);

      return HouseStateService.UpdatePowerTokens(house, newPowerByte);
    }

    public Result UpdatePlayerPowerTokens(RoomState room, string playerId, byte newPowerTokens)
    {
      if (room.RoomStatus != RoomStatus.InProgress)
        return Result.FAILURE("Cannot update power tokens if game is not in progress.");

      if (!room.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = room.Players[playerId];
      return HouseStateService.UpdatePowerTokens(player.HouseState, newPowerTokens);
    }

    public Result UpdatePlayerSupplyLevel(RoomState room, string playerId, byte newSupplyLevel)
    {
      if (room.RoomStatus != RoomStatus.InProgress)
        return Result.FAILURE("Cannot update supply level if game is not in progress.");

      if (!room.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      HouseState house = room.Players[playerId].HouseState;
      HouseStateService.UpdateHouseSupplyLevel(house, newSupplyLevel);

      return Result.SUCCESS();
    }

    public Result UpdatePlayerVictoryPoints(RoomState room, string playerId, byte newVictoryPoints)
    {
      if (room.RoomStatus != RoomStatus.InProgress)
        return Result.FAILURE("Cannot update victory points if game is not in progress.");

      if (!room.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = room.Players[playerId];
      player.HouseState.VictoryPoints = newVictoryPoints;

      CheckWinCondition(room);
      return Result.SUCCESS();
    }

    public Result UpdatePlayerDragonStrength(RoomState room, string playerId, byte newDragonStrength)
    {
      if (room.RoomStatus != RoomStatus.InProgress)
        return Result.FAILURE("Cannot update dragon strength if game is not in progress.");

      if (!room.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = room.Players[playerId];
      return HouseStateService.UpdateDragonStrength(player.HouseState, newDragonStrength);
    }

    public Result UpdatePlayerPowerTokensBid(RoomState room, string playerId, byte newBid)
    {
      if (room.RoomStatus != RoomStatus.InProgress)
        return Result.FAILURE("Cannot update bid if game is not in progress.");

      if (room.Round.CurrentPhase != RoundPhaseType.KingsCourtBidding &&
        room.Round.CurrentPhase != RoundPhaseType.FiefdomsBidding &&
        room.Round.CurrentPhase != RoundPhaseType.IronThroneBidding &&
        room.Round.CurrentPhase != RoundPhaseType.WildlingsBidding)
      {
        return Result.FAILURE("Cannot update bid if the current phase is not a bidding phase.");
      }

      if (!room.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = room.Players[playerId];

      return HouseStateService.UpdatePowerTokensBid(player.HouseState, newBid);
    }

    public Result TransferPowerTokens(
      RoomState room,
      string fromPlayerId,
      string toPlayerId,
      byte amount
    )
    {
      if (room.RoomStatus != RoomStatus.InProgress)
        return Result.FAILURE("Cannot transfer power tokens if game is not in progress.");

      if (!room.Players.ContainsKey(fromPlayerId))
        return Result.FAILURE($"Player with ID {fromPlayerId} does not exist in the room.");

      if (!room.Players.ContainsKey(toPlayerId))
        return Result.FAILURE($"Player with ID {toPlayerId} does not exist in the room.");

      PlayerState fromPlayer = room.Players[fromPlayerId];
      PlayerState toPlayer = room.Players[toPlayerId];

      return HouseStateService.TransferPowerTokens(
        fromPlayer.HouseState,
        toPlayer.HouseState,
        amount
      );
    }

    public Result MakeVassalageStatus(RoomState room, string commanderPlayerId, HouseType vassalHouseType)
    {
      if (room.RoomStatus != RoomStatus.InProgress)
        return Result.FAILURE("Cannot make a player a vassal if the game is not in progress.");

      if (!room.Players.ContainsKey(commanderPlayerId))
        return Result.FAILURE($"Commander player with ID {commanderPlayerId} does not exist in the room.");

      if (!room.Vassals.ContainsKey(vassalHouseType))
        return Result.FAILURE($"Vassal of type: {vassalHouseType} does not exist in the room.");

      PlayerState commanderPlayer = room.Players[commanderPlayerId];
      HouseState vassalHouse = room.Vassals[vassalHouseType];

      return HouseStateService.MakeVassalageStatus(commanderPlayer.HouseState, vassalHouse);
    }

    public Result BreakVassalageStatus(RoomState room, string commanderPlayerId, HouseType vassalHouseType)
    {
      if (room.RoomStatus != RoomStatus.InProgress)
        return Result.FAILURE("Cannot break a vassalage status if the game is not in progress.");

      if (!room.Players.ContainsKey(commanderPlayerId))
        return Result.FAILURE($"Commander player with ID {commanderPlayerId} does not exist in the room.");

      if (!room.Vassals.ContainsKey(vassalHouseType))
        return Result.FAILURE($"Vassal of type: {vassalHouseType} does not exist in the room.");

      PlayerState commanderPlayer = room.Players[commanderPlayerId];
      HouseState vassalHouse = room.Vassals[vassalHouseType];

      return HouseStateService.BreakVassalageStatus(commanderPlayer.HouseState, vassalHouse);
    }

    public Result ModifyVassalSupplyLevel(
      RoomState room,
      HouseType vassalHouseType,
      byte newSupplyLevel
      )
    {
      if (room.RoomStatus != RoomStatus.InProgress)
        return Result.FAILURE("Cannot modify a vassal's supply level if the game is not in progress.");

      if (!room.Vassals.ContainsKey(vassalHouseType))
        return Result.FAILURE($"Vassal of type: {vassalHouseType} does not exist in the room.");

      HouseState vassalHouse = room.Vassals[vassalHouseType];
      HouseStateService.UpdateHouseSupplyLevel(vassalHouse, newSupplyLevel);

      return Result.SUCCESS();
    }

    public Result DefeatPlayer(RoomState room, string playerId)
    {
      if (room.RoomStatus != RoomStatus.InProgress)
        return Result.FAILURE("Cannot defeat a player before the game has started.");

      // TODO: Consider check the game round is on a safe phase to defeat a player, like
      // after the planning phase and action phase.

      if (!room.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = room.Players[playerId];
      if (player.HouseState.IsDefeated)
        return Result.FAILURE($"Player with ID {playerId} is already defeated.");

      List<HouseType> vassalsToBreak = player.HouseState.VassalHouseTypes.ToList();
      foreach (HouseType vassalHouseType in vassalsToBreak)
      {
        Result breakResult = BreakVassalageStatus(room, playerId, vassalHouseType);
        if (!breakResult.Success)
          return Result.FAILURE($"Failed to break vassalage status for vassal {vassalHouseType}: {breakResult.Message}");
      }

      return HouseStateService.SetHouseAsDefeated(player.HouseState);
    }

    public void CheckWinCondition(RoomState room)
    {
      foreach (var player in room.Players)
      {
        if (player.Value.HouseState.VictoryPoints >= GameConstants.NumVictoryPointsToWin
          && !player.Value.HouseState.IsDefeated)
        {
          room.Winner = player.Value.HouseState.Type;
          room.RoomStatus = RoomStatus.Finished;
          return;
        }
      }
    }

    private static void AssertPlayerDescriptorsAreValidForCreation(List<PlayerDescriptor> playerDescriptors)
    {
      var houseTypes = new HashSet<HouseType>();

      foreach (var playerDescriptor in playerDescriptors)
      {
        if (playerDescriptor.HouseType == HouseType.Undefined)
          throw new InvalidOperationException($"Player '{playerDescriptor.Name}' has not selected a house.");

        if (!houseTypes.Add(playerDescriptor.HouseType))
          throw new InvalidOperationException($"House '{playerDescriptor.HouseType}' is already selected by another player.");
      }
    }

    private static void CreatePlayerHouses(RoomState room)
    {
      foreach (var playerDescriptor in room.PlayersDescriptors)
      {
        if (playerDescriptor.HouseType == HouseType.Undefined)
          throw new InvalidOperationException($"Player '{playerDescriptor.Name}' has not selected a house.");

        if (room.Players.ContainsKey(playerDescriptor.PlayerId))
          throw new InvalidOperationException($"Player ID '{playerDescriptor.PlayerId}' is already in use.");

        HouseState houseState = HouseStateService.Create(playerDescriptor.HouseType);
        PlayerState playerState = new PlayerState()
        {
          PlayerId = playerDescriptor.PlayerId,
          PlayerName = playerDescriptor.Name,
          HouseState = houseState
        };

        room.Players[playerDescriptor.PlayerId] = playerState;
      }
    }

    private static void CreateVassalHouses(RoomState room)
    {
      for (byte i = 0; i < (byte)HouseType.Count; ++i)
      {
        HouseType houseType = (HouseType)i;
        if (houseType == HouseType.Undefined || houseType == HouseType.Targaryen)
          continue; // Skip undefined type. Targaryen cannot be a vassal house

        if (room.Players.Values.Any(p => p.HouseState.Type == houseType))
          continue; // Skip if the house is already taken by a player

        if (room.Vassals.ContainsKey(houseType))
          continue; // Skip if the house is already added as a vassal

        room.Vassals[houseType] = HouseStateService.CreateVassal(houseType);
      }
    }

    private static List<HouseState> GetAllHouses(RoomState room)
    {
      List<HouseState> allHouses = new List<HouseState>();
      foreach (var player in room.Players.Values)
        allHouses.Add(player.HouseState);

      foreach (var vassal in room.Vassals.Values)
        allHouses.Add(vassal);

      return allHouses;
    }
  }
}
