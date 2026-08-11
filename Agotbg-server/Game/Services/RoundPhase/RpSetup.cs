using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
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
  ///   <item><see cref="RoundPhaseType.VassalAssignment"/></item>
  ///   <item><see cref="RoundPhaseType.Planning"/></item>
  /// </list>
  /// </para>
  /// </summary>
  public class RpSetup : ARoundPhase
  {
    /// <inheritdoc/>
    public override RoundPhaseType Type => RoundPhaseType.Setup;

    /// <summary>
    /// Creates a new instance of the <see cref="RpSetup"/> class.
    /// </summary>
    ///
    /// <param name="gameStateService">The game state service.</param>
    /// <param name="houseStateService">The house state service.</param>
    /// <param name="vassalAssignmentStateService">The vassal assignment state
    /// service.</param>
    public RpSetup(
      IGameStateService gameStateService,
      IHouseStateService houseStateService,
      IVassalAssignmentStateService vassalAssignmentStateService
    ) : base(gameStateService, houseStateService)
    {
      m_vassalAssignmentStateService = vassalAssignmentStateService;
    }

    /// <inheritdoc/>
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (gameState.Vassals.Count == 0)
      {
        gameState.CurrentPhase = RoundPhaseType.Planning;
      }
      else
      {
        m_vassalAssignmentStateService.Prepare(gameState);
        gameState.CurrentPhase = RoundPhaseType.VassalAssignment;
      }
      return Result.SUCCESS();
    }

    /// <inheritdoc/>
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      return commandType == RoundPhaseCommandType.Resolve;
    }

    private IVassalAssignmentStateService m_vassalAssignmentStateService;
  }
}
