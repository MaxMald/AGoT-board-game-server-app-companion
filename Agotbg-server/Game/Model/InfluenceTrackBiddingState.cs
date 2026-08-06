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
    public List<HouseBet> HouseBets { get; set; } = [];

    /// <summary>
    /// The power token gifts of the Targaryen house for the current influence track
    /// bidding.
    /// </summary>
    public List<PowerTokenGift> TargaryenPowerTokenGifts { get; set; } = [];

    /// <summary>
    /// The list of tied groups of houses in the influence track bidding, along with
    /// their starting positions on the influence track.
    /// </summary>
    public List<InfluenceTrackTiedGroup> TiedGroups { get; set; } = [];

    /// <summary>
    /// List of houses with their influence positions on the current influence track.
    /// </summary>
    public List<HouseInfluencePositionItem> HouseInfluencePositions { get; set; } = [];
  }
}
