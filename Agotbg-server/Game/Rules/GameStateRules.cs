using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;
using System.Numerics;

namespace Agotbg.Server.Game.Rules
{
  public static class GameStateRules
  {
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

      if (room.UseVassals)
      {
        result = HasValidHousesForVassals(room);
        if (!result.Success)
          return result;

        result = HasUniqueHousesForPlayersAndVassals(room);
        if (!result.Success)
          return result;
      }

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

    private static Result HasValidHousesForPlayers(RoomState room)
    {
      List<HouseType> selectedHouses = room.Players.Values.Select(p => p.House).ToList();
      if (selectedHouses.Count != selectedHouses.Distinct().Count())
        return Result.FAILURE("Players must select different houses.");

      if (room.Players.Values.Any(p => p.House == HouseType.Undefined))
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
      List<HouseType> selectedHouses = room.Players.Values.Select(p => p.House).ToList();
      List<HouseType> selectedHousesAsVassals = room.Vassals.Values.Select(p => p.House).ToList();
      List<HouseType> allSelectedHouses = selectedHouses.Concat(selectedHousesAsVassals).ToList();

      if (allSelectedHouses.Count != allSelectedHouses.Distinct().Count())
        return Result.FAILURE("Players and vassals houses must be unique.");

      return Result.SUCCESS();
    }
  }
}
