using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// <para>
  /// Represents the setup phase of the game. During this phase players prepare the game
  /// board for the upcoming rounds.
  /// </para>
  /// <para>
  /// Possible transitions from this phase include:
  /// <list type="bullet">
  ///   <item>Planning</item>
  /// </list>
  /// </para>
  /// </summary>
  public class RpSetup : ARoundPhase
  {
    /// <inheritdoc/>
    public override RoundPhaseType Type => RoundPhaseType.Setup;

    /// <inheritdoc/>
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      gameState.CurrentPhase = RoundPhaseType.Planning; // Transtion
      return Result.SUCCESS();
    }

    /// <inheritdoc/>
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      return commandType == RoundPhaseCommandType.Resolve;
    }
  }
}
