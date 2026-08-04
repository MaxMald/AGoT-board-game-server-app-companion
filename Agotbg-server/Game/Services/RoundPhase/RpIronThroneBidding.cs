using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  public class RpIronThroneBidding : RpABiddingPhase
  {
    /// <inheritdoc/>
    public override RoundPhaseType Type => RoundPhaseType.IronThroneBidding;

    /// <inheritdoc/>
    protected override Result ExecuteDerivedBidResolution(
      GameState gameState,
      List<HouseBet> houseBets
    )
    {
      gameState.InfluenceTrackBiddingState.InfluenceTrackType = InfluenceTrackType.IronThrone;
      gameState.InfluenceTrackBiddingState.HouseBets.Clear();
      gameState.InfluenceTrackBiddingState.HouseBets.AddRange(houseBets);

      if (ShouldMoveToTargaryenResolution(houseBets))
      {
        gameState.CurrentPhase = RoundPhaseType.IronThroneBiddingTargaryenResolution;
        return Result.SUCCESS();
      }

      // TODO
      // 1. Determines if it has ties and move to tie resolution
      // 2. or resolve new iron throne track order and move to next bidding presentation phase.

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
