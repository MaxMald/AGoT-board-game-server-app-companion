namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to resolve the current round phase. This command is used to
  /// indicate that the players are ready to proceed to the next phase of the game.
  /// </summary>
  public class RpcResolve : IRoundPhaseCommand
  {
    /// <inheritdoc/>
    public RoundPhaseCommandType Type => RoundPhaseCommandType.Resolve;

    /// <summary>
    /// The ID of the player who issued the resolve command.
    /// </summary>
    public string PlayerId { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="playerId">The ID of the player who issued the resolve
    /// command.</param>
    public RpcResolve(string playerId)
    {
      PlayerId = playerId;
    }
  }
}
