using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;

namespace Agotbg.Server.Utests.Game.Services
{
  internal class InfluenceTracksServiceTests
  {
    [Test]
    public void Initialize_ShouldInitializeCorrectly_WhenAllHousesArePlayers()
    {
      // Arrange
      HouseState stark = HouseStateService.Create(HouseType.Stark);
      HouseState greyjoy = HouseStateService.Create(HouseType.Greyjoy);
      HouseState lannister = HouseStateService.Create(HouseType.Lannister);
      HouseState martell = HouseStateService.Create(HouseType.Martell);
      HouseState tyrell = HouseStateService.Create(HouseType.Tyrell);
      HouseState baratheon = HouseStateService.Create(HouseType.Baratheon);
      HouseState arryn = HouseStateService.Create(HouseType.Arryn);
      HouseState targaryen = HouseStateService.Create(HouseType.Targaryen);

      List<HouseState> houses = new()
      {
        stark, greyjoy, lannister, martell, tyrell, baratheon, arryn, targaryen
      };

      // Act
      InfluenceTracksService.Initialize(houses);

      // Assert
      Assert.That(baratheon.IronThroneTrackPosition, Is.EqualTo(1), "Baratheon should be at position 1 on the Iron Throne track.");
      Assert.That(lannister.IronThroneTrackPosition, Is.EqualTo(2), "Lannister should be at position 2 on the Iron Throne track.");
      Assert.That(stark.IronThroneTrackPosition, Is.EqualTo(3), "Stark should be at position 3 on the Iron Throne track.");
      Assert.That(martell.IronThroneTrackPosition, Is.EqualTo(4), "Martell should be at position 4 on the Iron Throne track.");
      Assert.That(greyjoy.IronThroneTrackPosition, Is.EqualTo(5), "Greyjoy should be at position 5 on the Iron Throne track.");
      Assert.That(tyrell.IronThroneTrackPosition, Is.EqualTo(6), "Tyrell should be at position 6 on the Iron Throne track.");
      Assert.That(arryn.IronThroneTrackPosition, Is.EqualTo(7), "Arryn should be at position 7 on the Iron Throne track.");
      Assert.That(targaryen.IronThroneTrackPosition, Is.EqualTo(8), "Targaryen should be at position 8 on the Iron Throne track.");

      Assert.That(greyjoy.FiefdomTrackPosition, Is.EqualTo(1), "Greyjoy should be at position 1 on the Fiefdoms track.");
      Assert.That(tyrell.FiefdomTrackPosition, Is.EqualTo(2), "Tyrell should be at position 2 on the Fiefdoms track.");
      Assert.That(martell.FiefdomTrackPosition, Is.EqualTo(3), "Martell should be at position 3 on the Fiefdoms track.");
      Assert.That(arryn.FiefdomTrackPosition, Is.EqualTo(4), "Arryn should be at position 4 on the Fiefdoms track.");
      Assert.That(stark.FiefdomTrackPosition, Is.EqualTo(5), "Stark should be at position 5 on the Fiefdoms track.");
      Assert.That(baratheon.FiefdomTrackPosition, Is.EqualTo(6), "Baratheon should be at position 6 on the Fiefdoms track.");
      Assert.That(lannister.FiefdomTrackPosition, Is.EqualTo(7), "Lannister should be at position 7 on the Fiefdoms track.");
      Assert.That(targaryen.FiefdomTrackPosition, Is.EqualTo(8), "Targaryen should be at position 8 on the Fiefdoms track.");

      Assert.That(lannister.KingsCourtTrackPosition, Is.EqualTo(1), "Lannister should be at position 1 on the King's Court track.");
      Assert.That(stark.KingsCourtTrackPosition, Is.EqualTo(2), "Stark should be at position 2 on the King's Court track.");
      Assert.That(martell.KingsCourtTrackPosition, Is.EqualTo(3), "Martell should be at position 3 on the King's Court track.");
      Assert.That(tyrell.KingsCourtTrackPosition, Is.EqualTo(4), "Tyrell should be at position 4 on the King's Court track.");
      Assert.That(arryn.KingsCourtTrackPosition, Is.EqualTo(5), "Arryn should be at position 5 on the King's Court track.");
      Assert.That(baratheon.KingsCourtTrackPosition, Is.EqualTo(6), "Baratheon should be at position 6 on the King's Court track.");
      Assert.That(greyjoy.KingsCourtTrackPosition, Is.EqualTo(7), "Greyjoy should be at position 7 on the King's Court track.");
      Assert.That(targaryen.KingsCourtTrackPosition, Is.EqualTo(8), "Targaryen should be at position 8 on the King's Court track.");
    }

