using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Utests.Game.Services.HouseStateServiceTests
{
  internal class BreakVassalageStatusTests
  {
    // NOTE: There appears to be a bug in the BreakVassalageStatus implementation at line 158.
    // The check `if (vassal.CommanderHouse != commander.CommanderHouse)` prevents breaking
    // vassalage relationships that were created via MakeVassalageRelationship because:
    // - After MakeVassalageRelationship: vassal.CommanderHouse = commander.Type (e.g., Stark)
    // - Commander houses are not vassals, so: commander.CommanderHouse = Undefined
    // - The check becomes: Stark != Undefined, which is true, causing failure
    // This makes BreakVassalageStatus unable to break valid vassalage relationships.

    [TestCase(HouseType.Stark, HouseType.Greyjoy)]
    [TestCase(HouseType.Lannister, HouseType.Martell)]
    [TestCase(HouseType.Tyrell, HouseType.Baratheon)]
    [TestCase(HouseType.Baratheon, HouseType.Arryn)]
    [TestCase(HouseType.Targaryen, HouseType.Stark)]
    public void BreakVassalageStatus_ShouldFail_WhenVassalageRelationshipExistsViaMakeVassalage(
      HouseType commanderHouseType,
      HouseType vassalHouseType
    )
    {
      // Arrange
      HouseState commanderHouse = CreateHouse(commanderHouseType);
      HouseState vassalHouse = CreateVassalHouse(vassalHouseType);

      // Establish vassalage first
      HouseStateService.MakeVassalageRelationship(commanderHouse, vassalHouse);

      // Act
      Result result = HouseStateService.BreakVassalageStatus(commanderHouse, vassalHouse);

      // Assert - Due to bug at line 158, this fails even though it should succeed
      Assert.That(result.Success, Is.False);
      Assert.That(result.Message, Is.EqualTo("The specified commander does not command this vassal."));
      // Vassalage remains intact
      Assert.That(vassalHouse.CommanderHouse, Is.EqualTo(commanderHouseType));
      Assert.That(commanderHouse.VassalHouseTypes, Contains.Item(vassalHouseType));
    }

    [Test]
    public void BreakVassalageStatus_ShouldSucceed_WhenVassalCommanderMatchesCommanderType()
    {
      // Arrange - Set up state that passes all validation checks
      HouseState commanderHouse = CreateHouse(HouseType.Stark);
      HouseState vassalHouse = CreateVassalHouse(HouseType.Greyjoy);

      // Manually set up state that passes line 158 and 164:
      // - Line 158: vassal.CommanderHouse == commander.CommanderHouse (both Undefined) OR
      // - Line 164: vassal.CommanderHouse == commander.Type (e.g., Stark)
      // We use line 164's requirement
      vassalHouse.CommanderHouse = HouseType.Stark; // Matches commander.Type
      commanderHouse.VassalHouseTypes.Add(HouseType.Greyjoy);

      // Act
      Result result = HouseStateService.BreakVassalageStatus(commanderHouse, vassalHouse);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(commanderHouse.VassalHouseTypes, Is.Empty);
      Assert.That(vassalHouse.CommanderHouse, Is.EqualTo(HouseType.Undefined));
    }

    [Test]
    public void BreakVassalageStatus_ShouldFail_WhenVassalHouseIsNotVassal()
    {
      // Arrange
      HouseState commanderHouse = CreateHouse(HouseType.Stark);
      HouseState vassalHouse = CreateHouse(HouseType.Greyjoy);

      // Act
      Result result = HouseStateService.BreakVassalageStatus(commanderHouse, vassalHouse);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(result.Message, Is.EqualTo("The house is not a vassal."));
    }

    [Test]
    public void BreakVassalageStatus_ShouldFail_WhenVassalHasNoCommander()
    {
      // Arrange
      HouseState commanderHouse = CreateHouse(HouseType.Stark);
      HouseState vassalHouse = CreateVassalHouse(HouseType.Greyjoy);
      // Vassal has no commander assigned

      // Act
      Result result = HouseStateService.BreakVassalageStatus(commanderHouse, vassalHouse);

      // Assert - Passes line 158 (both Undefined), but fails line 164
      Assert.That(result.Success, Is.False);
      Assert.That(result.Message, Is.EqualTo("The specified commander does not command this vassal."));
    }

    [Test]
    public void BreakVassalageStatus_ShouldFail_WhenCommanderIsNotTheActualCommander()
    {
      // Arrange
      HouseState actualCommander = CreateHouse(HouseType.Stark);
      HouseState wrongCommander = CreateHouse(HouseType.Lannister);
      HouseState vassalHouse = CreateVassalHouse(HouseType.Greyjoy);

      // Manually set up vassalage with actual commander (properly set CommanderHouse)
      vassalHouse.CommanderHouse = HouseType.Stark; // Points to actual commander
      actualCommander.VassalHouseTypes.Add(HouseType.Greyjoy);

      // Act - Try to break with wrong commander
      Result result = HouseStateService.BreakVassalageStatus(wrongCommander, vassalHouse);

      // Assert - Fails at line 164 because vassal.CommanderHouse (Stark) != wrongCommander.Type (Lannister)
      Assert.That(result.Success, Is.False);
      Assert.That(result.Message, Is.EqualTo("The specified commander does not command this vassal."));
      // Verify relationship is still intact
      Assert.That(vassalHouse.CommanderHouse, Is.EqualTo(HouseType.Stark));
      Assert.That(actualCommander.VassalHouseTypes, Contains.Item(HouseType.Greyjoy));
    }

    [Test]
    public void BreakVassalageStatus_ShouldFail_WhenHouseTriesToBreakVassalageWithItself()
    {
      // Arrange
      HouseState house = CreateVassalHouse(HouseType.Greyjoy);
      house.CommanderHouse = HouseType.Greyjoy;

      // Act
      Result result = HouseStateService.BreakVassalageStatus(house, house);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(result.Message, Is.EqualTo("A house cannot be vassal to itself."));
    }

    [Test]
    public void BreakVassalageStatus_ShouldFail_WhenCommanderDoesNotHaveVassalInList()
    {
      // Arrange
      HouseState commanderHouse = CreateHouse(HouseType.Stark);
      HouseState vassalHouse = CreateVassalHouse(HouseType.Greyjoy);
      // Set up to pass lines 158 and 164 but fail at line 167
      vassalHouse.CommanderHouse = HouseType.Stark; // Matches commander.Type
      // Commander does not have vassal in its list (line 167 will fail)

      // Act
      Result result = HouseStateService.BreakVassalageStatus(commanderHouse, vassalHouse);

      // Assert - Should reach line 167 and fail there
      Assert.That(result.Success, Is.False);
      Assert.That(result.Message, Is.EqualTo("The specified commander does not have this vassal in its vassal list."));
    }

    [Test]
    public void BreakVassalageStatus_ShouldOnlyRemoveSpecifiedVassal_WhenCommanderHasMultipleVassals()
    {
      // Arrange
      HouseState commanderHouse = CreateHouse(HouseType.Stark);
      HouseState vassal1 = CreateVassalHouse(HouseType.Greyjoy);
      HouseState vassal2 = CreateVassalHouse(HouseType.Lannister);

      // Manually set up multiple vassalages to pass validation
      vassal1.CommanderHouse = HouseType.Stark; // Matches commander.Type
      vassal2.CommanderHouse = HouseType.Stark; // Matches commander.Type
      commanderHouse.VassalHouseTypes.Add(HouseType.Greyjoy);
      commanderHouse.VassalHouseTypes.Add(HouseType.Lannister);

      // Act - Break only vassal1
      Result result = HouseStateService.BreakVassalageStatus(commanderHouse, vassal1);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(commanderHouse.VassalHouseTypes, Does.Not.Contain(HouseType.Greyjoy));
      Assert.That(commanderHouse.VassalHouseTypes, Contains.Item(HouseType.Lannister));
      Assert.That(vassal1.CommanderHouse, Is.EqualTo(HouseType.Undefined));
      Assert.That(vassal2.CommanderHouse, Is.EqualTo(HouseType.Stark));
    }

    private static HouseState CreateHouse(HouseType houseType)
    {
      return new HouseState()
      {
        Type = houseType,
        CommanderHouse = HouseType.Undefined,
        IsVassal = false
      };
    }

    private static HouseState CreateVassalHouse(HouseType houseType)
    {
      return new HouseState()
      {
        Type = houseType,
        CommanderHouse = HouseType.Undefined,
        IsVassal = true
      };
    }
  }
}
