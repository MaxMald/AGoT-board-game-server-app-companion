using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services
{
  public static class VassalsInitializer
  {
    public static void Initialize(RoomState room)
    {
      for (byte i = 0; i < (byte)HouseType.Count; ++i)
      {
        HouseType houseType = (HouseType)i;
        if (houseType == HouseType.Undefined || houseType == HouseType.Targaryen)
          continue; // Skip undefined type. Targaryen cannot be a vassal house

        if (room.Players.Values.Any(p => p.HouseState.Type == houseType))
          continue; // Skip if the house is already taken by a player

        if (room.Vassals.ContainsKey(houseType))
          continue; // Skip if the house is already added as a vassal

        VassalState vassal = houseType switch
        {
          HouseType.Stark => CreateStark(),
          HouseType.Greyjoy => CreateGreyjoy(),
          HouseType.Lannister => CreateLannister(),
          HouseType.Martell => CreateMartell(),
          HouseType.Tyrell => CreateTyrell(),
          HouseType.Baratheon => CreateBaratheon(),
          HouseType.Arryn => CreateArryn(),
          _ => throw new ArgumentOutOfRangeException($"Unsupported house type: {houseType}")
        };

        room.Vassals[houseType] = vassal;
      }
    }

    private static VassalState CreateStark()
    {
      return new VassalState
      {
        House = HouseType.Stark,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 1
      };
    }

    private static VassalState CreateGreyjoy()
    {
      return new VassalState
      {
        House = HouseType.Greyjoy,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 2
      };
    }

    private static VassalState CreateLannister()
    {
      return new VassalState
      {
        House = HouseType.Lannister,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 3
      };
    }

    private static VassalState CreateMartell()
    {
      return new VassalState
      {
        House = HouseType.Martell,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 3
      };
    }

    private static VassalState CreateTyrell()
    {
      return new VassalState
      {
        House = HouseType.Tyrell,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 3
      };
    }

    private static VassalState CreateBaratheon()
    {
      return new VassalState
      {
        House = HouseType.Baratheon,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 2
      };
    }

    private static VassalState CreateArryn()
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
