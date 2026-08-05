using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provide utility methods for working with InfluenceTrackBiddingState objects.
  /// </summary>
  public static class InfluenceTrackBiddingStateService
  {
    /// <summary>
    /// Indicates whether there are tied house bets in the given <see
    /// cref="InfluenceTrackBiddingState"/>. This method also considers any <see
    /// cref="PowerTokenGift"/> allocated on the bidding state when determining if there
    /// are tied bets.
    /// </summary>
    ///
    /// <param name="state">The InfluenceTrackBiddingState to evaluate.</param>
    ///
    /// <returns>Returns true if there are tied bets, and false otherwise.</returns>
    public static bool HasTiedHouseBets(InfluenceTrackBiddingState state)
    {
      if (state.HouseBets.Count < 2)
        return false;

      if (state.TargaryenPowerTokenGifts.Count > 0)
        return HasTiedHouseBetsWithGifts(state);

      List<HouseBet> sortedBets = state.HouseBets
                                       .OrderByDescending(bet => bet.BetAmount)
                                       .ToList();

      RemoveTargaryenBetIfAny(sortedBets);

      for (int i = 0; i < sortedBets.Count -1; i++)
      {
        if (sortedBets[i].BetAmount == sortedBets[i + 1].BetAmount)
          return true;
      }
      return false;
    }

    /// <summary>
    /// Gets house bet amounts with any Targaryen power token gifts included.
    /// </summary>
    ///
    /// <param name="state">The InfluenceTrackBiddingState to evaluate.</param>
    ///
    /// <returns>A list of HouseBet objects with Targaryen power token gifts
    /// included.</returns>
    public static List<HouseBet> GetHouseBetsWithGiftsIncluded(
      InfluenceTrackBiddingState state
    )
    {
      List<HouseBet> houseBets = state.HouseBets.ToList();
      foreach (PowerTokenGift gift in state.TargaryenPowerTokenGifts)
      {
        HouseBet? houseBet = houseBets.FirstOrDefault(
          bet => bet.HouseType == gift.Receiver
        );

        if (houseBet != null)
          houseBet.BetAmount += gift.Amount;
      }

      return houseBets;
    }

    private static bool HasTiedHouseBetsWithGifts(InfluenceTrackBiddingState state)
    {
      List<HouseBet> houseBets = GetHouseBetsWithGiftsIncluded(state);
      List<HouseBet> sortedBets = houseBets
                                  .OrderByDescending(bet => bet.BetAmount)
                                  .ToList();

      RemoveTargaryenBetIfAny(sortedBets);

      for (int i = 0; i < sortedBets.Count -1; i++)
      {
        if (sortedBets[i].BetAmount == sortedBets[i + 1].BetAmount)
          return true;
      }
      return false;
    }

    private static void RemoveTargaryenBetIfAny(List<HouseBet> houseBets)
    {
      houseBets.RemoveAll(bet => bet.HouseType == HouseType.Targaryen);
    }
  }
}
