namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to resolve a round phase with a specified winner.
  /// </summary>
  public class RpcResolveWithWinner : IRoundPhaseCommand
  {
    /// <inheritdoc/>
    public RoundPhaseCommandType Type => RoundPhaseCommandType.ResolveWithWinner;

    /// <summary>
    /// The player id of the winner of the round phase.
    /// </summary>
    public string WinnerPlayerId { get; }

    public RpcResolveWithWinner(string winnerPlayerId)
    {
      WinnerPlayerId = winnerPlayerId;
    }
  }
}
