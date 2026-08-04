namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to transfer power tokens from one player to another during a
  /// round phase.
  /// </summary>
  public class RpcTransferPowerTokens
  {
    /// <inheritdoc/>
    public RoundPhaseCommandType Type => RoundPhaseCommandType.TransferPowerTokens;

    /// <summary>
    /// The id of the player from whom the power tokens are being transferred.
    /// </summary>
    public string FromPlayerId { get; }

    /// <summary>
    /// The id of the player to whom the power tokens are being transferred.
    /// </summary>
    public string ToPlayerId { get; }

    /// <summary>
    /// The amount of power tokens to transfer.
    /// </summary>
    public byte Amount { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="fromPlayerId">The id of the player from whom the power tokens are
    /// being transferred.</param>
    /// <param name="toPlayerId">The id of the player to whom the power tokens are being
    /// transferred.</param>
    /// <param name="amount">The amount of power tokens to transfer.</param>
    public RpcTransferPowerTokens(string fromPlayerId, string toPlayerId, byte amount)
    {
      FromPlayerId = fromPlayerId;
      ToPlayerId = toPlayerId;
      Amount = amount;
    }
  }
}
