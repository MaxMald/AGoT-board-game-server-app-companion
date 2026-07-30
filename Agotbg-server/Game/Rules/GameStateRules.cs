using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Rules
{
  public static class GameStateRules
  {
    public static readonly ushort MaxPlayerNameLength = 20;

    public static Result CanStartGame(RoomState room)
    {
      if (room.IsGameStarted)
        return Result.FAILURE("Game has already started.");

      if (room.Players.Count < GameRules.MinPlayers)
        return Result.FAILURE($"Not enough players to start the game. Minimum required is {GameRules.MinPlayers}.");

      if (room.Players.Count > room.MaxPlayers)
        return Result.FAILURE($"Too many players to start the game. Current Number of players: {room.Players.Count}. Maximum allowed is {room.MaxPlayers}.");

      Result result = HasValidHousesForPlayers(room);
      if (!result.Success)
        return result;

      result = HasValidPlayerNames(room);
      if (!result.Success)
        return result;

      return Result.SUCCESS();
    }

    public static Result CanAddNewPlayer(RoomState room, string playerId, string playerName)
    {
      if (room.IsGameStarted)
        return Result.FAILURE("Cannot add new players after the game has started.");

      if (room.Players.Count >= room.MaxPlayers)
        return Result.FAILURE($"Cannot add new player. Maximum number of players ({room.MaxPlayers}) reached.");

      Result result = IsValidPlayerName(playerName);
      if (!result.Success)
        return result;

      if (room.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player ID '{playerId}' is already in use.");

      return Result.SUCCESS();
    }

    public static Result CanRemovePlayer(RoomState room, string playerId)
    {
      if (room.IsGameStarted)
        return Result.FAILURE("Cannot remove players after the game has started.");

      if (!room.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player ID '{playerId}' does not exist.");

      return Result.SUCCESS();
    }

    public static Result CanModifyPlayerHouse(RoomState room, string playerId, HouseType newHouse)
    {
      if (room.IsGameStarted)
        return Result.FAILURE("Cannot modify player house after the game has started.");

      if (!room.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player ID '{playerId}' does not exist.");

      if (newHouse == HouseType.Undefined)
        return Result.SUCCESS(); // Allow players to deselect their house by setting it to Undefined.

      if (room.Players.Values.Any(p => p.HouseState.Type == newHouse))
        return Result.FAILURE($"House '{newHouse}' is already selected by another player.");

      if (room.Vassals.Values.Any(v => v.House == newHouse))
        return Result.FAILURE($"House '{newHouse}' is already selected by a vassal.");

      return Result.SUCCESS();
    }

    public static Result CanAddVassalHouse(RoomState room, HouseType vassalHouse)
    {
      if (room.Vassals.Values.Any(v => v.House == vassalHouse))
        return Result.FAILURE($"Vassal house '{vassalHouse}' is already added.");

      if (vassalHouse == HouseType.Targaryen)
        return Result.FAILURE("Targaryen house cannot be selected for vassals.");

      if (room.Players.Values.Any(p => p.HouseState.Type == vassalHouse))
        return Result.FAILURE($"House '{vassalHouse}' is already selected by a player.");

      return Result.SUCCESS();
    }

    public static Result CanRemoveVassalHouse(RoomState room, HouseType vassalHouse)
    {
      if (room.IsGameStarted)
        return Result.FAILURE("Cannot remove vassal houses after the game has started.");

      if (!room.Vassals.Values.Any(v => v.House == vassalHouse))
        return Result.FAILURE($"Vassal house '{vassalHouse}' does not exist.");

      return Result.SUCCESS();
    }

    public static Result CanAdvancePhase(RoomState room, GamePhaseType targetPhase)
    {
      if (!room.IsGameStarted)
        return Result.FAILURE("Game has not started yet.");

      if (room.IsGameFinished)
        return Result.FAILURE("Game has already finished.");

      bool isValidTransition = room.Round.CurrentPhase switch
      {
        GamePhaseType.Setup => targetPhase == GamePhaseType.Westeros,
        GamePhaseType.Westeros => targetPhase == GamePhaseType.Planning,
        GamePhaseType.Planning => targetPhase == GamePhaseType.Action,
        GamePhaseType.Action => targetPhase == GamePhaseType.EndOfRound,
        GamePhaseType.EndOfRound => targetPhase == GamePhaseType.Westeros,
        _ => false
      };

      if (!isValidTransition)
        return Result.FAILURE($"Invalid phase transition from {room.Round.CurrentPhase} to {targetPhase}.");

      return Result.SUCCESS();
    }

    private static Result HasValidPlayerNames(RoomState room)
    {
      foreach (var player in room.Players.Values)
      {
        Result result = IsValidPlayerName(player.PlayerName);
        if (!result.Success)
          return result;
      }

      return Result.SUCCESS();
    }

    private static Result IsValidPlayerName(String playerName)
    {
      if (string.IsNullOrWhiteSpace(playerName))
        return Result.FAILURE("Player name cannot be empty or whitespace.");

      if (playerName.Length > MaxPlayerNameLength)
        return Result.FAILURE($"Player name cannot exceed {GameStateRules.MaxPlayerNameLength} characters.");

      if (!playerName.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
        return Result.FAILURE("Player name can only contain letters, numbers, and spaces.");

      return Result.SUCCESS();
    }

    private static Result HasValidHousesForPlayers(RoomState room)
    {
      List<HouseType> selectedHouses = room.Players.Values.Select(p => p.HouseState.Type).ToList();
      if (selectedHouses.Count != selectedHouses.Distinct().Count())
        return Result.FAILURE("Players must select different houses.");

      if (room.Players.Values.Any(p => p.HouseState.Type == HouseType.Undefined))
        return Result.FAILURE("All players must select a house before starting the game.");

      return Result.SUCCESS();
    }

    private static Result HasValidHousesForVassals(RoomState room)
    {
      List<HouseType> selectedHousesAsVassals = room.Vassals.Values.Select(p => p.House).ToList();
      if (selectedHousesAsVassals.Count != selectedHousesAsVassals.Distinct().Count())
        return Result.FAILURE("Vassal houses must be different.");

      if (room.Vassals.Values.Any(p => p.House == HouseType.Undefined))
        return Result.FAILURE("All vassal houses must be defined before starting the game.");

      if (room.Vassals.Values.Any(p => p.House == HouseType.Targaryen))
        return Result.FAILURE("Targaryen house cannot be selected for vassals.");

      return Result.SUCCESS();
    }

    private static Result HasUniqueHousesForPlayersAndVassals(RoomState room)
    {
      List<HouseType> selectedHouses = room.Players.Values.Select(p => p.HouseState.Type).ToList();
      List<HouseType> selectedHousesAsVassals = room.Vassals.Values.Select(p => p.House).ToList();
      List<HouseType> allSelectedHouses = selectedHouses.Concat(selectedHousesAsVassals).ToList();

      if (allSelectedHouses.Count != allSelectedHouses.Distinct().Count())
        return Result.FAILURE("Players and vassals houses must be unique.");

      return Result.SUCCESS();
    }
  }
}
