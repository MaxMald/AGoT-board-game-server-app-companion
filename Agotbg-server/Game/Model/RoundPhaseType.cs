namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the different phases of the game.
  /// </summary>
  public enum RoundPhaseType : byte
  {
    Setup,
    Westeros,
    WildlingsBidding,
    WildlingsBiddingResolution,
    IronThroneBidding,
    IronThroneBiddingResolution,
    FiefdomsBidding,
    FiefdomsBiddingResolution,
    KingsCourtBidding,
    KingsCourtBiddingResolution,
    Planning,
    Action,
    TieResolution,
    GameOver
  }
}
