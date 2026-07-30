using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;

namespace Agotbg.Server.Utests.Game.Services
{
  internal class InfluenceTracksInitializerTests
  {
    [Test]
    public void Initialize_ShouldInitializeCorrectly_WhenAllHousesArePlayers()
    {
      // Arrange
      var influenceState = new InfluenceState();
      var playerHouses = new List<HouseType>
      {
        HouseType.Stark,
        HouseType.Greyjoy,
        HouseType.Lannister,
        HouseType.Martell,
        HouseType.Tyrell,
        HouseType.Baratheon,
        HouseType.Arryn,
        HouseType.Targaryen
      };
      var vassalHouses = new List<HouseType>();

      // Act
      InfluenceTracksService.Initialize(influenceState, playerHouses, vassalHouses);

      // Assert
      List<HouseType> expectedIronThroneOrder = new()
      {
        HouseType.Baratheon,
        HouseType.Lannister,
        HouseType.Stark,
        HouseType.Martell,
        HouseType.Greyjoy,
        HouseType.Tyrell,
        HouseType.Arryn,
        HouseType.Targaryen
      };

      List<HouseType> expectedFiefdomsOrder = new()
      {
        HouseType.Greyjoy,
        HouseType.Tyrell,
        HouseType.Martell,
        HouseType.Arryn,
        HouseType.Stark,
        HouseType.Baratheon,
        HouseType.Lannister,
        HouseType.Targaryen
      };

      List<HouseType> expectedKingCourtOrder = new()
      {
        HouseType.Lannister,
        HouseType.Stark,
        HouseType.Martell,
        HouseType.Tyrell,
        HouseType.Arryn,
        HouseType.Baratheon,
        HouseType.Greyjoy,
        HouseType.Targaryen
      };

      Assert.That(influenceState.IronThroneTrack, Is.EqualTo(expectedIronThroneOrder));
      Assert.That(influenceState.FiefdomsTrack, Is.EqualTo(expectedFiefdomsOrder));
      Assert.That(influenceState.KingsCourtTrack, Is.EqualTo(expectedKingCourtOrder));
    }

    [Test]
    public void Initialize_ShouldInitializeCorrectly_WhenSomeHousesAreVassals_AndTargaryenPlays()
    {
      // Arrange
      var influenceState = new InfluenceState();
      var playerHouses = new List<HouseType>
      {
        HouseType.Stark,
        HouseType.Greyjoy,
        HouseType.Lannister,
        HouseType.Martell,
        HouseType.Targaryen
      };

      var vassalHouses = new List<HouseType>
      {
        HouseType.Tyrell,
        HouseType.Baratheon,
        HouseType.Arryn
      };

      // Act
      InfluenceTracksService.Initialize(influenceState, playerHouses, vassalHouses);

      // Assert
      List<HouseType> expectedIronThroneOrder = new()
      {
        HouseType.Lannister,
        HouseType.Stark,
        HouseType.Martell,
        HouseType.Greyjoy,
        HouseType.Baratheon,
        HouseType.Tyrell,
        HouseType.Arryn,
        HouseType.Targaryen
      };

      List<HouseType> expectedFiefdomsOrder = new()
      {
        HouseType.Greyjoy,
        HouseType.Martell,
        HouseType.Stark,
        HouseType.Lannister,
        HouseType.Tyrell,
        HouseType.Arryn,
        HouseType.Baratheon,
        HouseType.Targaryen
      };

      List<HouseType> expectedKingCourtOrder = new()
      {
        HouseType.Lannister,
        HouseType.Stark,
        HouseType.Martell,
        HouseType.Greyjoy,
        HouseType.Tyrell,
        HouseType.Arryn,
        HouseType.Baratheon,
        HouseType.Targaryen
      };

      Assert.That(influenceState.IronThroneTrack, Is.EqualTo(expectedIronThroneOrder));
      Assert.That(influenceState.FiefdomsTrack, Is.EqualTo(expectedFiefdomsOrder));
      Assert.That(influenceState.KingsCourtTrack, Is.EqualTo(expectedKingCourtOrder));
    }

    [Test]
    public void Initialize_ShouldInitializeCorrectly_WhenSomeHousesAreVassals_AndTargaryenDoesNotPlay()
    {
      // Arrange
      var influenceState = new InfluenceState();
      var playerHouses = new List<HouseType>
      {
        HouseType.Stark,
        HouseType.Greyjoy,
        HouseType.Lannister,
        HouseType.Martell
      };

      var vassalHouses = new List<HouseType>
      {
        HouseType.Tyrell,
        HouseType.Baratheon,
        HouseType.Arryn
      };

      // Act
      InfluenceTracksService.Initialize(influenceState, playerHouses, vassalHouses);

      // Assert
      List<HouseType> expectedIronThroneOrder = new()
      {
        HouseType.Lannister,
        HouseType.Stark,
        HouseType.Martell,
        HouseType.Greyjoy,
        HouseType.Baratheon,
        HouseType.Tyrell,
        HouseType.Arryn
      };

      List<HouseType> expectedFiefdomsOrder = new()
      {
        HouseType.Greyjoy,
        HouseType.Martell,
        HouseType.Stark,
        HouseType.Lannister,
        HouseType.Tyrell,
        HouseType.Arryn,
        HouseType.Baratheon
      };

      List<HouseType> expectedKingCourtOrder = new()
      {
        HouseType.Lannister,
        HouseType.Stark,
        HouseType.Martell,
        HouseType.Greyjoy,
        HouseType.Tyrell,
        HouseType.Arryn,
        HouseType.Baratheon
      };

      Assert.That(influenceState.IronThroneTrack, Is.EqualTo(expectedIronThroneOrder));
      Assert.That(influenceState.FiefdomsTrack, Is.EqualTo(expectedFiefdomsOrder));
      Assert.That(influenceState.KingsCourtTrack, Is.EqualTo(expectedKingCourtOrder));
    }
  }
}
