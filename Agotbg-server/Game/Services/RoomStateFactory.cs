using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services
{
  public class RoomStateFactory
  {
    public RoomState Create(
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
        hosterPlayerId,
        new PlayerDescriptor()
        {
          Name = hosterPlayerName,
          PlayerId = hosterPlayerId,
          HouseType = HouseType.Undefined
        }
      );

      return room;
    }
  }
}
