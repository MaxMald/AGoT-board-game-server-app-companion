using Agotbg.Server.Game.Model;
using System.Collections.Generic;

namespace Agotbg.Server.Utests.Game.Services.InfluenceTrackBiddingStateServiceTests
{
  internal class ITBSSUtils
  {
    internal static HouseBet CreateBet(HouseType houseType, byte betAmount)
    {
      return new HouseBet
      {
        HouseType = houseType,
        BetAmount = betAmount
      };
    }

    internal static PowerTokenGift CreatePowerTokenGift(
      HouseType houseType,
      byte giftAmount
    )
    {
      return new PowerTokenGift
      {
        Receiver = houseType,
        Amount = giftAmount
      };
    }

    internal static InfluenceTrackTiedGroup CreateTiedGroup2(
      HouseType house1,
      HouseType house2,
      byte position
     )
    {
      return new InfluenceTrackTiedGroup
      {
        StartingPosition = position,
        TiedHouses = new List<HouseType> { house1, house2 }
      };
    }

    internal static InfluenceTrackTiedGroup CreateTiedGroup3(
      HouseType house1,
      HouseType house2,
      HouseType house3,
      byte position
    )
    {
      return new InfluenceTrackTiedGroup
      {
        StartingPosition = position,
        TiedHouses = new List<HouseType> { house1, house2, house3 }
      };
    }

    internal static HouseInfluencePositionItem CreateInfluencePositionItem(
      HouseType houseType,
      byte position
    )
    {
      return new HouseInfluencePositionItem
      {
        HouseType = houseType,
        InfluencePosition = position
      };
    }

    internal static void OrderInfluencePositionItemsByPosition(
      List<HouseInfluencePositionItem> items
    )
    {
      items.Sort((item1, item2) => item1.InfluencePosition.CompareTo(item2.InfluencePosition));
    }

    internal static void OrderTiedGroupsByStartingPosition(
      List<InfluenceTrackTiedGroup> groups
    )
    {
      groups.Sort((group1, group2) => group1.StartingPosition.CompareTo(group2.StartingPosition));
    }
  }
}
