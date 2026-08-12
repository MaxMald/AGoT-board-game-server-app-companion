using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Utests.Game.Services.HouseStateServiceTests
{
  internal class TransferPowerTokensTests
  {
    HouseStateService HouseStateService { get; } = new();

    [Test]
    public void TransferPowerTokens_ShouldSucceed_WhenHousesAreNotVassals()
    {
      // Arrange
      HouseState stark = HouseStateFactory.Create(HouseType.Stark);
      HouseState greyjoy = HouseStateFactory.Create(HouseType.Greyjoy);

      stark.PowerTokens = 5;
      greyjoy.PowerTokens = 5;

      // Act
      Result result = HouseStateService.TransferPowerTokens(stark, greyjoy, 3);

      // Assert
      Assert.That(result.Success, Is.True);
    }

    [Test]
    public void TransferPowerTokens_ShouldTransferTokens_WhenHousesAreNotVassals()
    {
      // Arrange
      HouseState martell = HouseStateFactory.Create(HouseType.Martell);
      HouseState baratheon = HouseStateFactory.Create(HouseType.Baratheon);
      martell.PowerTokens = 5;
      baratheon.PowerTokens = 5;

      // Act
      HouseStateService.TransferPowerTokens(martell, baratheon, 3);

      // Assert
      Assert.That(martell.PowerTokens, Is.EqualTo(2));
      Assert.That(baratheon.PowerTokens, Is.EqualTo(8));
    }

    [Test]
    public void TransferPowerTokens_ShouldFail_WhenSenderIsVassal()
    {
      // Arrange
      HouseState lannister = HouseStateFactory.CreateVassal(HouseType.Lannister);
      HouseState tyrell = HouseStateFactory.Create(HouseType.Tyrell);
      lannister.PowerTokens = 5;
      tyrell.PowerTokens = 5;

      // Act
      Result result = HouseStateService.TransferPowerTokens(lannister, tyrell, 3);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(lannister.PowerTokens, Is.EqualTo(5));
      Assert.That(tyrell.PowerTokens, Is.EqualTo(5));
    }

    [Test]
    public void TransferPowerTokens_ShouldFail_WhenReceiverIsVassal()
    {
      // Arrange
      HouseState targaryen = HouseStateFactory.Create(HouseType.Targaryen);
      HouseState martell = HouseStateFactory.CreateVassal(HouseType.Martell);
      targaryen.PowerTokens = 5;
      martell.PowerTokens = 5;

      // Act
      Result result = HouseStateService.TransferPowerTokens(targaryen, martell, 3);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(targaryen.PowerTokens, Is.EqualTo(5));
      Assert.That(martell.PowerTokens, Is.EqualTo(5));
    }

    [Test]
    public void TransferPowerTokens_ShouldFail_WhenSenderHasInsufficientTokens()
    {
      // Arrange
      HouseState baratheon = HouseStateFactory.Create(HouseType.Baratheon);
      HouseState greyjoy = HouseStateFactory.Create(HouseType.Greyjoy);
      baratheon.PowerTokens = 2;
      greyjoy.PowerTokens = 5;

      // Act
      Result result = HouseStateService.TransferPowerTokens(baratheon, greyjoy, 3);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(baratheon.PowerTokens, Is.EqualTo(2));
      Assert.That(greyjoy.PowerTokens, Is.EqualTo(5));
    }

    [Test]
    public void TransferPowerTokens_ShouldFail_WhenReceiverHasMaximumTokens()
    {
      // Arrange
      HouseState stark = HouseStateFactory.Create(HouseType.Stark);
      HouseState lannister = HouseStateFactory.Create(HouseType.Lannister);
      stark.PowerTokens = 5;
      lannister.PowerTokens = GameConstants.MaximumPowerTokens;

      // Act
      Result result = HouseStateService.TransferPowerTokens(stark, lannister, 3);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(stark.PowerTokens, Is.EqualTo(5));
      Assert.That(lannister.PowerTokens, Is.EqualTo(GameConstants.MaximumPowerTokens));
    }

    [Test]
    public void TransferPowerTokens_ShouldFail_WhenReceiverIsNearMaximum()
    {
      // Arrange
      HouseState martell = HouseStateFactory.Create(HouseType.Martell);
      HouseState tyrell = HouseStateFactory.Create(HouseType.Tyrell);
      martell.PowerTokens = 5;
      tyrell.PowerTokens = (byte)(GameConstants.MaximumPowerTokens - 1);

      // Act
      Result result = HouseStateService.TransferPowerTokens(martell, tyrell, 3);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(martell.PowerTokens, Is.EqualTo(5));
      Assert.That(tyrell.PowerTokens, Is.EqualTo((byte)(GameConstants.MaximumPowerTokens - 1)));
    }
  }
}
