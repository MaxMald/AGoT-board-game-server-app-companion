using Agotbg.Server.Game.Services.Interfaces;

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
    /// <param name="gameStateService">The game state service to be used by the round
    /// phase manager.</param>
    /// <param name="houseStateService">The house state service to be used by the round
    /// phase manager.</param>
    /// <param name="vassalAssignmentStateService">The vassal assignment state service to
    /// be used by the round phase manager.</param>
    /// <param name="dragonTokensStateService">The dragon tokens state service to be used
    /// by the round phase manager.</param>
    /// <param name="influenceTrackBiddingStateService">The influence track bidding state
    /// service to be used by the round phase manager.</param>
    /// <param name="influenceTrackService">The influence track service to be used by the
    /// round phase manager.</param>
    /// <param name="wildlingsStateService">The wildlings state service to be used by the
    /// round phase manager.</param>
    /// <param name="fireMadeFleshStateService">The fire made flesh state service to be
    /// used by the round phase manager.</param>
    /// <param name="ironBankInterestPaymentStateService">The iron bank interest payment
    /// state service to be used by the round phase manager.</param>
    ///
    /// <returns>A configured round phase manager with all default phases
    /// registered.</returns>
    public static IRoundPhaseManager CreateDefault(
      IGameStateService gameStateService,
      IHouseStateService houseStateService,
      IVassalAssignmentStateService vassalAssignmentStateService,
      IDragonTokensStateService dragonTokensStateService,
      IInfluenceTrackBiddingStateService influenceTrackBiddingStateService,
      IInfluenceTrackService influenceTrackService,
      IWildlingsStateService wildlingsStateService,
      IFireMadeFleshStateService fireMadeFleshStateService,
      IIronBankInterestPaymentStateService ironBankInterestPaymentStateService
    )
    {
      RoundPhaseManager manager = new();
      manager.RegisterRoundPhase(new RpSetup(gameStateService, houseStateService, vassalAssignmentStateService));
      manager.RegisterRoundPhase(new RpWesterosWildlingIconsResolution(gameStateService, houseStateService));
      manager.RegisterRoundPhase(new RpWesteros(gameStateService, houseStateService, vassalAssignmentStateService, influenceTrackBiddingStateService, influenceTrackService, wildlingsStateService, fireMadeFleshStateService));
      manager.RegisterRoundPhase(new RpWildlingsBidding(gameStateService, houseStateService));
      manager.RegisterRoundPhase(new RpWildlingsBiddingPresentation(gameStateService, houseStateService, wildlingsStateService));
      manager.RegisterRoundPhase(new RpInfluenceTrackBidding(gameStateService, houseStateService, influenceTrackBiddingStateService, influenceTrackService));
      manager.RegisterRoundPhase(new RpInfluenceTrackBiddingTargaryenResolution(gameStateService, houseStateService));
      manager.RegisterRoundPhase(new RpInfluenceTrackBiddingTargaryenPresentation(gameStateService, houseStateService, influenceTrackBiddingStateService, influenceTrackService));
      manager.RegisterRoundPhase(new RpInfluenceTrackBiddingTieResolution(gameStateService, houseStateService, influenceTrackBiddingStateService));
      manager.RegisterRoundPhase(new RpInfluenceTrackBiddingPresentation(gameStateService, houseStateService, influenceTrackBiddingStateService));
      manager.RegisterRoundPhase(new RpVassalAssignment(gameStateService, houseStateService, vassalAssignmentStateService));
      manager.RegisterRoundPhase(new RpPlanning(gameStateService, houseStateService));
      manager.RegisterRoundPhase(new RpAction(gameStateService, houseStateService, dragonTokensStateService, influenceTrackService, ironBankInterestPaymentStateService));
      manager.RegisterRoundPhase(new RpWinnerTieResolution(gameStateService, houseStateService));
      manager.RegisterRoundPhase(new RpGameOver(gameStateService, houseStateService));
      manager.RegisterRoundPhase(new RpFireMadeFlesh(gameStateService, houseStateService, dragonTokensStateService, fireMadeFleshStateService));
      manager.RegisterRoundPhase(new RpIronBankInterestPaymentResolution(gameStateService, houseStateService, ironBankInterestPaymentStateService));
      return manager;
    }
  }
}
