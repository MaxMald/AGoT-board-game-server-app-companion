using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to resolve the current round phase and transition to a
  /// specified next phase. This command is used to indicate that the players are ready
  /// to proceed to the next phase of the game, and it specifies which phase to move to
  /// after resolution.
  /// </summary>
  public class RpcResolveAndMoveTo : IRoundPhaseCommand
  {
    /// <inheritdoc/>
    public RoundPhaseCommandType Type => RoundPhaseCommandType.ResolveAndMoveTo;

    /// <summary>
    /// Indicates the next phase to move to after resolving the current round phase. This
    /// property is used to specify the desired next phase in the game flow.
    /// </summary>
    public RoundPhaseType NextRoundPhase { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RpcResolveAndMoveTo"/> class.
    /// </summary>
    /// 
    /// <param name="nextRoundPhase">The round phase to move to.</param>
    public RpcResolveAndMoveTo(RoundPhaseType nextRoundPhase)
    {
      NextRoundPhase = nextRoundPhase;
    }
  }
}
