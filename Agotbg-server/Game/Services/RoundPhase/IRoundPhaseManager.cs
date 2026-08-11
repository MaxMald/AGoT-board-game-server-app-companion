using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Defines the interface for managing the execution of game round phases.
  /// Implementations of this interface are responsible for routing commands to the
  /// appropriate phase handler based on the current game state.
  /// </summary>
  public interface IRoundPhaseManager
  {
    /// <summary>
    /// Register a IRoundPhase implementation to the manager.
    /// </summary>
    /// 
    /// <param name="roundPhase">The round phase implementation to register.</param>
    public void RegisterRoundPhase(IRoundPhase roundPhase);

    /// <summary>
    /// Executes the specified round phase command against the provided game state.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// <param name="command">The round phase command to execute.</param>
    /// 
    /// <returns>The result of the command execution.</returns>
    public Result ExecuteCommand(GameState gameState, IRoundPhaseCommand command);
  }
}
