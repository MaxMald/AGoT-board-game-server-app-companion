using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides services for managing the influence tracks of houses in the game.
  /// </summary>
  public class InfluenceTracksService : IInfluenceTrackService
  {
    /// <inheritdoc/>
    public void Initialize(List<HouseState> houses)
    {
      InitializeHousesOrderForIronThrone(houses);
      SetInfluenceTrackPositions(houses, InfluenceTrackType.IronThrone);
      InitializeHousesOrderForFiefdoms(houses);
      SetInfluenceTrackPositions(houses, InfluenceTrackType.Fiefdom);
      InitializeHouseOrderForKingsCourt(houses);
      SetInfluenceTrackPositions(houses, InfluenceTrackType.KingsCourt);
    }

    /// <inheritdoc/>
    public void MoveInfluenceTrackPositionForHouse(
      List<HouseState> houses,
      HouseType houseType,
      InfluenceTrackType trackType,
      byte newPosition
    )
    {
      if (houses.Count <= 0)
        throw new ArgumentException("The list of houses is empty. Cannot move house position.");

      HouseState? houseToMove = houses.FirstOrDefault(h => h.Type == houseType);
      if (houseToMove == null)
        throw new ArgumentException($"House {houseType} not found in the provided list.");

      int maxPosition = houses.Count;
      if (newPosition > maxPosition)
        newPosition = (byte)maxPosition;

      SortHousesByInfluenceTrackPosition(houses, trackType);
      houses.Remove(houseToMove);

      byte zeroBasedPosition = (byte)(newPosition - 1);
      houses.Insert(zeroBasedPosition, houseToMove);

      SetInfluenceTrackPositions(houses, trackType);
    }

    /// <inheritdoc/>
    public void UpdateInfluenceTrackPositions(
      List<HouseState> houses,
      List<HouseInfluencePositionItem> houseInfluencePositions,
      InfluenceTrackType trackType
    )
    {
      foreach (var item in houseInfluencePositions)
      {
        HouseState? house = houses.FirstOrDefault(h => h.Type == item.HouseType);
        if (house == null)
          throw new Exception($"House {item.HouseType} not found in the provided list.");

        switch (trackType)
        {
          case InfluenceTrackType.IronThrone:
            house.IronThroneTrackPosition = item.InfluencePosition;
            break;
          case InfluenceTrackType.Fiefdom:
            house.FiefdomTrackPosition = item.InfluencePosition;
            break;
          case InfluenceTrackType.KingsCourt:
            house.KingsCourtTrackPosition = item.InfluencePosition;
            break;
          default:
            throw new Exception($"Unknown InfluenceTrackType: {trackType}");
        }
      }
    }

    /// <summary>
    /// Sorts the houses list according to their initial Iron Throne track order, with
    /// the highest weighted house first.
    /// </summary>
    ///
    /// <param name="houses">The list of houses to sort.</param>
    private static void InitializeHousesOrderForIronThrone(List<HouseState> houses)
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
    private static void InitializeHousesOrderForFiefdoms(List<HouseState> houses)
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
    private static void InitializeHouseOrderForKingsCourt(List<HouseState> houses)
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

    /// <summary>
    /// Sorts the list of houses based on their current position on the specified
    /// influence track. The house with the lowest position value (highest influence)
    /// will be first in the list.
    /// </summary>
    /// 
    /// <param name="houses">The list of houses to sort.</param>
    /// <param name="trackType">The influence track to sort by.</param>
    private static void SortHousesByInfluenceTrackPosition(
      List<HouseState> houses,
      InfluenceTrackType trackType
    )
    {
      houses.Sort((h1, h2) =>
      {
        byte pos1 = trackType switch
        {
          InfluenceTrackType.IronThrone => h1.IronThroneTrackPosition,
          InfluenceTrackType.Fiefdom => h1.FiefdomTrackPosition,
          InfluenceTrackType.KingsCourt => h1.KingsCourtTrackPosition,
          _ => 0
        };

        byte pos2 = trackType switch
        {
          InfluenceTrackType.IronThrone => h2.IronThroneTrackPosition,
          InfluenceTrackType.Fiefdom => h2.FiefdomTrackPosition,
          InfluenceTrackType.KingsCourt => h2.KingsCourtTrackPosition,
          _ => 0
        };

        return pos1.CompareTo(pos2);
      });
    }
  }
}
