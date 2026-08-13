using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Utests.Game.Services.VassalAssignmentStateServiceTests
{
  internal class MoveToNextPlayerTests : AVassalAssignmetStateServiceTest
  {
    [Test]
    public void MoveToNextPlayer_ShouldMoveToNextPlayer_WhenCurrentPlayerIsNotLast()
    {
      // Arrange
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "Lannister", 1);
      VassalAssignmentPlayer lannisterVAP = CreateVassalAssignmentPlayer("Lannister", "", 1);

      state.Players.Add(starkVAP);
      state.Players.Add(lannisterVAP);
      state.CurrentPlayerID = starkVAP.PlayerId;

      state.AvailableVassalHouses.Add(HouseType.Arryn);
      state.AvailableVassalHouses.Add(HouseType.Greyjoy);

      // Simulate Stark player selecting Greyjoy as a vassal house
      VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy }
      );

      // Act - Move to the next player (Lannister)
      Result result = VASS.MoveToNextPlayer(state);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.IsCompleted, Is.False);
      Assert.That(state.CurrentPlayerID, Is.EqualTo(lannisterVAP.PlayerId));
    }

    [Test]
    public void MoveToNextPlayer_ShouldNotPassOrderTokenSets_IfCurrentPlayerDoesNotHave()
    {
      // Arrange
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "Lannister", 2);
      VassalAssignmentPlayer lannisterVAP = CreateVassalAssignmentPlayer("Lannister", "", 1);

      state.Players.Add(starkVAP);
      state.Players.Add(lannisterVAP);
      state.CurrentPlayerID = starkVAP.PlayerId;

      state.AvailableVassalHouses.Add(HouseType.Arryn);
      state.AvailableVassalHouses.Add(HouseType.Greyjoy);
      state.AvailableVassalHouses.Add(HouseType.Martell);

      // Simulate Stark player selecting Greyjoy and Martell as a vassal house. By doing
      // this, Stark player will consume its 2 order token set, leaving Stark with 0
      // order token set left.

      VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy, HouseType.Martell }
      );

      // Stark player have no order token set left. So when moving to the next player,
      // no order token set should be passed to the next player (Lannister).

      // Act - Move to the next player (Lannister)
      Result result = VASS.MoveToNextPlayer(state);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.IsCompleted, Is.False);

      // Verify that no order token set was passed to Lannister. Lannister should still
      // have 1 order token set (original)
      Assert.That(lannisterVAP.PossesedOrderTokenSets.Count, Is.EqualTo(1));
    }

    [Test]
    public void MoveToNextPlayer_ShouldPassOrderTokenSets_IfCurrentPlayerHave()
    {
      // Arrange
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "Lannister", 2);
      VassalAssignmentPlayer lannisterVAP = CreateVassalAssignmentPlayer("Lannister", "", 1);

      state.Players.Add(starkVAP);
      state.Players.Add(lannisterVAP);
      state.CurrentPlayerID = starkVAP.PlayerId;

      state.AvailableVassalHouses.Add(HouseType.Arryn);
      state.AvailableVassalHouses.Add(HouseType.Greyjoy);
      state.AvailableVassalHouses.Add(HouseType.Martell);

      // Simulate Stark player selecting Greyjoy as a vassal house. By doing this, Stark
      // player will consume just 1 order token set, leaving Stark with 1 order token set
      // left.
      VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy }
      );

      // Stark player still have 1 order token set left. Stark player has decided to not
      // use it, so the order token set should be passed to the next player (Lannister)
      // when moving to the next player.

      // Act - Move to the next player (Lannister)
      Result result = VASS.MoveToNextPlayer(state);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.IsCompleted, Is.False);

      // Verify that the order token set was passed to Lannister. Lannister should now
      // have 2 order token sets (1 original + 1 passed from Stark)
      Assert.That(lannisterVAP.PossesedOrderTokenSets.Count, Is.EqualTo(2));

      // Verify that stark player does not have any order token sets left
      Assert.That(starkVAP.PossesedOrderTokenSets.Count, Is.EqualTo(0));
    }

    [Test]
    public void MoveToNextPlayer_ShouldCompleteState_WhenNextPlayerDoesNotHaveAnyOrderTokenSet()
    {
      // Arrange
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "Lannister", 2);
      VassalAssignmentPlayer lannisterVAP = CreateVassalAssignmentPlayer("Lannister", "", 0);

      state.Players.Add(starkVAP);
      state.Players.Add(lannisterVAP);
      state.CurrentPlayerID = starkVAP.PlayerId;

      state.AvailableVassalHouses.Add(HouseType.Arryn);
      state.AvailableVassalHouses.Add(HouseType.Greyjoy);
      state.AvailableVassalHouses.Add(HouseType.Martell);

      // Simulate Stark player selecting Greyjoy and Arryn as a vassal house. By doing
      // this, Stark player will consume all their order token sets, leaving Stark with 0
      // order token set left.
      VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy, HouseType.Arryn }
      );

      // When moving to next player, Lannister does not have any order token set, so the
      // state should be marked as completed.

      // Act - Move to the next player (Lannister)
      Result result = VASS.MoveToNextPlayer(state);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.IsCompleted, Is.True);
    }

    [Test]
    public void MoveToNextPlayer_ShouldCompleteState_WhenCurrentPlayerIsLast()
    {
      // Arrange
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "Lannister", 1);
      VassalAssignmentPlayer lannisterVAP = CreateVassalAssignmentPlayer("Lannister", "", 1);

      state.Players.Add(starkVAP);
      state.Players.Add(lannisterVAP);
      state.CurrentPlayerID = starkVAP.PlayerId;

      state.AvailableVassalHouses.Add(HouseType.Arryn);
      state.AvailableVassalHouses.Add(HouseType.Greyjoy);
      state.AvailableVassalHouses.Add(HouseType.Martell);

      // Simulate Stark player selecting Greyjoy as a vassal house.
      VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy }
      );

      // Move to the next player (Lannister)
      Result result = VASS.MoveToNextPlayer(state);

      // Simulate Lannister player selecting Arryn as a vassal house.
      VASS.AssignVassals(
        state,
        lannisterVAP.PlayerId,
        new List<HouseType> { HouseType.Arryn }
      );

      // When moving to next player, Lannister is the last player, so the state should be
      // marked as completed.

      result = VASS.MoveToNextPlayer(state);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.IsCompleted, Is.True);
    }

    [Test]
    public void MoveToNextPlayer_ShouldNotCompleteState_WhenPassingOrderTokenSetToNextPlayer()
    {
      // Arrange
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "Lannister", 2);
      VassalAssignmentPlayer lannisterVAP = CreateVassalAssignmentPlayer("Lannister", "", 0);

      state.Players.Add(starkVAP);
      state.Players.Add(lannisterVAP);
      state.CurrentPlayerID = starkVAP.PlayerId;

      state.AvailableVassalHouses.Add(HouseType.Arryn);
      state.AvailableVassalHouses.Add(HouseType.Greyjoy);

      // Simulate Stark player selecting Greyjoy as a vassal house. By doing this, Stark
      // player still has 1 order token sets left.
      VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy }
      );

      // When moving to next player, Lannister will receive 1 order token set from Stark,
      // so the state should not be marked as completed.

      // Act - Move to the next player (Lannister)
      Result result = VASS.MoveToNextPlayer(state);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.IsCompleted, Is.False);
    }

    [Test]
    public void MoveToNextPlayer_ShouldFail_WhenStateIsCompleted()
    {
      // Arrange
      VassalAssignmentState state = new();
      state.IsCompleted = true;

      // Act
      Result result = VASS.MoveToNextPlayer(state);

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void MoveToNextPlayer_ShouldFail_WhenCurrentPlayerIdIsEmpty()
    {
      // Arrange
      VassalAssignmentState state = new();
      state.CurrentPlayerID = string.Empty;

      // Act
      Result result = VASS.MoveToNextPlayer(state);

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void MoveToNextPlayer_ShouldFail_WhenCurrentPlayerIdWasNotFound()
    {
      // Arrange
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "Lannister", 1);
      VassalAssignmentPlayer lannisterVAP = CreateVassalAssignmentPlayer("Lannister", "", 1);

      state.Players.Add(starkVAP);
      state.Players.Add(lannisterVAP);
      state.CurrentPlayerID = starkVAP.PlayerId;

      state.AvailableVassalHouses.Add(HouseType.Arryn);
      state.AvailableVassalHouses.Add(HouseType.Greyjoy);

      // Simulate Stark player selecting Greyjoy as a vassal house.
      VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy }
      );

      // Change the CurrentPlayerID to a non-existent player ID to simulate the scenario
      // where the current player ID was not found in the list of players. Although this
      // is an unlikely scenario, it is still possible if the state was manually modified
      // or corrupted.

      state.CurrentPlayerID = "NonExistentPlayerId";

      // When moving to next player, the method should fail because the current player ID
      // was not found in the list of players.

      // Act
      Result result = VASS.MoveToNextPlayer(state);

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void MoveToNextPlayer_ShouldFail_WhenNextPlayerIdWasNotFound()
    {
      // Arrange
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "NonExistentPlayerId", 1);
      VassalAssignmentPlayer lannisterVAP = CreateVassalAssignmentPlayer("Lannister", "", 1);

      state.Players.Add(starkVAP);
      state.Players.Add(lannisterVAP);
      state.CurrentPlayerID = starkVAP.PlayerId;

      state.AvailableVassalHouses.Add(HouseType.Arryn);
      state.AvailableVassalHouses.Add(HouseType.Greyjoy);

      // Simulate Stark player selecting Greyjoy as a vassal house.
      VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy }
      );

      // When moving to next player, the Stark's NextPlayerId is set to a non-existent
      // player ID, so the method should fail.

      // Act - Move to the next player (NonExistentPlayerId)
      Result result = VASS.MoveToNextPlayer(state);

      // Assert
      Assert.That(result.Success, Is.False);
    }
  }
}
