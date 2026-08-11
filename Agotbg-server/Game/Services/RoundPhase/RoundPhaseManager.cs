using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Manages the execution of game round phases by routing commands to the appropriate
  /// phase handler based on the current game state.
  /// </summary>
  public class RoundPhaseManager : IRoundPhaseManager
  {
    /// <inheritdoc/>
    public void RegisterRoundPhase(IRoundPhase roundPhase)
    {
      m_roundPhases[roundPhase.Type] = roundPhase;
    }

    /// <inheritdoc/>
    public Result ExecuteCommand(GameState gameState, IRoundPhaseCommand command)
    {
      try
      {
        RoundPhaseType currentRoundPhaseType = gameState.CurrentPhase;
        if (!m_roundPhases.TryGetValue(currentRoundPhaseType, out IRoundPhase? roundPhase))
          return Result.FAILURE($"No round phase found for type {currentRoundPhaseType}");

        return roundPhase.Execute(gameState, command);
      }
      catch (Exception e)
      {
        return Result.FAILURE($"Error executing command: {e.Message}");
      }
    }

    /// <summary>
    /// Map of all available round phases, keyed by their corresponding <see
    /// cref="RoundPhaseType"/>.
    /// </summary>
    private readonly Dictionary<RoundPhaseType, IRoundPhase> m_roundPhases = [];
  }
}
