using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Utilities
{
  public static class Helpers
  {
    public static List<HouseType> GetPlayerHouseTypesFromRoom(RoomState room)
    {
      return room.Players.Values.Select(p => p.HouseState.Type).ToList();
    }

    public static List<HouseType> GetVassalHouseTypesFromRoom(RoomState room)
    {
      return room.Vassals.Keys.ToList();
    }
  }
}
