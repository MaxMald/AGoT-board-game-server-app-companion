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
      room.Round.RoundNumber = 1;
      room.Round.CurrentPhase = GamePhaseType.Planning;

      for (int i = 0; i < room.Players.Count; i++)
      {
        PlayerState player = room.Players.ElementAt(i).Value;

        result = PlayerStateInitializer.InitializeForHouse(player, player.House);
        if (!result.Success)
          return result;
      }

      for (int i = 0; i < room.Vassals.Count; i++)
      {
        VassalState vassal = room.Vassals.ElementAt(i).Value;
        result = VassalStateInitializer.InitializeForHouse(vassal, vassal.House);
        if (!result.Success)
          return result;
      }

      return new Result {
        Success = true,
        Message = string.Empty
      };
    }
  }
}
