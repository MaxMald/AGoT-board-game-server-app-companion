using Agotbg.Server.Game.Model;
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
  ///
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
  ///
  /// <para>
  /// Although there is no explicit action during this phase that could lead to a
  /// modification of any of these properties, the app companion let them be modified in
  /// case of special scenarios. For example, if someone forgets to update the supply
  /// level during the Westeros Phase, the app companion will let them fix that.
  /// </para>
  ///
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

    /// <inheritdoc/>
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      switch (command.Type)
      {
        case RoundPhaseCommandType.Resolve:
          gameState.CurrentPhase = RoundPhaseType.Action; // Transition
          return Result.SUCCESS();
        case RoundPhaseCommandType.TransferPowerTokens:
          return RoundPhaseSharedCommandExecutions.ExecuteTransferPowerTokens(
            gameState,
            command
          );
        case RoundPhaseCommandType.ModifyPowerTokens:
          return RoundPhaseSharedCommandExecutions.ExecuteModifyPowerTokens(
            gameState,
            command
          );
        case RoundPhaseCommandType.UpdateSupplyLevel:
          return RoundPhaseSharedCommandExecutions.ExecuteUpdateSupplyLevel(
            gameState,
            command
          );
        case RoundPhaseCommandType.UpdateVictoryPoints:
          return RoundPhaseSharedCommandExecutions.ExecuteUpdateVictoryPoints(
            gameState,
            command
          );
        case RoundPhaseCommandType.UpdateIronBankLoanInterest:
          return RoundPhaseSharedCommandExecutions.ExecuteUpdateIronBankLoanInterest(
            gameState,
            command
          );
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
  }
}
