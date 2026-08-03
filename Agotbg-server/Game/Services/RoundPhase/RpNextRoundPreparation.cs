using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// <para>
  /// Represents a phase for preparing the next round, which includes updating the
  /// Targaryen dragon strength and resolving Iron Bank interest payments.
  /// </para>
  ///
  /// <para>
  /// Possible transitions from this phase:
  /// <list type="bullet">
  ///   <item>Westeros</item>
  /// </list>
  /// </para>
  /// </summary>
  public class RpNextRoundPreparation : ARoundPhase
  {
    public override RoundPhaseType Type => RoundPhaseType.NextRoundPreparation;

    /// <inheritdoc />
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      byte nextRound = (byte)(gameState.CurrentRound + 1);
      if (nextRound > GameConstants.NumRounds)
        return Result.FAILURE($"Cannot advance to round {nextRound} because it exceeds the maximum number of rounds ({GameConstants.NumRounds}).");

      List<PlayerState> playerStates = gameState.Players.Values.ToList();

      if (ShouldUpdateTargaryenDragonStrength(nextRound))
        UpdateTargaryenDragonStrength(playerStates, nextRound);

      ResolveIronBankInterestPayment(playerStates);

      gameState.CurrentRound = nextRound; // TODO: Event
      gameState.CurrentPhase = RoundPhaseType.Westeros; // Transition
      return Result.SUCCESS();
    }

    /// <inheritdoc />
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      return commandType == RoundPhaseCommandType.Resolve;
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
