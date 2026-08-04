namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to update a player's power tokens bid.
  /// </summary>
  public class RpcUpdatePowerTokensBid : IRoundPhaseCommand
  {
    /// <inheritdoc/>
    public RoundPhaseCommandType Type => RoundPhaseCommandType.UpdatePowerTokensBid;

    /// <summary>
    /// The ID of the player whose power tokens bid is being updated.
    /// </summary>
    public string PlayerId { get; }

    /// <summary>
    /// The new bid amount.
    /// </summary>
    public byte NewBid { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="playerId">The ID of the player whose power tokens bid is being
    /// updated.</param>
    /// <param name="newBid">The new bid amount.</param>
    public RpcUpdatePowerTokensBid(string playerId, byte newBid)
    {
      PlayerId = playerId;
      NewBid = newBid;
    }
  }
}
