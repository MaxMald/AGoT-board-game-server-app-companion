using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Represents a phase in a game round that can execute round phase commands.
  /// </summary>
  public interface IRoundPhase
  {
    /// <summary>
    /// The type of this round phase.
    /// </summary>
    public RoundPhaseType Type { get; }

    /// <summary>
    /// Executes the specified round phase command on the game state.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// <param name="command">The round phase command to execute.</param>
    /// 
    /// <returns>The result of the command execution.</returns>
    public Result Execute(GameState gameState, IRoundPhaseCommand command);
  }
}
