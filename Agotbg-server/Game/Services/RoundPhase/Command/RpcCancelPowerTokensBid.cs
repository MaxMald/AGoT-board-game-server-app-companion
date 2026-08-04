namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to cancel a power tokens bid for a player during a round
  /// phase.
  /// </summary>
  public class RpcCancelPowerTokensBid : IRoundPhaseCommand
  {
    /// <inheritdoc/>
    public RoundPhaseCommandType Type => RoundPhaseCommandType.CancelPowerTokensBid;

    /// <summary>
    /// The ID of the player whose power tokens bid is being canceled.
    /// </summary>
    public string PlayerId { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="playerId">The ID of the player whose power tokens bid is being
    /// canceled.</param>
    public RpcCancelPowerTokensBid(string playerId)
    {
      PlayerId = playerId;
    }
  } 
}
