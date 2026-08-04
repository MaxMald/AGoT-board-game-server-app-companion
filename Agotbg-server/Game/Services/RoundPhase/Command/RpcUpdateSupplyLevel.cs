namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to update a player's supply level during a round phase.
  /// </summary>
  public class RpcUpdateSupplyLevel : IRoundPhaseCommand
  {
    /// <inheritdoc/>
    public RoundPhaseCommandType Type => RoundPhaseCommandType.UpdateSupplyLevel;

    /// <summary>
    /// The ID of the player whose supply level is being updated.
    /// </summary>
    public string PlayerId { get; }

    /// <summary>
    /// The new supply level.
    /// </summary>
    public byte NewSupplyLevel { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="playerId">The ID of the player whose supply level is being
    /// updated.</param>
    /// <param name="newSupplyLevel">The new supply level.</param>
    public RpcUpdateSupplyLevel(string playerId, byte newSupplyLevel)
    {
      PlayerId = playerId;
      NewSupplyLevel = newSupplyLevel;
    }
  }
}
