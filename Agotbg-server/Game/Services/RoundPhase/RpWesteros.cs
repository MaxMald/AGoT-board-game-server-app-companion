using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// <para>
  /// Represents the Westeros phase of the game. During this phase, players resolve the
  /// effects of the Westeros cards and may transition to other sub-phases based on these
  /// events.
  /// </para>
  /// <para>
  /// Possible transitions from this phase include:
  /// <list type="bullet">
  ///   <item><see cref="RoundPhaseType.Planning"/></item>
  ///   <item><see cref="RoundPhaseType.VassalAssignment"/></item>
  ///   <item><see cref="RoundPhaseType.WildlingsBidding"/></item>
  ///   <item><see cref="RoundPhaseType.InfluenceTrackBidding"/></item>
  ///   <item><see cref="RoundPhaseType.FireMadeFlesh"/></item>
  /// </list>
  /// </para>
  /// </summary>
  public class RpWesteros : ARoundPhase
  {
    /// <inheritdoc/>
    public override RoundPhaseType Type => RoundPhaseType.Westeros;

    /// <inheritdoc/>
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      switch (command.Type)
      {
        case RoundPhaseCommandType.Resolve:
          return ExecuteResolve(gameState);
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
        case RoundPhaseCommandType.MoveInfluenceTrackPositionForHouse:
          return RoundPhaseSharedCommandExecutions.ExecuteMoveInfluenceTrackPositionForHouse(
            gameState,
            command
          );
        case RoundPhaseCommandType.StartPreemptiveRaid:
          return ExecuteStartPreemptiveRaid(gameState);
      }
      return Result.FAILURE($"Invalid command type {command.Type} for round phase {Type}");
    }

    /// <inheritdoc/>
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
        case RoundPhaseCommandType.MoveInfluenceTrackPositionForHouse:
        case RoundPhaseCommandType.StartPreemptiveRaid:
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
        case RoundPhaseType.FireMadeFlesh:
          FireMadeFleshStateService.Prepare(gameState.FireMadeFleshState);
          gameState.CurrentPhase = RoundPhaseType.FireMadeFlesh;
          return Result.SUCCESS();

        case RoundPhaseType.WildlingsBidding:
          WildlingsStateServices.PrepareForBidding(gameState.Wildlings, false);
          gameState.CurrentPhase = RoundPhaseType.WildlingsBidding;
          return Result.SUCCESS();

        case RoundPhaseType.InfluenceTrackBidding:
          GameStateService.PrepareForInfluenceTrackBidding(gameState, InfluenceTrackType.IronThrone);
          gameState.CurrentPhase = RoundPhaseType.InfluenceTrackBidding;
          return Result.SUCCESS();
      }
      return Result.FAILURE($"Invalid next round phase {nextPhase} for resolving and moving to another phase.");
    }

    private static Result ExecuteStartPreemptiveRaid(GameState gameState)
    {
      WildlingsStateServices.PrepareForBidding(gameState.Wildlings, true);
      gameState.CurrentPhase = RoundPhaseType.WildlingsBidding;
      return Result.SUCCESS();
    }

    private static Result ExecuteResolve(
      GameState gameState
    )
    {
      if (gameState.Vassals.Count == 0)
      {
        gameState.CurrentPhase = RoundPhaseType.Planning;
      }
      else
      {
        VassalAssignmentStateServices.Prepare(gameState);
        gameState.CurrentPhase = RoundPhaseType.VassalAssignment;
      }
      return Result.SUCCESS();
    }
  }
}
