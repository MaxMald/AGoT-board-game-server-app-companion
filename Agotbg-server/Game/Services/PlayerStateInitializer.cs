using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides methods to initialize the state of a player based on their chosen house.
  /// </summary>
  public static class PlayerStateInitializer
  {
    public static Result InitializeForHouse(PlayerState playerState, HouseType house)
    {
      switch (house)
      {
        case HouseType.Stark:
          InitializeForHouseStark(playerState);
          break;
        case HouseType.Greyjoy:
          InitializeForHouseGreyjoy(playerState);
          break;
        case HouseType.Lannister:
          InitializeForHouseLannister(playerState);
          break;
        case HouseType.Martell:
          InitializeForHouseMartell(playerState);
          break;
        case HouseType.Tyrell:
          InitializeForHouseTyrell(playerState);
          break;
        case HouseType.Baratheon:
          InitializeForHouseBaratheon(playerState);
          break;
        case HouseType.Arryn:
          InitializeForHouseArryn(playerState);
          break;
        case HouseType.Targaryen:
          InitializeForHouseTargaryen(playerState);
          break;
        default:
          return Result.FAILURE($"Initialization method for House {house} is not supported.");
      }

      return Result.SUCCESS();
    }

    public static void InitializeForHouseStark(PlayerState playerState)
    {
      playerState.House = HouseType.Stark;
      playerState.PowerTokens = 5;
      playerState.SupplyLevel = 1;
      playerState.VictoryPoints = 2;
    }

    public static void InitializeForHouseGreyjoy(PlayerState playerState)
    {
      playerState.House = HouseType.Greyjoy;
      playerState.PowerTokens = 5;
      playerState.SupplyLevel = 2;
      playerState.VictoryPoints = 1;
    }

    public static void InitializeForHouseLannister(PlayerState playerState)
    {
      playerState.House = HouseType.Lannister;
      playerState.PowerTokens = 5;
      playerState.SupplyLevel = 2;
      playerState.VictoryPoints = 1;
    }

    public static void InitializeForHouseMartell(PlayerState playerState)
    {
      playerState.House = HouseType.Martell;
      playerState.PowerTokens = 5;
      playerState.SupplyLevel = 2;
      playerState.VictoryPoints = 1;
    }

    public static void InitializeForHouseTyrell(PlayerState playerState)
    {
      playerState.House = HouseType.Tyrell;
      playerState.PowerTokens = 5;
      playerState.SupplyLevel = 2;
      playerState.VictoryPoints = 1;
    }

    public static void InitializeForHouseBaratheon(PlayerState playerState)
    {
      playerState.House = HouseType.Baratheon;
      playerState.PowerTokens = 5;
      playerState.SupplyLevel = 2;
      playerState.VictoryPoints = 1;
    }

    public static void InitializeForHouseArryn(PlayerState playerState)
    {
      playerState.House = HouseType.Arryn;
      playerState.PowerTokens = 5;
      playerState.SupplyLevel = 1;
      playerState.VictoryPoints = 2;
    }

    public static void InitializeForHouseTargaryen(PlayerState playerState)
    {
      playerState.House = HouseType.Targaryen;
      playerState.PowerTokens = 5;
      playerState.SupplyLevel = 4;
      playerState.VictoryPoints = 1;
    }
  }
}
