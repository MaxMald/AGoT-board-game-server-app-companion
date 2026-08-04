using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  public class RpWildlingsBidding : RpABiddingPhase
  {
    /// <inheritdoc />
    public override RoundPhaseType Type => RoundPhaseType.WildlingsBidding;

    /// <inheritdoc />
    protected override Result ExecuteDerivedBidResolution(GameState gameState)
    {
      List<HouseState> houses = GameStateService.GetAllHouses(gameState);
      List<HouseBet> housesBets = new();

      Result result = ResolveHousesBets(gameState, houses, housesBets);
      if (!result.Success)
        return result;

      gameState.Wildlings.StrengthWhenBiddingStarted = gameState.Wildlings.Strength;
      gameState.Wildlings.HouseBets.Clear();
      gameState.Wildlings.HouseBets.AddRange(housesBets);

      short totalBetAmount = GetTotalBetAmount(housesBets);
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
    /// Resolves power token bids for all houses that have placed bids and populates the
    /// output collection with the resolved bets.
    /// </summary>
    ///
    /// <remarks>If any house bid resolution fails, all previously resolved bets in the
    /// current operation are rolled back.</remarks>
    ///
    /// <param name="gameState">The current game state used for rollback operations if
    /// bid resolution fails.</param>
    /// <param name="houses">The collection of house states to process for bid
    /// resolution.</param>
    /// <param name="outHouseBets">The output collection that will be populated with
    /// successfully resolved house bets.</param>
    ///
    /// <returns>A result indicating success if all house bids were resolved
    /// successfully, or a failure result if any bid resolution failed.</returns>
    private static Result ResolveHousesBets(
      GameState gameState,
      List<HouseState> houses,
      List<HouseBet> outHouseBets
    )
    {
      foreach (HouseState houseState in houses)
      {
        if (!houseState.HasBidPowerTokens)
          continue;

        HouseBet houseBet = new()
        {
          HouseType = houseState.Type,
          BetAmount = houseState.PowerTokensBid
        };

        Result result = HouseStateService.ResolveBid(houseState);
        if (!result.Success)
        {
          UndoBetResolution(gameState, outHouseBets);
          return result;
        }

        outHouseBets.Add(houseBet);
      }

      return Result.SUCCESS();
    }

    private static short GetTotalBetAmount(List<HouseBet> houseBets)
    {
      short totalBid = 0;
      foreach (HouseBet houseBet in houseBets)
        totalBid += houseBet.BetAmount;
      return totalBid;
    }

    /// <summary>
    /// Undoes the bid resolution for all houses in the provided list of house bets. This
    /// method is called when an error occurs during the bid resolution process, allowing
    /// the game state to revert to its previous state before the bids were resolved.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// <param name="houseBets">The list of house bets to undo.</param>
    private static void UndoBetResolution(GameState gameState, List<HouseBet> houseBets)
    {
      foreach (HouseBet houseBet in houseBets)
      {
        if (gameState.Players.Values.Any(p => p.HouseState.Type == houseBet.HouseType))
        {
          PlayerState playerState = gameState.Players.Values.First(p => p.HouseState.Type == houseBet.HouseType);
          HouseStateService.UndoBidResolution(playerState.HouseState, houseBet.BetAmount);
        }
        else if (gameState.Vassals.ContainsKey(houseBet.HouseType))
        {
          HouseState vassalHouseState = gameState.Vassals[houseBet.HouseType];
          HouseStateService.UndoBidResolution(vassalHouseState, houseBet.BetAmount);
        }
      }
    }
  }
}
