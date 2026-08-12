using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Utests.Game.Services.HouseStateServiceTests
{
  internal class PowerTokensBidTests
  {
    HouseStateService HouseStateService { get; } = new();

    [Test]
    public void SubmitPowerTokens_ShouldSucceed_WhenHasNotSubmitYet()
    {
      // Arrange
      HouseState stark = HouseStateFactory.Create(HouseType.Stark);
      stark.HasBidPowerTokens = false;
      stark.PowerTokens = 5;
      stark.PowerTokensBid = 0;

      // Act
      Result result = HouseStateService.SubmitPowerTokensBid(stark, 3);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(stark.HasBidPowerTokens, Is.True);
      Assert.That(stark.PowerTokensBid, Is.EqualTo(3));
      Assert.That(stark.PowerTokens, Is.EqualTo(2));
    }

    [Test]
    public void SubmitPowerTokens_ShouldFail_WhenAlreadySubmitted()
    {
      // Arrange
      HouseState lannister = HouseStateFactory.Create(HouseType.Lannister);
      lannister.HasBidPowerTokens = true;
      lannister.PowerTokens = 5;
      lannister.PowerTokensBid = 2;

      // Act
      Result result = HouseStateService.SubmitPowerTokensBid(lannister, 3);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(lannister.HasBidPowerTokens, Is.True);
      Assert.That(lannister.PowerTokensBid, Is.EqualTo(2));
      Assert.That(lannister.PowerTokens, Is.EqualTo(5));
    }

    [Test]
    public void SubmitPowerTokens_ShouldFail_WhenNotEnoughTokens()
    {
      // Arrange
      HouseState baratheon = HouseStateFactory.Create(HouseType.Baratheon);
      baratheon.HasBidPowerTokens = false;
      baratheon.PowerTokens = 2;
      baratheon.PowerTokensBid = 0;

      // Act
      Result result = HouseStateService.SubmitPowerTokensBid(baratheon, 3);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(baratheon.HasBidPowerTokens, Is.False);
      Assert.That(baratheon.PowerTokensBid, Is.EqualTo(0));
      Assert.That(baratheon.PowerTokens, Is.EqualTo(2));
    }

    [Test]
    public void CancelPowerTokensBid_ShouldReturnTokens_WhenBidWasSubmitted()
    {
      // Arrange
      HouseState targaryen = HouseStateFactory.Create(HouseType.Targaryen);
      targaryen.HasBidPowerTokens = true;
      targaryen.PowerTokens = 2;
      targaryen.PowerTokensBid = 3;

      // Act
      HouseStateService.CancelPowerTokensBid(targaryen);

      // Assert
      Assert.That(targaryen.HasBidPowerTokens, Is.False);
      Assert.That(targaryen.PowerTokensBid, Is.EqualTo(0));
      Assert.That(targaryen.PowerTokens, Is.EqualTo(5));
    }

    [Test]
    public void CancelPowerTokensBid_ShouldDoNothing_WhenNoBidWasSubmitted()
    {
      // Arrange
      HouseState martell = HouseStateFactory.Create(HouseType.Martell);
      martell.HasBidPowerTokens = false;
      martell.PowerTokens = 4;
      martell.PowerTokensBid = 0;

      // Act
      HouseStateService.CancelPowerTokensBid(martell);

      // Assert
      Assert.That(martell.HasBidPowerTokens, Is.False);
      Assert.That(martell.PowerTokensBid, Is.EqualTo(0));
      Assert.That(martell.PowerTokens, Is.EqualTo(4));
    }

    [Test]
    public void ClearSubmittedPowerTokenBid_ShouldResetBidProperties()
    {
      // Arrange
      HouseState greyjoy = HouseStateFactory.Create(HouseType.Greyjoy);
      greyjoy.HasBidPowerTokens = true;
      greyjoy.PowerTokens = 1;
      greyjoy.PowerTokensBid = 2;

      // Act
      HouseStateService.ClearSubmittedPowerTokenBid(greyjoy);

      // Assert
      Assert.That(greyjoy.HasBidPowerTokens, Is.False);
      Assert.That(greyjoy.PowerTokensBid, Is.EqualTo(0));
    }
  }
}
