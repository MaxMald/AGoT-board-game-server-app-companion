namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Provides a factory for creating instances of <see cref="IRoundPhaseManager"/>.
  /// </summary>
  public static class RoundPhaseManagerFactory
  {
    /// <summary>
    /// Creates a round phase manager with the default round phase configuration.
    /// </summary>
    /// 
    /// <returns>A configured round phase manager with all default phases
    /// registered.</returns>
    public static IRoundPhaseManager CreateDefault()
    {
      RoundPhaseManager manager = new();
      manager.RegisterRoundPhase(new RpSetup());
      manager.RegisterRoundPhase(new RpWesterosWildlingIconsResolution());
      manager.RegisterRoundPhase(new RpWesteros());
      manager.RegisterRoundPhase(new RpWildlingsBidding());
      manager.RegisterRoundPhase(new RpWildlingsBiddingPresentation());
      manager.RegisterRoundPhase(new RpInfluenceTrackBidding());
      manager.RegisterRoundPhase(new RpInfluenceTrackBiddingTargaryenResolution());
      manager.RegisterRoundPhase(new RpInfluenceTrackBiddingTargaryenPresentation());
      manager.RegisterRoundPhase(new RpInfluenceTrackBiddingTieResolution());
      manager.RegisterRoundPhase(new RpInfluenceTrackBiddingPresentation());
      manager.RegisterRoundPhase(new RpVassalAssignment());
      manager.RegisterRoundPhase(new RpPlanning());
      manager.RegisterRoundPhase(new RpAction());
      manager.RegisterRoundPhase(new RpWinnerTieResolution());
      manager.RegisterRoundPhase(new RpGameOver());
      manager.RegisterRoundPhase(new RpFireMadeFlesh());
      return manager;
    }
  }
}
