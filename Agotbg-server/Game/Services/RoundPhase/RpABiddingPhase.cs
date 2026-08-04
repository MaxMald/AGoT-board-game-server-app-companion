using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// <para>
  /// Abstract base class for round phases that involve bidding with power tokens. This
  /// class provides common functionality for handling power token bids, including
  /// updating, canceling, and resolving bids.
  /// </para>
  /// <para>
  /// Derived classes must implement the specific bid resolution logic through the <see
  /// cref="ExecuteDerivedBidResolution(GameState)"/> method, which is called after all
  /// players have submitted their bets and the <see
  /// cref="RoundPhaseCommandType.Resolve"/> command type is received.
  /// </para>
  /// <para>
  /// Possible commands that can be executed in this phase include:
  /// <list type="bullet">
  ///   <item><see cref="RoundPhaseCommandType.UpdatePowerTokensBid"/></item>
  ///   <item><see cref="RoundPhaseCommandType.CancelPowerTokensBid"/></item>
  ///   <item><see cref="RoundPhaseCommandType.Resolve"/></item>
  /// </list>
  /// </para>
  /// <para>
  /// This class does not make any transitions on its own; derived classes are
  /// responsible for determining the next phase.
  /// </para>
  /// </summary>
  public abstract class RpABiddingPhase : ARoundPhase
  {
    /// <inheritdoc/>
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      switch (command.Type)
      {
        case RoundPhaseCommandType.UpdatePowerTokensBid:
          return ExecuteUpdatePowerTokensBid(gameState, command);
        case RoundPhaseCommandType.CancelPowerTokensBid:
          return ExecuteCancelPowerTokensBid(gameState, command);
        case RoundPhaseCommandType.Resolve:
          return ExecuteResolve(gameState);
        default:
          return Result.FAILURE($"Invalid command type: {command.Type}");
      }
    }

    /// <inheritdoc/>
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      return commandType == RoundPhaseCommandType.UpdatePowerTokensBid ||
             commandType == RoundPhaseCommandType.CancelPowerTokensBid ||
             commandType == RoundPhaseCommandType.Resolve;
    }

    /// <summary>
    /// Executes the bid resolution logic specific to the derived class. This method is
    /// called after all players have submitted their bids and the bids are ready to be
    /// resolved.
    /// </summary>
    /// 
    /// <param name="gameState">The game state.</param>
    /// 
    /// <returns>The result of the bid resolution.</returns>
    protected abstract Result ExecuteDerivedBidResolution(GameState gameState);

    private static Result ExecuteUpdatePowerTokensBid(
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

    private static Result ExecuteCancelPowerTokensBid(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcCancelPowerTokensBid cancelPowerTokensCommand)
        return Result.FAILURE("Invalid command type for canceling power tokens bid.");

      if (!gameState.Players.ContainsKey(cancelPowerTokensCommand.PlayerId))
        return Result.FAILURE($"Player with ID {cancelPowerTokensCommand.PlayerId} does not exist.");

      HouseState house = gameState.Players[cancelPowerTokensCommand.PlayerId].HouseState;
      HouseStateService.CancelPowerTokensBid(house);

      return Result.SUCCESS();
    }

    private Result ExecuteResolve(GameState gameState)
    {
      if (!AllPlayersHavePlacedTheirBets(gameState))
        return Result.FAILURE("Not all players have placed their bets.");
      return ExecuteDerivedBidResolution(gameState);
    }

    private static bool AllPlayersHavePlacedTheirBets(GameState gameState)
    {
      foreach (var player in gameState.Players)
      {
        if (!player.Value.HouseState.HasBidPowerTokens)
          return false;
      }
      return true;
    }
  }
}
