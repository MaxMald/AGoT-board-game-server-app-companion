using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Utests.Game.Services.VassalAssignmentStateServiceTests
{
  internal class AutomaticallyAssignVassalsForCurrentPlayerTests : AVassalAssignmetStateServiceTest
  {
    [Test]
    public void AutomaticallyAssignVassalsForCurrentPlayer_ShouldNotExceedOrderTokenSets()
    {
      // Arrange
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "", 2);

      state.Players.Add(starkVAP);
      state.CurrentPlayerID = starkVAP.PlayerId;

      state.AvailableVassalHouses.Add(HouseType.Arryn);
      state.AvailableVassalHouses.Add(HouseType.Greyjoy);
      state.AvailableVassalHouses.Add(HouseType.Baratheon);

      // There are 2 order token sets, so only 2 vassals should be assigned, even though
      // there are 3 available vassal houses.

      // Act
      VASS.AutomaticallyAssignVassalsForCurrentPlayer(state);

      // Assert
      Assert.That(starkVAP.PossesedOrderTokenSets.Count, Is.EqualTo(0));
      Assert.That(starkVAP.SelectedVassalHouses.Count, Is.EqualTo(2));
      Assert.That(state.AvailableVassalHouses.Count, Is.EqualTo(1));
    }

    [Test]
    public void AutomaticallyAssignVassalsForCurrentPlayer_ShouldNotExceedAvailableVassalHouses()
    {
      // Arrange
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "", 3);

      state.Players.Add(starkVAP);
      state.CurrentPlayerID = starkVAP.PlayerId;

      state.AvailableVassalHouses.Add(HouseType.Arryn);
      state.AvailableVassalHouses.Add(HouseType.Greyjoy);

      // There are 3 order token sets, but only 2 available vassal houses, so only 2
      // vassals should be assigned.

      // Act
      VASS.AutomaticallyAssignVassalsForCurrentPlayer(state);

      // Assert
      Assert.That(starkVAP.PossesedOrderTokenSets.Count, Is.EqualTo(1));
      Assert.That(starkVAP.SelectedVassalHouses.Count, Is.EqualTo(2));
      Assert.That(state.AvailableVassalHouses.Count, Is.EqualTo(0));
    }

    [Test]
    public void AutomaticallyAssignVassalsForCurrentPlayer_ShouldCompleteState_WhenNoAvailableVassalHouses()
    {
      // Arrange
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "", 1);

      state.Players.Add(starkVAP);
      state.CurrentPlayerID = starkVAP.PlayerId;
      state.AvailableVassalHouses.Add(HouseType.Arryn);

      // Act
      VASS.AutomaticallyAssignVassalsForCurrentPlayer(state);

      // Assert
      Assert.That(state.AvailableVassalHouses.Count, Is.EqualTo(0));
      Assert.That(state.IsCompleted, Is.True);
    }

    [Test]
    public void AutomaticallyAssignVassalsForCurrentPlayer_ShouldNotCompleteState_WhenAvailableVassalHousesRemain()
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
      VASS.AutomaticallyAssignVassalsForCurrentPlayer(state);

      // Assert
      Assert.That(state.AvailableVassalHouses.Count, Is.EqualTo(1));
      Assert.That(state.IsCompleted, Is.False);
    }

    [Test]
    public void AutomaticallyAssignVassalsForCurrentPlayer_ShouldAssignAvaiableVassalHouses()
    {
      // Arrange
      VassalAssignmentState state = new();
      VassalAssignmentPlayer starkVAP = CreateVassalAssignmentPlayer("Stark", "", 2);

      state.Players.Add(starkVAP);
      state.CurrentPlayerID = starkVAP.PlayerId;

      state.AvailableVassalHouses.Add(HouseType.Arryn);
      state.AvailableVassalHouses.Add(HouseType.Greyjoy);
      state.AvailableVassalHouses.Add(HouseType.Baratheon);

      // Act
      VASS.AutomaticallyAssignVassalsForCurrentPlayer(state);

      // Assert
      Assert.That(starkVAP.PossesedOrderTokenSets.Count, Is.EqualTo(0));
      Assert.That(starkVAP.SelectedVassalHouses.Count, Is.EqualTo(2));
      Assert.That(state.AvailableVassalHouses.Count, Is.EqualTo(1));

      // Check that the assigned vassal houses are from the available vassal houses
      List<HouseType> validHouses = new() { HouseType.Baratheon, HouseType.Arryn, HouseType.Greyjoy };
      foreach (var houseDescriptor in starkVAP.SelectedVassalHouses)
        Assert.That(validHouses.Contains(houseDescriptor.HouseType), Is.True);

      // Check that assigned vassal houses are unique
      var assignedHouseTypes = starkVAP.SelectedVassalHouses.Select(h => h.HouseType).ToList();
      Assert.That(assignedHouseTypes.Distinct().Count(), Is.EqualTo(assignedHouseTypes.Count));
    }
  }
}
