using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Represents the round phase that presents influence track bidding results and
  /// transitions to the next bidding phase or Westeros phase. During this phase, players
  /// are able to see the results of the influence track bidding and the new influence
  /// track order.
  /// </summary>
  ///
  /// <remarks>
  /// Possible transitions from this phase include:
  /// <list type="bullet">
  ///   <item><see cref="RoundPhaseType.InfluenceTrackBidding"/></item>
  ///   <item><see cref="RoundPhaseType.Westeros"/></item>
  /// </list>
  /// </remarks>
  public class RpInfluenceTrackBiddingPresentation : ARoundPhase
  {
    /// <inheritdoc />
    public override RoundPhaseType Type => RoundPhaseType.InfluenceTrackBiddingPresentation;

    /// <inheritdoc />
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      InfluenceTrackType currentBiddingType = gameState.InfluenceTrackBiddingState
                                                       .InfluenceTrackType;

      if (currentBiddingType == InfluenceTrackType.IronThrone)
      {
        GameStateService.PrepareForInfluenceTrackBidding(gameState, InfluenceTrackType.Fiefdom);
        gameState.CurrentPhase = RoundPhaseType.InfluenceTrackBidding;
      } 
      else if (currentBiddingType == InfluenceTrackType.Fiefdom)
      {
        GameStateService.PrepareForInfluenceTrackBidding(gameState, InfluenceTrackType.KingsCourt);
        gameState.CurrentPhase = RoundPhaseType.InfluenceTrackBidding;
      } 
      else
      {
        GameStateService.ClearInfluenceTrackBiddingState(gameState);
        gameState.CurrentPhase = RoundPhaseType.Westeros;
      }
      return Result.SUCCESS();
    }

    /// <inheritdoc />
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      return commandType == RoundPhaseCommandType.Resolve;
    }
  }
}
