using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// A static class that contains shared execution logic for round phase commands that
  /// can be used across different round phases.
  /// </summary>
  public static class RoundPhaseSharedCommandExecutions
  {
    public static Result ExecuteModifyPowerTokens(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcModifyPowerTokens modifyPowerTokensCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for modifying power tokens.");

      return GameStateService.ModifyPlayerPowerTokens(
        gameState,
        modifyPowerTokensCommand.PlayerId,
        modifyPowerTokensCommand.Delta
      );
    }

    public static Result ExecuteTransferPowerTokens(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcTransferPowerTokens transferPowerTokensCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for transferring power tokens.");

      return GameStateService.TransferPowerTokens(
        gameState,
        transferPowerTokensCommand.FromPlayerId,
        transferPowerTokensCommand.ToPlayerId,
        transferPowerTokensCommand.Amount
      );
    }

    public static Result ExecuteUpdateSupplyLevel(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcUpdateSupplyLevel updateSupplyLevelCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for updating supply level.");

      return GameStateService.UpdatePlayerSupplyLevel(
        gameState,
        updateSupplyLevelCommand.PlayerId,
        updateSupplyLevelCommand.NewSupplyLevel
      );
    }

    public static Result ExecuteUpdateVassalSupplyLevel(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcUpdateVassalSupplyLevel updateVassalSupplyLevelCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for updating vassal supply level.");

      return GameStateService.UpdateVassalSupplyLevel(
        gameState,
        updateVassalSupplyLevelCommand.VassalHouseType,
        updateVassalSupplyLevelCommand.NewSupplyLevel
      );
    }

    public static Result ExecuteUpdateVictoryPoints(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcUpdateVictoryPoints updateVictoryPointsCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for updating victory points.");

      return GameStateService.UpdatePlayerVictoryPoints(
        gameState,
        updateVictoryPointsCommand.PlayerId,
        updateVictoryPointsCommand.NewVictoryPoints
      );
    }

    public static Result ExecuteUpdatePowerTokensBid(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcUpdatePowerTokensBid updatePowerTokensBidCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for updating power tokens bid.");

      return GameStateService.UpdatePlayerPowerTokensBid(
        gameState,
        updatePowerTokensBidCommand.PlayerId,
        updatePowerTokensBidCommand.NewBid
      );
    }

    public static Result ExecuteMakeVassalStatus(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcMakeVassalageStatus makeVassalageStatusCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for making vassalage status.");

      return GameStateService.MakeVassalageStatus(
        gameState,
        makeVassalageStatusCommand.CommanderPlayerId,
        makeVassalageStatusCommand.VassalHouseType
      );
    }

    public static Result ExecuteBreakVassalStatus(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcBreakVassalageStatus breakVassalageStatusCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for breaking vassalage status.");

      return GameStateService.BreakVassalageStatus(
        gameState,
        breakVassalageStatusCommand.CommanderPlayerId,
        breakVassalageStatusCommand.VassalHouseType
      );
    }

    public static Result ExecuteUpdateIronBankLoanInterest(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcUpdateIronBankLoanInterest updateIronBankLoanInterestCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for updating Iron Bank loan interest.");

      return GameStateService.UpdateIronBankLoanInterest(
        gameState,
        updateIronBankLoanInterestCommand.PlayerId,
        updateIronBankLoanInterestCommand.NewInterest
      );
    }
  }
}
