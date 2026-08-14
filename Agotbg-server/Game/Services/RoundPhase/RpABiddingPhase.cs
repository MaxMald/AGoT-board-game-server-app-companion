using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
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
  ///
  /// <remarks>
  /// Provides the following utilities methods for derived classes:
  /// <list type="bullet">
  ///   <item><see cref="ResolveHousesBets(GameState, List{HouseState}, List{HouseBet})"/></item>
  /// </list>
  /// </remarks>
  public abstract class RpABiddingPhase : ARoundPhase
  {
    /// <summary>
    /// Instantiates a new instance of the <see cref="RpABiddingPhase"/> class.
    /// </summary>
    ///
    /// <param name="gameStateService">The game state service.</param>
    /// <param name="houseStateService">The house state service.</param>
    protected RpABiddingPhase(
      IGameStateService gameStateService,
      IHouseStateService houseStateService
    ) : base(gameStateService, houseStateService)
    {
      m_houseStateService = houseStateService;
    }

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
          return ExecuteResolve(gameState, command);
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
    /// called after all players have submitted their bids and is safe to process the
    /// output of the resolved house bets.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    /// At the moment this method is called, all house bids have been resolved and the
    /// <paramref name="houseBets"/> parameter contains the list of resolved house bets.
    /// That is, the <see cref="HouseState.HasBidPowerTokens"/> property for all houses
    /// is false, the <see cref="HouseState.PowerTokensBid"/> property for all houses is
    /// zero and the <see cref="HouseState.PowerTokens"/> property for all houses has
    /// been updated to reflect the resolved bids.
    /// </para>
    /// <para>
    /// Take into account that only houses that had submitted a bid will be present in
    /// the list of house bets.
    /// </para>
    /// </remarks>
    ///
    /// <param name="gameState">The game state.</param>
    /// <param name="houseBets">The list of resolved house bets.</param>
    ///
    /// <returns>The result of the bid resolution.</returns>
    protected abstract Result ExecuteDerivedBidResolution(
      GameState gameState,
      List<HouseBet> houseBets
    );

    private Result ExecuteUpdatePowerTokensBid(
     GameState gameState,
     IRoundPhaseCommand command
   )
    {
      if (command is not RpcUpdatePowerTokensBid updatePowerTokensBidCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for updating power tokens bid.");

      PlayerState? playerState = m_gameStateService.GetPlayerState(
        gameState,
        updatePowerTokensBidCommand.PlayerId
      );

      if (playerState == null)
        return Result.FAILURE($"Player with ID {updatePowerTokensBidCommand.PlayerId} does not exist.");

      return m_houseStateService.SubmitPowerTokensBid(
        playerState.HouseState,
        updatePowerTokensBidCommand.NewBid
      );
    }

    private Result ExecuteCancelPowerTokensBid(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcCancelPowerTokensBid cancelPowerTokensCommand)
        return Result.FAILURE("Invalid command type for canceling power tokens bid.");

      if (!gameState.Players.ContainsKey(cancelPowerTokensCommand.PlayerId))
        return Result.FAILURE($"Player with ID {cancelPowerTokensCommand.PlayerId} does not exist.");

      HouseState house = gameState.Players[cancelPowerTokensCommand.PlayerId].HouseState;
      m_houseStateService.CancelPowerTokensBid(house);

      return Result.SUCCESS();
    }

    private Result ExecuteResolve(
      GameState gameState,
      IRoundPhaseCommand roundPhaseCommand
    )
    {
      if (roundPhaseCommand is not RpcResolve resolveCommand)
        return Result.FAILURE("Invalid command type for resolving bids.");

      if (!m_gameStateService.IsHoster(gameState, resolveCommand.PlayerId))
        return Result.FAILURE("Only the hoster can resolve bids.");

      if (!m_gameStateService.HaveAllPlayersSubmittedTheirBids(gameState))
        return Result.FAILURE("Not all players have submitted their bids.");

      List<HouseBet> housesBets = m_gameStateService.CreateHouseBets(gameState);
      m_gameStateService.ClearAllHousesSubmittedBids(gameState);

      return ExecuteDerivedBidResolution(gameState, housesBets);
    }
  }
}
