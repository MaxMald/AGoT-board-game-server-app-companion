using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhaseTransitions
{
  /// <summary>
  /// Transtion from Action to End Of Round phase.
  ///
  /// This transtion is responsible for:
  ///
  /// * Handling the increment of the round number.
  /// * Checking if the game has reached its final round.
  /// * Set the game state to over the transition correspond to the last round.
  /// * Determining the winner based on victory points and Iron Throne track position.
  /// * Updating the Targaryen dragon strength at specified rounds.
  /// </summary>
  public class RPTActionToEndOfRound : IRoundPhaseTransition
  {
    public RoundPhaseType To => RoundPhaseType.EndOfRound;

    public Result Execute(GameState state)
    {
      byte nextRound = (byte)(state.CurrentRound + 1);
      if (nextRound > GameConstants.NumRounds)
      {
        SelectWinner(state);

        state.IsGameOver = true;
        state.CurrentPhase = To;
        return Result.SUCCESS();
      }

      if (ShouldUpdateTargaryenDragonStrength(nextRound))
        UpdateTargaryenDragonStrength(state, nextRound);

      state.CurrentPhase = To;
      state.CurrentRound = nextRound;
      return Result.SUCCESS();
    }

    private static void SelectWinner(GameState state)
    {
      List<PlayerState> validPlayerHouses = new List<PlayerState>();
      foreach (PlayerState playerHouse in state.Players.Values)
      {
        if (playerHouse.HouseState.Type == HouseType.Targaryen)
          continue; // Skip Targaryen for victory point comparison
        validPlayerHouses.Add(playerHouse);
      }

      if (validPlayerHouses.Count == 0)
      {
        state.Winner = null;
        return;
      }

      if (validPlayerHouses.Count == 1)
      {
        state.Winner = validPlayerHouses[0].HouseState.Type;
        return;
      }

      validPlayerHouses.Sort((a, b) => b.HouseState.VictoryPoints.CompareTo(a.HouseState.VictoryPoints));

      List<PlayerState> tiedPlayers = new List<PlayerState>();
      byte highestVictoryPoints = validPlayerHouses[0].HouseState.VictoryPoints;
      for (int i = 0; i < validPlayerHouses.Count; i++)
      {
        if (validPlayerHouses[i].HouseState.VictoryPoints == highestVictoryPoints)
          tiedPlayers.Add(validPlayerHouses[i]);
        else
          break;
      }

      if (tiedPlayers.Count == 1)
      {
        state.Winner = tiedPlayers[0].HouseState.Type;
      }
      else
      {
        tiedPlayers.Sort((a, b) => a.HouseState.IronThroneTrackPosition
                                    .CompareTo(b.HouseState.IronThroneTrackPosition));
        state.Winner = tiedPlayers[0].HouseState.Type;
      }
    }

    private static bool ShouldUpdateTargaryenDragonStrength(byte nextRound)
    {
      return nextRound > 0 && nextRound % 2 == 0;
    }

    private static void UpdateTargaryenDragonStrength(GameState state, byte nextRound)
    {
      foreach (PlayerState house in state.Players.Values)
      {
        if (house.HouseState.Type == HouseType.Targaryen)
        {
          house.HouseState.DragonStrength = CalculateDragonStrength(nextRound);
          return;
        }
      }
    }

    private static byte CalculateDragonStrength(byte round)
    {
      return (byte)(round / 2);
    }
  }
}
