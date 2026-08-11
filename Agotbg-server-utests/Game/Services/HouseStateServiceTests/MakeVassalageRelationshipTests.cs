using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Utests.Game.Services.HouseStateServiceTests
{
  internal class MakeVassalageRelationshipTests
  {
    [TestCase(HouseType.Stark, HouseType.Greyjoy)]
    [TestCase(HouseType.Lannister, HouseType.Martell)]
    [TestCase(HouseType.Tyrell, HouseType.Baratheon)]
    [TestCase(HouseType.Baratheon, HouseType.Arryn)]
    [TestCase(HouseType.Targaryen, HouseType.Stark)]
    public void MakeVassalageRelationship_ShouldUpdateVassalageCorrectly(
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