    [Test]
    public void Initialize_ShouldInitializeCorrectly_WhenSomeHousesAreVassals_AndTargaryenPlays()
    {
      // Arrange
      // Player houses
      HouseState stark = HouseStateService.Create(HouseType.Stark);
      HouseState greyjoy = HouseStateService.Create(HouseType.Greyjoy);
      HouseState lannister = HouseStateService.Create(HouseType.Lannister);
      HouseState martell = HouseStateService.Create(HouseType.Martell);
      HouseState targaryen = HouseStateService.Create(HouseType.Targaryen);

      // Vassal houses
      HouseState tyrell = HouseStateService.CreateVassal(HouseType.Tyrell);
      HouseState baratheon = HouseStateService.CreateVassal(HouseType.Baratheon);
      HouseState arryn = HouseStateService.CreateVassal(HouseType.Arryn);

      List<HouseState> houses = new()
      {
        stark, greyjoy, lannister, martell, tyrell, baratheon, arryn, targaryen
      };

      // Act
      InfluenceTracksService.Initialize(houses);

      // Assert
      Assert.That(lannister.IronThroneTrackPosition, Is.EqualTo(1), "Lannister should be at position 1 on the Iron Throne track.");
      Assert.That(stark.IronThroneTrackPosition, Is.EqualTo(2), "Stark should be at position 2 on the Iron Throne track.");
      Assert.That(martell.IronThroneTrackPosition, Is.EqualTo(3), "Martell should be at position 3 on the Iron Throne track.");
      Assert.That(greyjoy.IronThroneTrackPosition, Is.EqualTo(4), "Greyjoy should be at position 4 on the Iron Throne track.");
      Assert.That(baratheon.IronThroneTrackPosition, Is.EqualTo(5), "Baratheon should be at position 5 on the Iron Throne track.");
      Assert.That(tyrell.IronThroneTrackPosition, Is.EqualTo(6), "Tyrell should be at position 6 on the Iron Throne track.");
      Assert.That(arryn.IronThroneTrackPosition, Is.EqualTo(7), "Arryn should be at position 7 on the Iron Throne track.");
      Assert.That(targaryen.IronThroneTrackPosition, Is.EqualTo(8), "Targaryen should be at position 8 on the Iron Throne track.");

      Assert.That(greyjoy.FiefdomTrackPosition, Is.EqualTo(1), "Greyjoy should be at position 1 on the Fiefdoms track.");
      Assert.That(martell.FiefdomTrackPosition, Is.EqualTo(2), "Martell should be at position 2 on the Fiefdoms track.");
      Assert.That(stark.FiefdomTrackPosition, Is.EqualTo(3), "Stark should be at position 3 on the Fiefdoms track.");
      Assert.That(lannister.FiefdomTrackPosition, Is.EqualTo(4), "Lannister should be at position 4 on the Fiefdoms track.");
      Assert.That(tyrell.FiefdomTrackPosition, Is.EqualTo(5), "Tyrell should be at position 5 on the Fiefdoms track.");
      Assert.That(arryn.FiefdomTrackPosition, Is.EqualTo(6), "Arryn should be at position 6 on the Fiefdoms track.");
      Assert.That(baratheon.FiefdomTrackPosition, Is.EqualTo(7), "Baratheon should be at position 7 on the Fiefdoms track.");
      Assert.That(targaryen.FiefdomTrackPosition, Is.EqualTo(8), "Targaryen should be at position 8 on the Fiefdoms track.");

      Assert.That(lannister.KingsCourtTrackPosition, Is.EqualTo(1), "Lannister should be at position 1 on the King's Court track.");
      Assert.That(stark.KingsCourtTrackPosition, Is.EqualTo(2), "Stark should be at position 2 on the King's Court track.");
      Assert.That(martell.KingsCourtTrackPosition, Is.EqualTo(3), "Martell should be at position 3 on the King's Court track.");
      Assert.That(greyjoy.KingsCourtTrackPosition, Is.EqualTo(4), "Greyjoy should be at position 4 on the King's Court track.");
      Assert.That(tyrell.KingsCourtTrackPosition, Is.EqualTo(5), "Tyrell should be at position 5 on the King's Court track.");
      Assert.That(arryn.KingsCourtTrackPosition, Is.EqualTo(6), "Arryn should be at position 6 on the King's Court track.");
      Assert.That(baratheon.KingsCourtTrackPosition, Is.EqualTo(7), "Baratheon should be at position 7 on the King's Court track.");
      Assert.That(targaryen.KingsCourtTrackPosition, Is.EqualTo(8), "Targaryen should be at position 8 on the King's Court track.");
    }

