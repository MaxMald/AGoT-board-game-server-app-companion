namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the tie-breaker information for the influence track bidding phase,
  /// including the starting position on the influence track and the ordered list of
  /// houses based on their priority.
  /// </summary>
  public class InfluenceTrackTiedGroupBreaker
  {
    /// <summary>
    /// The starting position on the influence track for the tie-breaker.
    /// </summary>
    public byte StartingPosition { get; set; } = 0;

    /// <summary>
    /// The ordered list of houses based on their priority for the tie-breaker.
    /// </summary>
    public List<HouseType> HouseOrderedByPriority { get; set; } = [];
  }
}
