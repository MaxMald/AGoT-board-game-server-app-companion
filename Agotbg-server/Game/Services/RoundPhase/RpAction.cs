using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// <para>
  /// The Action phase is the final phase of a round in the board game (not the app
  /// companion). During this phase, players resolves their orders in the following
  /// order: Raid, March and Consolidate Power. The app companion does not have
  /// information about the orders of each player and does not care about it, however
  /// many game state operations can be performed during this phase, such as:
  /// </para>
  /// <list type="bullet">
  ///   <item>Transferring power tokens</item>
  ///   <item>Modifying power tokens</item>
  ///   <item>Updating supply levels</item>
  ///   <item>Updating victory points</item>
  ///   <item>Updating Iron Bank loan interest</item>
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
  ///   <item>Westeros</item>
  ///   <item>Tie Resolution</item>
  ///   <item>Game Over</item>
  /// </list>
  /// </summary>
  public class RpAction : ARoundPhase
  {
    /// <inheritdoc/>
    public override RoundPhaseType Type => RoundPhaseType.Action;

    /// <inheritdoc/>
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      switch (command.Type)
      {
        case RoundPhaseCommandType.Resolve:
          return ResolveActionPhase(gameState);
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

    private Result ResolveActionPhase(GameState gameState)
    {
      List<PlayerState> playerStates = gameState.Players.Values.ToList();

      if (GameStateService.IsLastRound(gameState))
      {
        if (GameStateService.HasTiedPlayersByVictoryPoints(gameState))
        {
          gameState.CurrentPhase = RoundPhaseType.TieResolution; // Transition to Tie Resolution phase if there are tied players
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

      if (ShouldUpdateTargaryenDragonStrength(nextRound))
        UpdateTargaryenDragonStrength(playerStates, nextRound);

      ResolveIronBankInterestPayment(playerStates);

      gameState.CurrentRound = nextRound; // TODO: Event
      gameState.CurrentPhase = RoundPhaseType.Westeros; // Transition
      return Result.SUCCESS();
    }

    private static PlayerState? GetPlayerWithHighestVictoryPoints(List<PlayerState> players)
    {
      return players
        .OrderByDescending(p => p.HouseState.VictoryPoints)
        .FirstOrDefault();
    }

    private static bool ShouldUpdateTargaryenDragonStrength(byte nextRound)
    {
      return nextRound % 2 == 0;
    }

    private static void UpdateTargaryenDragonStrength(List<PlayerState> players, byte nextRound)
    {
      foreach (PlayerState house in players)
      {
        if (house.HouseState.Type == HouseType.Targaryen)
        {
          // TODO: Event
          house.HouseState.DragonStrength = (byte)(nextRound / 2);
          return;
        }
      }
    }

    private static void ResolveIronBankInterestPayment(List<PlayerState> players)
    {
      foreach (PlayerState house in players)
      {
        byte interest = house.HouseState.IronBankLoanInterest;
        if (interest == 0)
          continue;

        byte housePowerTokens = house.HouseState.PowerTokens;
        if (housePowerTokens < interest)
        {
          house.HouseState.PowerTokens = 0;

          byte remainingInterest = (byte)(interest - housePowerTokens);
          // TODO: Event this player has defaulted on their Iron Bank loan
        }
        else
        {
          house.HouseState.PowerTokens -= interest;
          // TODO: Event this player has paid their Iron Bank loan interest
        }
      }
    }
  }
}
