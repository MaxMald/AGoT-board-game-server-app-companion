using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Utests.Game.Services.HouseStateServiceTests
{
  internal class MakeVassalageRelationshipTests
  {
    HouseStateService HouseStateService { get; } = new HouseStateService();

    [TestCase(HouseType.Stark, HouseType.Greyjoy)]
    [TestCase(HouseType.Lannister, HouseType.Martell)]
    [TestCase(HouseType.Tyrell, HouseType.Baratheon)]
    [TestCase(HouseType.Baratheon, HouseType.Arryn)]
    [TestCase(HouseType.Targaryen, HouseType.Stark)]
    public void MakeVassalageRelationship_ShouldSucceed_WhenValid(
      HouseType commanderHouseType,
      HouseType vassalHouseType
    )
    {
      // Arrange
      HouseState commanderHouse = CreateHouse(commanderHouseType);
      HouseState vassalHouse = CreateVassalHouse(vassalHouseType);

      // Act
      Result result = HouseStateService.MakeVassalageRelationship(
        commanderHouse,
        vassalHouse
      );

      // Assert
      List<HouseType> expectedVassalsHouseType = new List<HouseType> { vassalHouseType };

      Assert.That(result.Success, Is.True);
      Assert.That(commanderHouse.VassalHouseTypes, Is.EquivalentTo(expectedVassalsHouseType));
      Assert.That(commanderHouse.CommanderHouse, Is.EqualTo(HouseType.Undefined));
      Assert.That(vassalHouse.CommanderHouse, Is.EqualTo(commanderHouseType));
      Assert.That(vassalHouse.VassalHouseTypes, Is.Empty);
    }

    [TestCase(HouseType.Stark, HouseType.Greyjoy, HouseType.Lannister)]
    [TestCase(HouseType.Lannister, HouseType.Martell, HouseType.Tyrell)]
    [TestCase(HouseType.Tyrell, HouseType.Baratheon, HouseType.Arryn)]
    [TestCase(HouseType.Baratheon, HouseType.Arryn, HouseType.Targaryen)]
    [TestCase(HouseType.Targaryen, HouseType.Stark, HouseType.Greyjoy)]
    public void MakeVassalageRelationship_ShouldSucceed_WhenValidAndMultiple(
      HouseType commanderHouseType,
      HouseType vassalHouseType1,
      HouseType vassalHouseType2
    )
    {
      // Arrange
      HouseState commanderHouse = CreateHouse(commanderHouseType);
      HouseState vassalHouse1 = CreateVassalHouse(vassalHouseType1);
      HouseState vassalHouse2 = CreateVassalHouse(vassalHouseType2);

      // Act
      Result result1 = HouseStateService.MakeVassalageRelationship(
        commanderHouse,
        vassalHouse1
      );

      Result result2 = HouseStateService.MakeVassalageRelationship(
        commanderHouse,
        vassalHouse2
      );

      // Assert
      List<HouseType> expectedVassalsHouseType = new List<HouseType>
      {
        vassalHouseType1,
        vassalHouseType2
      };

      Assert.That(result1.Success, Is.True);
      Assert.That(result2.Success, Is.True);
      Assert.That(commanderHouse.VassalHouseTypes, Is.EquivalentTo(expectedVassalsHouseType));
      Assert.That(commanderHouse.CommanderHouse, Is.EqualTo(HouseType.Undefined));
      Assert.That(vassalHouse1.CommanderHouse, Is.EqualTo(commanderHouseType));
      Assert.That(vassalHouse1.VassalHouseTypes, Is.Empty);
      Assert.That(vassalHouse2.CommanderHouse, Is.EqualTo(commanderHouseType));
      Assert.That(vassalHouse2.VassalHouseTypes, Is.Empty);
    }

    [Test]
    public void MakeVassalageRelationship_ShouldFail_WhenVassalHouseIsNotVassal()
    {
      // Arrange
      HouseState commanderHouse = CreateHouse(HouseType.Stark);
      HouseState vassalHouse = CreateHouse(HouseType.Greyjoy); // Not a vassal

      // Act
      Result result = HouseStateService.MakeVassalageRelationship(
        commanderHouse,
        vassalHouse
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(commanderHouse.VassalHouseTypes, Is.Empty);
      Assert.That(commanderHouse.CommanderHouse, Is.EqualTo(HouseType.Undefined));
      Assert.That(vassalHouse.CommanderHouse, Is.EqualTo(HouseType.Undefined));
      Assert.That(vassalHouse.VassalHouseTypes, Is.Empty);
    }

    [Test]
    public void MakeVassalageRelationship_ShouldFail_WhenVassalHouseAlreadyHasCommander()
    {
      // Arrange
      HouseState starkCommander = CreateHouse(HouseType.Stark);
      HouseState lannisterCommander = CreateHouse(HouseType.Lannister);
      HouseState greyjoyVassal = CreateVassalHouse(HouseType.Greyjoy);

      // Vassal Relationship Stark -> Greyjoy
      HouseStateService.MakeVassalageRelationship(starkCommander, greyjoyVassal);

      // Act - Attempt to make the same vassal a vassal of lannisterCommander
      Result result = HouseStateService.MakeVassalageRelationship(
        lannisterCommander,
        greyjoyVassal
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(starkCommander.VassalHouseTypes, Is.EquivalentTo(new List<HouseType> { HouseType.Greyjoy }));
      Assert.That(lannisterCommander.VassalHouseTypes, Is.Empty);
      Assert.That(greyjoyVassal.CommanderHouse, Is.EqualTo(HouseType.Stark));
      Assert.That(greyjoyVassal.VassalHouseTypes, Is.Empty);
    }

    [Test]
    public void MakeVassalageRelationship_ShouldFail_WhenCommanderHouseIsVassal()
    {
      // Arrange
      HouseState starkCommander = CreateVassalHouse(HouseType.Stark); // Stark is a vassal
      HouseState greyjoyVassal = CreateVassalHouse(HouseType.Greyjoy);

      // Act
      Result result = HouseStateService.MakeVassalageRelationship(
        starkCommander,
        greyjoyVassal
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(starkCommander.VassalHouseTypes, Is.Empty);
      Assert.That(starkCommander.CommanderHouse, Is.EqualTo(HouseType.Undefined));
      Assert.That(greyjoyVassal.CommanderHouse, Is.EqualTo(HouseType.Undefined));
      Assert.That(greyjoyVassal.VassalHouseTypes, Is.Empty);
    }

    [Test]
    public void MakeVassalageRelationship_ShouldFail_WhenCommanderHouseIsSameAsVassalHouse()
    {
      // Arrange
      HouseState starkHouse = CreateHouse(HouseType.Stark);

      // Act
      Result result = HouseStateService.MakeVassalageRelationship(
        starkHouse,
        starkHouse
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(starkHouse.VassalHouseTypes, Is.Empty);
      Assert.That(starkHouse.CommanderHouse, Is.EqualTo(HouseType.Undefined));
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
