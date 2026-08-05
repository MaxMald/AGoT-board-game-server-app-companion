using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Represents the round phase where houses bid power tokens against the wildlings
  /// threat.
  /// </summary>
  /// 
  /// <remarks>
  /// <para>
  /// Resolves all house bids and determines whether the Night's Watch wins
  /// based on the total bet amount versus the wildlings strength. Updates the game state
  /// accordingly and transitions to the bidding presentation phase.
  /// </para>
  /// <para>
  /// Possible transitions from this phase:
  /// <list type="bullet">
  ///   <item><see cref="RoundPhaseType.WildlingsBiddingPresentation"/></item>
  /// </list>
  /// </para>
  /// </remarks>
  /// 
  public class RpWildlingsBidding : RpABiddingPhase
  {
    /// <inheritdoc />
    public override RoundPhaseType Type => RoundPhaseType.WildlingsBidding;

    /// <inheritdoc />
    protected override Result ExecuteDerivedBidResolution(
      GameState gameState,
      List<HouseBet> houseBets
    )
    {
      houseBets.RemoveAll(bet => gameState.Vassals.ContainsKey(bet.HouseType));

      gameState.Wildlings.StrengthWhenBiddingStarted = gameState.Wildlings.Strength;
      gameState.Wildlings.HouseBets.Clear();
      gameState.Wildlings.HouseBets.AddRange(houseBets);

      short totalBetAmount = GetTotalBetAmount(houseBets);
      gameState.Wildlings.TotalBetAmount = totalBetAmount;

      if (totalBetAmount >= gameState.Wildlings.Strength)
      {
        gameState.Wildlings.NightWatchWins = true;
        gameState.Wildlings.Strength = 0;
      }
      else
      {
        gameState.Wildlings.NightWatchWins = false;
        if (gameState.Wildlings.Strength <= GameConstants.WildlingStrengthReduction)
          gameState.Wildlings.Strength = 0;
        else
          gameState.Wildlings.Strength -= GameConstants.WildlingStrengthReduction;
      }

      gameState.CurrentPhase = RoundPhaseType.WildlingsBiddingPresentation;
      return Result.SUCCESS();
    }

    private static short GetTotalBetAmount(List<HouseBet> houseBets)
    {
      short totalBid = 0;
      foreach (HouseBet houseBet in houseBets)
        totalBid += houseBet.BetAmount;
      return totalBid;
    }
  }
}
