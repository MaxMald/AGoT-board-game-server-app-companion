using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// The Action phase is the final phase of a round in the board game (not the app
  /// companion). During this phase, players resolves their orders in the game board.
  /// </summary>
  ///
  /// <remarks>
  /// Many game state operations can be performed during this phase, such as:
  /// <list type="bullet">
  ///   <item><see cref="RoundPhaseCommandType.Resolve"/></item>
  ///   <item><see cref="RoundPhaseCommandType.TransferPowerTokens"/></item>
  ///   <item><see cref="RoundPhaseCommandType.ModifyPowerTokens"/></item>
  ///   <item><see cref="RoundPhaseCommandType.UpdateSupplyLevel"/></item>
  ///   <item><see cref="RoundPhaseCommandType.UpdateVictoryPoints"/></item>
  ///   <item><see cref="RoundPhaseCommandType.UpdateIronBankLoanInterest"/></item>
  ///   <item><see cref="RoundPhaseCommandType.MoveInfluenceTrackPositionForHouse"/></item>
  /// </list>
  /// <para>
  /// When resolving this phase, this phase will check if the current round is the last
  /// round of the game. If it is, it will check if there are any tied players by victory
  /// points. If there are tied players, it will transition to the Tie Resolution phase.
  /// If there are no tied players, it will determine the winner and transition to the
  /// Game Over phase.
  /// </para>
  /// <para>
  /// If it is not the last round, it will increment the current round and transition to
  /// the Westeros phase. Before transitioning, it will check if the Targaryen dragon
  /// strength should be updated (every 2 rounds) and resolve Iron Bank interest payments
  /// for all players. Players that have insufficient power tokens to pay their Iron Bank
  /// interest will be notified.
  /// </para>
  /// <para>
  /// Possible transitions from this phase:
  /// </para>
  /// <list type="bullet">
  ///   <item><see cref="RoundPhaseType.IronBankInterestPaymentResolution"/></item>
  ///   <item><see cref="RoundPhaseType.WesterosWildlingIconsResolution"/></item>
  ///   <item><see cref="RoundPhaseType.WinnerTieResolution"/></item>
  ///   <item><see cref="RoundPhaseType.GameOver"/></item>
  /// </list>
  /// </remarks>
  public class RpAction : ARoundPhase
  {
    /// <inheritdoc/>
    public override RoundPhaseType Type => RoundPhaseType.Action;

    /// <summary>
    /// Instantiates a action round phase..
    /// </summary>
    ///
    /// <param name="gameStateService">The game state service.</param>
    /// <param name="houseStateService">The house state service.</param>
    /// <param name="dragonTokensStateService">The dragon tokens state service.</param>
    /// <param name="influenceTrackService">The influence track service.</param>
    /// <param name="ironBankInterestPaymentStateService">The Iron Bank interest payment
    /// state service.</param>
    public RpAction(
      IGameStateService gameStateService,
      IHouseStateService houseStateService,
      IDragonTokensStateService dragonTokensStateService,
      IInfluenceTrackService influenceTrackService,
      IIronBankInterestPaymentStateService ironBankInterestPaymentStateService
    ) : base(gameStateService, houseStateService)
    {
      DragonTokenStateService = dragonTokensStateService;
      InfluenceTrackService = influenceTrackService;
      IronBankInterestPaymentStateService = ironBankInterestPaymentStateService;
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
            InfluenceTrackService
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
        case RoundPhaseCommandType.MoveInfluenceTrackPositionForHouse:
          return true;
      }
      return false;
    }

    private IDragonTokensStateService DragonTokenStateService { get; }
    private IInfluenceTrackService InfluenceTrackService { get; }
    private IIronBankInterestPaymentStateService IronBankInterestPaymentStateService { get; }

    private Result ExecuteResolve(
      GameState gameState,
      IRoundPhaseCommand roundPhaseCommand
    )
    {
      if (roundPhaseCommand is not RpcResolve resolveCommand)
        return Result.FAILURE($"Invalid command type {roundPhaseCommand.Type} for round phase {Type}");

      if (!m_gameStateService.IsHoster(gameState, resolveCommand.PlayerId))
        return Result.FAILURE("Only the administrator can resolve the action phase.");

      List<PlayerState> playerStates = gameState.Players.Values.ToList();

      if (m_gameStateService.IsLastRound(gameState))
      {
        if (m_gameStateService.HasTiedPlayersByVictoryPoints(gameState))
        {
          gameState.CurrentPhase = RoundPhaseType.WinnerTieResolution; // Transition to Tie Resolution phase if there are tied players
          return Result.SUCCESS();
        }
        else
        {
          PlayerState? winner = GetPlayerWithHighestVictoryPoints(playerStates);
          if (winner == null)
            return Result.FAILURE("No players found to determine the winner.");

          gameState.Winner = winner.HouseState.Type; // TODO: Event
          gameState.CurrentPhase = RoundPhaseType.GameOver; // Transition to Game Over phase if there are no tied players
          return Result.SUCCESS();
        }
      }

      byte nextRound = (byte)(gameState.CurrentRound + 1);

      if (HasTargaryenPlayer(playerStates))
      {
        DragonTokenStateService.PrepareForNextRound(
          gameState.DragonTokensState,
          nextRound
        );
      }

      IronBankInterestPaymentState ironBankState = gameState.IronBankLoanInterestState;
      ResolveIronBankInterestPayment(ironBankState, playerStates);

      gameState.CurrentRound = nextRound;
      if (IronBankInterestPaymentStateService.HasAnyResolvedInterestPayment(ironBankState))
      {
        gameState.CurrentPhase = RoundPhaseType.IronBankInterestPaymentResolution;
      }
      else
      {
        IronBankInterestPaymentStateService.Clear(ironBankState);
        gameState.CurrentPhase = RoundPhaseType.WesterosWildlingIconsResolution;
      }
      
      return Result.SUCCESS();
    }

    private static PlayerState? GetPlayerWithHighestVictoryPoints(List<PlayerState> players)
    {
      return players.OrderByDescending(p => p.HouseState.VictoryPoints)
                    .FirstOrDefault();
    }

    private static bool HasTargaryenPlayer(List<PlayerState> playerStates)
    {
      return playerStates.Any(p => p.HouseState.Type == HouseType.Targaryen);
    }

    private void ResolveIronBankInterestPayment(
      IronBankInterestPaymentState state,
      List<PlayerState> players
    )
    {
      foreach (PlayerState playerState in players)
      {
        if (playerState.HouseState.IronBankLoanInterest == 0)
          continue;

        IronBankInterestPaymentStateService.ResolvePlayerInterestPayment(
          state,
          playerState
        );
      }
    }
  }
}
