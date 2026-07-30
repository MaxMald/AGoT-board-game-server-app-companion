using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Rules;

namespace Agotbg.Server.Utilities
{
  public static class Helpers
  {
    public static readonly int MaxPlayerNameLength = 20;

    public static List<HouseType> GetPlayerHouseTypesFromRoom(RoomState room)
    {
      return room.Players.Values.Select(p => p.HouseState.Type).ToList();
    }

    public static List<HouseType> GetVassalHouseTypesFromRoom(RoomState room)
    {
      return room.Vassals.Keys.ToList();
    }

    public static Result IsValidPlayerName(string playerName)
    {
      if (string.IsNullOrWhiteSpace(playerName))
        return Result.FAILURE("Player name cannot be empty or whitespace.");

      if (playerName.Length > MaxPlayerNameLength)
        return Result.FAILURE($"Player name cannot exceed {MaxPlayerNameLength} characters.");

      if (!playerName.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
        return Result.FAILURE("Player name can only contain letters, numbers, and spaces.");

      return Result.SUCCESS();
    }
  }
}
