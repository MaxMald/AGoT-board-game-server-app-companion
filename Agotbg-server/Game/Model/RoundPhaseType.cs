namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the different phases of the game.
  /// </summary>
  public enum RoundPhaseType : byte
  {
    Setup,
    WesterosWildlingIconsResolution,
    Westeros,
    WildlingsBidding,
    WildlingsBiddingTieResolution,
    IronThroneBidding,
    IronThroneBiddingTargaryenResolution,
    IronThroneBiddingTieResolution,
    FiefdomsBidding,
    FiefdomsBiddingTargaryenResolution,
    FiefdomsBiddingTieResolution,
    KingsCourtBidding,
    KingsCourtBiddingTargaryenResolution,
    KingsCourtBiddingTieResolution,
    Planning,
    Action,
    WinnerTieResolution,
    GameOver
  }
}
