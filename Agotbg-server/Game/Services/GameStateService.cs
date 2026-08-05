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

      foreach (HouseState house in allHouses)
        HouseStateService.UpdateNumSpecialOrdersBasedOnKingsCourtPosition(house);

      gameState.CurrentPhase = RoundPhaseType.Setup;
      return gameState;
    }

    public static Result MoveToRoundPhase(GameState gameState, RoundPhaseType newPhase)
    {
      // TODO Round Transitions
      return Result.SUCCESS();
    }

    public static Result ModifyPlayerPowerTokens(GameState gameState, string playerId, short delta)
    {
      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      HouseState house = gameState.Players[playerId].HouseState;
      short power = house.PowerTokens;
      short newPower = (short)Math.Max(0, power + delta);
      byte newPowerByte = (byte)Math.Min(newPower, byte.MaxValue);

      return HouseStateService.UpdatePowerTokens(house, newPowerByte);
    }

    public static Result UpdatePlayerPowerTokens(GameState gameState, string playerId, byte newPowerTokens)
    {
      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = gameState.Players[playerId];
      return HouseStateService.UpdatePowerTokens(player.HouseState, newPowerTokens);
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

    public static Result UpdatePlayerDragonStrength(GameState gameState, string playerId, byte newDragonStrength)
    {
      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = gameState.Players[playerId];
      return HouseStateService.UpdateDragonStrength(player.HouseState, newDragonStrength);
    }

    public static Result UpdatePlayerPowerTokensBid(GameState gameState, string playerId, byte newBid)
    {
     if (gameState.CurrentPhase != RoundPhaseType.KingsCourtBidding &&
        gameState.CurrentPhase != RoundPhaseType.FiefdomsBidding &&
        gameState.CurrentPhase != RoundPhaseType.IronThroneBidding &&
        gameState.CurrentPhase != RoundPhaseType.WildlingsBidding)
      {
        return Result.FAILURE("Cannot update bid if the current phase is not a bidding phase.");
      }

      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = gameState.Players[playerId];
      return HouseStateService.UpdatePowerTokensBid(player.HouseState, newBid);
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

    public static Result MakeVassalageStatus(GameState gameState, string commanderPlayerId, HouseType vassalHouseType)
    {
      if (!gameState.Players.ContainsKey(commanderPlayerId))
        return Result.FAILURE($"Commander player with ID {commanderPlayerId} does not exist in the room.");

      if (!gameState.Vassals.ContainsKey(vassalHouseType))
        return Result.FAILURE($"Vassal of type: {vassalHouseType} does not exist in the room.");

      PlayerState commanderPlayer = gameState.Players[commanderPlayerId];
      HouseState vassalHouse = gameState.Vassals[vassalHouseType];

      return HouseStateService.MakeVassalageStatus(commanderPlayer.HouseState, vassalHouse);
    }

    public static Result BreakVassalageStatus(GameState gameState, string commanderPlayerId, HouseType vassalHouseType)
    {
      if (!gameState.Players.ContainsKey(commanderPlayerId))
        return Result.FAILURE($"Commander player with ID {commanderPlayerId} does not exist in the room.");

      if (!gameState.Vassals.ContainsKey(vassalHouseType))
        return Result.FAILURE($"Vassal of type: {vassalHouseType} does not exist in the room.");

      PlayerState commanderPlayer = gameState.Players[commanderPlayerId];
      HouseState vassalHouse = gameState.Vassals[vassalHouseType];

      return HouseStateService.BreakVassalageStatus(commanderPlayer.HouseState, vassalHouse);
    }

    // TODO: Defeat Player is not fully implemented.
    public static Result DefeatPlayer(GameState gameState, string playerId)
    {
      // TODO: Consider check the game round is on a safe phase to defeat a player, like
      // after the planning phase and action phase.

      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = gameState.Players[playerId];
      if (player.HouseState.IsDefeated)
        return Result.FAILURE($"Player with ID {playerId} is already defeated.");

      List<HouseType> vassalsToBreak = player.HouseState.VassalHouseTypes.ToList();
      foreach (HouseType vassalHouseType in vassalsToBreak)
      {
        Result breakResult = BreakVassalageStatus(gameState, playerId, vassalHouseType);
        if (!breakResult.Success)
          return Result.FAILURE($"Failed to break vassalage status for vassal {vassalHouseType}: {breakResult.Message}");
      }

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
      gameState.InfluenceTrackBiddingState.InfluenceTrackType = influenceTrackType;
      gameState.InfluenceTrackBiddingState.TargaryenPowerTokenGifts.Clear();
      gameState.InfluenceTrackBiddingState.HouseBets.Clear();

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
    /// Clears the influence track bidding state in the game state, resetting the
    /// influence track type and clearing any Targaryen power token gifts and house bets.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    public static void ClearInfluenceTrackBiddingState(GameState gameState)
    {
      gameState.InfluenceTrackBiddingState.InfluenceTrackType = InfluenceTrackType.None;
      gameState.InfluenceTrackBiddingState.TargaryenPowerTokenGifts.Clear();
      gameState.InfluenceTrackBiddingState.HouseBets.Clear();
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
