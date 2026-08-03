using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// <para>
  /// Represents a phase for tie resolution, which determines the game winner when
  /// multiple houses are tied with the highest victory points.
  /// </para>
  ///
  /// <para>
  /// The tie could be broken by the number of land areas controlled. Since the companion
  /// app does not have information about the number of land areas controlled, players
  /// could tell the app who the winner is by using the "Resolve with Winner" command.
  /// </para>
  ///
  /// <para>
  /// If the tie cannot be resolved by the number of land areas controlled, the tie could
  /// be broken by the supply level or Iron Throne position of the tied houses. Players
  /// can use the "Resolve Tie by Supply Level or Iron Throne Position" command to
  /// indicate that they are ready to proceed after resolving the tie using these
  /// criterias.
  /// </para>
  ///
  /// The priority of tie-breaking criteria is as follows:
  ///
  /// <list type="number">
  ///   <item>Most total land areas controlled (not implemented in the app)</item>
  ///   <item>Highest supply level</item>
  ///   <item>Lowest Iron Throne position</item>
  /// </list>
  ///
  /// <para>
  /// Based on the Fantasy Flight Errata and FAQ Version 2.0 document: <see
  /// href="https://images-cdn.fantasyflightgames.com/filer_public/cf/06/cf06eb26-48e3-46b9-b57c-f053beb2518d/agotbg_faq_v2_forweb.pdf"/>
  /// </para>
  ///
  /// <para>
  /// Possible transitions from this phase:
  /// <list type="bullet">
  ///   <item>GameOver</item>
  /// </list>
  /// </para>
  /// </summary>
  public class RpTieResolution : ARoundPhase
  {
    /// <inheritdoc/>
    public override RoundPhaseType Type => RoundPhaseType.TieResolution;

    /// <inheritdoc/>
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command.Type == RoundPhaseCommandType.ResolveTieBySupplyLevelOrIronThronePosition)
      {
        RpResolveTieBySupplyLevelOrIronThronePosition? tieResolutionCommand
          = command as RpResolveTieBySupplyLevelOrIronThronePosition;

        if (tieResolutionCommand == null)
          return Result.FAILURE("Invalid command type for tie resolution by area.");

        return ExecuteResolveTieBySupplyLevelOrIronThronePosition(
          gameState,
          tieResolutionCommand
        );
      }
      else if (command.Type == RoundPhaseCommandType.ResolveWithWinner)
      {
        RpResolveWithWinner? resolveWithWinnerCommand = command as RpResolveWithWinner;

        if (resolveWithWinnerCommand == null)
          return Result.FAILURE("Invalid command type for resolving with a winner.");

        return ExecuteResolveWithWinner(gameState, resolveWithWinnerCommand);
      }

      return Result.FAILURE("Unsupported command type for tie resolution by area.");
    }

    /// <inheritdoc/>
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      return commandType == RoundPhaseCommandType.ResolveTieBySupplyLevelOrIronThronePosition
        || commandType == RoundPhaseCommandType.ResolveWithWinner;
    }

    private static Result ExecuteResolveTieBySupplyLevelOrIronThronePosition(
      GameState gameState,
      RpResolveTieBySupplyLevelOrIronThronePosition command
    )
    {
      List<HouseState> tiedHouses = GetPlayerHousesWithHightestNumberOfVictoryPoints(gameState);
      if (tiedHouses.Count == 0)
      {
        return Result.FAILURE("No players found with the highest number of victory points.");
      }

      if (CanResolveTieBySupplyLevel(tiedHouses))
      {
        HouseState winnerBySupplyLevel = ResolveTieBySupplyLevel(tiedHouses);

        gameState.Winner = winnerBySupplyLevel.Type; // TODO: Event
        gameState.CurrentPhase = RoundPhaseType.GameOver; // Transition

        return Result.SUCCESS();
      }

      HouseState winnerByIronThronePosition = ResolveTieByIronThronePosition(tiedHouses);

      gameState.Winner = winnerByIronThronePosition.Type; // TODO: Event
      gameState.CurrentPhase = RoundPhaseType.GameOver; // Transition

      return Result.SUCCESS();
    }

    private static Result ExecuteResolveWithWinner(
      GameState gameState,
      RpResolveWithWinner command
    )
    {
      if (!gameState.Players.ContainsKey(command.WinnerPlayerId))
      {
        return Result.FAILURE($"Player with ID {command.WinnerPlayerId} does not exist.");
      }

      PlayerState winnerPlayerState = gameState.Players[command.WinnerPlayerId];

      gameState.Winner = winnerPlayerState.HouseState.Type; // TODO: Event
      gameState.CurrentPhase = RoundPhaseType.GameOver; // Transition

      return Result.SUCCESS();
    }

    private static List<HouseState> GetPlayerHousesWithHightestNumberOfVictoryPoints(GameState gameState)
    {
      int highestVictoryPoints = gameState.Players
                                          .Values
                                          .Max(player => player.HouseState.VictoryPoints);

      return gameState.Players
                      .Values
                      .Where(player => player.HouseState.VictoryPoints == highestVictoryPoints)
                      .Select(player => player.HouseState)
                      .ToList();
    }

    private static bool CanResolveTieBySupplyLevel(List<HouseState> tiedHouses)
    {
      int highestSupplyLevel = tiedHouses.Max(house => house.SupplyLevel);

      List<HouseState> housesWithHighestSupplyLevel = tiedHouses
        .Where(house => house.SupplyLevel == highestSupplyLevel)
        .ToList();

      return housesWithHighestSupplyLevel.Count == 1;
    }

    private static HouseState ResolveTieBySupplyLevel(List<HouseState> tiedHouses)
    {
      int highestSupplyLevel = tiedHouses.Max(house => house.SupplyLevel);
      return tiedHouses
        .First(house => house.SupplyLevel == highestSupplyLevel);
    }

    private static HouseState ResolveTieByIronThronePosition(List<HouseState> tiedHouses)
    {
      int highestIronThronePosition = tiedHouses.Min(house => house.IronThroneTrackPosition);
      return tiedHouses
        .First(house => house.IronThroneTrackPosition == highestIronThronePosition);
    }
  }
}
