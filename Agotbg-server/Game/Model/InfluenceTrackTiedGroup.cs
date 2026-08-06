namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Defines a group of houses that are tied in the influence track bidding, along with
  /// the starting position on the influence track for the tied group.
  /// </summary>
  public class InfluenceTrackTiedGroup
  {
    /// <summary>
    /// The starting position on the influence track for the tied group.
    /// </summary>
    public byte StartingPosition { get; set; } = 0;

    /// <summary>
    /// The list of houses that are tied in the influence track bidding.
    /// </summary>
    public List<HouseType> TiedHouses { get; set; } = new List<HouseType>();
  }
}
