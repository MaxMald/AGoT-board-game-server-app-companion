using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services.Interfaces
{
  /// <summary>
  /// Defines the interface for a service that manages the state of Wildlings in the
  /// game.
  /// </summary>
  public interface IWildlingsStateService
  {
    /// <summary>
    /// Initialize the wildlings state for a new game.
    /// </summary>
    /// 
    /// <param name="wildlingsState">The WildlingsState to initialize.</param>
    public void Initialize(WildlingsState wildlingsState);

    /// <summary>
    /// Prepares the WildlingsState for bidding on a "Wildlings Attack" phase.
    /// </summary>
    ///
    /// <param name="state">The WildlingsState to prepare for bidding.</param>
    /// <param name="isPreemptiveRaid">Indicates whether the bidding is for a preemptive
    /// raid.</param>
    public void PrepareForBidding(WildlingsState state, bool isPreemptiveRaid);

    /// <summary>
    /// Clears the bidding properties of the WildlingsState.
    /// </summary>
    ///
    /// <param name="state">The WildlingsState to clear bidding properties for.</param>
    public void ClearBiddingProperties(WildlingsState state);
  }
}