    [Test]
    public void Initialize_ShouldInitializeCorrectly_WhenFewerThanEightHousesArePlaying()
    {
      // Arrange
      // Player Houses
      HouseState stark = HouseStateService.Create(HouseType.Stark);
      HouseState greyjoy = HouseStateService.Create(HouseType.Greyjoy);
      HouseState lannister = HouseStateService.Create(HouseType.Lannister);
      HouseState targaryen = HouseStateService.Create(HouseType.Targaryen);

      // Vassal Houses
      HouseState tyrell = HouseStateService.CreateVassal(HouseType.Tyrell);
      HouseState arryn = HouseStateService.CreateVassal(HouseType.Arryn);

      List<HouseState> houses = new()
      {
        stark, greyjoy, lannister, tyrell, arryn, targaryen
      };

      // Act
      InfluenceTracksService.Initialize(houses);

      // Assert
      Assert.That(lannister.IronThroneTrackPosition, Is.EqualTo(1), "Lannister should be at position 1 on the Iron Throne track.");
      Assert.That(stark.IronThroneTrackPosition, Is.EqualTo(2), "Stark should be at position 2 on the Iron Throne track.");
      Assert.That(greyjoy.IronThroneTrackPosition, Is.EqualTo(3), "Greyjoy should be at position 3 on the Iron Throne track.");
      Assert.That(tyrell.IronThroneTrackPosition, Is.EqualTo(4), "Tyrell should be at position 4 on the Iron Throne track.");
      Assert.That(arryn.IronThroneTrackPosition, Is.EqualTo(5), "Arryn should be at position 5 on the Iron Throne track.");
      Assert.That(targaryen.IronThroneTrackPosition, Is.EqualTo(8), "Targaryen should be at position 8 on the Iron Throne track.");

      Assert.That(greyjoy.FiefdomTrackPosition, Is.EqualTo(1), "Greyjoy should be at position 1 on the Fiefdoms track.");
      Assert.That(stark.FiefdomTrackPosition, Is.EqualTo(2), "Stark should be at position 2 on the Fiefdoms track.");
      Assert.That(lannister.FiefdomTrackPosition, Is.EqualTo(3), "Lannister should be at position 3 on the Fiefdoms track.");
      Assert.That(tyrell.FiefdomTrackPosition, Is.EqualTo(4), "Tyrell should be at position 4 on the Fiefdoms track.");
      Assert.That(arryn.FiefdomTrackPosition, Is.EqualTo(5), "Arryn should be at position 5 on the Fiefdoms track.");
      Assert.That(targaryen.FiefdomTrackPosition, Is.EqualTo(8), "Targaryen should be at position 8 on the Fiefdoms track.");

      Assert.That(lannister.KingsCourtTrackPosition, Is.EqualTo(1), "Lannister should be at position 1 on the King's Court track.");
      Assert.That(stark.KingsCourtTrackPosition, Is.EqualTo(2), "Stark should be at position 2 on the King's Court track.");
      Assert.That(greyjoy.KingsCourtTrackPosition, Is.EqualTo(3), "Greyjoy should be at position 3 on the King's Court track.");
      Assert.That(tyrell.KingsCourtTrackPosition, Is.EqualTo(4), "Tyrell should be at position 4 on the King's Court track.");
      Assert.That(arryn.KingsCourtTrackPosition, Is.EqualTo(5), "Arryn should be at position 5 on the King's Court track.");
      Assert.That(targaryen.KingsCourtTrackPosition, Is.EqualTo(8), "Targaryen should be at position 8 on the King's Court track.");
    }
  }
}
