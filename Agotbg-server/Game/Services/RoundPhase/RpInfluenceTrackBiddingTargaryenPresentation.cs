using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Represents the round phase where the Targaryen player presents their influence
  /// gifts results. Other players can then see the alteration of the influence track
  /// bidding based on the Targaryen player's gifts.
  /// </summary>
  ///
  /// <remarks>
  /// Possible transitions from this phase include:
  /// <list type="bullet">
  /// <item>
  ///   <item><see cref="RoundPhaseType.InfluenceTrackBiddingTieResolution"/></item>
  ///   <item><see cref="RoundPhaseType.InfluenceTrackBiddingPresentation"/></item>
  /// </item>
  /// </list>
  /// </remarks>
  public class RpInfluenceTrackBiddingTargaryenPresentation : ARoundPhase
  {
    /// <inheritdoc />
    public override RoundPhaseType Type => RoundPhaseType.InfluenceTrackBiddingTargaryenPresentation;

    /// <inheritdoc />
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      InfluenceTrackBiddingStateService.ProcessBetsAndDeterminePositions(
        gameState.InfluenceTrackBiddingState
      );

      if (InfluenceTrackBiddingStateService.HasTiedGroups(gameState.InfluenceTrackBiddingState))
      {
        gameState.CurrentPhase = RoundPhaseType.InfluenceTrackBiddingTieResolution;
      }
      else
      {
        try
        {
          List<HouseState> houses = GameStateService.GetAllHouses(gameState);
          InfluenceTracksService.UpdateInfluenceTrackPositions(
            houses,
            gameState.InfluenceTrackBiddingState.HouseInfluencePositions,
            gameState.InfluenceTrackBiddingState.InfluenceTrackType
          );
        }
        catch (Exception e)
        {
          return Result.FAILURE($"Failed to resolve new influence track order: {e.Message}");
        }

        gameState.CurrentPhase = RoundPhaseType.InfluenceTrackBiddingPresentation;
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
