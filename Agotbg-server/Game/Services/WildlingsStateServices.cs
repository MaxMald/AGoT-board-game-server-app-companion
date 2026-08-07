using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services
{
  public static class WildlingsStateServices
  {
    /// <summary>
    /// Prepares the WildlingsState for bidding on a "Wildlings Attack" phase.
    /// </summary>
    ///
    /// <param name="state">The WildlingsState to prepare for bidding.</param>
    /// <param name="isPreemptiveRaid">Indicates whether the bidding is for a preemptive
    /// raid.</param>
    public static void PrepareForBidding(WildlingsState state, bool isPreemptiveRaid)
    {
      if (isPreemptiveRaid)
      {
        state.StrengthWhenBiddingStarted = GameConstants.PreemptiveRaidWildlingStrength;
        state.IsPreemptiveRaid = true;
      } 
      else
        state.StrengthWhenBiddingStarted = state.Strength;

      state.TotalBetAmount = 0;
      state.HouseBets.Clear();
      state.NightWatchWins = false;
    }

    /// <summary>
    /// Clears the bidding properties of the WildlingsState.
    /// </summary>
    ///
    /// <param name="state">The WildlingsState to clear bidding properties for.</param>
    public static void ClearBiddingProperties(WildlingsState state)
    {
      state.StrengthWhenBiddingStarted = 0;
      state.TotalBetAmount = 0;
      state.HouseBets.Clear();
      state.NightWatchWins = false;
      state.IsPreemptiveRaid = false;
    }
  }
}
