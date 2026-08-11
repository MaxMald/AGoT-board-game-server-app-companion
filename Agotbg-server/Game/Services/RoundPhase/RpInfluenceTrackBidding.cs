using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Represents the round phase where houses bid power tokens to determine their
  /// positions on an influence track.
  /// </summary>
  ///
  /// <remarks>
  /// Possible transitions from this phase:
  /// <list type="bullet">
  ///   <item><see cref="RoundPhaseType.InfluenceTrackBiddingTargaryenResolution"/></item>
  ///   <item><see cref="RoundPhaseType.InfluenceTrackBiddingTieResolution"/></item>
  ///   <item><see cref="RoundPhaseType.InfluenceTrackBiddingPresentation"/></item>
  /// </list>
  /// </remarks>
  public class RpInfluenceTrackBidding : RpABiddingPhase
  {
    /// <inheritdoc/>
    public override RoundPhaseType Type => RoundPhaseType.InfluenceTrackBidding;

    /// <summary>
    /// Creates a new instance of the <see cref="RpInfluenceTrackBidding"/> class.
    /// </summary>
    ///
    /// <param name="gameStateService">The game state service.</param>
    /// <param name="houseStateService">The house state service.</param>
    public RpInfluenceTrackBidding(
      IGameStateService gameStateService,
      IHouseStateService houseStateService
    ) : base(gameStateService, houseStateService)
    { }

    /// <inheritdoc/>
    protected override Result ExecuteDerivedBidResolution(
      GameState gameState,
      List<HouseBet> houseBets
    )
    {
      if (gameState.InfluenceTrackBiddingState.InfluenceTrackType == InfluenceTrackType.None)
        return Result.FAILURE("Influence track type is not set.");

      gameState.InfluenceTrackBiddingState.HouseBets.Clear();
      gameState.InfluenceTrackBiddingState.HouseBets.AddRange(houseBets);

      if (ShouldMoveToTargaryenResolution(houseBets))
      {
        gameState.CurrentPhase = RoundPhaseType.InfluenceTrackBiddingTargaryenResolution;
        return Result.SUCCESS();
      }

      InfluenceTrackBiddingStateService.ProcessBetsAndDeterminePositions(
        gameState.InfluenceTrackBiddingState
      );

      if (InfluenceTrackBiddingStateService.HasTiedGroups(gameState.InfluenceTrackBiddingState))
      {
        gameState.CurrentPhase = RoundPhaseType.InfluenceTrackBiddingTieResolution;
        return Result.SUCCESS();
      }

      try
      {
        m_gameStateService.GetAllHouseStates(gameState, m_houseStates);
        InfluenceTracksService.UpdateInfluenceTrackPositions(
          m_houseStates,
          gameState.InfluenceTrackBiddingState.HouseInfluencePositions,
          gameState.InfluenceTrackBiddingState.InfluenceTrackType
        );
      }
      catch(Exception e)
      {
        return Result.FAILURE($"Failed to resolve new influence track order: {e.Message}");
      }

      gameState.CurrentPhase = RoundPhaseType.InfluenceTrackBiddingPresentation;
      return Result.SUCCESS();
    }

    private static bool ShouldMoveToTargaryenResolution(List<HouseBet> houses)
    {
      HouseBet? targaryenHouseBet = houses.FirstOrDefault(
        house => house.HouseType == HouseType.Targaryen
      );

      if (targaryenHouseBet == null)
        return false;

      return targaryenHouseBet.BetAmount > 0;
    }
  }
}
