using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Default implementation of the IHouseStateFactory interface. This factory is
  /// responsible for creating HouseState objects based on the specified HouseType,
  /// including both player houses and vassal houses.
  /// </summary>
  public class HouseStateFactory : IHouseStateFactory
  {
    /// <inheritdoc/>
    public HouseState Create(HouseType houseType)
    {
      switch (houseType)
      {
        case HouseType.Undefined:
          return CreateUndefined();
        case HouseType.Stark:
          return CreateStark();
        case HouseType.Greyjoy:
          return CreateGreyjoy();
        case HouseType.Lannister:
          return CreateLannister();
        case HouseType.Martell:
          return CreateMartell();
        case HouseType.Tyrell:
          return CreateTyrell();
        case HouseType.Baratheon:
          return CreateBaratheon();
        case HouseType.Arryn:
          return CreateArryn();
        case HouseType.Targaryen:
          return CreateTargaryen();
        default:
          throw new NotImplementedException("HouseStateService.Create: HouseType not implemented: " + houseType);
      }
    }

    /// <inheritdoc/>
    public HouseState CreateVassal(HouseType houseType)
    {
      if (houseType == HouseType.Undefined)
        throw new ArgumentException("Vassal house type cannot be undefined.", nameof(houseType));

      if (houseType == HouseType.Targaryen)
        throw new ArgumentException("House Targaryen cannot be a vassal.", nameof(houseType));

      switch (houseType)
      {
        case HouseType.Stark:
          return CreateVassalStark();
        case HouseType.Greyjoy:
          return CreateVassalGreyjoy();
        case HouseType.Lannister:
          return CreateVassalLannister();
        case HouseType.Martell:
          return CreateVassalMartell();
        case HouseType.Tyrell:
          return CreateVassalTyrell();
        case HouseType.Baratheon:
          return CreateVassalBaratheon();
        case HouseType.Arryn:
          return CreateVassalArryn();
      }

      throw new NotImplementedException("HouseStateService.CreateVassal: Vassal HouseType not implemented: " + houseType);
    }

    // Player Houses Factory methods

    private static HouseState CreateUndefined()
    {
      return new HouseState
      {
        Type = HouseType.Undefined,
        PowerTokens = 0,
        SupplyLevel = 0,
        VictoryPoints = 0,

        IronThroneTrackPosition = 0,
        FiefdomTrackPosition = 0,
        KingsCourtTrackPosition = 0
      };
    }

    private static HouseState CreateStark()
    {
      HouseState house = new HouseState
      {
        Type = HouseType.Stark,
        PowerTokens = GameConstants.StartingPowerTokens,
        VictoryPoints = 2,
        SupplyLevel = 1,

        IronThroneTrackPosition = 3,
        FiefdomTrackPosition = 5,
        KingsCourtTrackPosition = 2
      };

      return house;
    }

    private static HouseState CreateGreyjoy()
    {
      HouseState house = new HouseState
      {
        Type = HouseType.Greyjoy,
        PowerTokens = GameConstants.StartingPowerTokens,
        VictoryPoints = 1,
        SupplyLevel = 2,

        IronThroneTrackPosition = 5,
        FiefdomTrackPosition = 1,
        KingsCourtTrackPosition = 7
      };

      return house;
    }

    private static HouseState CreateLannister()
    {
      HouseState house = new HouseState
      {
        Type = HouseType.Lannister,
        PowerTokens = GameConstants.StartingPowerTokens,
        VictoryPoints = 1,
        SupplyLevel = 2,

        IronThroneTrackPosition = 2,
        FiefdomTrackPosition = 7,
        KingsCourtTrackPosition = 1
      };

      return house;
    }

    private static HouseState CreateMartell()
    {
      HouseState house = new HouseState
      {
        Type = HouseType.Martell,
        PowerTokens = GameConstants.StartingPowerTokens,
        VictoryPoints = 1,
        SupplyLevel = 2,

        IronThroneTrackPosition = 4,
        FiefdomTrackPosition = 3,
        KingsCourtTrackPosition = 3
      };

      return house;
    }

    private static HouseState CreateTyrell()
    {
      HouseState house = new HouseState
      {
        Type = HouseType.Tyrell,
        PowerTokens = GameConstants.StartingPowerTokens,
        VictoryPoints = 1,
        SupplyLevel = 2,

        IronThroneTrackPosition = 6,
        FiefdomTrackPosition = 2,
        KingsCourtTrackPosition = 4
      };

      return house;
    }

    private static HouseState CreateBaratheon()
    {
      HouseState house = new HouseState
      {
        Type = HouseType.Baratheon,
        PowerTokens = GameConstants.StartingPowerTokens,
        VictoryPoints = 1,
        SupplyLevel = 2,

        IronThroneTrackPosition = 1,
        FiefdomTrackPosition = 6,
        KingsCourtTrackPosition = 6
      };

      return house;
    }

    private static HouseState CreateArryn()
    {
      HouseState house = new HouseState
      {
        Type = HouseType.Arryn,
        PowerTokens = GameConstants.StartingPowerTokens,
        VictoryPoints = 2,
        SupplyLevel = 1,

        IronThroneTrackPosition = 7,
        FiefdomTrackPosition = 4,
        KingsCourtTrackPosition = 5
      };

      return house;
    }

    private static HouseState CreateTargaryen()
    {
      HouseState house = new HouseState
      {
        Type = HouseType.Targaryen,
        PowerTokens = GameConstants.StartingPowerTokens,
        VictoryPoints = 1,
        SupplyLevel = 4,

        IronThroneTrackPosition = 8,
        FiefdomTrackPosition = 8,
        KingsCourtTrackPosition = 8
      };

      return house;
    }

    // Vassal Houses Factory methods

    private static HouseState CreateVassalStark()
    {
      return new HouseState
      {
        Type = HouseType.Stark,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 1,
        IsVassal = true
      };
    }

    private static HouseState CreateVassalGreyjoy()
    {
      return new HouseState
      {
        Type = HouseType.Greyjoy,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 2,
        IsVassal = true
      };
    }

    private static HouseState CreateVassalLannister()
    {
      return new HouseState
      {
        Type = HouseType.Lannister,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 3,
        IsVassal = true
      };
    }

    private static HouseState CreateVassalMartell()
    {
      return new HouseState
      {
        Type = HouseType.Martell,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 3,
        IsVassal = true
      };
    }

    private static HouseState CreateVassalTyrell()
    {
      return new HouseState
      {
        Type = HouseType.Tyrell,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 3,
        IsVassal = true
      };
    }

    private static HouseState CreateVassalBaratheon()
    {
      return new HouseState
      {
        Type = HouseType.Baratheon,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 2,
        IsVassal = true
      };
    }

    private static HouseState CreateVassalArryn()
    {
      return new HouseState
      {
        Type = HouseType.Arryn,
        CommanderHouse = HouseType.Undefined,
        SupplyLevel = 2,
        IsVassal = true
      };
    }
  }
}
