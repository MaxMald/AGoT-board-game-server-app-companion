using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;
using Agotbg.Server.Utilities;
using System.Resources;

namespace Agotbg.Server.Utests.Game.Services
{
  internal class IronBankInterestPaymentServiceTests
  {
    [Test]
    public void Initialize_ShouldAddPlayerResultsForAllPlayers()
    {
      // Arrange
      IronBankInterestPaymentState state = new();
      IronBankInterestPaymentStateService service = new();

      PlayerState starkPlayer = new()
      {
        PlayerId = "Stark",
        HouseState = HouseStateFactory.Create(HouseType.Stark)
      };

      PlayerState lannisterPlayer = new()
      {
        PlayerId = "Lannister",
        HouseState = HouseStateFactory.Create(HouseType.Lannister)
      };

      PlayerState greyjoyPlayer = new()
      {
        PlayerId = "Greyjoy",
        HouseState = HouseStateFactory.Create(HouseType.Greyjoy)
      };

      PlayerState targaryenPlayer = new()
      {
        PlayerId = "Targaryen",
        HouseState = HouseStateFactory.Create(HouseType.Targaryen)
      };

      List<PlayerState> players = new()
      {
        starkPlayer, lannisterPlayer, greyjoyPlayer, targaryenPlayer
      };

      // Act
      service.Initialize(state, players);

      // Assert
      List<string> expectedPlayerResultPlayerIds = new()
      { "Stark", "Lannister", "Greyjoy", "Targaryen" };
      List<string> playerResultPlayerIds =
        state.PlayerResults.Select(pr => pr.PlayerId).ToList();
      Assert.That(playerResultPlayerIds, Is.EquivalentTo(expectedPlayerResultPlayerIds));
    }

    [Test]
    public void HasAnyResolvedInterestPayment_ShouldReturnTrue_WhenAnyPlayerHasResolved()
    {
      // Arrange
      IronBankInterestPaymentState state = new();
      IronBankInterestPaymentStateService service = new();

      PlayerState starkPlayer = new()
      {
        PlayerId = "Stark",
        HouseState = HouseStateFactory.Create(HouseType.Stark)
      };

      PlayerState lannisterPlayer = new()
      {
        PlayerId = "Lannister",
        HouseState = HouseStateFactory.Create(HouseType.Lannister)
      };

      List<PlayerState> players = new() { starkPlayer, lannisterPlayer };
      service.Initialize(state, players);

      // Act
      state.PlayerResults[0].Resolved = true; // Mark Stark as resolved

      // Assert
      bool hasAnyResolved = service.HasAnyResolvedInterestPayment(state);
      Assert.That(hasAnyResolved, Is.True);
    }

    [Test]
    public void HasAnyResolvedInterestPayment_ShouldReturnFalse_WhenNoPlayerHasResolved()
    {
      // Arrange
      IronBankInterestPaymentState state = new();
      IronBankInterestPaymentStateService service = new();

      PlayerState starkPlayer = new()
      {
        PlayerId = "Stark",
        HouseState = HouseStateFactory.Create(HouseType.Stark)
      };

      PlayerState lannisterPlayer = new()
      {
        PlayerId = "Lannister",
        HouseState = HouseStateFactory.Create(HouseType.Lannister)
      };

      List<PlayerState> players = new() { starkPlayer, lannisterPlayer };
      service.Initialize(state, players);

      state.PlayerResults[0].Resolved = false; // Mark Stark as unresolved
      state.PlayerResults[1].Resolved = false; // Mark Lannister as unresolved

      // Act
      bool hasAnyResolved = service.HasAnyResolvedInterestPayment(state);

      // Assert
      Assert.That(hasAnyResolved, Is.False);
    }

    [Test]
    public void ResolvePlayerInterestPayment_ShouldResolveSuccessfully_WhenPlayerHasSufficientPowerTokens()
    {
      // Arrange
      IronBankInterestPaymentState state = new();
      IronBankInterestPaymentStateService service = new();

      PlayerState starkPlayer = new()
      {
        PlayerId = "Stark",
        HouseState = HouseStateFactory.Create(HouseType.Stark)
      };

      starkPlayer.HouseState.PowerTokens = 5;
      starkPlayer.HouseState.IronBankLoanInterest = 3;

      List<PlayerState> players = new() { starkPlayer };
      service.Initialize(state, players);

      // Act
      Result result = service.ResolvePlayerInterestPayment(state, starkPlayer);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.PlayerResults[0].Resolved, Is.True);
      Assert.That(state.PlayerResults[0].Succeeded, Is.True);
      Assert.That(state.PlayerResults[0].InterestAmount, Is.EqualTo(3));
      Assert.That(state.PlayerResults[0].InterestPaid, Is.EqualTo(3));
      Assert.That(starkPlayer.HouseState.PowerTokens, Is.EqualTo(2)); // 5 - 3 = 2
    }

    [Test]
    public void ResolvePlayerInterestPayment_ShouldResolveUnsuccessfully_WhenPlayerHasInsufficientPowerTokens()
    {
      // Arrange
      IronBankInterestPaymentState state = new();
      IronBankInterestPaymentStateService service = new();

      PlayerState starkPlayer = new()
      {
        PlayerId = "Stark",
        HouseState = HouseStateFactory.Create(HouseType.Stark)
      };

      starkPlayer.HouseState.PowerTokens = 2;
      starkPlayer.HouseState.IronBankLoanInterest = 3;

      List<PlayerState> players = new() { starkPlayer };
      service.Initialize(state, players);

      // Act
      Result result = service.ResolvePlayerInterestPayment(state, starkPlayer);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.PlayerResults[0].Resolved, Is.True);
      Assert.That(state.PlayerResults[0].Succeeded, Is.False);
      Assert.That(state.PlayerResults[0].InterestAmount, Is.EqualTo(3));
      Assert.That(state.PlayerResults[0].InterestPaid, Is.EqualTo(2));
      Assert.That(starkPlayer.HouseState.PowerTokens, Is.EqualTo(0)); // 2 - 2 = 0
    }

    [Test]
    public void Clear_ShouldResetPlayerResults()
    {
      // Arrange
      IronBankInterestPaymentState state = new();
      IronBankInterestPaymentStateService service = new();

      PlayerState starkPlayer = new()
      {
        PlayerId = "Stark",
        HouseState = HouseStateFactory.Create(HouseType.Stark)
      };

      PlayerState lannisterPlayer = new()
      {
        PlayerId = "Lannister",
        HouseState = HouseStateFactory.Create(HouseType.Lannister)
      };

      List<PlayerState> players = new() { starkPlayer, lannisterPlayer };
      service.Initialize(state, players);

      // Act
      service.Clear(state);

      // Assert
      Assert.That(state.PlayerResults.Count, Is.EqualTo(2));

      Assert.That(state.PlayerResults[0].Resolved, Is.False);
      Assert.That(state.PlayerResults[0].Succeeded, Is.False);
      Assert.That(state.PlayerResults[0].InterestPaid, Is.EqualTo(0));
      Assert.That(state.PlayerResults[1].InterestAmount, Is.EqualTo(0));

      Assert.That(state.PlayerResults[1].Resolved, Is.False);
      Assert.That(state.PlayerResults[1].Succeeded, Is.False);
      Assert.That(state.PlayerResults[1].InterestPaid, Is.EqualTo(0));
      Assert.That(state.PlayerResults[1].InterestAmount, Is.EqualTo(0));
    }
  }
}
