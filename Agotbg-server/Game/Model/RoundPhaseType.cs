namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the different phases of the game.
  /// </summary>
  public enum RoundPhaseType : byte
  {
    Setup,
    IronBankInterestPaymentResolution,
    WesterosWildlingIconsResolution,
    Westeros,
    WildlingsBidding,
    WildlingsBiddingPresentation,
    InfluenceTrackBidding,
    InfluenceTrackBiddingTargaryenResolution,
    InfluenceTrackBiddingTargaryenPresentation,
    InfluenceTrackBiddingTieResolution,
    InfluenceTrackBiddingPresentation,
    VassalAssignment,
    Planning,
    Action,
    WinnerTieResolution,
    GameOver,

    // Special phases

    FireMadeFlesh
  }
}
