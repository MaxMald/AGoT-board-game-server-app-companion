using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides operations for managing and querying game state, including player and
  /// house information, win conditions, and influence track bidding.
  /// </summary>
  public interface IGameStateService
  {
    /// <summary>
    /// Get a player state from the game state based on the specified player ID.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// <param name="playerId">The ID of the player to retrieve.</param>
    /// 
    /// <returns>The player state if found; otherwise, null.</returns>
    public PlayerState? GetPlayerState(GameState gameState, string playerId);

    /// <summary>
    /// Get a vassal house state from the game state based on the specified house type.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// <param name="houseType">The type of house to retrieve.</param>
    /// 
    /// <returns>The vassal house state if found; otherwise, null.</returns>
    public HouseState? GetVassalHouseState(GameState gameState, HouseType houseType);

    /// <summary>
    /// Gets a house state from the game state based on the specified house type. The method
    /// searches both player houses and vassal houses in the game state.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// <param name="houseType">The type of house to retrieve.</param>
    /// 
    /// <returns>The house state if found; otherwise, null.</returns>
    public HouseState? GetHouseState(GameState gameState, HouseType houseType);

    /// <summary>
    /// Gets all player states from the game state and adds them to the provided list of
    /// player house states.
    /// </summary>
    ///
    /// <remarks>
    /// The provided list of player house states will be cleared before adding the player
    /// states from the game state.
    /// </remarks>
    ///
    /// <param name="gameState">The current game state.</param>
    /// <param name="oPlayerHouseStates">The list where all player house states in the game
    /// state.</param>
    public void GetAllPlayerStates(
      GameState gameState,
      List<PlayerState> oPlayerHouseStates
    );

    /// <summary>
    /// Gets all house states from the game state, including both player houses and
    /// vassal houses and adds them to the provided list of house states.
    /// </summary>
    ///
    /// <remarks>
    /// The provided list of house states will be cleared before adding the house
    /// states from the game state.
    /// </remarks>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// <param name="oHouseStates">The list where all house states in the game
    /// state will be added.</param>
    public void GetAllHouseStates(
      GameState gameState,
      List<HouseState> oHouseStates
    );

    /// <summary>
    /// Gets all player house states from the game state and adds them to the provided
    /// list of player house states.
    /// </summary>
    ///
    /// <remarks>
    /// The provided list of house states will be cleared before adding the house states
    /// from the game state.
    /// </remarks>
    ///
    /// <param name="gameState">The game state to retrieve player house states
    /// from.</param>
    /// <param name="oPlayerHouseStates">The list where all player house states in the
    /// game state will be added.</param>
    public void GetAllPlayerHouseStates(
      GameState gameState,
      List<HouseState> oPlayerHouseStates
    );

    /// <summary>
    /// Gets all vassal house states from the game state and add them to the provided
    /// list of vassal house states.
    /// </summary>
    ///
    /// <remarks>
    /// The provided list of house states will be cleared before adding the house states
    /// from the game state.
    /// </remarks>
    ///
    /// <param name="gameState">The current game state.</param>
    /// <param name="oVassalHouseStates">The list where all vassal house states in the
    /// game state will be added.</param>
    public void GetAllVassalHouseStates(
      GameState gameState,
      List<HouseState> oVassalHouseStates
    );

    /// <summary>
    /// Indicates if the given player id is the administrator of the game based on the
    /// game state.
    /// </summary>
    ///
    /// <param name="gameState">The current game state.</param>
    /// <param name="playerId">The ID of the player to check.</param>
    ///
    /// <returns>True if the player is the administrator; otherwise, false.</returns>
    public bool IsAdministrator(GameState gameState, string playerId);

    /// <summary>
    /// Checks the win condition based on the current game state and updates the game
    /// state accordingly.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    public void CheckWinCondition(GameState gameState);

    /// <summary>
    /// Indicates if the current round is the last round of the game based on the game
    /// state.
    /// </summary>
    ///
    /// <param name="gameState">The current game state.</param>
    ///
    /// <returns>True if the current round is the last round of the game, otherwise
    /// false.</returns>
    public bool IsLastRound(GameState gameState);

    /// <summary>
    /// Indicates whether there are multiple players with the same highest victory points
    /// in the game state.
    /// </summary>
    ///
    /// <param name="gameState">The current game state.</param>
    ///
    /// <returns>True if there are multiple players with the same highest victory points,
    /// otherwise false.</returns>
    public bool HasTiedPlayersByVictoryPoints(GameState gameState);

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
    public void PrepareForInfluenceTrackBidding(
      GameState gameState,
      InfluenceTrackType influenceTrackType
    );

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
    public string GetPlayerIdThatHoldsTheIronThroneToken(GameState gameState);

    /// <summary>
    /// Indicates whether all players have submitted their bids for the current influence
    /// track bidding round.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// 
    /// <returns>True if all players have submitted their bids; otherwise, false.</returns>
    public bool HaveAllPlayersSubmittedTheirBids(GameState gameState);

    /// <summary>
    /// Creates a list with the submitted house bets for the current influence track
    /// bidding round, including both player houses and vassal houses. Houses that have
    /// not submitted a bid will not be included in the list.
    /// </summary>
    ///
    /// <param name="gameState">The current game state.</param>
    /// 
    /// <returns>A list of submitted house bets.</returns>
    public List<HouseBet> CreateHouseBets(GameState gameState);

    /// <summary>
    /// Clears all houses submitted bids in the game state, including both player houses
    /// and vassal houses.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    public void ClearAllHousesSubmittedBids(GameState gameState);

    /// <summary>
    /// Gets a list of players in the game state sorted by their Iron Throne track
    /// position, which determines the turn order. Players with lower Iron Throne track
    /// positions will appear earlier in the list.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// 
    /// <returns>A list of players in turn order.</returns>
    public List<PlayerState> GetPlayersInTurnOrder(GameState gameState);
  }
}
