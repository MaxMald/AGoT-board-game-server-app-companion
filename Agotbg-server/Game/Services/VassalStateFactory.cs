using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides methods to initialize the state of a vassal based on their chosen house.
  /// </summary>
  public static class VassalStateFactory
  {
    public static VassalState CreateStark()
    {
      return new VassalState
      {
        House = HouseType.Stark,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 1
      };
    }

    public static VassalState CreateGreyjoy()
    {
      return new VassalState
      {
        House = HouseType.Greyjoy,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 2
      };
    }

    public static VassalState CreateLannister()
    {
      return new VassalState
      {
        House = HouseType.Lannister,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 3
      };
    }

    public static VassalState CreateMartell()
    {
      return new VassalState
      {
        House = HouseType.Martell,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 3
      };
    }

    public static VassalState CreateTyrell()
    {
      return new VassalState
      {
        House = HouseType.Tyrell,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 3
      };
    }

    public static VassalState CreateBaratheon()
    {
      return new VassalState
      {
        House = HouseType.Baratheon,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 2
      };
    }

    public static VassalState CreateArryn()
    {
      return new VassalState
      {
        House = HouseType.Arryn,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 2
      };
    }
  }
}
