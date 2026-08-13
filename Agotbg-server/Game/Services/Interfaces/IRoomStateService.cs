using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.Interfaces
{
  /// <summary>
  /// Defines the contract for a service that manages the state of a room.
  /// </summary>
  public interface IRoomStateService
  {
    /// <summary>
    /// Adds a new player descriptor to the room state.
    /// </summary>
    ///
    /// <param name="room">The state of the room to which the player descriptor will be
    /// added.</param>
    /// <param name="playerId">The unique identifier of the player.</param>
    /// <param name="playerName">The name of the player.</param>
    ///
    /// <returns>A <see cref="Result"/> indicating the success or failure of the
    /// operation.</returns>
    public Result AddNewPlayerDescriptor(
      RoomState room,
      string playerId,
      string playerName
    );

    /// <summary>
    /// Removes a player descriptor from the specified room.
    /// </summary>
    /// 
    /// <param name="room">The room state from which to remove the player
    /// descriptor.</param>
    /// <param name="playerId">The identifier of the player to remove.</param>
    /// 
    /// <returns>A result indicating whether the operation succeeded.</returns>
    public Result RemovePlayerDescriptor(RoomState room, string playerId);

    /// <summary>
    /// Change the type of the house of a player descriptor in the specified room.
    /// </summary>
    ///
    /// <param name="room">The room state containing the player descriptor to
    /// modify.</param>
    /// <param name="playerId">The identifier of the player whose house type is to be
    /// changed.</param>
    /// <param name="newHouse">The new house type to assign to the player.</param>
    /// 
    /// <returns>A result indicating whether the operation succeeded.</returns>
    public Result ModifyPlayerDescriptorHouse(
      RoomState room,
      string playerId,
      HouseType newHouse
    );

    /// <summary>
    /// Modifies the maximum number of players allowed in the specified room.
    /// </summary>
    ///
    /// <param name="room">The room state to modify.</param>
    /// <param name="newMaxPlayers">The new maximum number of players.</param>
    ///
    /// <returns>A result indicating whether the operation succeeded.</returns>
    public Result ModifyMaxNumberOfPlayers(RoomState room, byte newMaxPlayers);

    /// <summary>
    /// Creates a new game instances for the given room state.
    /// </summary>
    ///
    /// <remarks>
    /// This method instantiantes and sets a new <see cref="GameState"/> object in the
    /// given room state. If the current configuration of the room state is invalid, it
    /// will return a failure result with an appropriate message.
    /// </remarks>
    ///
    /// <param name="roomState">The room state for which to create a new game
    /// instance.</param>
    ///
    /// <returns>A result indicating whether the operation succeeded.</returns>
    public Result CreateGame(RoomState roomState);
  }
}
