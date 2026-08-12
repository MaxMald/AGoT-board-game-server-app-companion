using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;

namespace Agotbg.Server.Utests.Game.Services
{
  internal class InfluenceTracksServiceTests
  {
    InfluenceTracksService InfluenceTracksService { get; } = new();

    [Test]
    public void Initialize_ShouldInitializeCorrectly_WhenAllHousesArePlayers()
    {
      // Arrange
      HouseState stark = HouseStateFactory.Create(HouseType.Stark);
      HouseState greyjoy = HouseStateFactory.Create(HouseType.Greyjoy);
      HouseState lannister = HouseStateFactory.Create(HouseType.Lannister);
      HouseState martell = HouseStateFactory.Create(HouseType.Martell);
      HouseState tyrell = HouseStateFactory.Create(HouseType.Tyrell);
      HouseState baratheon = HouseStateFactory.Create(HouseType.Baratheon);
      HouseState arryn = HouseStateFactory.Create(HouseType.Arryn);
      HouseState targaryen = HouseStateFactory.Create(HouseType.Targaryen);

      List<HouseState> houses = new()
      {
        stark, greyjoy, lannister, martell, tyrell, baratheon, arryn, targaryen
      };

      // Act
      InfluenceTracksService.Initialize(houses);

      // Assert

      // Expected positions after initialization:
      //      Iron Throne track   Fiefdoms track   King's Court track
      //      -------------------------------------------------------
      // 1.   Baratheon           Greyjoy          Lannister
      // 2.   Lannister           Tyrell           Stark
      // 3.   Stark               Martell          Martell
      // 4.   Martell             Arryn            Tyrell
      // 5.   Greyjoy             Stark            Arryn
      // 6.   Tyrell              Baratheon        Baratheon
      // 7.   Arryn               Lannister        Greyjoy
      // 8.   Targaryen           Targaryen        Targaryen

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
      HouseState stark = HouseStateFactory.Create(HouseType.Stark);
      HouseState greyjoy = HouseStateFactory.Create(HouseType.Greyjoy);
      HouseState lannister = HouseStateFactory.Create(HouseType.Lannister);
      HouseState martell = HouseStateFactory.Create(HouseType.Martell);
      HouseState targaryen = HouseStateFactory.Create(HouseType.Targaryen);

      // Vassal houses
      HouseState tyrell = HouseStateFactory.CreateVassal(HouseType.Tyrell);
      HouseState baratheon = HouseStateFactory.CreateVassal(HouseType.Baratheon);
      HouseState arryn = HouseStateFactory.CreateVassal(HouseType.Arryn);

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
      HouseState stark = HouseStateFactory.Create(HouseType.Stark);
      HouseState greyjoy = HouseStateFactory.Create(HouseType.Greyjoy);
      HouseState lannister = HouseStateFactory.Create(HouseType.Lannister);
      HouseState targaryen = HouseStateFactory.Create(HouseType.Targaryen);

      // Vassal Houses
      HouseState tyrell = HouseStateFactory.CreateVassal(HouseType.Tyrell);
      HouseState arryn = HouseStateFactory.CreateVassal(HouseType.Arryn);

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

    [Test]
    public void MoveInfluenceTrackPositionForHouse_ShouldShiftHousesPositions_WhenMovingUp()
    {
      // Arrange
      HouseState stark = HouseStateFactory.Create(HouseType.Stark);
      HouseState lannister = HouseStateFactory.Create(HouseType.Lannister);
      HouseState greyjoy = HouseStateFactory.Create(HouseType.Greyjoy);
      HouseState martell = HouseStateFactory.Create(HouseType.Martell);
      HouseState tyrell = HouseStateFactory.Create(HouseType.Tyrell);
      HouseState arryn = HouseStateFactory.Create(HouseType.Arryn);
      HouseState baratheon = HouseStateFactory.Create(HouseType.Baratheon);

      List<HouseState> houses = new()
      {
        stark, greyjoy, lannister, martell, tyrell, baratheon, arryn
      };

      // Initialize the influence tracks
      InfluenceTracksService.Initialize(houses);

      //      Iron Throne track   Fiefdoms track   King's Court track
      //      -------------------------------------------------------
      // 1.   Baratheon           Greyjoy          Lannister
      // 2.   Lannister           Tyrell           Stark
      // 3.   Stark               Martell          Martell
      // 4.   Martell             Arryn            Tyrell
      // 5.   Greyjoy             Stark            Arryn
      // 6.   Tyrell              Baratheon        Baratheon
      // 7.   Arryn               Lannister        Greyjoy

      // Act
      InfluenceTracksService.MoveInfluenceTrackPositionForHouse(
        houses,
        HouseType.Stark,
        InfluenceTrackType.IronThrone,
        1
      );

      InfluenceTracksService.MoveInfluenceTrackPositionForHouse(
        houses,
        HouseType.Baratheon,
        InfluenceTrackType.Fiefdom,
        2
      );

      InfluenceTracksService.MoveInfluenceTrackPositionForHouse(
        houses,
        HouseType.Greyjoy,
        InfluenceTrackType.KingsCourt,
        1
      );

      // Assert

      // Expected positions after moving:
      //      Iron Throne track   Fiefdoms track   King's Court track
      //      -------------------------------------------------------
      // 1.   Stark               Greyjoy          Greyjoy
      // 2.   Baratheon           Baratheon        Lannister
      // 3.   Lannister           Tyrell           Stark
      // 4.   Martell             Martell          Martell
      // 5.   Greyjoy             Arryn            Tyrell
      // 6.   Tyrell              Stark            Arryn
      // 7.   Arryn               Lannister        Baratheon

      // Assert - Iron Throne track positions
      Assert.That(stark.IronThroneTrackPosition, Is.EqualTo(1));
      Assert.That(baratheon.IronThroneTrackPosition, Is.EqualTo(2));
      Assert.That(lannister.IronThroneTrackPosition, Is.EqualTo(3));
      Assert.That(martell.IronThroneTrackPosition, Is.EqualTo(4));
      Assert.That(greyjoy.IronThroneTrackPosition, Is.EqualTo(5));
      Assert.That(tyrell.IronThroneTrackPosition, Is.EqualTo(6));
      Assert.That(arryn.IronThroneTrackPosition, Is.EqualTo(7));

      // Assert - Fiefdoms track positions
      Assert.That(greyjoy.FiefdomTrackPosition, Is.EqualTo(1));
      Assert.That(baratheon.FiefdomTrackPosition, Is.EqualTo(2));
      Assert.That(tyrell.FiefdomTrackPosition, Is.EqualTo(3));
      Assert.That(martell.FiefdomTrackPosition, Is.EqualTo(4));
      Assert.That(arryn.FiefdomTrackPosition, Is.EqualTo(5));
      Assert.That(stark.FiefdomTrackPosition, Is.EqualTo(6));
      Assert.That(lannister.FiefdomTrackPosition, Is.EqualTo(7));

      // Assert - King's Court track positions
      Assert.That(greyjoy.KingsCourtTrackPosition, Is.EqualTo(1));
      Assert.That(lannister.KingsCourtTrackPosition, Is.EqualTo(2));
      Assert.That(stark.KingsCourtTrackPosition, Is.EqualTo(3));
      Assert.That(martell.KingsCourtTrackPosition, Is.EqualTo(4));
      Assert.That(tyrell.KingsCourtTrackPosition, Is.EqualTo(5));
      Assert.That(arryn.KingsCourtTrackPosition, Is.EqualTo(6));
      Assert.That(baratheon.KingsCourtTrackPosition, Is.EqualTo(7));
    }

    [Test]
    public void MoveInfluenceTrackPositionForHouse_ShouldShiftHousesPositions_WhenMovingDown()
    {
      // Arrange
      HouseState stark = HouseStateFactory.Create(HouseType.Stark);
      HouseState lannister = HouseStateFactory.Create(HouseType.Lannister);
      HouseState greyjoy = HouseStateFactory.Create(HouseType.Greyjoy);
      HouseState martell = HouseStateFactory.Create(HouseType.Martell);
      HouseState tyrell = HouseStateFactory.Create(HouseType.Tyrell);
      HouseState arryn = HouseStateFactory.Create(HouseType.Arryn);
      HouseState baratheon = HouseStateFactory.Create(HouseType.Baratheon);

      List<HouseState> houses = new()
      {
        stark, greyjoy, lannister, martell, tyrell, baratheon, arryn
      };

      // Initialize the influence tracks
      InfluenceTracksService.Initialize(houses);

      //      Iron Throne track   Fiefdoms track   King's Court track
      //      -------------------------------------------------------
      // 1.   Baratheon           Greyjoy          Lannister
      // 2.   Lannister           Tyrell           Stark
      // 3.   Stark               Martell          Martell
      // 4.   Martell             Arryn            Tyrell
      // 5.   Greyjoy             Stark            Arryn
      // 6.   Tyrell              Baratheon        Baratheon
      // 7.   Arryn               Lannister        Greyjoy

      // Act
      InfluenceTracksService.MoveInfluenceTrackPositionForHouse(
        houses,
        HouseType.Stark,
        InfluenceTrackType.IronThrone,
        5
      );

      InfluenceTracksService.MoveInfluenceTrackPositionForHouse(
        houses,
        HouseType.Greyjoy,
        InfluenceTrackType.Fiefdom,
        7
      );

      InfluenceTracksService.MoveInfluenceTrackPositionForHouse(
        houses,
        HouseType.Stark,
        InfluenceTrackType.KingsCourt,
        6
      );

      // Expected positions after moving:
      //      Iron Throne track   Fiefdoms track   King's Court track
      //      -------------------------------------------------------
      // 1.   Baratheon           Tyrell           Lannister
      // 2.   Lannister           Martell          Martell
      // 3.   Martell             Arryn            Tyrell
      // 4.   Greyjoy             Stark            Arryn
      // 5.   Stark               Baratheon        Baratheon
      // 6.   Tyrell              Lannister        Stark
      // 7.   Arryn               Greyjoy          Greyjoy

      // Assert - Iron Throne track positions
      Assert.That(baratheon.IronThroneTrackPosition, Is.EqualTo(1));
      Assert.That(lannister.IronThroneTrackPosition, Is.EqualTo(2));
      Assert.That(martell.IronThroneTrackPosition, Is.EqualTo(3));
      Assert.That(greyjoy.IronThroneTrackPosition, Is.EqualTo(4));
      Assert.That(stark.IronThroneTrackPosition, Is.EqualTo(5));
      Assert.That(tyrell.IronThroneTrackPosition, Is.EqualTo(6));
      Assert.That(arryn.IronThroneTrackPosition, Is.EqualTo(7));

      // Assert - Fiefdoms track positions
      Assert.That(tyrell.FiefdomTrackPosition, Is.EqualTo(1));
      Assert.That(martell.FiefdomTrackPosition, Is.EqualTo(2));
      Assert.That(arryn.FiefdomTrackPosition, Is.EqualTo(3));
      Assert.That(stark.FiefdomTrackPosition, Is.EqualTo(4));
      Assert.That(baratheon.FiefdomTrackPosition, Is.EqualTo(5));
      Assert.That(lannister.FiefdomTrackPosition, Is.EqualTo(6));
      Assert.That(greyjoy.FiefdomTrackPosition, Is.EqualTo(7));

      // Assert - King's Court track positions
      Assert.That(lannister.KingsCourtTrackPosition, Is.EqualTo(1));
      Assert.That(martell.KingsCourtTrackPosition, Is.EqualTo(2));
      Assert.That(tyrell.KingsCourtTrackPosition, Is.EqualTo(3));
      Assert.That(arryn.KingsCourtTrackPosition, Is.EqualTo(4));
      Assert.That(baratheon.KingsCourtTrackPosition, Is.EqualTo(5));
      Assert.That(stark.KingsCourtTrackPosition, Is.EqualTo(6));
      Assert.That(greyjoy.KingsCourtTrackPosition, Is.EqualTo(7));
    }

    [Test]
    public void MoveInfluenceTrackPositionForHouse_ShouldCapPositionToMax_WhenMovingBeyondMax()
    {
      // Arrange
      HouseState stark = HouseStateFactory.Create(HouseType.Stark);
      HouseState greyjoy = HouseStateFactory.Create(HouseType.Greyjoy);
      HouseState lannister = HouseStateFactory.Create(HouseType.Lannister);
      HouseState martell = HouseStateFactory.Create(HouseType.Martell);

      List<HouseState> houses = new()
      {
        stark, lannister, greyjoy, martell
      };

      // Initialize the influence tracks
      InfluenceTracksService.Initialize(houses);

      //      Iron Throne track   Fiefdoms track   King's Court track
      //      -------------------------------------------------------
      // 1.   Lannister           Greyjoy          Lannister
      // 2.   Stark               Martell          Stark
      // 3.   Martell             Stark            Martell
      // 4.   Greyjoy             Lannister        Greyjoy

      // Act
      InfluenceTracksService.MoveInfluenceTrackPositionForHouse(
        houses,
        HouseType.Stark,
        InfluenceTrackType.IronThrone,
        5
      );

      InfluenceTracksService.MoveInfluenceTrackPositionForHouse(
        houses,
        HouseType.Greyjoy,
        InfluenceTrackType.Fiefdom,
        8
      );

      InfluenceTracksService.MoveInfluenceTrackPositionForHouse(
        houses,
        HouseType.Lannister,
        InfluenceTrackType.KingsCourt,
        20
      );

      // Assert

      // Expected positions after moving (capped at max positions):
      //      Iron Throne track   Fiefdoms track   King's Court track
      //      -------------------------------------------------------
      // 1.   Lannister           Martell          Stark
      // 2.   Martell             Stark            Martell
      // 3.   Greyjoy             Lannister        Greyjoy
      // 4.   Stark               Greyjoy          Lannister

      // Assert - Iron Throne track positions
      Assert.That(lannister.IronThroneTrackPosition, Is.EqualTo(1));
      Assert.That(martell.IronThroneTrackPosition, Is.EqualTo(2));
      Assert.That(greyjoy.IronThroneTrackPosition, Is.EqualTo(3));
      Assert.That(stark.IronThroneTrackPosition, Is.EqualTo(4));

      // Assert - Fiefdoms track positions
      Assert.That(martell.FiefdomTrackPosition, Is.EqualTo(1));
      Assert.That(stark.FiefdomTrackPosition, Is.EqualTo(2));
      Assert.That(lannister.FiefdomTrackPosition, Is.EqualTo(3));
      Assert.That(greyjoy.FiefdomTrackPosition, Is.EqualTo(4));

      // Assert - King's Court track positions
      Assert.That(stark.KingsCourtTrackPosition, Is.EqualTo(1));
      Assert.That(martell.KingsCourtTrackPosition, Is.EqualTo(2));
      Assert.That(greyjoy.KingsCourtTrackPosition, Is.EqualTo(3));
      Assert.That(lannister.KingsCourtTrackPosition, Is.EqualTo(4));
    }

    [Test]
    public void MoveInfluenceTrackPositionForHouse_ShouldThrowException_WhenHouseNotFound()
    {
      // Arrange
      HouseState stark = HouseStateFactory.Create(HouseType.Stark);
      HouseState lannister = HouseStateFactory.Create(HouseType.Lannister);

      List<HouseState> houses = new()
      {
        stark, lannister
      };

      // Initialize the influence tracks
      InfluenceTracksService.Initialize(houses);

      // Act & Assert
      Assert.Throws<ArgumentException>(() =>
        InfluenceTracksService.MoveInfluenceTrackPositionForHouse(
          houses,
          HouseType.Greyjoy, // Greyjoy is not in the list of houses
          InfluenceTrackType.IronThrone,
          1
        )
      );

      Assert.Throws<ArgumentException>(() =>
        InfluenceTracksService.MoveInfluenceTrackPositionForHouse(
          houses,
          HouseType.Martell, // Martell is not in the list of houses
          InfluenceTrackType.Fiefdom,
          2
        )
      );

      Assert.Throws<ArgumentException>(() =>
        InfluenceTracksService.MoveInfluenceTrackPositionForHouse(
          houses,
          HouseType.Arryn, // Arryn is not in the list of houses
          InfluenceTrackType.KingsCourt,
          3
        )
      );
    }

    [Test]
    public void MoveInfluenceTrackPositionForHouse_ShouldThrowException_WhenHouseListIsEmpty()
    {
      // Arrange
      List<HouseState> houses = new();

      // Act & Assert
      Assert.Throws<ArgumentException>(() =>
        InfluenceTracksService.MoveInfluenceTrackPositionForHouse(
          houses,
          HouseType.Stark,
          InfluenceTrackType.IronThrone,
          1
        )
      );

      Assert.Throws<ArgumentException>(() =>
        InfluenceTracksService.MoveInfluenceTrackPositionForHouse(
          houses,
          HouseType.Lannister,
          InfluenceTrackType.Fiefdom,
          2
        )
      );

      Assert.Throws<ArgumentException>(() =>
        InfluenceTracksService.MoveInfluenceTrackPositionForHouse(
          houses,
          HouseType.Greyjoy,
          InfluenceTrackType.KingsCourt,
          3
        )
      );
    }
  }
}
