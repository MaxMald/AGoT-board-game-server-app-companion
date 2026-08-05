using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  public class RpInflueceTrackBiddingTargaryenPresentation : ARoundPhase
  {
    /// <inheritdoc />
    public override RoundPhaseType Type => RoundPhaseType.InfluenceTrackBiddingTargaryenPresentation;

    /// <inheritdoc />
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (InfluenceTrackBiddingStateService.HasTiedHouseBets(gameState.InfluenceTrackBiddingState)
      {
        gameState.CurrentPhase = RoundPhaseType.InfluenceTrackBiddingTieResolution;
        return Result.SUCCESS();
      }
      else
      {
        List<HouseState> houses = GameStateService.GetAllHouses(gameState);
        List<HouseBet> houseBets = InfluenceTrackBiddingStateService
                                    .GetHouseBetsWithGiftsIncluded(gameState.InfluenceTrackBiddingState);

        InfluenceTracksService.UpdateInfluenceTrackPositionsByHouseBets(
          houses,
          houseBets,
          gameState.InfluenceTrackBiddingState.InfluenceTrackType
        );

        return Result.SUCCESS();
      }
    }

    /// <inheritdoc />
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      return commandType == RoundPhaseCommandType.Resolve;
    }
  }
}
