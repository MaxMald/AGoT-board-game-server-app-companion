using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides services for managing the vassal assignment phase, including preparation,
  /// vassal selection, order token set distribution, and player turn progression.
  /// </summary>
  public class VassalAssignmentStateServices : IVassalAssignmentStateService
  {
    /// <summary>
    /// Reference to the game state service.
    /// </summary>
    IGameStateService m_gameStateService;

    /// <inheritdoc/>
    public VassalAssignmentStateServices(IGameStateService gameStateService)
    {
      m_gameStateService = gameStateService;
    }

    /// <inheritdoc/>
    public void Prepare(GameState gameState)
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
        = m_gameStateService.GetPlayersInTurnOrder(gameState);

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

    /// <inheritdoc/>
    public Result AssignVassals(
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

    /// <inheritdoc/>
    public Result MoveToNextPlayer(VassalAssignmentState vaState)
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

    /// <inheritdoc/>
    public bool HasCurrentPlayer(VassalAssignmentState vaState)
    {
      return !string.IsNullOrEmpty(vaState.CurrentPlayerID);
    }

    /// <inheritdoc/>
    public bool HasAvailableVassalHouses(VassalAssignmentState vaState)
    {
      return vaState.AvailableVassalHouses.Count > 0;
    }

    /// <inheritdoc/>
    public bool IsCompleted(VassalAssignmentState vaState)
    {
      return vaState.IsCompleted;
    }

    /// <inheritdoc/>
    public bool HasVassalOrderTokenSets(VassalAssignmentState vaState)
    {
      foreach (VassalAssignmentPlayer vaPlayer in vaState.Players)
      {
        if (vaPlayer.PossesedOrderTokenSets.Count > 0)
          return true;
      }
      return false;
    }

    /// <inheritdoc/>
    public bool IsLastPlayer(VassalAssignmentState vaState, string playerId)
    {
      if (string.IsNullOrEmpty(playerId))
        return false;

      VassalAssignmentPlayer? vaPlayer = vaState.Players.Find(p => p.PlayerId == playerId);
      if (vaPlayer == null)
        return false;

      return string.IsNullOrEmpty(vaPlayer.NextPlayerId);
    }

    /// <inheritdoc/>
    public void AutomaticallyResolveOrderTokenSetsForCurrentPlayer(
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

    /// <inheritdoc/>
    public void Clear(VassalAssignmentState state)
    {
      state.AvailableVassalHouses.Clear();
      state.Players.Clear();
      state.CurrentPlayerID = string.Empty;
      state.IsCompleted = false;
    }
  }
}
