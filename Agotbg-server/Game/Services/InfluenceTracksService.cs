using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides services for initializing the three influence tracks (Iron Throne,
  /// Fiefdoms, and King's Court) in the game.
  /// </summary>
  public static class InfluenceTracksService
  {
    /// <summary>
    /// Initializes all three influence tracks for the given houses based on their
    /// starting positions defined in the game rules.
    /// </summary>
    ///
    /// <param name="houses">The list of houses to position on the influence
    /// tracks.</param>
    public static void Initialize(List<HouseState> houses)
    {
      OrderHousesForIronThrone(houses);
      SetInfluenceTrackPositions(houses, InfluenceTrackType.IronThrone);
      OrderHousesForFiefdoms(houses);
      SetInfluenceTrackPositions(houses, InfluenceTrackType.Fiefdom);
      OrderHousesForKingsCourt(houses);
      SetInfluenceTrackPositions(houses, InfluenceTrackType.KingsCourt);
    }

    /// <summary>
    /// Sorts the houses list according to their initial Iron Throne track order, with
    /// the highest weighted house first.
    /// </summary>
    ///
    /// <param name="houses">The list of houses to sort.</param>
    private static void OrderHousesForIronThrone(List<HouseState> houses)
    {
      houses.Sort((h1, h2) => GetInitialIronThroneWeightForHouse(h2)
                              .CompareTo(GetInitialIronThroneWeightForHouse(h1)));
    }

    /// <summary>
    /// Sorts the houses list according to their initial Fiefdoms track order, with the
    /// highest weighted house first.
    /// </summary>
    /// 
    /// <param name="houses">The list of houses to sort.</param>
    private static void OrderHousesForFiefdoms(List<HouseState> houses)
    {
      houses.Sort((h1, h2) => GetInitialFiefdomsWeightForHouse(h2)
                              .CompareTo(GetInitialFiefdomsWeightForHouse(h1)));
    }

    /// <summary>
    /// Sorts the houses list according to their initial King's Court track order, with
    /// the highest weighted house first.
    /// </summary>
    /// 
    /// <param name="houses">The list of houses to sort.</param>
    private static void OrderHousesForKingsCourt(List<HouseState> houses)
    {
      houses.Sort((h1, h2) => GetInitialKingCourtWeightForHouse(h2)
                              .CompareTo(GetInitialKingCourtWeightForHouse(h1)));
    }

    /// <summary>
    /// Gets the sorting weight for a house on the Iron Throne track. Player houses have
    /// higher weights (12-18) than vassal houses (2-8). Targaryen always has weight 1.
    /// </summary>
    ///
    /// <param name="house">The house to get the weight for.</param>
    /// 
    /// <returns>The weight value used for sorting.</returns>
    private static int GetInitialIronThroneWeightForHouse(HouseState house)
    {
      if (!house.IsVassal)
      {
        // Player houses weights
        return house.Type switch
        {
          HouseType.Baratheon => 18,
          HouseType.Lannister => 17,
          HouseType.Stark => 16,
          HouseType.Martell => 15,
          HouseType.Greyjoy => 14,
          HouseType.Tyrell => 13,
          HouseType.Arryn => 12,
          HouseType.Targaryen => 1,
          _ => 0
        };
      }

      // Vassal houses weights
      return house.Type switch
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

    /// <summary>
    /// Gets the sorting weight for a house on the Fiefdoms track. Player houses have
    /// higher weights (12-18) than vassal houses (2-8). Targaryen always has weight 1.
    /// </summary>
    /// 
    /// <param name="house">The house to get the weight for.</param>
    /// 
    /// <returns>The weight value used for sorting.</returns>
    private static int GetInitialFiefdomsWeightForHouse(HouseState house)
    {
      if (!house.IsVassal)
      {
        // Player houses weights
        return house.Type switch
        {
          HouseType.Greyjoy => 18,
          HouseType.Tyrell => 17,
          HouseType.Martell => 16,
          HouseType.Arryn => 15,
          HouseType.Stark => 14,
          HouseType.Baratheon => 13,
          HouseType.Lannister => 12,
          HouseType.Targaryen => 1,
          _ => 0
        };
      }

      // Vassal houses weights
      return house.Type switch
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

    /// <summary>
    /// Gets the sorting weight for a house on the King's Court track. Player houses have
    /// higher weights (12-18) than vassal houses (2-8). Targaryen always has weight 1.
    /// </summary>
    ///
    /// <param name="house">The house to get the weight for.</param>
    ///
    /// <returns>The weight value used for sorting.</returns>
    private static int GetInitialKingCourtWeightForHouse(HouseState house)
    {
      if (!house.IsVassal)
      {
        // Player houses weights
        return house.Type switch
        {
          HouseType.Lannister => 18,
          HouseType.Stark => 17,
          HouseType.Martell => 16,
          HouseType.Tyrell => 15,
          HouseType.Arryn => 14,
          HouseType.Baratheon => 13,
          HouseType.Greyjoy => 12,
          HouseType.Targaryen => 1,
          _ => 0
        };
      }

      // Vassal houses weights
      return house.Type switch
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
    /// Assigns track positions to each house based on their order in the list. Positions
    /// start at 1 (highest). Targaryen always receives a constant position defined in
    /// GameConstants.TargaryenInfluencePosition.
    /// </summary>
    ///
    /// <param name="orderedHouses">The houses in their desired track order.</param>
    /// <param name="trackType">The influence track to update.</param>
    ///
    /// <exception cref="Exception">Thrown when an unknown track type is
    /// provided.</exception>
    private static void SetInfluenceTrackPositions(
      List<HouseState> orderedHouses,
      InfluenceTrackType trackType
      )
    {
      for (int i = 0; i < orderedHouses.Count; i++)
      {
        HouseState houseState = orderedHouses[i];
        byte trackPosition = (byte)(i + 1);

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
              houseState.KingsCourtTrackPosition = GameConstants.TargaryenInfluencePosition;
              break;
            default:
              throw new Exception($"Influence Track Service: Unknown InfluenceTrackType: {trackType}");
          }

          continue;
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
            houseState.KingsCourtTrackPosition = trackPosition;
            break;
          default:
            throw new Exception($"Influence Track Service: Unknown InfluenceTrackType: {trackType}");
        }
      }
    }
  }
}
