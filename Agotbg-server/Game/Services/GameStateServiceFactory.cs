using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides a factory for creating instances of <see cref="GameState"/> with the
  /// specified configuration.
  /// </summary>
  public static class GameStateServiceFactory
  {
    /// <summary>
    /// Creates and initializes a new game state with the specified players and
    /// configuration.
    /// </summary>
    ///
    /// <param name="playersDescriptors">The list of player descriptors containing player
    /// names and house selections.</param>
    /// <param name="maxPlayers">The maximum number of players allowed in the
    /// game.</param>
    ///
    /// <returns>A newly initialized <see cref="GameState"/> object ready for
    /// gameplay.</returns>
    ///
    /// <exception cref="ArgumentException">Thrown when the number of players exceeds
    /// <paramref name="maxPlayers"/>, is less than the minimum required, a player has
    /// not selected a house, Targaryen is selected with fewer than 4 players, or
    /// multiple players select the same house.</exception>
    public static GameState Create(
      List<PlayerDescriptor> playersDescriptors,
      int maxPlayers
    )
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

      List<HouseState> allHouses = GetAllHouseStates(gameState);
      InfluenceTracksService.Initialize(allHouses);

      gameState.CurrentPhase = RoundPhaseType.Setup;
      return gameState;
    }

    private static void CreatePlayerHouses(
      GameState gameState,
      List<PlayerDescriptor> playersDescriptors
    )
    {
      foreach (var playerDescriptor in playersDescriptors)
      {
        HouseState houseState = HouseStateFactory.Create(playerDescriptor.HouseType);
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

        gameState.Vassals[houseType] = HouseStateFactory.CreateVassal(houseType);
      }
    }

    private static List<HouseState> GetAllHouseStates(GameState gameState)
    {
      List<HouseState> allHouses = new();
      foreach (var player in gameState.Players.Values)
        allHouses.Add(player.HouseState);

      foreach (var vassal in gameState.Vassals.Values)
        allHouses.Add(vassal);
      return allHouses;
    }
  }
}
