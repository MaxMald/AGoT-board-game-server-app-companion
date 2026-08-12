using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;

namespace Agotbg.Server.Utests.Game.Services.InfluenceTrackBiddingStateServiceTests
{
  internal class PrepareTests
  {
    InfluenceTrackBiddingStateService ITBSService { get; } = new InfluenceTrackBiddingStateService();

    [Test]
    public void Prepare_ShouldClearStateAndSetInfluenceTrackType()
    {
      InfluenceTrackBiddingState ITBS = new InfluenceTrackBiddingState();

      // Arrange
      ITBS.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Stark, 5));
      ITBS.HouseBets.Add(ITBSSUtils.CreateBet(HouseType.Lannister, 3));
      ITBS.TargaryenPowerTokenGifts.Add(ITBSSUtils.CreatePowerTokenGift(HouseType.Targaryen, 2));
      ITBS.TargaryenPowerTokenGifts.Add(ITBSSUtils.CreatePowerTokenGift(HouseType.Martell, 1));
      ITBS.TiedGroups.Add(ITBSSUtils.CreateTiedGroup2(HouseType.Stark, HouseType.Greyjoy, 1));
      ITBS.InfluenceTrackType = InfluenceTrackType.None;
      ITBS.HouseInfluencePositions.Add(ITBSSUtils.CreateInfluencePositionItem(HouseType.Stark, 1));

      // Act
      ITBSService.Prepare(ITBS, InfluenceTrackType.IronThrone);

      // Assert
      Assert.That(ITBS.HouseBets, Is.Empty);
      Assert.That(ITBS.InfluenceTrackType, Is.EqualTo(InfluenceTrackType.IronThrone));
      Assert.That(ITBS.TargaryenPowerTokenGifts, Is.Empty);
      Assert.That(ITBS.TiedGroups, Is.Empty);
      Assert.That(ITBS.HouseInfluencePositions, Is.Empty);
    }
  }
}
