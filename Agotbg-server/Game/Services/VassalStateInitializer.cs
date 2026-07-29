using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  public static class VassalStateInitializer
  {
    public static Result InitializeForHouse(
      VassalState vassalState,
      HouseType house
     )
    {
      vassalState.House = house;
      vassalState.CommanderHouse = HouseType.Undefined;

      switch (house)
      {
        case HouseType.Stark:
          vassalState.SupplyLevel = 1;
          break;
        case HouseType.Greyjoy:
          vassalState.SupplyLevel = 2;
          break;
        case HouseType.Lannister:
          vassalState.SupplyLevel = 3;
          break;
        case HouseType.Martell:
          vassalState.SupplyLevel = 3;
          break;
        case HouseType.Tyrell:
          vassalState.SupplyLevel = 3;
          break;
        case HouseType.Baratheon:
          vassalState.SupplyLevel = 2;
          break;
        case HouseType.Arryn:
          vassalState.SupplyLevel = 2;
          break;
        default:
          return new Result
          {
            Success = false,
            Message = "Invalid house type."
          };
      }

      return Result.SUCCESS();
    }
  }
}
