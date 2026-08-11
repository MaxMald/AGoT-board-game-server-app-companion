using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.Interfaces
{
  /// <summary>
  /// Defines a service that provides methods for managing the vassal assignment phase,
  /// including preparation, vassal selection, order token set distribution, and player
  /// turn progression.
  /// </summary>
  public interface IVassalAssignmentStateService
  {
    /// <summary>
    /// Prepares the vassal assignment state for a new vassal selection phase. Clears
    /// previous state, populates available vassals, distributes order token sets to
    /// players based on turn order, and sets the first player as current.
    /// </summary>
    ///
    /// <param name="gameState">The game state containing vassal and player
    /// information.</param>
    public void Prepare(GameState gameState);

    /// <summary>
    /// Assigns vassal houses to the current player, consuming one of their order token
    /// sets for each vassal house. Validates that the player is the current player, has
    /// order token sets available, and the vassal houses are available for assignment.
    /// </summary>
    ///
    /// <param name="vaState">The vassal assignment state to update.</param>
    /// <param name="playerId">The ID of the player requesting the assignment.</param>
    /// <param name="vassalHouseTypes">The types of vassal houses to assign.</param>
    ///
    /// <returns>A Result indicating success or failure with an error message.</returns>
    public Result AssignVassals(
      VassalAssignmentState vaState,
      string playerId,
      List<HouseType> vassalHouseTypes
    );

    /// <summary>
    /// Moves the current player to the next player in the vassal assignment state. If
    /// there is no next player, the current player ID is set to an empty string.
    /// </summary>
    ///
    /// <param name="vaState">The Vassal Assignment State.</param>
    ///
    /// <returns>A Result indicating the success or failure of the operation.</returns>
    public Result MoveToNextPlayer(VassalAssignmentState vaState);

    /// <summary>
    /// Indicates if the vassal assignment state has a current player set. Returns true
    /// if the CurrentPlayerID is not null or empty; otherwise, false.
    /// </summary>
    /// 
    /// <param name="vaState">The Vassal Assignment State.</param>
    /// 
    /// <returns>True if there is a current player; otherwise, false.</returns>
    public bool HasCurrentPlayer(VassalAssignmentState vaState);

    /// <summary>
    /// Indicates if the vassal assignment state has any available vassal houses for
    /// assignment.
    /// </summary>
    ///
    /// <param name="vaState">The Vassal Assignment State.</param>
    ///
    /// <returns>True if there are available vassal houses; otherwise, false.</returns>
    public bool HasAvailableVassalHouses(VassalAssignmentState vaState);

    /// <summary>
    /// Indicates if the vassal assignment state is completed, meaning it cannot assign
    /// any more vassal houses to players.
    /// </summary>
    ///
    /// <param name="vaState">The Vassal Assignment State.</param>
    ///
    /// <returns>True if the vassal assignment state is completed; otherwise,
    /// false.</returns>
    public bool IsCompleted(VassalAssignmentState vaState);

    /// <summary>
    /// Indicates if the vassal assignment state has any vassal order token sets in
    /// possession.
    /// </summary>
    ///
    /// <param name="vaState">The Vassal Assignment State.</param>
    ///
    /// <returns>True if there are any vassal order token sets; otherwise,
    /// false.</returns>
    public bool HasVassalOrderTokenSets(VassalAssignmentState vaState);


    /// <summary>
    /// Indicates if the given player is the last player in the vassal assignment state.
    /// This is true if the player has no next player ID.
    /// </summary>
    ///
    /// <param name="vaState">The Vassal Assignment State.</param>
    /// <param name="playerId">The ID of the player to check.</param>
    ///
    /// <returns>True if the player is the last player; otherwise, false.</returns>
    public bool IsLastPlayer(VassalAssignmentState vaState, string playerId);

    /// <summary>
    /// Automatically resolves vassal order token sets for the current player by
    /// assigning available vassal houses to the player until they run out of order
    /// token sets or there are no more available vassal houses.
    /// </summary>
    /// 
    /// <param name="vaState">The Vassal Assignment State.</param>
    public void AutomaticallyResolveOrderTokenSetsForCurrentPlayer(
      VassalAssignmentState vaState
    );

    /// <summary>
    /// Clears the given vassal assignment state, resetting to default values.
    /// </summary>
    /// 
    /// <param name="state">The vassal assignment state to clear.</param>
    public void Clear(VassalAssignmentState state);
  }
}
