using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Utests.Game.Services.InfluenceTrackBiddingStateServiceTests
{
  internal class ResolveTieGroupTests
  {
    InfluenceTrackBiddingStateService ITBSService { get; } = new InfluenceTrackBiddingStateService();

    [Test]
    public void ResolveTieGroup_ShouldSuceed_WhenResolutionIsValid()
    {
      // Arrange
      InfluenceTrackBiddingState state = new InfluenceTrackBiddingState();

      // Add some influence positions for houses
      state.HouseInfluencePositions
        .Add(ITBSSUtils.CreateInfluencePositionItem(HouseType.Stark, 1));
      state.HouseInfluencePositions
        .Add(ITBSSUtils.CreateInfluencePositionItem(HouseType.Lannister, 4));

      // Add a tied group of Arryn and Greyjoy at position 2
      state.TiedGroups.Add(
        ITBSSUtils.CreateTiedGroup2(HouseType.Arryn, HouseType.Greyjoy, 2)
      );

      // Create a tied group breaker
      InfluenceTrackTiedGroupBreaker breaker = new()
      {
        StartingPosition = 2,
        HouseOrderedByPriority = new List<HouseType> { HouseType.Greyjoy, HouseType.Arryn }
      };

      // Act
      Result result = ITBSService.ResolveTieGroup(state, breaker);

      // Assert
      Assert.That(result.Success, Is.True);

      // Verify that tied group has been resolved and the positions are updated correctly.
      ITBSSUtils.OrderInfluencePositionItemsByPosition(state.HouseInfluencePositions);
      Assert.That(state.HouseInfluencePositions[0].HouseType, Is.EqualTo(HouseType.Stark));
      Assert.That(state.HouseInfluencePositions[0].InfluencePosition, Is.EqualTo(1));
      Assert.That(state.HouseInfluencePositions[1].HouseType, Is.EqualTo(HouseType.Greyjoy));
      Assert.That(state.HouseInfluencePositions[1].InfluencePosition, Is.EqualTo(2));
      Assert.That(state.HouseInfluencePositions[2].HouseType, Is.EqualTo(HouseType.Arryn));
      Assert.That(state.HouseInfluencePositions[2].InfluencePosition, Is.EqualTo(3));
      Assert.That(state.HouseInfluencePositions[3].HouseType, Is.EqualTo(HouseType.Lannister));
      Assert.That(state.HouseInfluencePositions[3].InfluencePosition, Is.EqualTo(4));

      // Verify the tied group has been removed
      Assert.That(state.TiedGroups, Is.Empty);
    }

    [Test]
    public void ResolveTieGroup_ShouldSucceed_WhenResolutionIsValidForMultipleTiedGroups()
    {
      // Arrange
      InfluenceTrackBiddingState state = new InfluenceTrackBiddingState();

      // Add some influence positions for houses
      state.HouseInfluencePositions
        .Add(ITBSSUtils.CreateInfluencePositionItem(HouseType.Stark, 1));
      state.HouseInfluencePositions
        .Add(ITBSSUtils.CreateInfluencePositionItem(HouseType.Lannister, 4));

      // Add a tied group of Arryn and Greyjoy at position 2
      state.TiedGroups.Add(
        ITBSSUtils.CreateTiedGroup2(HouseType.Arryn, HouseType.Greyjoy, 2)
      );

      // Add another tied group of Martell, Baratheon and Tyrell at position 5
      state.TiedGroups.Add(
        ITBSSUtils.CreateTiedGroup3(HouseType.Martell, HouseType.Baratheon, HouseType.Tyrell, 5)
      );

      // Create a tied group breaker for the first tied group
      InfluenceTrackTiedGroupBreaker breaker1 = new()
      {
        StartingPosition = 2,
        HouseOrderedByPriority = new List<HouseType> { HouseType.Greyjoy, HouseType.Arryn }
      };

      // Act
      Result result1 = ITBSService.ResolveTieGroup(state, breaker1);

      // Assert
      Assert.That(result1.Success, Is.True);

      // Create a tied group breaker for the second tied group
      InfluenceTrackTiedGroupBreaker breaker2 = new()
      {
        StartingPosition = 5,
        HouseOrderedByPriority = new List<HouseType> { HouseType.Baratheon, HouseType.Tyrell, HouseType.Martell }
      };

      // Act
      Result result2 = ITBSService.ResolveTieGroup(state, breaker2);

      // Assert
      Assert.That(result2.Success, Is.True);

      // Verify that all tied groups have been resolved and the positions are updated
      // correctly.
      ITBSSUtils.OrderInfluencePositionItemsByPosition(state.HouseInfluencePositions);
      Assert.That(state.HouseInfluencePositions.Count, Is.EqualTo(7));
      Assert.That(state.HouseInfluencePositions[0].HouseType, Is.EqualTo(HouseType.Stark));
      Assert.That(state.HouseInfluencePositions[0].InfluencePosition, Is.EqualTo(1));
      Assert.That(state.HouseInfluencePositions[1].HouseType, Is.EqualTo(HouseType.Greyjoy));
      Assert.That(state.HouseInfluencePositions[1].InfluencePosition, Is.EqualTo(2));
      Assert.That(state.HouseInfluencePositions[2].HouseType, Is.EqualTo(HouseType.Arryn));
      Assert.That(state.HouseInfluencePositions[2].InfluencePosition, Is.EqualTo(3));
      Assert.That(state.HouseInfluencePositions[3].HouseType, Is.EqualTo(HouseType.Lannister));
      Assert.That(state.HouseInfluencePositions[3].InfluencePosition, Is.EqualTo(4));
      Assert.That(state.HouseInfluencePositions[4].HouseType, Is.EqualTo(HouseType.Baratheon));
      Assert.That(state.HouseInfluencePositions[4].InfluencePosition, Is.EqualTo(5));
      Assert.That(state.HouseInfluencePositions[5].HouseType, Is.EqualTo(HouseType.Tyrell));
      Assert.That(state.HouseInfluencePositions[5].InfluencePosition, Is.EqualTo(6));
      Assert.That(state.HouseInfluencePositions[6].HouseType, Is.EqualTo(HouseType.Martell));
      Assert.That(state.HouseInfluencePositions[6].InfluencePosition, Is.EqualTo(7));
      Assert.That(state.TiedGroups, Is.Empty);
    }

    [Test]
    public void ResolveTieGroup_ShouldFail_WhenNoTiedGroupExistsAtPosition()
    {
      // Arrange
      InfluenceTrackBiddingState state = new InfluenceTrackBiddingState();

      // Add a tied group of Arryn and Greyjoy at position 2
      state.TiedGroups.Add(
        ITBSSUtils.CreateTiedGroup2(HouseType.Arryn, HouseType.Greyjoy, 2)
      );

      // Create a tied group breaker for a non-existent tied group at position 3
      InfluenceTrackTiedGroupBreaker breaker = new()
      {
        StartingPosition = 3,
        HouseOrderedByPriority = new List<HouseType> { HouseType.Greyjoy, HouseType.Arryn }
      };

      // Act
      Result result = ITBSService.ResolveTieGroup(state, breaker);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(state.TiedGroups.Count, Is.EqualTo(1));
    }

    [Test]
    public void ResolveTieGroup_ShouldFail_WhenTiedHousesDoNotMatchBreakerHouses_OneHouseDifferent()
    {
      // Arrange
      InfluenceTrackBiddingState state = new();

      // Add a tied group of Arryn and Greyjoy at position 2
      state.TiedGroups.Add(
        ITBSSUtils.CreateTiedGroup2(HouseType.Arryn, HouseType.Greyjoy, 2)
      );

      // Create a tied group breaker with a house not in the tied group
      InfluenceTrackTiedGroupBreaker breaker = new()
      {
        StartingPosition = 2,
        HouseOrderedByPriority = new List<HouseType> { HouseType.Greyjoy, HouseType.Lannister }
      };

      // Act
      Result result = ITBSService.ResolveTieGroup(state, breaker);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(state.TiedGroups.Count, Is.EqualTo(1));
    }

    [Test]
    public void ResolveTieGroup_ShouldFail_WhenTiedHousesDoNotMatchBreakerHouses_AdditionalHouse()
    {
      // Arrange
      InfluenceTrackBiddingState state = new();

      // Add a tied group of Arryn and Greyjoy at position 2
      state.TiedGroups.Add(
        ITBSSUtils.CreateTiedGroup2(HouseType.Arryn, HouseType.Greyjoy, 2)
      );

      InfluenceTrackTiedGroupBreaker breaker = new()
      {
        StartingPosition = 2,
        HouseOrderedByPriority = new List<HouseType> { HouseType.Arryn, HouseType.Greyjoy, HouseType.Lannister }
      };

      // Act
      Result result = ITBSService.ResolveTieGroup(state, breaker);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(state.TiedGroups.Count, Is.EqualTo(1));
    }

    [Test]
    public void ResolveTieGroup_ShouldFail_WhenTiedHousesDoNotMatchBreakerHouses_HouseIsMissing()
    {
      // Arrange
      InfluenceTrackBiddingState state = new();

      // Add a tied group of Arryn and Greyjoy at position 2
      state.TiedGroups.Add(
        ITBSSUtils.CreateTiedGroup2(HouseType.Arryn, HouseType.Greyjoy, 2)
      );

      InfluenceTrackTiedGroupBreaker breaker = new()
      {
        StartingPosition = 2,
        HouseOrderedByPriority = new List<HouseType> { HouseType.Greyjoy }
      };

      // Act
      Result result = ITBSService.ResolveTieGroup(state, breaker);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(state.TiedGroups.Count, Is.EqualTo(1));
    }
  }
}
