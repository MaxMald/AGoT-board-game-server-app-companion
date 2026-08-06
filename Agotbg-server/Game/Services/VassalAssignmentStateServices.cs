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
    }

    /// <summary>
    /// Assigns a vassal house to the current player, consuming one of their order token
    /// sets. Validates that the player is the current player, has order token sets
    /// available, and the vassal house is available for assignment.
    /// </summary>
    /// 
    /// <param name="vaState">The vassal assignment state to update.</param>
    /// <param name="playerId">The ID of the player requesting the assignment.</param>
    /// <param name="vassalHouseType">The type of vassal house to assign.</param>
    /// 
    /// <returns>A Result indicating success or failure with an error message.</returns>
    public static Result AssignVassal(
      VassalAssignmentState vaState,
      string playerId,
      HouseType vassalHouseType
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

      if (!vaState.AvailableVassalHouses.Contains(vassalHouseType))
        return Result.FAILURE("Given Vassal House is not available for assignment.");

      VassalAssignmentPlayer? vaPlayer = vaState.Players.Find(p => p.PlayerId == playerId);
      if (vaPlayer == null)
        return Result.FAILURE("Given Player ID does not correspond to a Vassal Assignment Player.");

      if (vaPlayer.PossesedOrderTokenSets.Count == 0)
        return Result.FAILURE("Given Player does not possess any Order Token Sets.");

      VassalOrderTokenSetType vOrderTokenSet = vaPlayer.PossesedOrderTokenSets[0];
      vaPlayer.PossesedOrderTokenSets.RemoveAt(0);
      vaState.AvailableVassalHouses.Remove(vassalHouseType);

      VassalHouseSelectionDescriptor vHouseSelectionDescriptor = new()
      {
        HouseType = vassalHouseType,
        VassalOrderTokenSetType = vOrderTokenSet
      };

      vaPlayer.SelectedVassalHouses.Add(vHouseSelectionDescriptor);
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
        return Result.SUCCESS();
      }

      VassalAssignmentPlayer? nextPlayer = vaState.Players.Find(p => p.PlayerId == nextPlayerId);
      if (nextPlayer == null)
        return Result.FAILURE("Next Player ID does not correspond to a Vassal Assignment Player.");

      if (currentPlayer.PossesedOrderTokenSets.Count > 0)
        nextPlayer.PossesedOrderTokenSets.AddRange(currentPlayer.PossesedOrderTokenSets);

      vaState.CurrentPlayerID = nextPlayerId;
      return Result.SUCCESS();
    }

    /// <summary>
    /// Indicates if the given vassal assignment state can move to the next player. This
    /// is true if the current player has a next player ID.
    /// </summary>
    ///
    /// <param name="vaState">The Vassal Assignment State.</param>
    ///
    /// <returns>True if the current player can move to the next player; otherwise,
    /// false.</returns>
    public static bool CanMoveToNextPlayer(VassalAssignmentState vaState)
    {
      if (string.IsNullOrEmpty(vaState.CurrentPlayerID))
        return false;

      VassalAssignmentPlayer? currentPlayer = vaState.Players.Find(p => p.PlayerId == vaState.CurrentPlayerID);
      if (currentPlayer == null)
        return false;

      return !string.IsNullOrEmpty(currentPlayer.NextPlayerId);
    }

    /// <summary>
    /// Indicates if the given player has any vassal order token sets in possesion.
    /// </summary>
    ///
    /// <param name="vaState">The Vassal Assignment State.</param>
    /// <param name="playerId">The ID of the player to check.</param>
    ///
    /// <returns>True if the player has any vassal order token sets; otherwise,
    /// false.</returns>
    public static bool HasVassalOrderTokenSets(
      VassalAssignmentState vaState,
      string playerId
    )
    {
      if (string.IsNullOrEmpty(playerId))
        return false;

      VassalAssignmentPlayer? vaPlayer = vaState.Players.Find(p => p.PlayerId == playerId);
      if (vaPlayer == null)
        return false;

      return vaPlayer.PossesedOrderTokenSets.Count > 0;
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
    /// Clears the given vassal assignment state, resetting to default values.
    /// </summary>
    /// 
    /// <param name="state">The vassal assignment state to clear.</param>
    public static void Clear(VassalAssignmentState state)
    {
      state.AvailableVassalHouses.Clear();
      state.Players.Clear();
      state.CurrentPlayerID = string.Empty;
    }
  }
}
