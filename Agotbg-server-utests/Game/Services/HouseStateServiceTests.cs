using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;

namespace Agotbg.Server.Utests.Game.Services
{
  internal class HouseStateServiceTests
  {
    [TestCase(HouseType.Stark)]
    [TestCase(HouseType.Greyjoy)]
    [TestCase(HouseType.Lannister)]
    [TestCase(HouseType.Martell)]
    [TestCase(HouseType.Tyrell)]
    [TestCase(HouseType.Baratheon)]
    [TestCase(HouseType.Arryn)]
    public void UpdateNumSpecialOrdersBasedOnKingsCourtPosition_ShouldUpdateCorrectly_ForPlayerHouses_ExcludeTargaryen(HouseType type)
    {
      HouseState house = HouseStateService.Create(type);
      for (int trackPosition = 1; trackPosition < 9; trackPosition++)
      {
        // Arrange
        house.KingsCourtTrackPosition = (byte)trackPosition;

        // Act
        HouseStateService.UpdateNumSpecialOrdersBasedOnKingsCourtPosition(house);

        // Assert
        // Expected number of special orders based on the mother of dragons expasion rules.
        byte expectedNumSpecialOrders = trackPosition switch
        {
          1 => 3,
          2 => 3,
          3 => 2,
          4 => 1,
          _ => 0
        };

        Assert.That(house.NumSpecialOrders, Is.EqualTo(expectedNumSpecialOrders), $"Num of Special Orders Failed for track position {trackPosition}, for House: {type}");
      }
    }

    [Test]
    public void UpdateNumSpecialOrdersBasedOnKingsCourtPosition_ShouldUpdateCorrectly_ForTargaryen()
    {
      HouseState house = HouseStateService.Create(HouseType.Targaryen);
      for (int trackPosition = 1; trackPosition < 9; trackPosition++)
      {
        // Arrange
        house.KingsCourtTrackPosition = (byte)trackPosition;
        // Act
        HouseStateService.UpdateNumSpecialOrdersBasedOnKingsCourtPosition(house);
        // Assert
        Assert.That(house.NumSpecialOrders, Is.EqualTo(3), $"Num of Special Orders Failed for track position {trackPosition}, for House Targaryen should be 3");
      }
    }

    [TestCase(HouseType.Stark)]
    [TestCase(HouseType.Greyjoy)]
    [TestCase(HouseType.Lannister)]
    [TestCase(HouseType.Martell)]
    [TestCase(HouseType.Tyrell)]
    [TestCase(HouseType.Baratheon)]
    [TestCase(HouseType.Arryn)]
    public void UpdateNumSpecialOrdersBasedOnKingsCourtPosition_ShouldUpdateCorrectly_ForVassalHouses(HouseType type)
    {
      HouseState house = HouseStateService.CreateVassal(type);
      for (int trackPosition = 1; trackPosition < 9; trackPosition++)
      {
        // Arrange
        house.KingsCourtTrackPosition = (byte)trackPosition;

        // Act
        HouseStateService.UpdateNumSpecialOrdersBasedOnKingsCourtPosition(house);
        Assert.That(house.NumSpecialOrders, Is.EqualTo(0), $"Num of Special Orders Failed for track position {trackPosition}, for House: {type}");
      }
    }
  }
}
