using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// <para>
  /// The Game Over round phase represents the end of the game. In this phase, no further
  /// commands can be executed, and the game state is considered final.
  /// </para>
  /// <para>
  /// No possible transitions exist from this phase.
  /// </para>
  /// </summary>
  public class RpGameOver : ARoundPhase
  {
    /// <inheritdoc/>
    public override RoundPhaseType Type => RoundPhaseType.GameOver;

    /// <summary>
    /// Instantiates a new instance of the <see cref="RpGameOver"/> class.
    /// </summary>
    ///
    /// <param name="gameStateService">The game state service.</param>
    /// <param name="houseStateService">The house state service.</param>
    public RpGameOver(
      IGameStateService gameStateService,
      IHouseStateService houseStateService
    ) : base(gameStateService, houseStateService)
    { }

    /// <inheritdoc/>
    protected override Result ExecuteDerived(GameState gameState, IRoundPhaseCommand command)
    {
      return Result.FAILURE("Game is over. No further commands can be executed.");
    }
    
    /// <inheritdoc/>
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      return false;
    }
  }
}
