using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  public static class GameStateService
  {
    public static GameState Create(List<PlayerDescriptor> playersDescriptors, int maxPlayers)
    {
      int numPlayers = playersDescriptors.Count;
      if (numPlayers > maxPlayers)
        throw new ArgumentException($"Too many players to start the game. Current Number of players: {numPlayers}. Maximum allowed is {maxPlayers}.");

      if (numPlayers < GameConstants.MinPlayers)
        throw new ArgumentException($"Not enough players to start the game. Minimum required is {GameConstants.MinPlayers}.");

      List<HouseType> selectedHouses = [];
      foreach (PlayerDescriptor playerDescriptor in playersDescriptors)
      {
        if (playerDescriptor.HouseType == HouseType.Undefined)
          throw new ArgumentException($"Player '{playerDescriptor.Name}' has not selected a house.");

        if (playerDescriptor.HouseType == HouseType.Targaryen && numPlayers < 4)
          throw new ArgumentException("Targaryen house can only be selected if there are at least 4 players.");

        if (selectedHouses.Contains(playerDescriptor.HouseType))
          throw new ArgumentException($"House '{playerDescriptor.HouseType}' has been selected by multiple players.");

        selectedHouses.Add(playerDescriptor.HouseType);
      }

      GameState gameState = new();
      gameState.Wildlings.Strength = GameConstants.WildingStartingStrength;

      CreatePlayerHouses(gameState, playersDescriptors);
      CreateVassalHouses(gameState);

      List<HouseState> allHouses = GetAllHouses(gameState);
      InfluenceTracksService.Initialize(allHouses);

      gameState.CurrentPhase = RoundPhaseType.Setup;
      return gameState;
    }

    public static Result ModifyPlayerPowerTokens(GameState gameState, string playerId, short delta)
    {
      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      HouseState house = gameState.Players[playerId].HouseState;
      short power = house.PowerTokens;
      short newPower = (short)Math.Max(0, power + delta);
      byte newPowerByte = (byte)Math.Min(newPower, byte.MaxValue);

      HouseStateService.UpdatePowerTokens(house, newPowerByte);

      return Result.SUCCESS();
    }

    public static Result UpdatePlayerPowerTokens(GameState gameState, string playerId, byte newPowerTokens)
    {
      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = gameState.Players[playerId];
      HouseStateService.UpdatePowerTokens(player.HouseState, newPowerTokens);

      return Result.SUCCESS();
    }

    public static Result UpdatePlayerSupplyLevel(GameState gameState, string playerId, byte newSupplyLevel)
    {
      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      HouseState house = gameState.Players[playerId].HouseState;
      HouseStateService.UpdateHouseSupplyLevel(house, newSupplyLevel);

      return Result.SUCCESS();
    }

    public static Result UpdateVassalSupplyLevel(
      GameState gameState,
      HouseType vassalHouseType,
      byte newSupplyLevel
      )
    {
      if (!gameState.Vassals.ContainsKey(vassalHouseType))
        return Result.FAILURE($"Vassal of type: {vassalHouseType} does not exist in the room.");

      HouseState vassalHouse = gameState.Vassals[vassalHouseType];
      HouseStateService.UpdateHouseSupplyLevel(vassalHouse, newSupplyLevel);

      return Result.SUCCESS();
    }

    public static Result UpdatePlayerVictoryPoints(GameState gameState, string playerId, byte newVictoryPoints)
    {
      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = gameState.Players[playerId];
      player.HouseState.VictoryPoints = newVictoryPoints;

      CheckWinCondition(gameState);
      return Result.SUCCESS();
    }

    public static Result SubmitPlayerPowerTokensBid(GameState gameState, string playerId, byte newBid)
    {
      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = gameState.Players[playerId];
      return HouseStateService.SubmitPowerTokensBid(player.HouseState, newBid);
    }

    public static Result CancelPlayerPowerTokenBid(GameState gameState, string playerId)
    {
      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = gameState.Players[playerId];
      HouseStateService.CancelPowerTokensBid(player.HouseState);

      return Result.SUCCESS();
    }

    public static Result UpdateIronBankLoanInterest(GameState gameState, string playerId, byte newInterest)
    {
      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = gameState.Players[playerId];
      return HouseStateService.UpdateIronBankLoanInterest(player.HouseState, newInterest);
    }

    public static Result Pillage(GameState gameState, string saboteurPlayerId, string sabotagedPlayerId)
    {
      if (gameState.CurrentPhase != RoundPhaseType.Action)
        return Result.FAILURE("Cannot pillage a house if the current phase is not the action phase.");

      if (!gameState.Players.ContainsKey(saboteurPlayerId))
        return Result.FAILURE($"Player with ID {saboteurPlayerId} does not exist in the room.");

      if (!gameState.Players.ContainsKey(sabotagedPlayerId))
        return Result.FAILURE($"Player with ID {sabotagedPlayerId} does not exist in the room.");

      PlayerState saboteurPlayer = gameState.Players[saboteurPlayerId];
      PlayerState sabotagedPlayer = gameState.Players[sabotagedPlayerId];

      HouseStateService.PillageHouse(saboteurPlayer.HouseState, sabotagedPlayer.HouseState);
      return Result.SUCCESS();
    }

    public static Result PillageVassal(GameState gameState, string saboteurPlayerId, HouseType sabotagedVassalHouse)
    {
      if (gameState.CurrentPhase != RoundPhaseType.Action)
        return Result.FAILURE("Cannot pillage a house if the current phase is not the action phase.");

      if (!gameState.Vassals.ContainsKey(sabotagedVassalHouse))
        return Result.FAILURE($"Vassal of type: {sabotagedVassalHouse} does not exist in the room.");

      if (!gameState.Players.ContainsKey(saboteurPlayerId))
        return Result.FAILURE($"Player with ID {saboteurPlayerId} does not exist in the room.");

      HouseState vassal = gameState.Vassals[sabotagedVassalHouse];
      PlayerState saboteurPlayer = gameState.Players[saboteurPlayerId];

      HouseStateService.PillageHouse(saboteurPlayer.HouseState, vassal);
      return Result.SUCCESS();
    }

    public static Result TransferPowerTokens(
      GameState gameState,
      string fromPlayerId,
      string toPlayerId,
      byte amount
    )
    {
      if (!gameState.Players.ContainsKey(fromPlayerId))
        return Result.FAILURE($"Player with ID {fromPlayerId} does not exist in the room.");

      if (!gameState.Players.ContainsKey(toPlayerId))
        return Result.FAILURE($"Player with ID {toPlayerId} does not exist in the room.");

      PlayerState fromPlayer = gameState.Players[fromPlayerId];
      PlayerState toPlayer = gameState.Players[toPlayerId];

      return HouseStateService.TransferPowerTokens(
        fromPlayer.HouseState,
        toPlayer.HouseState,
        amount
      );
    }

    public static Result MakeVassalageRelationship(GameState gameState, string commanderPlayerId, HouseType vassalHouseType)
    {
      if (!gameState.Players.ContainsKey(commanderPlayerId))
        return Result.FAILURE($"Commander player with ID {commanderPlayerId} does not exist in the room.");

      if (!gameState.Vassals.ContainsKey(vassalHouseType))
        return Result.FAILURE($"Vassal of type: {vassalHouseType} does not exist in the room.");

      PlayerState commanderPlayer = gameState.Players[commanderPlayerId];
      HouseState vassalHouse = gameState.Vassals[vassalHouseType];

      return HouseStateService.MakeVassalageRelationship(
        commanderPlayer.HouseState,
        vassalHouse
      );
    }

    /// <summary>
    /// Clears all vassalage relationships in the game state, effectively removing all
    /// vassal relationships between player houses and vassal houses.
    /// </summary>
    ///
    /// <param name="gameState">The current game state.</param>
    public static void ClearVassalageRelationships(GameState gameState)
    {
      List<HouseState> allHouses = GetAllHouses(gameState);
      foreach (HouseState house in allHouses)
        HouseStateService.ClearVassalageProperties(house);
    }

    /// <summary>
    /// TODO: Defeat a player in the game state. This method is a placeholder and needs to be
    /// implemented according to the game rules and mechanics.
    /// </summary>
    public static Result DefeatPlayer(GameState gameState, string playerId)
    {
      // TODO:
      //
      // Looks like defeating a player would affect considerably the game state.
      //
      // Should we consider checking if the game round is on a safe phase to defeat a
      // player? The rules say a player cannot be defeated, but they can be eliminated
      // from the game by turning into a vassal.
      //
      // What would happen to the vassals of the defeated player? Should they be freed or
      // assigned to another player? Should we start a new round of vassal assignment?
      // When turning defeated player into a vassal, what if there are no more order
      // tokens sets to assign to the defeated player (new vassal)? Should we remove the
      // defeated player from the game state?
      //
      // What if the defeated player is a Targaryen player? Should we remove the
      // Targaryen player from the game state?
      //
      // What if, after converting the defeated player into a vassal, there are less than
      // the minimum number of players in the game state? Should we end the game and
      // declare a winner?

      return Result.FAILURE("DefeatPlayer method is not implemented yet.");

      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = gameState.Players[playerId];
      if (player.HouseState.IsDefeated)
        return Result.FAILURE($"Player with ID {playerId} is already defeated.");

      return HouseStateService.SetHouseAsDefeated(player.HouseState);
    }

    public static void CheckWinCondition(GameState gameState)
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

    /// <summary>
    /// Indicates if the current round is the last round of the game based on the game
    /// state.
    /// </summary>
    ///
    /// <param name="gameState">The current game state.</param>
    ///
    /// <returns>True if the current round is the last round of the game, otherwise
    /// false.</returns>
    public static bool IsLastRound(GameState gameState)
    {
      return gameState.CurrentRound == GameConstants.NumRounds;
    }

    /// <summary>
    /// Indicates whether there are multiple players with the same highest victory points
    /// in the game state.
    /// </summary>
    ///
    /// <param name="gameState">The current game state.</param>
    ///
    /// <returns>True if there are multiple players with the same highest victory points,
    /// otherwise false.</returns>
    public static bool HasTiedPlayersByVictoryPoints(GameState gameState)
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

    /// <summary>
    /// Gets a list of all houses in the game state, including both player houses and
    /// vassal houses.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// 
    /// <returns>A list of all houses in the game state.</returns>
    public static List<HouseState> GetAllHouses(GameState gameState)
    {
      List<HouseState> allHouses = new List<HouseState>();
      foreach (var player in gameState.Players.Values)
        allHouses.Add(player.HouseState);

      foreach (var vassal in gameState.Vassals.Values)
        allHouses.Add(vassal);

      return allHouses;
    }

    /// <summary>
    /// Prepares the game state for influence track bidding.
    /// </summary>
    ///
    /// <remarks>
    /// Prepares players and vassal for a bidding round. For the vassal houses it
    /// automatically submits their bids with 0 power tokens.
    /// </remarks>
    ///
    /// <param name="gameState">The current game state.</param>
    /// <param name="influenceTrackType">The type of influence track for bidding.</param>
    public static void PrepareForInfluenceTrackBidding(
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

    /// <summary>
    /// Returns the player ID of the player who currently holds the Iron Throne token
    /// based on the game state.
    /// </summary>
    ///
    /// <param name="gameState">The current game state.</param>
    /// <returns>The player ID of the player who currently holds the Iron Throne
    /// token.</returns>
    ///
    /// <exception cref="InvalidOperationException">Thrown if there are no players in the
    /// game state or no player found with the Iron Throne token.</exception>
    public static string GetPlayerIdThatHoldsTheIronThroneToken(GameState gameState)
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

    /// <summary>
    /// Indicates whether all players have submitted their bids for the current influence
    /// track bidding round.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// 
    /// <returns>True if all players have submitted their bids; otherwise, false.</returns>
    public static bool HaveAllPlayersSubmittedTheirBids(GameState gameState)
    {
      foreach (var player in gameState.Players.Values)
      {
        if (!player.HouseState.HasBidPowerTokens)
          return false;
      }
      return true;
    }

    /// <summary>
    /// Creates a list with the submitted house bets for the current influence track
    /// bidding round, including both player houses and vassal houses. Houses that have
    /// not submitted a bid will not be included in the list.
    /// </summary>
    ///
    /// <param name="gameState">The current game state.</param>
    /// 
    /// <returns>A list of submitted house bets.</returns>
    public static List<HouseBet> CreateHouseBets(GameState gameState)
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

    /// <summary>
    /// Clears all houses submitted bids in the game state, including both player houses
    /// and vassal houses.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    public static void ClearAllHousesSubmittedBids(GameState gameState)
    {
      foreach (var player in gameState.Players.Values)
        HouseStateService.ClearSubmittedPowerTokenBid(player.HouseState);
      foreach (var vassal in gameState.Vassals.Values)
        HouseStateService.ClearSubmittedPowerTokenBid(vassal);
    }

    /// <summary>
    /// Gets a list of players in the game state sorted by their Iron Throne track
    /// position, which determines the turn order. Players with lower Iron Throne track
    /// positions will appear earlier in the list.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// 
    /// <returns>A list of players in turn order.</returns>
    public static List<PlayerState> GetPlayersInTurnOrder(GameState gameState)
    {
      return gameState.Players
                      .Values
                      .OrderBy(player => player.HouseState.IronThroneTrackPosition)
                      .ToList();
    }

    private static void CreatePlayerHouses(GameState gameState, List<PlayerDescriptor> playersDescriptors)
    {
      foreach (var playerDescriptor in playersDescriptors)
      {
        HouseState houseState = HouseStateService.Create(playerDescriptor.HouseType);
        PlayerState playerState = new PlayerState()
        {
          PlayerId = playerDescriptor.PlayerId,
          HouseState = houseState
        };

        gameState.Players[playerDescriptor.PlayerId] = playerState;
      }
    }

    private static void CreateVassalHouses(GameState gameState)
    {
      for (byte i = 0; i < (byte)HouseType.Count; ++i)
      {
        HouseType houseType = (HouseType)i;
        if (houseType == HouseType.Undefined || houseType == HouseType.Targaryen)
          continue; // Skip undefined type. Targaryen cannot be a vassal house

        if (gameState.Players.Values.Any(p => p.HouseState.Type == houseType))
          continue; // Skip if the house is already taken by a player

        if (gameState.Vassals.ContainsKey(houseType))
          continue; // Skip if the house is already added as a vassal

        gameState.Vassals[houseType] = HouseStateService.CreateVassal(houseType);
      }
    }
  }
}
