using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services
{
  public static class InfluenceTracksInitializer
  {
    public static void Initialize(
      InfluenceState influenceState,
      List<HouseType> playerHouses,
      List<HouseType> vassalHouses
    )
    {
      bool hasTargaryenPlayer = playerHouses.Contains(HouseType.Targaryen);

      // Remove Targaryen from sorting, as it is always last in all influence tracks.
      if (hasTargaryenPlayer)
        playerHouses.RemoveAll(h => h == HouseType.Targaryen);

      // IRON THRONE TRACK
      playerHouses.Sort((h1, h2) => GetInitialIronThroneWeightForHouse(h2).CompareTo(GetInitialIronThroneWeightForHouse(h1)));
      vassalHouses.Sort((h1, h2) => GetInitialIronThroneWeightForHouse(h2).CompareTo(GetInitialIronThroneWeightForHouse(h1)));

      List<HouseType> finalInfluenceTrack = playerHouses.ToList();
      finalInfluenceTrack.AddRange(vassalHouses);

      if (hasTargaryenPlayer)
        finalInfluenceTrack.Add(HouseType.Targaryen);

      influenceState.IronThroneTrack = finalInfluenceTrack.ToList();

      // FIEFDOMS TRACK
      playerHouses.Sort((h1, h2) => GetInitialFiefdomsWeightForHouse(h2).CompareTo(GetInitialFiefdomsWeightForHouse(h1)));
      vassalHouses.Sort((h1, h2) => GetInitialFiefdomsWeightForHouse(h2).CompareTo(GetInitialFiefdomsWeightForHouse(h1)));

      finalInfluenceTrack = playerHouses.ToList();
      finalInfluenceTrack.AddRange(vassalHouses);

      if (hasTargaryenPlayer)
        finalInfluenceTrack.Add(HouseType.Targaryen);

      influenceState.FiefdomsTrack = finalInfluenceTrack.ToList();

      // KING COURT TRACK
      playerHouses.Sort((h1, h2) => GetInitialKingdomsWeightForHouse(h2).CompareTo(GetInitialKingdomsWeightForHouse(h1)));
      vassalHouses.Sort((h1, h2) => GetInitialKingdomsWeightForHouse(h2).CompareTo(GetInitialKingdomsWeightForHouse(h1)));

      finalInfluenceTrack = playerHouses.ToList();
      finalInfluenceTrack.AddRange(vassalHouses);

      if (hasTargaryenPlayer)
        finalInfluenceTrack.Add(HouseType.Targaryen);

      influenceState.KingsCourtTrack = finalInfluenceTrack.ToList();
    }

    private static int GetInitialIronThroneWeightForHouse(HouseType house)
    {
      return house switch
      {
        HouseType.Baratheon => 8,
        HouseType.Lannister => 7,
        HouseType.Stark => 6,
        HouseType.Martell => 5,
        HouseType.Greyjoy => 4,
        HouseType.Tyrell => 3,
        HouseType.Arryn => 2,
        HouseType.Targaryen => 1,
        _ => 0
      };
    }

    private static int GetInitialFiefdomsWeightForHouse(HouseType house)
    {
      return house switch
      {
        HouseType.Greyjoy => 8,
        HouseType.Tyrell => 7,
        HouseType.Martell => 6,
        HouseType.Arryn => 5,
        HouseType.Stark => 4,
        HouseType.Baratheon => 3,
        HouseType.Lannister => 2,
        HouseType.Targaryen => 1,
        _ => 0
      };
    }

    private static int GetInitialKingdomsWeightForHouse(HouseType house)
    {
      return house switch
      {
        HouseType.Lannister => 8,
        HouseType.Stark => 7,
        HouseType.Martell => 6,
        HouseType.Tyrell => 5,
        HouseType.Arryn => 4,
        HouseType.Baratheon => 3,
        HouseType.Greyjoy => 2,
        HouseType.Targaryen => 1,
        _ => 0
      };
    }
  }
}
