using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;

namespace Agotbg.Server.Utests.Game.Services.InfluenceTrackBiddingStateServiceTests
{
  internal class ProcessBetsAndDeterminePositionsTests
  {
    InfluenceTrackBiddingStateService ITBSService { get; } = new InfluenceTrackBiddingStateService();

    [Test]
    public void ProcessBetsAndDeterminePositions_ShouldDeterminePositionsCorrectly()
    {
      // Arrange
      InfluenceTrackBiddingState state = new InfluenceTrackBiddingState();
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Stark, 5));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Lannister, 3));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Greyjoy, 4));

      // Act
      ITBSService.ProcessBetsAndDeterminePositions(state);

      // Assert
      ITBSSUtils.OrderInfluencePositionItemsByPosition(state.HouseInfluencePositions);

      Assert.That(state.HouseInfluencePositions.Count, Is.EqualTo(3));
      Assert.That(state.HouseInfluencePositions[0].HouseType, Is.EqualTo(HouseType.Stark));
      Assert.That(state.HouseInfluencePositions[0].InfluencePosition, Is.EqualTo(1));
      Assert.That(state.HouseInfluencePositions[1].HouseType, Is.EqualTo(HouseType.Greyjoy));
      Assert.That(state.HouseInfluencePositions[1].InfluencePosition, Is.EqualTo(2));
      Assert.That(state.HouseInfluencePositions[2].HouseType, Is.EqualTo(HouseType.Lannister));
      Assert.That(state.HouseInfluencePositions[2].InfluencePosition, Is.EqualTo(3));
    }

    [Test]
    public void ProcessBetsAndDeterminePositions_ShouldExcludeTargaryenBet()
    {
      // Arrange
      InfluenceTrackBiddingState state = new InfluenceTrackBiddingState();
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Stark, 5));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Lannister, 3));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Targaryen, 4));

      // Act
      ITBSService.ProcessBetsAndDeterminePositions(state);

      // Assert
      ITBSSUtils.OrderInfluencePositionItemsByPosition(state.HouseInfluencePositions);

      Assert.That(state.HouseInfluencePositions.Count, Is.EqualTo(2));
      Assert.That(state.HouseInfluencePositions[0].HouseType, Is.EqualTo(HouseType.Stark));
      Assert.That(state.HouseInfluencePositions[0].InfluencePosition, Is.EqualTo(1));
      Assert.That(state.HouseInfluencePositions[1].HouseType, Is.EqualTo(HouseType.Lannister));
      Assert.That(state.HouseInfluencePositions[1].InfluencePosition, Is.EqualTo(2));
    }

    [Test]
    public void ProcessBetsAndDeterminePositions_ShouldApplyTargaryenGifts()
    {
      // Arrange
      InfluenceTrackBiddingState state = new InfluenceTrackBiddingState();
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Stark, 5));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Greyjoy, 3));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Lannister, 1));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Targaryen, 4));

      state.TargaryenPowerTokenGifts.Add(ITBSSUtils.CreatePowerTokenGift(HouseType.Lannister, 6));
      state.TargaryenPowerTokenGifts.Add(ITBSSUtils.CreatePowerTokenGift(HouseType.Greyjoy, 3));

      // Act
      ITBSService.ProcessBetsAndDeterminePositions(state);

      // Assert
      ITBSSUtils.OrderInfluencePositionItemsByPosition(state.HouseInfluencePositions);

      Assert.That(state.HouseInfluencePositions.Count, Is.EqualTo(3));
      Assert.That(state.HouseInfluencePositions[0].HouseType, Is.EqualTo(HouseType.Lannister));
      Assert.That(state.HouseInfluencePositions[0].InfluencePosition, Is.EqualTo(1));
      Assert.That(state.HouseInfluencePositions[1].HouseType, Is.EqualTo(HouseType.Greyjoy));
      Assert.That(state.HouseInfluencePositions[1].InfluencePosition, Is.EqualTo(2));
      Assert.That(state.HouseInfluencePositions[2].HouseType, Is.EqualTo(HouseType.Stark));
      Assert.That(state.HouseInfluencePositions[2].InfluencePosition, Is.EqualTo(3));
    }

    [Test]
    public void ProcessBetsAndDeterminePositions_ShouldIgnoreTargaryenGifts_WhenReceiverIsNotPresent()
    {
      // Arrange
      InfluenceTrackBiddingState state = new InfluenceTrackBiddingState();
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Stark, 5));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Greyjoy, 3));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Lannister, 1));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Targaryen, 4));

      state.TargaryenPowerTokenGifts.Add(ITBSSUtils.CreatePowerTokenGift(HouseType.Lannister, 6));
      state.TargaryenPowerTokenGifts.Add(ITBSSUtils.CreatePowerTokenGift(HouseType.Baratheon, 3));

      // Act
      ITBSService.ProcessBetsAndDeterminePositions(state);

      // Assert
      ITBSSUtils.OrderInfluencePositionItemsByPosition(state.HouseInfluencePositions);

      Assert.That(state.HouseInfluencePositions.Count, Is.EqualTo(3));
      Assert.That(state.HouseInfluencePositions[0].HouseType, Is.EqualTo(HouseType.Lannister));
      Assert.That(state.HouseInfluencePositions[0].InfluencePosition, Is.EqualTo(1));
      Assert.That(state.HouseInfluencePositions[1].HouseType, Is.EqualTo(HouseType.Stark));
      Assert.That(state.HouseInfluencePositions[1].InfluencePosition, Is.EqualTo(2));
      Assert.That(state.HouseInfluencePositions[2].HouseType, Is.EqualTo(HouseType.Greyjoy));
      Assert.That(state.HouseInfluencePositions[2].InfluencePosition, Is.EqualTo(3));
    }

    [Test]
    public void ProcessBetsAndDeterminePositions_ShouldClearTargaryenGifts()
    {
      // Arrange
      InfluenceTrackBiddingState state = new InfluenceTrackBiddingState();
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Stark, 5));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Greyjoy, 3));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Lannister, 1));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Targaryen, 4));

      state.TargaryenPowerTokenGifts.Add(ITBSSUtils.CreatePowerTokenGift(HouseType.Lannister, 6));
      state.TargaryenPowerTokenGifts.Add(ITBSSUtils.CreatePowerTokenGift(HouseType.Baratheon, 3));

      // Act
      ITBSService.ProcessBetsAndDeterminePositions(state);

      // Assert
      Assert.That(state.TargaryenPowerTokenGifts, Is.Empty);
    }

    [Test]
    public void ProcessBetsAndDeterminePositions_ShouldClearHouseBets()
    {
      // Arrange
      InfluenceTrackBiddingState state = new InfluenceTrackBiddingState();
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Stark, 5));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Greyjoy, 3));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Lannister, 1));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Targaryen, 4));

      state.TargaryenPowerTokenGifts.Add(ITBSSUtils.CreatePowerTokenGift(HouseType.Lannister, 6));
      state.TargaryenPowerTokenGifts.Add(ITBSSUtils.CreatePowerTokenGift(HouseType.Baratheon, 3));

      // Act
      ITBSService.ProcessBetsAndDeterminePositions(state);

      // Assert
      Assert.That(state.HouseBets, Is.Empty);
    }

    [Test]
    public void ProcessBetsAndDeterminePositions_ShouldCreateTiedGroup_WhenHasOneTieGroup()
    {
      // Arrange
      InfluenceTrackBiddingState state = new InfluenceTrackBiddingState();
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Greyjoy, 5));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Baratheon, 6));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Stark, 5));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Lannister, 3));

      // Act
      ITBSService.ProcessBetsAndDeterminePositions(state);

      // Assert - Verify house influence positions only have 2 items, since Stark and
      // Greyjoy are tied for position 2 and 3, and Baratheon is position 1, and Lannister is
      // position 4

      ITBSSUtils.OrderInfluencePositionItemsByPosition(state.HouseInfluencePositions);
      Assert.That(state.HouseInfluencePositions.Count, Is.EqualTo(2));
      Assert.That(state.HouseInfluencePositions[0].HouseType, Is.EqualTo(HouseType.Baratheon));
      Assert.That(state.HouseInfluencePositions[0].InfluencePosition, Is.EqualTo(1));
      Assert.That(state.HouseInfluencePositions[1].HouseType, Is.EqualTo(HouseType.Lannister));
      Assert.That(state.HouseInfluencePositions[1].InfluencePosition, Is.EqualTo(4));

      // Assert - Verify that the tied group is created correctly
      Assert.That(state.TiedGroups.Count, Is.EqualTo(1));
      InfluenceTrackTiedGroup tiedGroup = state.TiedGroups[0];

      Assert.That(tiedGroup.StartingPosition, Is.EqualTo(2));
      Assert.That(tiedGroup.TiedHouses.Count, Is.EqualTo(2));
      Assert.That(tiedGroup.TiedHouses.Contains(HouseType.Stark), Is.True);
      Assert.That(tiedGroup.TiedHouses.Contains(HouseType.Greyjoy), Is.True);
    }

    [Test]
    public void ProcessBetsAndDeterminePositions_ShouldCreateTiedGroups_WhenHasMultipleTieGroups()
    {
      // Arrange
      InfluenceTrackBiddingState state = new InfluenceTrackBiddingState();
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Stark, 6));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Arryn, 3));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Martell, 4));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Lannister, 3));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Baratheon, 1));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Greyjoy, 6));

      // Act
      ITBSService.ProcessBetsAndDeterminePositions(state);

      // Assert - Verify house influence positions only have 2 items,
      //
      // - Baratheon at position 6
      // - Martell at position 3

      ITBSSUtils.OrderInfluencePositionItemsByPosition(state.HouseInfluencePositions);
      Assert.That(state.HouseInfluencePositions.Count, Is.EqualTo(2));
      Assert.That(state.HouseInfluencePositions[0].HouseType, Is.EqualTo(HouseType.Martell));
      Assert.That(state.HouseInfluencePositions[0].InfluencePosition, Is.EqualTo(3));
      Assert.That(state.HouseInfluencePositions[1].HouseType, Is.EqualTo(HouseType.Baratheon));
      Assert.That(state.HouseInfluencePositions[1].InfluencePosition, Is.EqualTo(6));

      // Assert - Verify that the tied groups are created correctly

      ITBSSUtils.OrderTiedGroupsByStartingPosition(state.TiedGroups);
      Assert.That(state.TiedGroups.Count, Is.EqualTo(2));

      InfluenceTrackTiedGroup firstTiedGroup = state.TiedGroups[0];
      Assert.That(firstTiedGroup.StartingPosition, Is.EqualTo(1));
      Assert.That(firstTiedGroup.TiedHouses.Count, Is.EqualTo(2));
      Assert.That(firstTiedGroup.TiedHouses.Contains(HouseType.Stark), Is.True);
      Assert.That(firstTiedGroup.TiedHouses.Contains(HouseType.Greyjoy), Is.True);

      InfluenceTrackTiedGroup secondTiedGroup = state.TiedGroups[1];
      Assert.That(secondTiedGroup.StartingPosition, Is.EqualTo(4));
      Assert.That(secondTiedGroup.TiedHouses.Count, Is.EqualTo(2));
      Assert.That(secondTiedGroup.TiedHouses.Contains(HouseType.Arryn), Is.True);
      Assert.That(secondTiedGroup.TiedHouses.Contains(HouseType.Lannister), Is.True);
    }

    [Test]
    public void ProcessBetsAndDeterminePositions_ShouldNotCreateTiedGroupWithTargaryen_WhenTargaryenIsPresent()
    {
      // Arrange
      InfluenceTrackBiddingState state = new InfluenceTrackBiddingState();
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Greyjoy, 5));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Baratheon, 6));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Stark, 5));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Lannister, 3));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Targaryen, 5));

      // Act
      ITBSService.ProcessBetsAndDeterminePositions(state);

      // Assert - Verify house influence positions only have 2 items, since Stark and
      // Greyjoy are tied for position 2 and 3, and Baratheon is position 1, and Lannister is
      // position 4

      ITBSSUtils.OrderInfluencePositionItemsByPosition(state.HouseInfluencePositions);
      Assert.That(state.HouseInfluencePositions.Count, Is.EqualTo(2));
      Assert.That(state.HouseInfluencePositions[0].HouseType, Is.EqualTo(HouseType.Baratheon));
      Assert.That(state.HouseInfluencePositions[0].InfluencePosition, Is.EqualTo(1));
      Assert.That(state.HouseInfluencePositions[1].HouseType, Is.EqualTo(HouseType.Lannister));
      Assert.That(state.HouseInfluencePositions[1].InfluencePosition, Is.EqualTo(4));

      // Assert - Verify that the tied group is created correctly (no targaryen)
      Assert.That(state.TiedGroups.Count, Is.EqualTo(1));
      InfluenceTrackTiedGroup tiedGroup = state.TiedGroups[0];

      Assert.That(tiedGroup.StartingPosition, Is.EqualTo(2));
      Assert.That(tiedGroup.TiedHouses.Count, Is.EqualTo(2));
      Assert.That(tiedGroup.TiedHouses.Contains(HouseType.Stark), Is.True);
      Assert.That(tiedGroup.TiedHouses.Contains(HouseType.Greyjoy), Is.True);
    }

    [Test]
    public void ProcessBetsAndDeterminePositions_ShouldCreateTiedGroup_WhenTieGeneratedByGifts()
    {
      // Arrange
      InfluenceTrackBiddingState state = new InfluenceTrackBiddingState();
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Stark, 5));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Greyjoy, 3));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Lannister, 1));
      state.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Targaryen, 4));

      state.TargaryenPowerTokenGifts.Add(ITBSSUtils.CreatePowerTokenGift(HouseType.Lannister, 4));
      state.TargaryenPowerTokenGifts.Add(ITBSSUtils.CreatePowerTokenGift(HouseType.Greyjoy, 2));

      // Act
      ITBSService.ProcessBetsAndDeterminePositions(state);

      // Assert - Verify house influence positions has 0 items
      Assert.That(state.HouseInfluencePositions, Is.Empty);

      // Assert - Verify that the tied group is created correctly (Lannister and Greyjoy are tied)
      Assert.That(state.TiedGroups.Count, Is.EqualTo(1));

      InfluenceTrackTiedGroup tiedGroup = state.TiedGroups[0];
      Assert.That(tiedGroup.StartingPosition, Is.EqualTo(1));
      Assert.That(tiedGroup.TiedHouses.Count, Is.EqualTo(3));
      Assert.That(tiedGroup.TiedHouses.Contains(HouseType.Lannister), Is.True);
      Assert.That(tiedGroup.TiedHouses.Contains(HouseType.Greyjoy), Is.True);
      Assert.That(tiedGroup.TiedHouses.Contains(HouseType.Stark), Is.True);
    }
  }
}
