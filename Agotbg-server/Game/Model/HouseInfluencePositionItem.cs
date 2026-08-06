namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the influence position of a house on a specific influence track in the
  /// game.
  /// </summary>
  public class HouseInfluencePositionItem
  {
    /// <summary>
    /// The type of the house.
    /// </summary>
    public HouseType HouseType { get; set; }

    /// <summary>
    /// The influence position.
    /// </summary>
    public byte InfluencePosition { get; set; } = 0;
  }
}
