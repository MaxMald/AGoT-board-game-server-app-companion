using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// <para>
  /// Represents the Planning round phase in the game. During this phase, players
  /// secretly issue orders on the board. The game state should stay on this phase until
  /// all players finish placing orders.
  /// </para>
  /// <para>
  /// This phase allows players to perform some operations on the game state, such as:
  /// <list type="bullet">
  /// <item>
  /// <description>Transferring power tokens</description>
  /// </item>
  /// <item>
  /// <description>Modifying power tokens</description>
  /// </item>
  /// <item>
  /// <description>Updating supply levels</description>
  /// </item>
  /// <item>
  /// <description>Updating victory points</description>
  /// </item>
  /// <item>
  /// <description>Updating Iron Bank loan interest</description>
  /// </item>
  /// </list>
  /// </para>
  /// <para>
  /// Although there is no explicit action during this phase that could lead to a
  /// modification of any of these properties, the app companion let them be modified in
  /// case of special scenarios. For example, if someone forgets to update the supply
  /// level during the Westeros Phase, the app companion will let them fix that.
  /// </para>
  /// <para>
  /// Possible transitions from this phase:
  /// <list type="bullet">
  ///   <item>Action</item>
  /// </list>
  /// </para>
  /// </summary>
  public class RpPlanning : ARoundPhase
  {
    /// <inheritdoc/>
    public override RoundPhaseType Type => RoundPhaseType.Planning;

    /// <summary>
    /// Intantiates a new instance of the <see cref="RpPlanning"/> class.
    /// </summary>
    ///
    /// <param name="gameStateService">The game state service.</param>
    /// <param name="houseStateService">The house state service.</param>
    public RpPlanning(
      IGameStateService gameStateService,
      IHouseStateService houseStateService
    ) : base(gameStateService, houseStateService)
    { }

    /// <inheritdoc/>
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      switch (command.Type)
      {
        case RoundPhaseCommandType.Resolve:
          return ExecuteResolve(gameState, command);
        case RoundPhaseCommandType.TransferPowerTokens:
          return ExecuteTransferPowerTokens(gameState, command);
        case RoundPhaseCommandType.ModifyPowerTokens:
          return ExecuteModifyPowerTokens(gameState, command);
        case RoundPhaseCommandType.UpdateSupplyLevel:
          return ExecuteUpdateSupplyLevel(gameState, command);
        case RoundPhaseCommandType.UpdateVictoryPoints:
          return ExecuteUpdateVictoryPoints(gameState, command);
        case RoundPhaseCommandType.UpdateIronBankLoanInterest:
          return ExecuteUpdateIronBankLoanInterest(gameState, command);
      }
      return Result.FAILURE($"Invalid command type {command.Type} for round phase {Type}");
    }

    /// <inheritdoc/>
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      switch (commandType)
      {
        case RoundPhaseCommandType.Resolve:
        case RoundPhaseCommandType.TransferPowerTokens:
        case RoundPhaseCommandType.ModifyPowerTokens:
        case RoundPhaseCommandType.UpdateSupplyLevel:
        case RoundPhaseCommandType.UpdateVictoryPoints:
        case RoundPhaseCommandType.UpdateIronBankLoanInterest:
          return true;
      }
      return false;
    }

    private Result ExecuteResolve(GameState gameState, IRoundPhaseCommand roundPhaseCommand)
    {
      if (roundPhaseCommand is not RpcResolve resolveCommand)
        return Result.FAILURE($"Invalid command type {roundPhaseCommand.Type} for round phase {Type}");

      if (!m_gameStateService.IsHoster(gameState, resolveCommand.PlayerId))
        return Result.FAILURE("Only the administrator can resolve the planning phase.");

      gameState.CurrentPhase = RoundPhaseType.Action;
      return Result.SUCCESS();
    }
  }
}
