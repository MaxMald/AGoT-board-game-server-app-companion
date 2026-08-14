using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.Interfaces
{
  /// <summary>
  /// Defines operations for managing Iron Bank loan interest state.
  /// </summary>
  public interface IIronBankInterestPaymentStateService
  {
    /// <summary>
    /// Intialize the Iron Bank Loan Interest State with the given players.
    /// </summary>
    ///
    /// <param name="state">The Iron Bank Loan Interest State to initialize.</param>
    /// <param name="players">The list of players to include in the state.</param>
    public void Initialize(
      IronBankInterestPaymentState state,
      List<PlayerState> players
    );

    /// <summary>
    /// Indicates if the given state has any resolved interest payments.
    /// </summary>
    ///
    /// <param name="state">The Iron Bank Loan Interest State to check for resolved
    /// interest payments.</param>
    /// 
    /// <returns>True if any interest payments have been resolved; otherwise,
    /// false.</returns>
    public bool HasAnyResolvedInterestPayment(IronBankInterestPaymentState state);

    /// <summary>
    /// Resolve the interest payment for the given player in the Iron Bank Loan Interest
    /// State.
    /// </summary>
    ///
    /// <remarks>
    /// This method will update the state to reflect that the player has resolved their
    /// interest payment.
    /// </remarks>
    ///
    /// <param name="state">The Iron Bank Loan Interest State to update.</param>
    /// <param name="playerState">The player state for which to resolve the interest
    /// payment.</param>
    /// 
    /// <returns>The result of the operation.</returns>
    public Result ResolvePlayerInterestPayment(
      IronBankInterestPaymentState state,
      PlayerState playerState
    );

    /// <summary>
    /// Clears the specified loan interest state.
    /// </summary>
    ///
    /// <remarks>
    /// This method resets the payment resolution of each player.
    /// </remarks>
    ///
    /// <param name="state">The loan interest state to clear.</param>
    public void Clear(IronBankInterestPaymentState state);
  }
}
