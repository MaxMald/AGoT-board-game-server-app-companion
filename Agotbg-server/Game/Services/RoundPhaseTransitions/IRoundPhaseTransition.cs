using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhaseTransitions
{
  /// <summary>
  /// Defines a contract for transitioning between different phases of a game round.
  /// </summary>
  public interface IRoundPhaseTransition
  {
    public RoundPhaseType To { get; }

    /// <summary>
    /// Executes the transition from the current phase to the next phase, modifying the
    /// provided GameState accordingly.
    /// </summary>
    ///
    /// <param name="state">The current state of the game to be modified.</param>
    ///
    /// <returns>A Result indicating the success or failure of the transition.</returns>
    public Result Execute(GameState state);
  }
}
