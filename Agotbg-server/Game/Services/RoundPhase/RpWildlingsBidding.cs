using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Represents the round phase where houses bid power tokens against the wildlings
  /// threat.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Resolves all house bids and determines whether the Night's Watch wins based on the
  /// total bet amount versus the wildlings strength. Updates the game state accordingly
  /// and transitions to the bidding presentation phase.
  /// </para>
  /// <para>
  /// Possible transitions from this phase:
  /// <list type="bullet">
  ///   <item><see cref="RoundPhaseType.WildlingsBiddingPresentation"/></item>
  /// </list>
  /// </para>
  /// </remarks>
  public class RpWildlingsBidding : RpABiddingPhase
  {
    /// <inheritdoc />
    public override RoundPhaseType Type => RoundPhaseType.WildlingsBidding;

    /// <summary>
    /// Instantiates a new instance of the <see cref="RpWildlingsBidding"/> class.
    /// </summary>
    ///
    /// <param name="gameStateService">The game state service.</param>
    /// <param name="houseStateService">The house state service.</param>
    public RpWildlingsBidding(
      IGameStateService gameStateService,
      IHouseStateService houseStateService
    ) : base(gameStateService, houseStateService)
    {}

    /// <inheritdoc />
    protected override Result ExecuteDerivedBidResolution(
      GameState gameState,
      List<HouseBet> houseBets
    )
    {
      if (gameState.Wildlings.IsPreemptiveRaid)
        return ResolvePreemptiveRaid(gameState, houseBets);
      else
        return ResolveNormalWildlingsBidding(gameState, houseBets);
    }

    /// <summary>
    /// Resolves a bidding phase for a "Preemptive Raid" by calculating the total bet
    /// amount, determining if the Night's Watch wins, and updating the game state
    /// accordingly.
    /// </summary>
    ///
    /// <param name="gameState">The current game state.</param>
    /// <param name="houseBets">The list of house bets.</param>
    ///
    /// <returns>The result of the operation.</returns>
    private static Result ResolvePreemptiveRaid(
      GameState gameState,
      List<HouseBet> houseBets
    )
    {
      gameState.Wildlings.StrengthWhenBiddingStarted = GameConstants.PreemptiveRaidWildlingStrength;
      gameState.Wildlings.HouseBets.Clear();
      gameState.Wildlings.HouseBets.AddRange(houseBets);

      short totalBetAmount = GetTotalBetAmount(gameState.Wildlings.HouseBets);
      gameState.Wildlings.TotalBetAmount = totalBetAmount;

      if (totalBetAmount >= GameConstants.PreemptiveRaidWildlingStrength)
        gameState.Wildlings.NightWatchWins = true;
      else
        gameState.Wildlings.NightWatchWins = false;

      gameState.CurrentPhase = RoundPhaseType.WildlingsBiddingPresentation;
      return Result.SUCCESS();
    }

    /// <summary>
    /// Resolves a normal wildlings bidding phase by calculating the total bet amount,
    /// determining if the Night's Watch wins, and updating the game state accordingly.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// <param name="houseBets">The list of house bets.</param>
    /// 
    /// <returns>The result of the operation.</returns>
    private static Result ResolveNormalWildlingsBidding(
      GameState gameState,
      List<HouseBet> houseBets
    )
    {
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

    /// <summary>
    /// Gets the total bet amount from a list of house bets.
    /// </summary>
    ///
    /// <param name="houseBets">The list of house bets to calculate the total bet amount
    /// from.</param>
    /// 
    /// <returns>The total bet amount.</returns>
    private static short GetTotalBetAmount(List<HouseBet> houseBets)
    {
      short totalBid = 0;
      foreach (HouseBet houseBet in houseBets)
        totalBid += houseBet.BetAmount;
      return totalBid;
    }
  }
}
