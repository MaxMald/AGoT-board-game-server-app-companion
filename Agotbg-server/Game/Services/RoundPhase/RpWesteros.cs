using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  public class RpWesteros : ARoundPhase
  {
    /// <inheritdoc/>
    public override RoundPhaseType Type => RoundPhaseType.Westeros;

    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      switch (command.Type)
      {
        case RoundPhaseCommandType.Resolve:
          gameState.CurrentPhase = RoundPhaseType.Planning;
          return Result.SUCCESS();
        case RoundPhaseCommandType.ResolveAndMoveTo:
          return ExecuteResolveAndMoveTo(gameState, command);
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

        // TODO: Modify Wildings Strength Command
      }
      return Result.FAILURE($"Invalid command type {command.Type} for round phase {Type}");
    }

    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      switch (commandType)
      {
        case RoundPhaseCommandType.Resolve:
        case RoundPhaseCommandType.ResolveAndMoveTo:
        case RoundPhaseCommandType.TransferPowerTokens:
        case RoundPhaseCommandType.ModifyPowerTokens:
        case RoundPhaseCommandType.UpdateSupplyLevel:
        case RoundPhaseCommandType.UpdateVictoryPoints:
        case RoundPhaseCommandType.UpdateIronBankLoanInterest:
        // TODO: Modify Wildings Strength Command
          return true;
      }
      return false;
    }

    private static Result ExecuteResolveAndMoveTo(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      RpcResolveAndMoveTo? resolveAndMoveToCommand = command as RpcResolveAndMoveTo;
      if (resolveAndMoveToCommand == null)
        return Result.FAILURE("Invalid command type for resolving and moving to another phase.");

      RoundPhaseType nextPhase = resolveAndMoveToCommand.NextRoundPhase;
      switch (nextPhase)
      {
        case RoundPhaseType.Planning:
          gameState.CurrentPhase = RoundPhaseType.Planning;
          return Result.SUCCESS();
        case RoundPhaseType.WildlingsBidding:
          gameState.CurrentPhase = RoundPhaseType.WildlingsBidding;
          return Result.SUCCESS();
        case RoundPhaseType.IronThroneBidding:
          gameState.CurrentPhase = RoundPhaseType.IronThroneBidding;
          return Result.SUCCESS();
      }
      return Result.FAILURE($"Invalid next round phase {nextPhase} for resolving and moving to another phase.");
    }
  }
}
