namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to update the interest rate of an Iron Bank loan for a
  /// specific player during a round phase.
  /// </summary>
  public class RpcUpdateIronBankLoanInterest : IRoundPhaseCommand
  {
    /// <inheritdoc/>
    public RoundPhaseCommandType Type => RoundPhaseCommandType.UpdateIronBankLoanInterest;

    /// <summary>
    /// The ID of the player who is updating the interest rate of their Iron Bank loan.
    /// </summary>
    public string PlayerId { get; }

    /// <summary>
    /// The new interest rate of the Iron Bank loan.
    /// </summary>
    public byte NewInterest { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="playerId">The ID of the player who is updating the interest rate of
    /// their Iron Bank loan.</param>
    /// <param name="newInterest">The new interest rate of the Iron Bank loan.</param>
    public RpcUpdateIronBankLoanInterest(string playerId, byte newInterest)
    {
      PlayerId = playerId;
      NewInterest = newInterest;
    }
  }
}
