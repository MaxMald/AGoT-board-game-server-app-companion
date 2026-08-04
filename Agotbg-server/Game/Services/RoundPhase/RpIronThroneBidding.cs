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
      return Result.SUCCESS();
    }
  }
}
