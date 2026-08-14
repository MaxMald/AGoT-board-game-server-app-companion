using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
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

    /// <summary>
    /// Creates a new instance of the <see cref="RpWesteros"/> class.
    /// </summary>
    ///
    /// <param name="gameStateService">The game state service.</param>
    /// <param name="houseStateService">The house state service.</param>
    /// <param name="vassalAssignmentStateService">The vassal assignment state
    /// service.</param>
    /// <param name="influenceTrackBiddingStateService">The influence track bidding state
    /// service.</param>
    /// <param name="influenceTrackService">The influence track service.</param>
    /// <param name="wildlingsStateService">The wildlings state service.</param>
    public RpWesteros(
      IGameStateService gameStateService,
      IHouseStateService houseStateService,
      IVassalAssignmentStateService vassalAssignmentStateService,
      IInfluenceTrackBiddingStateService influenceTrackBiddingStateService,
      IInfluenceTrackService influenceTrackService,
      IWildlingsStateService wildlingsStateService,
      IFireMadeFleshStateService fireMadeFleshStateService
    ) : base(gameStateService, houseStateService)
    {
      m_vassalAssignmentStateService = vassalAssignmentStateService;
      m_influenceTrackBiddingStateService = influenceTrackBiddingStateService;
      m_influenceTrackService = influenceTrackService;
      m_wildlingsStateService = wildlingsStateService;
      m_fireMadeFleshStateService = fireMadeFleshStateService;
    }

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
        case RoundPhaseCommandType.ResolveAndMoveTo:
          return ExecuteResolveAndMoveTo(gameState, command);
        case RoundPhaseCommandType.TransferPowerTokens:
          return ExecuteTransferPowerTokens(
            gameState,
            command
          );
        case RoundPhaseCommandType.ModifyPowerTokens:
          return ExecuteModifyPowerTokens(
            gameState,
            command
          );
        case RoundPhaseCommandType.UpdateSupplyLevel:
          return ExecuteUpdateSupplyLevel(
            gameState,
            command
          );
        case RoundPhaseCommandType.UpdateVictoryPoints:
          return ExecuteUpdateVictoryPoints(
            gameState,
            command
          );
        case RoundPhaseCommandType.UpdateIronBankLoanInterest:
          return ExecuteUpdateIronBankLoanInterest(
            gameState,
            command
          );
        case RoundPhaseCommandType.MoveInfluenceTrackPositionForHouse:
          return ExecuteMoveInfluenceTrackPositionForHouse(
            gameState,
            command,
            m_influenceTrackService
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

    /// <summary>
    /// Reference to the vassal assignment state service.
    /// </summary>
    private IVassalAssignmentStateService m_vassalAssignmentStateService;

    /// <summary>
    /// Reference to the influence track bidding state service.
    /// </summary>
    private IInfluenceTrackBiddingStateService m_influenceTrackBiddingStateService;

    /// <summary>
    /// Reference to the influence track service.
    /// </summary>
    private IInfluenceTrackService m_influenceTrackService;

    /// <summary>
    /// Reference to the Fire Made Flesh state service.
    /// </summary>
    private IFireMadeFleshStateService m_fireMadeFleshStateService;

    /// <summary>
    /// Reference to the wildlings state service.
    /// </summary>
    private IWildlingsStateService m_wildlingsStateService;

    private Result ExecuteResolveAndMoveTo(
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
          m_fireMadeFleshStateService.Prepare(gameState.FireMadeFleshState);
          gameState.CurrentPhase = RoundPhaseType.FireMadeFlesh;
          return Result.SUCCESS();

        case RoundPhaseType.WildlingsBidding:
          m_wildlingsStateService.PrepareForBidding(gameState.Wildlings, false);
          gameState.CurrentPhase = RoundPhaseType.WildlingsBidding;
          return Result.SUCCESS();

        case RoundPhaseType.InfluenceTrackBidding:
          m_gameStateService.PrepareForInfluenceTrackBidding(
            gameState,
            InfluenceTrackType.IronThrone,
            m_influenceTrackBiddingStateService
          );
          gameState.CurrentPhase = RoundPhaseType.InfluenceTrackBidding;
          return Result.SUCCESS();
      }
      return Result.FAILURE($"Invalid next round phase {nextPhase} for resolving and moving to another phase.");
    }

    private Result ExecuteStartPreemptiveRaid(GameState gameState)
    {
      m_wildlingsStateService.PrepareForBidding(gameState.Wildlings, true);
      gameState.CurrentPhase = RoundPhaseType.WildlingsBidding;
      return Result.SUCCESS();
    }

    private Result ExecuteResolve(GameState gameState, IRoundPhaseCommand command)
    {
      if (command is not RpcResolve resolveCommand)
        return Result.FAILURE("Invalid command type for resolving the Westeros phase.");

      if (!m_gameStateService.IsHoster(gameState, resolveCommand.PlayerId))
        return Result.FAILURE("Only the administrator can resolve the Westeros phase.");

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
  }
}
