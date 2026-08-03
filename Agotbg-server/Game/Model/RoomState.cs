namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the complete state of a game room, including room configuration,
  /// players, and the game state.
  /// </summary>
  public class RoomState
  {
    /// <summary>
    /// The unique identifier for this room.
    /// </summary>
    public string RoomId { get; set; } = string.Empty;

    /// <summary>
    /// The player ID of the host who created this room.
    /// </summary>
    public string HosterPlayerId { get; set; } = string.Empty;

    /// <summary>
    /// The maximum number of players allowed in this room.
    /// </summary>
    public byte MaxPlayers { get; set; } = 3;

    /// <summary>
    /// The current status of the room (e.g., preparing game, in progress, completed).
    /// </summary>
    public RoomStatus RoomStatus { get; set; } = RoomStatus.PreparingGame;

    /// <summary>
    /// Dictionary of player descriptors in this room, keyed by player ID.
    /// </summary>
    public Dictionary<string, PlayerDescriptor> PlayersDescriptors { get; set; } = [];

    /// <summary>
    /// The current game state. Null if the game has not started yet.
    /// </summary>
    public GameState? GameState { get; set; } = null;
  }
}
