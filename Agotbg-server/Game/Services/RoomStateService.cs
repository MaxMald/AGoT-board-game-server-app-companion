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

    public Result AddNewPlayerDescriptor(RoomState room, string playerId, string playerName)
    {
      if (room.RoomStatus != RoomStatus.PreparingGame)
        return Result.FAILURE("Cannot add new players after the game has started.");

      if (room.PlayersDescriptors.Count >= room.MaxPlayers)
        return Result.FAILURE($"Cannot add new player. Maximum number of players ({room.MaxPlayers}) reached.");

      if (room.PlayersDescriptors.ContainsKey(playerId))
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

      room.PlayersDescriptors.Add(playerId, playerDescriptor);
      return Result.SUCCESS();
    }

    public Result RemovePlayerDescriptor(RoomState room, string playerId)
    {
      if (room.RoomStatus != RoomStatus.PreparingGame)
        return Result.FAILURE("Cannot remove players after the game has started.");

      if (room.HosterPlayerId == playerId)
        return Result.FAILURE("Cannot remove the hoster player from the room.");

      if (!room.PlayersDescriptors.ContainsKey(playerId))
        return Result.FAILURE($"Player ID '{playerId}' does not exist.");

      room.PlayersDescriptors.Remove(playerId);

      return Result.SUCCESS();
    }

    public Result ModifyPlayersDecriptorHouse(RoomState room, string playerId, HouseType newHouse)
    {
      if (room.RoomStatus != RoomStatus.PreparingGame)
        return Result.FAILURE("Cannot modify player house after the game has started.");

      if (!room.PlayersDescriptors.ContainsKey(playerId))
        return Result.FAILURE($"Player ID '{playerId}' does not exist.");

      PlayerDescriptor playerDescriptor = room.PlayersDescriptors[playerId];
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

      try
      {
        room.GameState = GameStateService.Create(
          room.PlayersDescriptors.Values.ToList(),
          room.MaxPlayers
        );
        room.RoomStatus = RoomStatus.InProgress;
      }
      catch (Exception ex)
      {
        room.GameState = null;
        room.RoomStatus = RoomStatus.PreparingGame;
        return Result.FAILURE($"Failed to start the game: {ex.Message}");
      }

      return Result.SUCCESS();
    }
  }
}
