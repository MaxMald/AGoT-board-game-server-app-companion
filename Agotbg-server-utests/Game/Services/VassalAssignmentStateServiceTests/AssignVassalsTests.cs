
using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Utests.Game.Services.VassalAssignmentStateServiceTests
{
  internal class AssignVassalsTests : AVassalAssignmetStateServiceTest
  {
    [Test]
    public void AssignVassals_ShouldAssignVassalsToPlayer_WhenValid()
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

      // Act
      Result result = VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy }
      );

      // Assert
      Assert.That(result.Success, Is.True);

      // Verify that the vassal house was assigned to the player
      Assert.That(starkVAP.SelectedVassalHouses.Count, Is.EqualTo(1));
      AssertHasSelectedVassalHouse(starkVAP, HouseType.Greyjoy);

      // Verify that the vassal house was removed from the available vassal houses
      Assert.That(state.AvailableVassalHouses, Does.Not.Contain(HouseType.Greyjoy));
      Assert.That(state.AvailableVassalHouses.Count, Is.EqualTo(1));
      Assert.That(state.AvailableVassalHouses, Does.Contain(HouseType.Arryn));

      // Verify that one order token sets were consumed from the player
      Assert.That(starkVAP.PossesedOrderTokenSets.Count, Is.EqualTo(0));
    }

    [Test]
    public void AssignVassals_ShouldAssignVassalsToPlayer_WhenMultipleAndValid()
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
      state.AvailableVassalHouses.Add(HouseType.Baratheon);

      // Act
      Result result = VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy, HouseType.Baratheon }
      );

      // Assert
      Assert.That(result.Success, Is.True);

      // Verify that the vassal house was assigned to the player
      Assert.That(starkVAP.SelectedVassalHouses.Count, Is.EqualTo(2));
      AssertHasSelectedVassalHouse(starkVAP, HouseType.Greyjoy);
      AssertHasSelectedVassalHouse(starkVAP, HouseType.Baratheon);

      // Verify that the vassal house was removed from the available vassal houses
      Assert.That(state.AvailableVassalHouses, Does.Not.Contain(HouseType.Greyjoy));
      Assert.That(state.AvailableVassalHouses, Does.Not.Contain(HouseType.Baratheon));
      Assert.That(state.AvailableVassalHouses.Count, Is.EqualTo(1));
      Assert.That(state.AvailableVassalHouses, Does.Contain(HouseType.Arryn));

      // Verify that two order token sets were consumed from the player
      Assert.That(starkVAP.PossesedOrderTokenSets.Count, Is.EqualTo(0));
    }

    [Test]
    public void AssignVassals_ShouldNotSetAsCompleted_WhenAvailableVassals()
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
      state.AvailableVassalHouses.Add(HouseType.Baratheon);

      // Act
      Result result = VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy, HouseType.Arryn }
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.IsCompleted, Is.False);
    }

    [Test]
    public void AssignVassals_ShouldSetAsCompleted_WhenNoAvailableVassals()
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

      // Act
      Result result = VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy, HouseType.Arryn }
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.IsCompleted, Is.True);
    }

    [Test]
    public void AssignVassals_ShouldFail_WhenStateIsCompleted()
    {
      // Arrange
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "Lannister", 1);
      VassalAssignmentPlayer lannisterVAP = CreateVassalAssignmentPlayer("Lannister", "", 1);

      state.IsCompleted = true;

      // Act
      Result result = VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy }
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void AssignVassals_ShouldFail_WhenPlayerIdIsEmpty()
    {
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "Lannister", 2);
      VassalAssignmentPlayer lannisterVAP = CreateVassalAssignmentPlayer("Lannister", "", 0);

      state.Players.Add(starkVAP);
      state.Players.Add(lannisterVAP);
      state.CurrentPlayerID = starkVAP.PlayerId;

      state.AvailableVassalHouses.Add(HouseType.Arryn);
      state.AvailableVassalHouses.Add(HouseType.Greyjoy);

      // Act
      Result result = VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy, HouseType.Arryn }
      );
    }

    [Test]
    public void AssignVassals_ShouldFail_WhenCurrentPlayerIdIsEmpty()
    {
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "Lannister", 2);
      VassalAssignmentPlayer lannisterVAP = CreateVassalAssignmentPlayer("Lannister", "", 0);

      state.Players.Add(starkVAP);
      state.Players.Add(lannisterVAP);
      state.CurrentPlayerID = "";

      state.AvailableVassalHouses.Add(HouseType.Arryn);
      state.AvailableVassalHouses.Add(HouseType.Greyjoy);

      // Act
      Result result = VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy, HouseType.Arryn }
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void AssignVassals_ShouldFail_WhenPlayerIdIsNotCurrentPlayerId()
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

      // Act
      Result result = VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy }
      );

      // Assert
      Assert.That(result.Success, Is.True);

      // Verify that the vassal house was assigned to the player
      Assert.That(starkVAP.SelectedVassalHouses.Count, Is.EqualTo(1));
      AssertHasSelectedVassalHouse(starkVAP, HouseType.Greyjoy);

      // Verify that the vassal house was removed from the available vassal houses
      Assert.That(state.AvailableVassalHouses, Does.Not.Contain(HouseType.Greyjoy));
      Assert.That(state.AvailableVassalHouses.Count, Is.EqualTo(1));
      Assert.That(state.AvailableVassalHouses, Does.Contain(HouseType.Arryn));

      // Verify that one order token sets were consumed from the player
      Assert.That(starkVAP.PossesedOrderTokenSets.Count, Is.EqualTo(0));
    }

    [Test]
    public void AssignVassals_ShouldFail_WhenNoAvailableVassalHouses()
    { 
      // Arrange
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "Lannister", 1);
      VassalAssignmentPlayer lannisterVAP = CreateVassalAssignmentPlayer("Lannister", "", 1);

      state.Players.Add(starkVAP);
      state.Players.Add(lannisterVAP);
      state.CurrentPlayerID = starkVAP.PlayerId;

      // Act
      Result result = VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy }
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void AssignVassals_ShouldFail_WhenVassalsAreNotDistinct()
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
      state.AvailableVassalHouses.Add(HouseType.Baratheon);

      // Act
      Result result = VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy, HouseType.Greyjoy }
      );

      // Assert
      Assert.That(result.Success, Is.False);
      // Verify that no vassal houses were assigned to the player
      Assert.That(starkVAP.SelectedVassalHouses.Count, Is.EqualTo(0));
      // Verify that the available vassal houses remain unchanged
      Assert.That(state.AvailableVassalHouses.Count, Is.EqualTo(3));
    }

    [Test]
    public void AssignVassals_ShouldFail_WhenAvailableHouseIsNotPresent()
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

      // Act
      Result result = VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Baratheon }
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void AssignVassals_ShouldFail_WhenPlayerIdWasNotFound()
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

      // Act
      Result result = VASS.AssignVassals(
        state,
        "NonExistentPlayerId",
        new List<HouseType> { HouseType.Greyjoy }
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void AssignVassals_ShouldFail_WhenPlayerDoesNotHaveEnoughOrderTokenSets()
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

      // Act
      Result result = VASS.AssignVassals(
        state,
        starkVAP.PlayerId,
        new List<HouseType> { HouseType.Greyjoy, HouseType.Arryn }
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }
  }
}
