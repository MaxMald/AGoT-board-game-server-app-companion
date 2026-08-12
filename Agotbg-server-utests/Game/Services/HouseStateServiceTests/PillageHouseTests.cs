using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;

namespace Agotbg.Server.Utests.Game.Services.HouseStateServiceTests
{
  internal class PillageHouseTests
  {
    HouseStateService HouseStateService { get; } = new();

    [Test]
    public void PillageHouse_ShouldReducePowerTokensFromSabotaged_WhenSabotagedHasPowerTokens()
    {
      // Arrange
      HouseState saboteur = HouseStateFactory.Create(HouseType.Stark);
      HouseState sabotaged = HouseStateFactory.Create(HouseType.Greyjoy);

      saboteur.PowerTokens = 5;
      sabotaged.PowerTokens = 5;

      // Act
      HouseStateService.PillageHouse(saboteur, sabotaged);

      // Assert
      Assert.That(sabotaged.PowerTokens, Is.EqualTo(4));
    }

    [Test]
    public void PillageHouse_ShouldNotReducePowerTokensFromSabotaged_WhenSabotagedHasNoPowerTokens()
    {
      // Arrange
      HouseState saboteur = HouseStateFactory.Create(HouseType.Stark);
      HouseState sabotaged = HouseStateFactory.Create(HouseType.Greyjoy);
      saboteur.PowerTokens = 5;
      sabotaged.PowerTokens = 0;

      // Act
      HouseStateService.PillageHouse(saboteur, sabotaged);

      // Assert
      Assert.That(sabotaged.PowerTokens, Is.EqualTo(0));
    }

    [Test]
    public void PillageHouse_ShouldReducePowerTokensFromSabotaged_WhenSaboteourIsVassal()
    {
      // Arrange
      HouseState saboteur = HouseStateFactory.CreateVassal(HouseType.Tyrell);
      HouseState sabotaged = HouseStateFactory.Create(HouseType.Greyjoy);
      sabotaged.PowerTokens = 5;

      // Act
      HouseStateService.PillageHouse(saboteur, sabotaged);

      // Assert
      Assert.That(sabotaged.PowerTokens, Is.EqualTo(4));
    }

    [Test]
    public void PillageHouse_ShouldAddPowerTokenToSaboteur_WhenSabotagedHasPowerTokens()
    {
      // Arrange
      HouseState saboteur = HouseStateFactory.Create(HouseType.Stark);
      HouseState sabotaged = HouseStateFactory.Create(HouseType.Greyjoy);
      saboteur.PowerTokens = 5;
      sabotaged.PowerTokens = 5;

      // Act
      HouseStateService.PillageHouse(saboteur, sabotaged);

      // Assert
      Assert.That(saboteur.PowerTokens, Is.EqualTo(6));
    }

    [Test]
    public void PillageHouse_ShouldAddPowerTokensToSaboteur_WhenSabotagedHasNoPowerTokens()
    {
      // Arrange
      HouseState saboteur = HouseStateFactory.Create(HouseType.Stark);
      HouseState sabotaged = HouseStateFactory.Create(HouseType.Greyjoy);
      saboteur.PowerTokens = 5;
      sabotaged.PowerTokens = 0;

      // Act
      HouseStateService.PillageHouse(saboteur, sabotaged);

      // Assert - Saboteour still gains a power token even if the sabotaged house has no
      // power tokens to steal.
      Assert.That(saboteur.PowerTokens, Is.EqualTo(6));
    }

    [Test]
    public void PillageHouse_ShouldNotExceedMaximumPowerTokens_WhenSaboteurHasMaximumPowerTokens()
    {
      // Arrange
      HouseState saboteur = HouseStateFactory.Create(HouseType.Stark);
      HouseState sabotaged = HouseStateFactory.Create(HouseType.Greyjoy);
      saboteur.PowerTokens = GameConstants.MaximumPowerTokens;
      sabotaged.PowerTokens = 5;

      // Act
      HouseStateService.PillageHouse(saboteur, sabotaged);

      // Assert
      Assert.That(saboteur.PowerTokens, Is.EqualTo(GameConstants.MaximumPowerTokens));
    }

    [Test]
    public void PillageHouse_ShouldNotAddPowerTokensToSaboteur_WhenSaboteurIsVassal()
    {
      // Arrange
      HouseState saboteur = HouseStateFactory.CreateVassal(HouseType.Tyrell);
      HouseState sabotaged = HouseStateFactory.Create(HouseType.Greyjoy);
      sabotaged.PowerTokens = 5;

      // Act
      HouseStateService.PillageHouse(saboteur, sabotaged);

      // Assert
      Assert.That(saboteur.PowerTokens, Is.EqualTo(0));
    }
  }
}
