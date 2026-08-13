using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;

namespace Agotbg.Server.Game.Services
{
  /// <inheritdoc/>
  public class WildlingsStateService : IWildlingsStateService
  {
    /// <inheritdoc/>
    public void Initialize(WildlingsState wildlingsState)
    {
      wildlingsState.Strength = GameConstants.WildingStartingStrength;
      wildlingsState.StrengthWhenBiddingStarted = 0;
      wildlingsState.TotalBetAmount = 0;
      wildlingsState.HouseBets.Clear();
      wildlingsState.NightWatchWins = false;
      wildlingsState.IsPreemptiveRaid = false;
    }

    /// <inheritdoc/>
    public void PrepareForBidding(WildlingsState state, bool isPreemptiveRaid)
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

    /// <inheritdoc/>
    public void ClearBiddingProperties(WildlingsState state)
    {
      state.StrengthWhenBiddingStarted = 0;
      state.TotalBetAmount = 0;
      state.HouseBets.Clear();
      state.NightWatchWins = false;
      state.IsPreemptiveRaid = false;
    }
  }
}
