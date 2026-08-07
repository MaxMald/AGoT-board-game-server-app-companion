using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides services for managing the vassal assignment phase, including preparation,
  /// vassal selection, order token set distribution, and player turn progression.
  /// </summary>
  public static class VassalAssignmentStateServices
  {
    /// <summary>
    /// Prepares the vassal assignment state for a new vassal selection phase. Clears
    /// previous state, populates available vassals, distributes order token sets to
    /// players based on turn order, and sets the first player as current.
    /// </summary>
    /// 
    /// <param name="gameState">The game state containing vassal and player
    /// information.</param>
    public static void Prepare(GameState gameState)
    {
      VassalAssignmentState vaState = gameState.VassalAssignmentState;
      Clear(vaState);

      foreach (HouseState vassalHouse in gameState.Vassals.Values)
        vaState.AvailableVassalHouses.Add(vassalHouse.Type);

      if (vaState.AvailableVassalHouses.Count == 0)
        return;

      int numOrderTokenSets = Math.Min(
        vaState.AvailableVassalHouses.Count,
        (int)VassalOrderTokenSetType.MaximumNumberOfSets
      );

      List<PlayerState> playersInTurnOrder
        = GameStateService.GetPlayersInTurnOrder(gameState);

      if (playersInTurnOrder.Count == 0)
      {
        vaState.AvailableVassalHouses.Clear();
        return;
      }

      int lastPlayerIndex = playersInTurnOrder.Count - 1;
      for (int i = 0; i < playersInTurnOrder.Count; i++)
      {
        string nextPlayerId = string.Empty;
        if (i < lastPlayerIndex)
          nextPlayerId = playersInTurnOrder[i + 1].PlayerId;

        VassalOrderTokenSetType orderTokenSet = VassalOrderTokenSetType.None;
        if (numOrderTokenSets > 0)
        {
          numOrderTokenSets--;
          orderTokenSet = (VassalOrderTokenSetType)(numOrderTokenSets);
        }

        VassalAssignmentPlayer vaPlayer = new VassalAssignmentPlayer()
        {
          PlayerId = playersInTurnOrder[i].PlayerId,
          NextPlayerId = nextPlayerId,
          PossesedOrderTokenSets = new List<VassalOrderTokenSetType>() { orderTokenSet },
          SelectedVassalHouses = []
        };

        vaState.Players.Add(vaPlayer);
      }

      vaState.CurrentPlayerID = playersInTurnOrder[0].PlayerId;
      vaState.IsCompleted = false;
    }

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
    public static Result AssignVassals(
      VassalAssignmentState vaState,
      string playerId,
      List<HouseType> vassalHouseTypes
    )
    {
      if (string.IsNullOrEmpty(playerId))
        return Result.FAILURE("Given Player ID is null or empty.");

      if (string.IsNullOrEmpty(vaState.CurrentPlayerID))
        return Result.FAILURE("Current Player ID is null or empty.");

      if (playerId != vaState.CurrentPlayerID)
        return Result.FAILURE("Given Player ID does not match the Current Player ID.");

      if (vaState.AvailableVassalHouses.Count == 0)
        return Result.FAILURE("No available Vassal Houses for assignment.");

      int distinctCount = vassalHouseTypes.Distinct().Count();
      if (distinctCount != vassalHouseTypes.Count)
        return Result.FAILURE("Given Vassal House Types contain duplicates.");

      foreach (HouseType vassalHouseType in vassalHouseTypes)
      {
        if (!vaState.AvailableVassalHouses.Contains(vassalHouseType))
          return Result.FAILURE($"Given Vassal House {vassalHouseType} is not available for assignment.");
      }

      VassalAssignmentPlayer? vaPlayer = vaState.Players.Find(p => p.PlayerId == playerId);
      if (vaPlayer == null)
        return Result.FAILURE("Given Player ID does not correspond to a Vassal Assignment Player.");

      if (vaPlayer.PossesedOrderTokenSets.Count < vassalHouseTypes.Count)
        return Result.FAILURE("Given Player does not possess enough Order Token Sets for the requested Vassal House assignments.");

      foreach (HouseType vassalHouseType in vassalHouseTypes)
      {
        VassalOrderTokenSetType vOrderTokenSet = vaPlayer.PossesedOrderTokenSets[0];
        vaPlayer.PossesedOrderTokenSets.RemoveAt(0);
        vaState.AvailableVassalHouses.Remove(vassalHouseType);

        VassalHouseSelectionDescriptor vHouseSelectionDescriptor = new()
        {
          HouseType = vassalHouseType,
          VassalOrderTokenSetType = vOrderTokenSet
        };

        vaPlayer.SelectedVassalHouses.Add(vHouseSelectionDescriptor);
      }

      if (vaState.AvailableVassalHouses.Count == 0)
      {
        vaState.CurrentPlayerID = string.Empty;
        vaState.IsCompleted = true;
      }

      return Result.SUCCESS();
    }

    /// <summary>
    /// Moves the current player to the next player in the vassal assignment state. If
    /// there is no next player, the current player ID is set to an empty string.
    /// </summary>
    ///
    /// <param name="vaState">The Vassal Assignment State.</param>
    ///
    /// <returns>A Result indicating the success or failure of the operation.</returns>
    public static Result MoveToNextPlayer(VassalAssignmentState vaState)
    {
      if (string.IsNullOrEmpty(vaState.CurrentPlayerID))
        return Result.FAILURE("Current Player ID is null or empty.");

      VassalAssignmentPlayer? currentPlayer = vaState.Players.Find(p => p.PlayerId == vaState.CurrentPlayerID);
      if (currentPlayer == null)
        return Result.FAILURE("Current Player ID does not correspond to a Vassal Assignment Player.");

      string nextPlayerId = currentPlayer.NextPlayerId;
      if (string.IsNullOrEmpty(nextPlayerId))
      {
        vaState.CurrentPlayerID = string.Empty;
        vaState.IsCompleted = true;
        return Result.SUCCESS();
      }

      VassalAssignmentPlayer? nextPlayer = vaState.Players.Find(p => p.PlayerId == nextPlayerId);
      if (nextPlayer == null)
        return Result.FAILURE("Next Player ID does not correspond to a Vassal Assignment Player.");

      if (currentPlayer.PossesedOrderTokenSets.Count > 0)
        nextPlayer.PossesedOrderTokenSets.AddRange(currentPlayer.PossesedOrderTokenSets);

      if (nextPlayer.PossesedOrderTokenSets.Count == 0)
      {
        vaState.CurrentPlayerID = string.Empty;
        vaState.IsCompleted = true;
        return Result.SUCCESS();
      }

      vaState.CurrentPlayerID = nextPlayerId;
      return Result.SUCCESS();
    }

    /// <summary>
    /// Indicates if the vassal assignment state has a current player set. Returns true
    /// if the CurrentPlayerID is not null or empty; otherwise, false.
    /// </summary>
    /// 
    /// <param name="vaState">The Vassal Assignment State.</param>
    /// 
    /// <returns>True if there is a current player; otherwise, false.</returns>
    public static bool HasCurrentPlayer(VassalAssignmentState vaState)
    {
      return !string.IsNullOrEmpty(vaState.CurrentPlayerID);
    }

    /// <summary>
    /// Indicates if the vassal assignment state has any available vassal houses for
    /// assignment.
    /// </summary>
    ///
    /// <param name="vaState">The Vassal Assignment State.</param>
    ///
    /// <returns>True if there are available vassal houses; otherwise, false.</returns>
    public static bool HasAvailableVassalHouses(VassalAssignmentState vaState)
    {
      return vaState.AvailableVassalHouses.Count > 0;
    }

    /// <summary>
    /// Indicates if the vassal assignment state is completed, meaning it cannot assign
    /// any more vassal houses to players.
    /// </summary>
    ///
    /// <param name="vaState">The Vassal Assignment State.</param>
    ///
    /// <returns>True if the vassal assignment state is completed; otherwise,
    /// false.</returns>
    public static bool IsCompleted(VassalAssignmentState vaState)
    {
      return vaState.IsCompleted;
    }

    /// <summary>
    /// Indicates if the vassal assignment state has any vassal order token sets in
    /// possession.
    /// </summary>
    ///
    /// <param name="vaState">The Vassal Assignment State.</param>
    ///
    /// <returns>True if there are any vassal order token sets; otherwise,
    /// false.</returns>
    public static bool HasVassalOrderTokenSets(
      VassalAssignmentState vaState
    )
    {
      foreach (VassalAssignmentPlayer vaPlayer in vaState.Players)
      {
        if (vaPlayer.PossesedOrderTokenSets.Count > 0)
          return true;
      }
      return false;
    }

    /// <summary>
    /// Indicates if the given player is the last player in the vassal assignment state.
    /// This is true if the player has no next player ID.
    /// </summary>
    ///
    /// <param name="vaState">The Vassal Assignment State.</param>
    /// <param name="playerId">The ID of the player to check.</param>
    ///
    /// <returns>True if the player is the last player; otherwise, false.</returns>
    public static bool IsLastPlayer(VassalAssignmentState vaState, string playerId)
    {
      if (string.IsNullOrEmpty(playerId))
        return false;

      VassalAssignmentPlayer? vaPlayer = vaState.Players.Find(p => p.PlayerId == playerId);
      if (vaPlayer == null)
        return false;

      return string.IsNullOrEmpty(vaPlayer.NextPlayerId);
    }

    /// <summary>
    /// Automatically resolves vassal order token sets for the current player by
    /// assigning available vassal houses to the player until they run out of order
    /// token sets or there are no more available vassal houses.
    /// </summary>
    /// 
    /// <param name="vaState">The Vassal Assignment State.</param>
    public static void AutomaticallyResolveOrderTokenSetsForCurrentPlayer(
      VassalAssignmentState vaState
    )
    {
      if (string.IsNullOrEmpty(vaState.CurrentPlayerID))
        return;

      VassalAssignmentPlayer? currentPlayer = vaState.Players.Find(p => p.PlayerId == vaState.CurrentPlayerID);
      if (currentPlayer == null)
        return;

      for (int i = vaState.AvailableVassalHouses.Count - 1; i >= 0; i--)
      {
        HouseType vassalHouseType = vaState.AvailableVassalHouses[i];

        if (currentPlayer.PossesedOrderTokenSets.Count == 0)
          break;

        VassalOrderTokenSetType vOrderTokenSet = currentPlayer.PossesedOrderTokenSets[0];
        currentPlayer.PossesedOrderTokenSets.RemoveAt(0);
        vaState.AvailableVassalHouses.RemoveAt(i);

        VassalHouseSelectionDescriptor vHouseSelectionDescriptor = new()
        {
          HouseType = vassalHouseType,
          VassalOrderTokenSetType = vOrderTokenSet
        };

        currentPlayer.SelectedVassalHouses.Add(vHouseSelectionDescriptor);
      }
    }

    /// <summary>
    /// Clears the given vassal assignment state, resetting to default values.
    /// </summary>
    /// 
    /// <param name="state">The vassal assignment state to clear.</param>
    public static void Clear(VassalAssignmentState state)
    {
      state.AvailableVassalHouses.Clear();
      state.Players.Clear();
      state.CurrentPlayerID = string.Empty;
      state.IsCompleted = false;
    }
  }
}
