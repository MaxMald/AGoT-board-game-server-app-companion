using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  public class GameStateService : IGameStateService
  {
    /// <inheritdoc/>
    public PlayerState? GetPlayerState(GameState gameState, string playerId)
    {
      if (gameState.Players.TryGetValue(playerId, out PlayerState? playerState))
        return playerState;
      return null;
    }

    /// <inheritdoc/>
    public HouseState? GetVassalHouseState(GameState gameState, HouseType houseType)
    {
      if (gameState.Vassals.TryGetValue(houseType, out HouseState? vassalHouseState))
        return vassalHouseState;
      return null;
    }

    /// <inheritdoc/>
    public HouseState? GetHouseState(GameState gameState, HouseType houseType)
    {
      if (gameState.Vassals.TryGetValue(houseType, out HouseState? vassalHouseState))
        return vassalHouseState;

      foreach (var player in gameState.Players.Values)
      {
        if (player.HouseState.Type == houseType)
          return player.HouseState;
      }
      return null;
    }

    /// <inheritdoc/>
    public void GetAllPlayerStates(
      GameState gameState,
      List<PlayerState> oPlayerHouseStates
    )
    {
      oPlayerHouseStates.Clear();
      foreach (PlayerState? player in gameState.Players.Values)
      {
        if (player != null)
          oPlayerHouseStates.Add(player);
      }
    }

    /// <inheritdoc/>
    public void GetAllHouseStates(
      GameState gameState,
      List<HouseState> oHouseStates
    )
    {
      oHouseStates.Clear();
      foreach (HouseState? houseState in gameState.Vassals.Values)
      {
        if (houseState != null)
          oHouseStates.Add(houseState);
      }
    }

    /// <inheritdoc/>
    public void GetAllPlayerHouseStates(
      GameState gameState,
      List<HouseState> oPlayerHouseStates
    )
    {
      oPlayerHouseStates.Clear();
      foreach (PlayerState? player in gameState.Players.Values)
      {
        if (player != null)
          oPlayerHouseStates.Add(player.HouseState);
      }
    }

    /// <inheritdoc/>
    public void GetAllVassalHouseStates(
      GameState gameState,
      List<HouseState> oVassalHouseStates
    )
    {
      oVassalHouseStates.Clear();
      foreach (HouseState? houseState in gameState.Vassals.Values)
      {
        if (houseState != null)
          oVassalHouseStates.Add(houseState);
      }
    }

    /// <inheritdoc/>
    public bool IsAdministrator(GameState gameState, string playerId)
    {
      // TODO
      return false;
    }

    /// <inheritdoc/>
    public void CheckWinCondition(GameState gameState)
    {
      foreach (var player in gameState.Players)
      {
        if (player.Value.HouseState.VictoryPoints >= GameConstants.NumVictoryPointsToWin
          && !player.Value.HouseState.IsDefeated)
        {
          gameState.Winner = player.Value.HouseState.Type;
          gameState.IsGameOver = true;
          return;
        }
      }
    }

    /// <inheritdoc/>
    public bool IsLastRound(GameState gameState)
    {
      return gameState.CurrentRound == GameConstants.NumRounds;
    }

    /// <inheritdoc/>
    public bool HasTiedPlayersByVictoryPoints(GameState gameState)
    {
      int highestVictoryPoints = gameState.Players
                                          .Values
                                          .Max(player => player.HouseState.VictoryPoints);

      List<PlayerState> playersWithHighestVictoryPoints
        = gameState.Players
                   .Values
                   .Where(player => player.HouseState.VictoryPoints == highestVictoryPoints)
                   .ToList();

      return playersWithHighestVictoryPoints.Count > 1;
    }

    /// <inheritdoc/>
    public List<HouseState> GetAllHouses(GameState gameState)
    {
      List<HouseState> allHouses = new List<HouseState>();
      foreach (var player in gameState.Players.Values)
        allHouses.Add(player.HouseState);

      foreach (var vassal in gameState.Vassals.Values)
        allHouses.Add(vassal);

      return allHouses;
    }

    /// <inheritdoc/>
    public void PrepareForInfluenceTrackBidding(
      GameState gameState,
      InfluenceTrackType influenceTrackType
    )
    {
      InfluenceTrackBiddingStateService.Prepare(
        gameState.InfluenceTrackBiddingState,
        influenceTrackType
      );

      foreach (PlayerState player in gameState.Players.Values)
      {
        player.HouseState.PowerTokensBid = 0;
        player.HouseState.HasBidPowerTokens = false;
      }

      foreach (HouseState vassalHouse in gameState.Vassals.Values)
      {
        vassalHouse.PowerTokensBid = 0;
        vassalHouse.HasBidPowerTokens = true;
      }
    }

    /// <inheritdoc/>
    public string GetPlayerIdThatHoldsTheIronThroneToken(GameState gameState)
    {
      if (gameState.Players.Count == 0)
        throw new InvalidOperationException("No players in the game state.");

      byte minIronThronePosition = gameState.Players
                                            .Values
                                            .Min(player => player.HouseState.IronThroneTrackPosition);

      PlayerState? playerWithIronThroneToken = gameState.Players
                                                        .Values
                                                        .FirstOrDefault(player => player.HouseState.IronThroneTrackPosition == minIronThronePosition);

      if (playerWithIronThroneToken == null)
        throw new InvalidOperationException("No player found with the Iron Throne token.");

      return playerWithIronThroneToken.PlayerId;
    }

    /// <inheritdoc/>
    public bool HaveAllPlayersSubmittedTheirBids(GameState gameState)
    {
      foreach (var player in gameState.Players.Values)
      {
        if (!player.HouseState.HasBidPowerTokens)
          return false;
      }
      return true;
    }

    /// <inheritdoc/>
    public List<HouseBet> CreateHouseBets(GameState gameState)
    {
      List<HouseBet> houseBets = new List<HouseBet>();
      foreach (var player in gameState.Players.Values)
      {
        if (player.HouseState.HasBidPowerTokens)
        {
          HouseBet houseBet = new HouseBet()
          {
            HouseType = player.HouseState.Type,
            BetAmount = player.HouseState.PowerTokensBid
          };
          houseBets.Add(houseBet);
        }
      }
      foreach (var vassal in gameState.Vassals.Values)
      {
        if (vassal.HasBidPowerTokens)
        {
          HouseBet houseBet = new HouseBet()
          {
            HouseType = vassal.Type,
            BetAmount = vassal.PowerTokensBid
          };
          houseBets.Add(houseBet);
        }
      }
      return houseBets;
    }

    /// <inheritdoc/>
    public void ClearAllHousesSubmittedBids(GameState gameState)
    {
      foreach (var player in gameState.Players.Values)
      {
        player.HouseState.HasBidPowerTokens = false;
        player.HouseState.PowerTokensBid = 0;
      }

      foreach (var vassal in gameState.Vassals.Values)
      {
        vassal.HasBidPowerTokens = false;
        vassal.PowerTokensBid = 0;
      }
    }

    /// <inheritdoc/>
    public List<PlayerState> GetPlayersInTurnOrder(GameState gameState)
    {
      return gameState.Players
                      .Values
                      .OrderBy(player => player.HouseState.IronThroneTrackPosition)
                      .ToList();
    }
  }
}
