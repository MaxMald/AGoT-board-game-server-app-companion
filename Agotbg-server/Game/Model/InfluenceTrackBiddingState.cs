namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Contains information used to resolve the influence track bidding phases of the
  /// game.
  /// </summary>
  public class InfluenceTrackBiddingState
  {
    /// <summary>
    /// The current influence track type which houses are bidding for.
    /// </summary>
    public InfluenceTrackType InfluenceTrackType { get; set; } = InfluenceTrackType.None;

    /// <summary>
    /// The bet of each house on the current influence track.
    /// </summary>
    public List<HouseBet> houseBets { get; set; } = [];
  }
}
