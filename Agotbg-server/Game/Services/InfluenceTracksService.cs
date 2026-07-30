using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Initializes the influence tracks for the game based on the player and vassal
  /// houses.
  /// </summary>
  public static class InfluenceTracksService
  {
    /// <summary>
    /// Initialize the influence tracks for the game based on the player and vassal houses.
    /// </summary>
    public static void Initialize(List<HouseState> houses)
    {
      List<HouseType> playerHouses = [];
      List<HouseType> vassalHouses = [];

      foreach (var house in houses)
      {
        if (house.IsVassal)
          vassalHouses.Add(house.Type);
        else
          playerHouses.Add(house.Type);
      }

      List<HouseType> ironThroneOrder = GetInitialOrderForIronThrone(playerHouses, vassalHouses);
      SetInfluenceTrackPositions(ironThroneOrder, houses, InfluenceTrackType.IronThrone);

      List<HouseType> fiefdomsOrder = GetInitialOrderForFiefdoms(playerHouses, vassalHouses);
      SetInfluenceTrackPositions(fiefdomsOrder, houses, InfluenceTrackType.Fiefdom);

      List<HouseType> kingsCourtOrder = GetInitialOrderForKingsCourt(playerHouses, vassalHouses);
      SetInfluenceTrackPositions(kingsCourtOrder, houses, InfluenceTrackType.KingsCourt);
    }

    private static List<HouseType> GetInitialOrderForIronThrone(
      List<HouseType> playerHouses,
      List<HouseType> vassalHouses
      )
    {
      bool hasTargaryenPlayer = false;
      if (playerHouses.Any(h => h == HouseType.Targaryen))
        hasTargaryenPlayer = true;

      playerHouses.Sort((h1, h2) => GetInitialIronThroneWeightForHouse(h2).CompareTo(GetInitialIronThroneWeightForHouse(h1)));
      vassalHouses.Sort((h1, h2) => GetInitialIronThroneWeightForHouse(h2).CompareTo(GetInitialIronThroneWeightForHouse(h1)));

      List<HouseType> finalInfluenceTrack = playerHouses.ToList();
      finalInfluenceTrack.AddRange(vassalHouses);

      if (hasTargaryenPlayer)
      {
        playerHouses.Add(HouseType.Targaryen);
        finalInfluenceTrack.Add(HouseType.Targaryen);
      }

      return finalInfluenceTrack;
    }

    private static List<HouseType> GetInitialOrderForFiefdoms(
      List<HouseType> playerHouses,
      List<HouseType> vassalHouses
      )
    {
      bool hasTargaryenPlayer = false;
      if (playerHouses.Any(h => h == HouseType.Targaryen))
        hasTargaryenPlayer = true;

      playerHouses.Sort((h1, h2) => GetInitialFiefdomsWeightForHouse(h2).CompareTo(GetInitialFiefdomsWeightForHouse(h1)));
      vassalHouses.Sort((h1, h2) => GetInitialFiefdomsWeightForHouse(h2).CompareTo(GetInitialFiefdomsWeightForHouse(h1)));

      List<HouseType> finalInfluenceTrack = playerHouses.ToList();
      finalInfluenceTrack.AddRange(vassalHouses);

      if (hasTargaryenPlayer)
      {
        playerHouses.Add(HouseType.Targaryen);
        finalInfluenceTrack.Add(HouseType.Targaryen);
      }

      return finalInfluenceTrack;
    }

    private static List<HouseType> GetInitialOrderForKingsCourt(
      List<HouseType> playerHouses,
      List<HouseType> vassalHouses
      )
    {
      bool hasTargaryenPlayer = false;
      if (playerHouses.Any(h => h == HouseType.Targaryen))
        hasTargaryenPlayer = true;

      playerHouses.Sort((h1, h2) => GetInitialKingCourtWeightForHouse(h2).CompareTo(GetInitialKingCourtWeightForHouse(h1)));
      vassalHouses.Sort((h1, h2) => GetInitialKingCourtWeightForHouse(h2).CompareTo(GetInitialKingCourtWeightForHouse(h1)));

      List<HouseType> finalInfluenceTrack = playerHouses.ToList();
      finalInfluenceTrack.AddRange(vassalHouses);

      if (hasTargaryenPlayer)
      {
        playerHouses.Add(HouseType.Targaryen);
        finalInfluenceTrack.Add(HouseType.Targaryen);
      }

      return finalInfluenceTrack;
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

    private static int GetInitialKingCourtWeightForHouse(HouseType house)
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

    /// <summary>
    /// Sets the influence track positions for all houses based on an ordered list of
    /// house types.
    /// </summary>
    ///
    /// <param name="orderedHouseTypes">The list of house types in their desired track
    /// order.</param>
    /// <param name="houses">The list of house states to update.</param>
    /// <param name="trackType">The type of influence track to update (IronThrone,
    /// Fiefdom, or KingsCourt).</param>
    ///
    /// <exception cref="Exception">Thrown when a house state is not found for a given
    /// house type, or when an unknown InfluenceTrackType is provided.</exception>
    private static void SetInfluenceTrackPositions(
      List<HouseType> orderedHouseTypes,
      List<HouseState> houses,
      InfluenceTrackType trackType
      )
    {
      for (int i = 0; i < orderedHouseTypes.Count; i++)
      {
        HouseType houseType = orderedHouseTypes[i];
        byte trackPosition = (byte)(i + 1);

        var houseState = houses.FirstOrDefault(h => h.Type == houseType);
        if (houseState == null)
          throw new Exception($"Influece Track Service: HouseState not found for HouseType: {houseType}");

        if (houseState.Type == HouseType.Targaryen)
        {
          switch (trackType)
          {
            case InfluenceTrackType.IronThrone:
              houseState.IronThroneTrackPosition = GameConstants.TargaryenInfluencePosition;
              break;
            case InfluenceTrackType.Fiefdom:
              houseState.FiefdomTrackPosition = GameConstants.TargaryenInfluencePosition;
              break;
            case InfluenceTrackType.KingsCourt:
              HouseStateService.UpdateKingsCourtTrackPosition(houseState, GameConstants.TargaryenInfluencePosition);
              break;
            default:
              throw new Exception($"Influence Track Service: Unknown InfluenceTrackType: {trackType}");
          }
        }

        switch (trackType)
        {
          case InfluenceTrackType.IronThrone:
            houseState.IronThroneTrackPosition = trackPosition;
            break;
          case InfluenceTrackType.Fiefdom:
            houseState.FiefdomTrackPosition = trackPosition;
            break;
          case InfluenceTrackType.KingsCourt:
            HouseStateService.UpdateKingsCourtTrackPosition(houseState, trackPosition);
            break;
          default:
            throw new Exception($"Influence Track Service: Unknown InfluenceTrackType: {trackType}");
        }
      }
    }
  }
}
