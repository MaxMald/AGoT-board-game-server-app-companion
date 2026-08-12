using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;
using Agotbg.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agotbg.Server.Utests.Game.Services
{
  internal class DragonTokensStateServiceTests
  {
    DragonTokensStateService DragonTokenStateService { get; } = new();

    [Test]
    public void Initialize_ShouldSetAvailablePositionsAndResetTakenTokens()
    {
      // Arrange
      var state = new DragonTokensState();

      // Act
      DragonTokenStateService.Initialize(state);

      // Assert
      Assert.That(state.AvailableDragonTokenPositions, Is.EquivalentTo(new List<byte> { 2, 4, 6, 8, 10 }));
      Assert.That(state.DragonTokensTaken, Is.EqualTo(0));
    }

    [Test]
    public void TakeDragonToken_ShouldRemoveTokenAndIncrementTakenCount_WhenPositionIsAvailable()
    {
      // Arrange
      var state = new DragonTokensState();
      DragonTokenStateService.Initialize(state);
      byte positionToTake = 4;

      // Act
      Result result = DragonTokenStateService.TakeDragonToken(state, positionToTake);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.AvailableDragonTokenPositions, Does.Not.Contain(positionToTake));
      Assert.That(state.DragonTokensTaken, Is.EqualTo(1));
    }

    [Test]
    public void DragonTokensToken_ShouldIncrement_WhenPositionsAreTaken()
    {
      // Arrange
      var state = new DragonTokensState();
      DragonTokenStateService.Initialize(state);
      byte positionToTake1 = 2;
      byte positionToTake2 = 6;

      // Act
      DragonTokenStateService.TakeDragonToken(state, positionToTake1);
      DragonTokenStateService.TakeDragonToken(state, positionToTake2);

      // Assert
      Assert.That(state.DragonTokensTaken, Is.EqualTo(2));
    }

    [Test]
    public void PrepareForNextRound_ShouldRemoveTokenAndIncrementTakenCount_WhenPositionIsAvailable()
    {
      // Arrange
      var state = new DragonTokensState();
      DragonTokenStateService.Initialize(state);
      byte nextRoundPosition = 6;

      // Act
      DragonTokenStateService.PrepareForNextRound(state, nextRoundPosition);

      // Assert
      Assert.That(state.AvailableDragonTokenPositions, Does.Not.Contain(nextRoundPosition));
      Assert.That(state.DragonTokensTaken, Is.EqualTo(1));
    }

    [Test]
    public void PrepareForNextRound_ShouldNotChangeState_WhenPositionIsNotAvailable()
    {
      // Arrange
      var state = new DragonTokensState();
      DragonTokenStateService.Initialize(state);
      byte nextRoundPosition = 3; // Not an available position

      // Act
      DragonTokenStateService.PrepareForNextRound(state, nextRoundPosition);

      // Assert
      Assert.That(state.AvailableDragonTokenPositions, Is.EquivalentTo(new List<byte> { 2, 4, 6, 8, 10 }));
      Assert.That(state.DragonTokensTaken, Is.EqualTo(0));
    }
  }
}
