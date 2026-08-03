using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Base class for round phases that validates and executes commands specific to each
  /// phase type.
  /// </summary>
  public abstract class ARoundPhase : IRoundPhase
  {
    /// <inheritdoc/>
    public abstract RoundPhaseType Type { get; }

    /// <summary>
    /// Executes the given command for the current round phase, if the command type is
    /// valid for this phase.
    /// </summary>
    ///
    /// <param name="gameState">The current state of the game.</param>
    /// <param name="command">The command to execute.</param>
    /// 
    /// <returns>The result of executing the command.</returns>
    public Result Execute(GameState gameState, IRoundPhaseCommand command)
    {
      if (!IsValidCommandType(command.Type))
      {
        return Result.FAILURE(
          $"Invalid command type {command.Type} for round phase {Type}."
        );
      }

      return ExecuteDerived(gameState, command);
    }

    /// <summary>
    /// Executes the command with derived class-specific logic.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// <param name="command">The round phase command to execute.</param>
    /// 
    /// <returns>The result of the command execution.</returns>
    protected abstract Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    );

    /// <summary>
    /// Determines whether the specified command type is valid for this round phase.
    /// </summary>
    ///
    /// <param name="commandType">The command type to validate.</param>
    /// 
    /// <returns>true if the command type is valid; otherwise, false.</returns>
    protected abstract bool IsValidCommandType(RoundPhaseCommandType commandType);
  }
}
