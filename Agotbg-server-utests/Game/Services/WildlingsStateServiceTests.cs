using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;

namespace Agotbg.Server.Utests.Game.Services
{
  internal class WildlingsStateServiceTests
  {
    WildlingsStateService WildlingStateService { get; } = new();

    [Test]
    public void PrepareForBidding_ShouldSetStrengthWhenBiddingStartedCorrectly()
    {
      // Arrange
      WildlingsState state = CreateState(8);

      // Act
      WildlingStateService.PrepareForBidding(state, false);

      // Assert
      Assert.That(state.StrengthWhenBiddingStarted, Is.EqualTo(8));
    }

    [Test]
    public void PrepareForBidding_ShouldSetStrenghtWhenBiddingStartedCorrectly_WhenPreemptiveRaid()
    {
      // Arrange
      WildlingsState state = CreateState(8);

      // Act - Preemptive raid set to true
      WildlingStateService.PrepareForBidding(state, true);

      // Assert
      Assert.That(state.StrengthWhenBiddingStarted, Is.EqualTo(GameConstants.PreemptiveRaidWildlingStrength));
    }

    [Test]
    public void Clear_ShouldSetNightWatchWinsToFalse()
    {
      // Arrange
      WildlingsState state = CreateState(8);
      state.NightWatchWins = true;
      // Act
      WildlingStateService.ClearBiddingProperties(state);
      // Assert
      Assert.That(state.NightWatchWins, Is.False);
    }

    [Test]
    public void Clear_ShouldSetTotalBetAmountToZero()
    {
      // Arrange
      WildlingsState state = CreateState(8);
      state.TotalBetAmount = 10;
      // Act
      WildlingStateService.ClearBiddingProperties(state);
      // Assert
      Assert.That(state.TotalBetAmount, Is.EqualTo(0));
    }

    [Test]
    public void Clear_ShouldSetIsPreemptiveRaidToFalse()
    {
      // Arrange
      WildlingsState state = CreateState(8);
      state.IsPreemptiveRaid = true;
      // Act
      WildlingStateService.ClearBiddingProperties(state);
      // Assert
      Assert.That(state.IsPreemptiveRaid, Is.False);
    }

    [Test]
    public void Clear_ShouldClearHouseBets()
    {
      // Arrange
      WildlingsState state = CreateState(8);
      state.HouseBets.Add(new HouseBet { HouseType = HouseType.Stark, BetAmount = 5 });
      // Act
      WildlingStateService.ClearBiddingProperties(state);
      // Assert
      Assert.That(state.HouseBets, Is.Empty);
    }

    [Test]
    public void Clear_ShouldSetStrengthWhenBiddingStartedToZero()
    {
      // Arrange
      WildlingsState state = CreateState(8);
      state.StrengthWhenBiddingStarted = 5;
      // Act
      WildlingStateService.ClearBiddingProperties(state);
      // Assert
      Assert.That(state.StrengthWhenBiddingStarted, Is.EqualTo(0));
    }

    [Test]
    public void Clear_ShouldNotChangeStrength()
    {
      // Arrange
      WildlingsState state = CreateState(8);
      // Act
      WildlingStateService.ClearBiddingProperties(state);
      // Assert
      Assert.That(state.Strength, Is.EqualTo(8));
    }

    private WildlingsState CreateState(byte strength)
    {
      return new WildlingsState
      {
        Strength = strength,
        StrengthWhenBiddingStarted = 0,
        NightWatchWins = false,
        TotalBetAmount = 0,
        IsPreemptiveRaid = false,
        HouseBets = new List<HouseBet>()
      };
    }
  }
}
