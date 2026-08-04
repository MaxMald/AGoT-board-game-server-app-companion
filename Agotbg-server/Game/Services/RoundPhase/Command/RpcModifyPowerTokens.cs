namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to modify a player's power tokens during a round phase.
  /// </summary>
  public class RpcModifyPowerTokens : IRoundPhaseCommand
  {
    /// <inheritdoc/>
    public RoundPhaseCommandType Type => RoundPhaseCommandType.ModifyPowerTokens;

    /// <summary>
    /// The id of the player whose power tokens are being modified.
    /// </summary>
    public string PlayerId { get; }

    /// <summary>
    /// The delta of power tokens to modify. A positive value indicates an increase,
    /// while a negative value indicates a decrease.
    /// </summary>
    public short Delta { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="playerId">The id of the player whose power tokens are being
    /// modified.</param>
    /// <param name="delta">The delta of power tokens to modify. A positive value
    /// indicates an increase, while a negative value indicates a decrease.</param>
    public RpcModifyPowerTokens(string playerId, short delta)
    {
      PlayerId = playerId;
      Delta = delta;
    }
  }
}
