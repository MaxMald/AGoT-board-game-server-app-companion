using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Utests.Game.Services.HouseStateServiceTests
{
  internal class BreakVassalageStatusTests
  {
    HouseStateService HouseStateService { get; } = new HouseStateService();

    [TestCase(HouseType.Stark, HouseType.Greyjoy)]
    [TestCase(HouseType.Lannister, HouseType.Martell)]
    [TestCase(HouseType.Tyrell, HouseType.Baratheon)]
    [TestCase(HouseType.Baratheon, HouseType.Arryn)]
    [TestCase(HouseType.Targaryen, HouseType.Stark)]
    public void BreakVassalageStatus_ShouldSucceed_WhenVassalageRelationshipExists(
      HouseType commanderHouseType,
      HouseType vassalHouseType
    )
    {
      // Arrange
      HouseState commanderHouse = CreateHouse(commanderHouseType);
      HouseState vassalHouse = CreateVassalHouse(vassalHouseType);

      HouseStateService.MakeVassalageRelationship(commanderHouse, vassalHouse);

      // Act
      Result result = HouseStateService.BreakVassalageStatus(commanderHouse, vassalHouse);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(vassalHouse.CommanderHouse, Is.EqualTo(HouseType.Undefined));
      Assert.That(commanderHouse.VassalHouseTypes, Is.Empty);
    }

    [Test]
    public void BreakVassalageStatus_ShouldFail_WhenVassalageReslationshipDoesNotExist()
    {
      // Arrange
      HouseState commanderHouse = CreateHouse(HouseType.Stark);
      HouseState vassalHouse = CreateHouse(HouseType.Greyjoy);

      // Act
      Result result = HouseStateService.BreakVassalageStatus(commanderHouse, vassalHouse);

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void BreakVassalageStatus_ShouldFail_WhenVassalHasNoCommander()
    {
      // Arrange
      HouseState commanderStark = CreateHouse(HouseType.Stark);
      HouseState vassalGreyjoy = CreateVassalHouse(HouseType.Greyjoy);
      HouseState vassalLannister = CreateVassalHouse(HouseType.Lannister);

      // Vassal relationship Stark -> Greyjoy exists
      HouseStateService.MakeVassalageRelationship(commanderStark, vassalGreyjoy);

      // Act - Attempt to break vassalage with Lannister, which has no commander
      Result result = HouseStateService.BreakVassalageStatus(commanderStark, vassalLannister);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(commanderStark.VassalHouseTypes, Contains.Item(HouseType.Greyjoy));
    }

    [Test]
    public void BreakVassalageStatus_ShouldFail_WhenCommanderIsNotTheActualCommander()
    {
      // Arrange
      HouseState starkCommander = CreateHouse(HouseType.Stark);
      HouseState lannisterCommander = CreateHouse(HouseType.Lannister);
      HouseState greyjoyVassal = CreateVassalHouse(HouseType.Greyjoy);
      HouseState martellVassal = CreateVassalHouse(HouseType.Martell);

      // Vassal relationship Stark -> Greyjoy exists
      HouseStateService.MakeVassalageRelationship(starkCommander, greyjoyVassal);

      // Vassal relationship Lannister -> Martell exists
      HouseStateService.MakeVassalageRelationship(lannisterCommander, martellVassal);

      // Act - Try to break with wrong commander
      Result result = HouseStateService.BreakVassalageStatus(lannisterCommander, greyjoyVassal);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(greyjoyVassal.CommanderHouse, Is.EqualTo(HouseType.Stark));
      Assert.That(lannisterCommander.VassalHouseTypes, Does.Not.Contain(HouseType.Greyjoy));
      Assert.That(lannisterCommander.VassalHouseTypes, Does.Contain(HouseType.Martell));
    }

    [Test]
    public void BreakVassalageStatus_ShouldFail_WhenCommanderDoesNotHaveVassalInList()
    {
      // Arrange
      HouseState starkCommander = CreateHouse(HouseType.Stark);
      HouseState vassalHouse = CreateVassalHouse(HouseType.Greyjoy);
      HouseState vassalLannister = CreateVassalHouse(HouseType.Lannister);

      // Vassal relationship Stark -> Greyjoy exists
      HouseStateService.MakeVassalageRelationship(starkCommander, vassalHouse);

      // Act
      Result result = HouseStateService.BreakVassalageStatus(starkCommander, vassalLannister);

      // Assert 
      Assert.That(result.Success, Is.False);
      Assert.That(starkCommander.VassalHouseTypes, Does.Contain(HouseType.Greyjoy));
      Assert.That(starkCommander.VassalHouseTypes, Does.Not.Contain(HouseType.Lannister));
    }

    [Test]
    public void BreakVassalageStatus_ShouldOnlyRemoveSpecifiedVassal_WhenCommanderHasMultipleVassals()
    {
      // Arrange
      HouseState starkCommander = CreateHouse(HouseType.Stark);
      HouseState greyjoyVassal = CreateVassalHouse(HouseType.Greyjoy);
      HouseState lannisterVassal = CreateVassalHouse(HouseType.Lannister);

      // Vassal relationships Stark -> Greyjoy and Stark -> Lannister exist
      HouseStateService.MakeVassalageRelationship(starkCommander, greyjoyVassal);
      HouseStateService.MakeVassalageRelationship(starkCommander, lannisterVassal);

      // Act - Break only greyjoyVassal
      Result result = HouseStateService.BreakVassalageStatus(starkCommander, greyjoyVassal);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(starkCommander.VassalHouseTypes, Does.Not.Contain(HouseType.Greyjoy));
      Assert.That(starkCommander.VassalHouseTypes, Contains.Item(HouseType.Lannister));
      Assert.That(greyjoyVassal.CommanderHouse, Is.EqualTo(HouseType.Undefined));
      Assert.That(lannisterVassal.CommanderHouse, Is.EqualTo(HouseType.Stark));
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
