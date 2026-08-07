using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Represents the vassal assignment phase of the game. In this phase, players take
  /// turns assigning vassal houses to themselves from the available options.
  /// </summary>
  ///
  /// <remarks>
  /// Possible transitions from this phase:
  /// <list type="bullet">
  ///   <item><see cref="RoundPhaseType.Planning"/></item>
  /// </list>
  /// </remarks>
  public class RpVassalAssignment : ARoundPhase
  {
    /// <inheritdoc />
    public override RoundPhaseType Type => RoundPhaseType.VassalAssignment;

    /// <inheritdoc />
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command.Type == RoundPhaseCommandType.AssignVassalHouses)
        return ExecuteAssignVassals(gameState, command);
      if (command.Type == RoundPhaseCommandType.Resolve)
        return ExecuteResolve(gameState);
      else
        return Result.FAILURE($"Invalid command type {command.Type} for round phase {Type}");
    }

    /// <inheritdoc />
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      return RoundPhaseCommandType.AssignVassalHouses == commandType ||
             RoundPhaseCommandType.Resolve == commandType;
    }

    /// <summary>
    /// Resolves the vassal assignment phase. This method sets up the vassalage
    /// relationships based on the selections made by players during the phase.
    /// </summary>
    ///
    /// <remarks>
    /// If any relationship fails to be established, all relationships are cleared and
    /// the vassal assignment phase is reset. If successful, the game state transitions
    /// to the planning phase.
    /// </remarks>
    ///
    /// <param name="gameState">The current game state.</param>
    ///
    /// <returns>A result indicating success or failure of the operation.</returns>
    private static Result ExecuteResolve(GameState gameState)
    {
      VassalAssignmentState vaState = gameState.VassalAssignmentState;
      if (!vaState.IsCompleted)
        return Result.FAILURE($"Cannot resolve vassal assignment phase because it is not completed");

      Result result = SetupVassalageRelationships(gameState);
      if (!result.Success)
      {
        VassalAssignmentStateServices.Clear(vaState);
        VassalAssignmentStateServices.Prepare(gameState);
        return Result.FAILURE(
          $"Failed to setup vassalage status: {result.Message}. The vassal assignment phase has been reset."
        );
      }

      VassalAssignmentStateServices.Clear(vaState);
      gameState.CurrentPhase = RoundPhaseType.Planning;
      return Result.SUCCESS();
    }

    /// <summary>
    /// Moves to the next player in the vassal assignment phase. If the current player is
    /// the last player, it automatically resolves the order token sets for the current
    /// player before moving to the next player.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// 
    /// <returns>A result indicating success or failure of the operation.</returns>
    private static Result MoveToNextPlayer(GameState gameState)
    {
      VassalAssignmentState vaState = gameState.VassalAssignmentState;
      if (VassalAssignmentStateServices.IsLastPlayer(vaState, vaState.CurrentPlayerID))
      {
        VassalAssignmentStateServices.AutomaticallyResolveOrderTokenSetsForCurrentPlayer(
          vaState
        );
      }

      return VassalAssignmentStateServices.MoveToNextPlayer(vaState);
    }

    /// <summary>
    /// Executes the command <see cref="RpcAssignVassalHouses"/> to assign vassal houses
    /// to the current player in the vassal assignment phase.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// <param name="command">The command to execute.</param>
    /// 
    /// <returns>A result indicating success or failure of the operation.</returns>
    private static Result ExecuteAssignVassals(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcAssignVassalHouses assignVassalHousesCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for round phase {RoundPhaseType.VassalAssignment}");

      string playerId = assignVassalHousesCommand.PlayerId;
      VassalAssignmentState vaState = gameState.VassalAssignmentState;

      if (vaState.IsCompleted)
        return Result.FAILURE($"Cannot assign vassals because vassal assignment phase is completed");

      if (playerId != vaState.CurrentPlayerID)
        return Result.FAILURE($"Player {playerId} is not the current player for vassal assignment phase");

      if (assignVassalHousesCommand.HouseTypes.Count == 0)
        return MoveToNextPlayer(gameState);

      Result result = VassalAssignmentStateServices.AssignVassals(
        vaState,
        playerId,
        assignVassalHousesCommand.HouseTypes
      );

      if (!result.Success)
        return result;

      if (vaState.IsCompleted)
        return result;

      return MoveToNextPlayer(gameState);
    }

    /// <summary>
    /// Setups the vassalage relationship of all Houses based on the selections made by
    /// players during the vassal assignment phase. If any relationship fails to be
    /// established, all relationships are cleared and an error result is returned.
    /// </summary>
    ///
    /// <param name="gameState">The current game state.</param>
    ///
    /// <returns>A result indicating success or failure of the operation.</returns>
    private static Result SetupVassalageRelationships(GameState gameState)
    {
      GameStateService.ClearVassalageRelationships(gameState);
      VassalAssignmentState vaState = gameState.VassalAssignmentState;

      foreach (VassalAssignmentPlayer vaPlayer in vaState.Players)
      {
        foreach (VassalHouseSelectionDescriptor vassalHouse in vaPlayer.SelectedVassalHouses)
        {
          Result result = GameStateService.MakeVassalageRelationship(
            gameState,
            vaPlayer.PlayerId,
            vassalHouse.HouseType
          );

          if (!result.Success)
          {
            GameStateService.ClearVassalageRelationships(gameState);
            return result;
          }
        }
      }

      return Result.SUCCESS();
    }
  }
}
