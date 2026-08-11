using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.Interfaces
{
  /// <summary>
  /// Defines the interface for services that manage influence track bidding state.
  /// </summary>
  public interface IInfluenceTrackBiddingStateService
  {
    /// <summary>
    /// Prepares the influence track bidding state for the specified track type.
    /// </summary>
    /// 
    /// <param name="state">The influence track bidding state to prepare.</param>
    /// <param name="trackType">The type of influence track.</param>
    public void Prepare(
      InfluenceTrackBiddingState state,
      InfluenceTrackType trackType
    );

    /// <summary>
    /// Processes all house bets to determine influence track positions. Applies
    /// Targaryen power token gifts, sorts houses by bet amount, and identifies groups of
    /// tied houses. Higher bets receive better positions (lower position numbers).
    /// </summary>
    ///
    /// <param name="state">The influence track bidding state to process.</param>
    public void ProcessBetsAndDeterminePositions(InfluenceTrackBiddingState state);

    /// <summary>
    /// Checks if there are any unresolved tied groups in the bidding state.
    /// </summary>
    /// 
    /// <param name="state">The influence track bidding state to check.</param>
    /// 
    /// <returns>True if there are tied groups, false otherwise.</returns>
    public bool HasTiedGroups(InfluenceTrackBiddingState state);

    /// <summary>
    /// Resolves a tied group by assigning influence positions based on the provided
    /// priority order. The tied group is removed from the state after resolution.
    /// </summary>
    ///
    /// <param name="state">The influence track bidding state containing the tied
    /// group.</param>
    /// <param name="tiedGroupBreaker">The tie breaker specifying house priority
    /// order.</param>
    ///
    /// <returns>A Result indicating success or failure with an error message.</returns>
    public Result ResolveTieGroup(
      InfluenceTrackBiddingState state,
      InfluenceTrackTiedGroupBreaker tiedGroupBreaker
    );

    /// <summary>
    /// Clears the given <see cref="InfluenceTrackBiddingState"/>, resetting its
    /// properties to default values and clearing all lists.
    /// </summary>
    ///
    /// <param name="state">The influence track bidding state to clear.</param>
    public void Clear(InfluenceTrackBiddingState state);
  }
}
