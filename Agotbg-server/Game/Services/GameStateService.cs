using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  public class GameStateService
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
      gameState.Wilding.Strength = GameConstants.WildingStartingStrength;

      CreatePlayerHouses(gameState, playersDescriptors);
      CreateVassalHouses(gameState);

      List<HouseState> allHouses = GetAllHouses(gameState);
      InfluenceTracksService.Initialize(allHouses);

      foreach (HouseState house in allHouses)
        HouseStateService.UpdateNumSpecialOrdersBasedOnKingsCourtPosition(house);

      gameState.CurrentPhase = RoundPhaseType.Setup;
      return gameState;
    }

    public Result MoveToRoundPhase(GameState gameState, RoundPhaseType newPhase)
    {
      // TODO Round Transitions
      return Result.SUCCESS();
    }

    public Result ModifyPlayerPowerTokens(GameState gameState, string playerId, short delta)
    {
      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      HouseState house = gameState.Players[playerId].HouseState;
      short power = house.PowerTokens;
      short newPower = (short)Math.Max(0, power + delta);
      byte newPowerByte = (byte)Math.Min(newPower, byte.MaxValue);

      return HouseStateService.UpdatePowerTokens(house, newPowerByte);
    }

    public Result UpdatePlayerPowerTokens(GameState gameState, string playerId, byte newPowerTokens)
    {
      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = gameState.Players[playerId];
      return HouseStateService.UpdatePowerTokens(player.HouseState, newPowerTokens);
    }

    public Result UpdatePlayerSupplyLevel(GameState gameState, string playerId, byte newSupplyLevel)
    {
      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      HouseState house = gameState.Players[playerId].HouseState;
      HouseStateService.UpdateHouseSupplyLevel(house, newSupplyLevel);

      return Result.SUCCESS();
    }

    public Result UpdatePlayerVictoryPoints(GameState gameState, string playerId, byte newVictoryPoints)
    {
      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = gameState.Players[playerId];
      player.HouseState.VictoryPoints = newVictoryPoints;

      CheckWinCondition(gameState);
      return Result.SUCCESS();
    }

    public Result UpdatePlayerDragonStrength(GameState gameState, string playerId, byte newDragonStrength)
    {
      if (!gameState.Players.ContainsKey(playerId))
        return Result.FAILURE($"Player with ID {playerId} does not exist in the room.");

      PlayerState player = gameState.Players[playerId];
      return HouseStateService.UpdateDragonStrength(player.HouseState, newDragonStrength);
    }

    public Result UpdatePlayerPowerTokensBid(GameState gameState, string playerId, byte newBid)
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

    public Result PillageHouse(GameState gameState, string saboteurPlayerId, string sabotagedPlayerId)
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

    public Result VassalPillageHouse(GameState gameState, HouseType vassalHouse, string sabotagePlayerId)
    {
      if (gameState.CurrentPhase != RoundPhaseType.Action)
        return Result.FAILURE("Cannot pillage a house if the current phase is not the action phase.");

      if (!gameState.Vassals.ContainsKey(vassalHouse))
        return Result.FAILURE($"Vassal of type: {vassalHouse} does not exist in the room.");

      if (!gameState.Players.ContainsKey(sabotagePlayerId))
        return Result.FAILURE($"Player with ID {sabotagePlayerId} does not exist in the room.");

      HouseState vassal = gameState.Vassals[vassalHouse];
      PlayerState sabotagePlayer = gameState.Players[sabotagePlayerId];

      HouseStateService.PillageHouse(vassal, sabotagePlayer.HouseState);
      return Result.SUCCESS();
    }

    public Result TransferPowerTokens(
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

    public Result MakeVassalageStatus(GameState gameState, string commanderPlayerId, HouseType vassalHouseType)
    {
      if (!gameState.Players.ContainsKey(commanderPlayerId))
        return Result.FAILURE($"Commander player with ID {commanderPlayerId} does not exist in the room.");

      if (!gameState.Vassals.ContainsKey(vassalHouseType))
        return Result.FAILURE($"Vassal of type: {vassalHouseType} does not exist in the room.");

      PlayerState commanderPlayer = gameState.Players[commanderPlayerId];
      HouseState vassalHouse = gameState.Vassals[vassalHouseType];

      return HouseStateService.MakeVassalageStatus(commanderPlayer.HouseState, vassalHouse);
    }

    public Result BreakVassalageStatus(GameState gameState, string commanderPlayerId, HouseType vassalHouseType)
    {
      if (!gameState.Players.ContainsKey(commanderPlayerId))
        return Result.FAILURE($"Commander player with ID {commanderPlayerId} does not exist in the room.");

      if (!gameState.Vassals.ContainsKey(vassalHouseType))
        return Result.FAILURE($"Vassal of type: {vassalHouseType} does not exist in the room.");

      PlayerState commanderPlayer = gameState.Players[commanderPlayerId];
      HouseState vassalHouse = gameState.Vassals[vassalHouseType];

      return HouseStateService.BreakVassalageStatus(commanderPlayer.HouseState, vassalHouse);
    }

    public Result ModifyVassalSupplyLevel(
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

    public Result DefeatPlayer(GameState gameState, string playerId)
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

    private static List<HouseState> GetAllHouses(GameState gameState)
    {
      List<HouseState> allHouses = new List<HouseState>();
      foreach (var player in gameState.Players.Values)
        allHouses.Add(player.HouseState);

      foreach (var vassal in gameState.Vassals.Values)
        allHouses.Add(vassal);

      return allHouses;
    }
  }
}
