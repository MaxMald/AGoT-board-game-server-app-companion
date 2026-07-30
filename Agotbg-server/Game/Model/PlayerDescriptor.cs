namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Defines a descriptor for a player in the game, including their name, unique
  /// identifier, and the type of house they are associated with.
  /// </summary>
  public class PlayerDescriptor
  {
    /// <summary>
    /// The name of the player.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The unique id of this player.
    /// </summary>
    public string PlayerId { get; set; } = string.Empty;

    /// <summary>
    /// The House associated to this player.
    /// </summary>
    public HouseType HouseType { get; set; } = HouseType.Undefined;
  }
}
