using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides methods to initialize the state of a player based on their chosen house.
  /// </summary>
  public static class HouseStateFactory
  {
    public static HouseState CreateUndefined()
    {
      return new HouseState
      {
        Type = HouseType.Undefined,
        PowerTokens = 0,
        SupplyLevel = 0,
        VictoryPoints = 0
      };
    }

    public static HouseState CreateStark()
    {
      return new HouseState
      {
        Type = HouseType.Stark,
        PowerTokens = 5,
        SupplyLevel = 1,
        VictoryPoints = 2
      };
    }

    public static HouseState CreateGreyjoy()
    {
      return new HouseState
      {
        Type = HouseType.Greyjoy,
        PowerTokens = 5,
        SupplyLevel = 2,
        VictoryPoints = 1
      };
    }

    public static HouseState CreateLannister()
    {
      return new HouseState
      {
        Type = HouseType.Lannister,
        PowerTokens = 5,
        SupplyLevel = 2,
        VictoryPoints = 1
      };
    }

    public static HouseState CreateMartell()
    {
      return new HouseState
      {
        Type = HouseType.Martell,
        PowerTokens = 5,
        SupplyLevel = 2,
        VictoryPoints = 1
      };
    }

    public static HouseState CreateTyrell()
    {
      return new HouseState
      {
        Type = HouseType.Tyrell,
        PowerTokens = 5,
        SupplyLevel = 2,
        VictoryPoints = 1
      };
    }

    public static HouseState CreateBaratheon()
    {
      return new HouseState
      {
        Type = HouseType.Baratheon,
        PowerTokens = 5,
        SupplyLevel = 2,
        VictoryPoints = 1
      };
    }

    public static HouseState CreateArryn()
    {
      return new HouseState
      {
        Type = HouseType.Arryn,
        PowerTokens = 5,
        SupplyLevel = 1,
        VictoryPoints = 2
      };
    }

    public static HouseState CreateTargaryen()
    {
      return new HouseState
      {
        Type = HouseType.Targaryen,
        PowerTokens = 5,
        SupplyLevel = 4,
        VictoryPoints = 1
      };
    }
  }
}
