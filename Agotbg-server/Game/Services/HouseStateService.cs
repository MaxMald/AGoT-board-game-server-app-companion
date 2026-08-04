using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides services for creating and managing house states, including initialization,
  /// resource updates, track positions, and vassalage relationships.
  /// </summary>
  public static class HouseStateService
  {
    /// <summary>
    /// Creates a HouseState based on the specified HouseType. Each house has its own
    /// starting attributes, including power tokens, victory points, and track positions.
    /// The starting properties are based on an eight player game, and may need to be
    /// adjusted for games with fewer players.
    /// </summary>
    ///
    /// <param name="houseType">The type of house for which to create a
    /// HouseState.</param>
    ///
    /// <returns>A HouseState object initialized with the starting attributes for the
    /// specified house.</returns>
    ///
    /// <exception cref="NotImplementedException">Thrown if the specified HouseType is
    /// not implemented.</exception>
    public static HouseState Create(HouseType houseType)
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

    public static HouseState CreateVassal(HouseType houseType)
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

    /// <summary>
    /// Updates the power token of a house, ensuring that vassal houses cannot have power
    /// tokens.
    /// </summary>
    /// 
    /// <param name="house">The house state to update.</param>
    /// <param name="newPowerTokens">The new power token value to set.</param>
    /// 
    /// <returns>A Result indicating success or failure of the operation.</returns>
    public static Result UpdatePowerTokens(HouseState house, byte newPowerTokens)
    {
      if (house.IsVassal)
        return Result.FAILURE("Vassal houses cannot have power tokens.");

      house.PowerTokens = newPowerTokens;
      return Result.SUCCESS();
    }

    /// <summary>
    /// Updates the supply level for a house, capping it at the maximum allowed value,
    /// and recalculates the maximum armies.
    /// </summary>
    /// 
    /// <param name="house">The house state to update.</param>
    /// <param name="newSupplyLevel">The new supply level value to set.</param>
    public static void UpdateHouseSupplyLevel(HouseState house, byte newSupplyLevel)
    {
      house.SupplyLevel = Math.Min(newSupplyLevel, GameConstants.MaximumSupplyLevel);
    }

    /// <summary>
    /// Updates the kings court track position for a house and recalculates the number of
    /// special orders based on the new position.
    /// </summary>
    /// 
    /// <param name="house">The house state to update.</param>
    /// <param name="newPosition">The new position on the King's Court track.</param>
    public static void UpdateKingsCourtTrackPosition(HouseState house, byte newPosition)
    {
      house.KingsCourtTrackPosition = newPosition;
      UpdateNumSpecialOrdersBasedOnKingsCourtPosition(house);
    }

    /// <summary>
    /// Updates the number of special orders based on the house's King's Court track
    /// position.
    /// </summary>
    ///
    /// <param name="house">The house whose number of special orders is to be
    /// updated.</param>
    public static void UpdateNumSpecialOrdersBasedOnKingsCourtPosition(HouseState house)
    {
      if (house.Type == HouseType.Targaryen) // Targaryen always has 3 special orders regardless of position
      {
        house.NumSpecialOrders = 3;
        return;
      }

      if (house.IsVassal)
      {
        house.NumSpecialOrders = 0;
        return;
      }

      if (house.KingsCourtTrackPosition <= 1)
      {
        house.NumSpecialOrders = 3;
      }
      else if (house.KingsCourtTrackPosition == 2)
      {
        house.NumSpecialOrders = 3;
      }
      else if (house.KingsCourtTrackPosition == 3)
      {
        house.NumSpecialOrders = 2;
      }
      else if (house.KingsCourtTrackPosition == 4)
      {
        house.NumSpecialOrders = 1;
      }
      else if (house.KingsCourtTrackPosition >= 5)
      {
        house.NumSpecialOrders = 0;
      }
    }

    /// <summary>
    /// Updates the dragon's strength of the given house. Only House Targaryen can have
    /// dragon strength, and the new value must not exceed the maximum allowed value
    /// defined in GameRules.
    /// </summary>
    /// 
    /// <param name="house">The house state to update.</param>
    /// <param name="newDragonStrength">The new dragon strength value.</param>
    /// 
    /// <returns>A Result indicating success or failure of the operation.</returns>
    public static Result UpdateDragonStrength(HouseState house, byte newDragonStrength)
    {
      if (house.Type != HouseType.Targaryen)
        return Result.FAILURE("Only House Targaryen can have dragon strength.");

      if (newDragonStrength > GameConstants.MaximumDragonStrength)
        return Result.FAILURE("Dragon strength cannot exceed the maximum allowed value.");

      house.DragonStrength = newDragonStrength;
      return Result.SUCCESS();
    }

    /// <summary>
    /// Establishes a vassalage relationship between two houses.
    /// </summary>
    /// 
    /// <param name="commander">The house that will command the vassal.</param>
    /// <param name="vassal">The house that will become a vassal.</param>
    /// 
    /// <returns>A Result indicating success, or failure with an error message if
    /// validation fails.</returns>
    public static Result MakeVassalageStatus(HouseState commander, HouseState vassal)
    {
      if (!vassal.IsVassal)
        return Result.FAILURE("The house is not a vassal.");

      if (vassal.IsDefeated)
        return Result.FAILURE("A defeated house cannot be a vassal.");

      if (vassal.CommanderHouse != HouseType.Undefined)
        return Result.FAILURE("The house is already a vassal to another house. Vassalage status must be broken first.");

      if (commander.Type == vassal.Type)
        return Result.FAILURE("A house cannot be vassal to itself.");

      if (commander.IsDefeated)
        return Result.FAILURE("A defeated house cannot command a vassal.");

      if (commander.IsVassal)
        return Result.FAILURE("A vassal house cannot command another vassal.");

      vassal.CommanderHouse = commander.Type;

      if (!commander.VassalHouseTypes.Contains(vassal.Type))
        commander.VassalHouseTypes.Add(vassal.Type);

      return Result.SUCCESS();
    }

    /// <summary>
    /// Removes a vassalage relationship between two houses, effectively breaking the
    /// vassalage status.
    /// </summary>
    ///
    /// <param name="commander">The house that commands the vassal.</param>
    /// <param name="vassal">The house that is a vassal.</param>
    ///
    /// <returns>A Result indicating success, or failure with an error message if
    /// validation fails.</returns>
    public static Result BreakVassalageStatus(HouseState commander, HouseState vassal)
    {
      if (!vassal.IsVassal)
        return Result.FAILURE("The house is not a vassal.");

      if (vassal.CommanderHouse != commander.CommanderHouse)
        return Result.FAILURE("The specified commander does not command this vassal.");

      if (commander.Type == vassal.Type)
        return Result.FAILURE("A house cannot be vassal to itself.");

      if (vassal.CommanderHouse != commander.Type)
        return Result.FAILURE("The specified commander does not command this vassal.");

      if (!commander.VassalHouseTypes.Contains(vassal.Type))
        return Result.FAILURE("The specified commander does not have this vassal in its vassal list.");

      vassal.CommanderHouse = HouseType.Undefined;
      commander.VassalHouseTypes.Remove(vassal.Type);

      return Result.SUCCESS();
    }

    /// <summary>
    /// Updates the loan interest this house owes to the Iron Bank.
    /// </summary>
    ///
    /// <param name="house">The house whose Iron Bank loan interest is to be
    /// updated.</param>
    /// <param name="newLoanInterest">The new loan interest amount.</param>
    ///
    /// <returns>A Result indicating success, or failure with an error message if
    /// validation fails.</returns>
    public static Result UpdateIronBankLoanInterest(HouseState house, byte newLoanInterest)
    {
      if (house.IsVassal)
        return Result.FAILURE("Vassal houses cannot have Iron Bank loan interest.");

      house.IronBankLoanInterest = newLoanInterest;
      return Result.SUCCESS();
    }

    /// <summary>
    /// Updates the power tokens bid
    /// </summary>
    ///
    /// <param name="house">The house state to update.</param>
    /// <param name="newBid">The bid amount to set.</param>
    ///
    /// <returns>A Result indicating success, or failure the bid exceeds available power
    /// tokens.</returns>
    public static Result UpdatePowerTokensBid(HouseState house, byte newBid)
    {
      if (newBid > house.PowerTokens)
        return Result.FAILURE("Bid cannot exceed available power tokens.");

      house.PowerTokensBid = newBid;
      house.HasBidPowerTokens = true;
      return Result.SUCCESS();
    }

    /// <summary>
    /// Cancels the power tokens bid for a house, resetting the bid amount to zero and
    /// indicating that the house has not bid any power tokens.
    /// </summary>
    /// 
    /// <param name="house">The house state for which to cancel the power tokens bid.</param>
    public static void CancelPowerTokensBid(HouseState house)
    {
      house.PowerTokensBid = 0;
      house.HasBidPowerTokens = false;
    }

    /// <summary>
    /// Resolves a bid by deducting the bid amount from the house's available power
    /// tokens and resetting the bid to zero.
    /// </summary>
    /// 
    /// <param name="house">The house state containing the bid to resolve.</param>
    /// 
    /// <returns>A success result if the bid is valid and resolved; otherwise, a failure
    /// result with an error message.</returns>
    public static Result ResolveBid(HouseState house)
    {
      if (house.PowerTokensBid > house.PowerTokens)
        return Result.FAILURE("Bid cannot exceed available power tokens.");

      house.PowerTokens -= house.PowerTokensBid;
      house.PowerTokensBid = 0;
      house.HasBidPowerTokens = false;

      return Result.SUCCESS();
    }

    /// <summary>
    /// Transfer a specified amount of power tokens from one house to another, ensuring
    /// that the transfer is valid and does not exceed the available tokens of the source
    /// house.
    /// </summary>
    ///
    /// <param name="from">The house transferring power tokens.</param>
    /// <param name="to">The house receiving power tokens.</param>
    /// <param name="amount">The amount of power tokens to transfer.</param>
    ///
    /// <returns>A Result indicating success, or failure with an error message if
    /// validation fails.</returns>
    public static Result TransferPowerTokens(HouseState from, HouseState to, byte amount)
    {
      if (to.IsVassal)
        return Result.FAILURE("Vassal houses cannot receive power tokens.");

      if (from.IsVassal)
        return Result.FAILURE("Vassal houses cannot transfer power tokens.");

      if (from.PowerTokens < amount)
        return Result.FAILURE("Insufficient power tokens to transfer.");

      ushort usAmount = amount;
      ushort usToPowerTokens = to.PowerTokens;
      ushort totalPowerTokens = (ushort)(usToPowerTokens + usAmount);

      from.PowerTokens -= amount;
      to.PowerTokens = (byte)(Math.Min(totalPowerTokens, byte.MaxValue));

      return Result.SUCCESS();
    }

    /// <summary>
    /// Set a house as defeated, resetting its supply level, maximum armies, victory
    /// points, and power tokens.
    /// </summary>
    /// 
    /// <param name="house">The house to set as defeated.</param>
    /// 
    /// <returns>A Result indicating success, or failure with an error message if
    /// validation fails.</returns>
    public static Result SetHouseAsDefeated(HouseState house)
    {
      if (house.IsVassal)
        return Result.FAILURE("Vassal houses cannot be set as defeated.");

      house.IsDefeated = true;
      house.SupplyLevel = 0;
      house.VictoryPoints = 0;
      house.PowerTokens = 0;

      return Result.SUCCESS();
    }

    /// <summary>
    /// Adds one power token to the saboteur house and removes one power token from the
    /// sabotaged house (if any).
    /// </summary>
    /// 
    /// <param name="saboteur">The house performing the pillage.</param>
    /// <param name="sabotaged">The house being pillaged.</param>
    public static void PillageHouse(HouseState saboteur, HouseState sabotaged)
    {
      if (!saboteur.IsVassal && saboteur.PowerTokens < byte.MaxValue)
        saboteur.PowerTokens += 1;

      if (sabotaged.PowerTokens > 0)
        sabotaged.PowerTokens -= 1;
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

        IronThroneTrackPosition = 3,
        FiefdomTrackPosition = 5,
        KingsCourtTrackPosition = 2
      };

      UpdateHouseSupplyLevel(house, 1);
      UpdateNumSpecialOrdersBasedOnKingsCourtPosition(house);
      return house;
    }

    private static HouseState CreateGreyjoy()
    {
      HouseState house = new HouseState
      {
        Type = HouseType.Greyjoy,
        PowerTokens = GameConstants.StartingPowerTokens,
        VictoryPoints = 1,

        IronThroneTrackPosition = 5,
        FiefdomTrackPosition = 1,
        KingsCourtTrackPosition = 7
      };

      UpdateHouseSupplyLevel(house, 2);
      UpdateNumSpecialOrdersBasedOnKingsCourtPosition(house);
      return house;
    }

    private static HouseState CreateLannister()
    {
      HouseState house = new HouseState
      {
        Type = HouseType.Lannister,
        PowerTokens = GameConstants.StartingPowerTokens,
        VictoryPoints = 1,

        IronThroneTrackPosition = 2,
        FiefdomTrackPosition = 7,
        KingsCourtTrackPosition = 1
      };

      UpdateHouseSupplyLevel(house, 2);
      UpdateNumSpecialOrdersBasedOnKingsCourtPosition(house);
      return house;
    }

    private static HouseState CreateMartell()
    {
      HouseState house = new HouseState
      {
        Type = HouseType.Martell,
        PowerTokens = GameConstants.StartingPowerTokens,
        VictoryPoints = 1,

        IronThroneTrackPosition = 4,
        FiefdomTrackPosition = 3,
        KingsCourtTrackPosition = 3
      };

      UpdateHouseSupplyLevel(house, 2);
      UpdateNumSpecialOrdersBasedOnKingsCourtPosition(house);
      return house;
    }

    private static HouseState CreateTyrell()
    {
      HouseState house = new HouseState
      {
        Type = HouseType.Tyrell,
        PowerTokens = GameConstants.StartingPowerTokens,
        VictoryPoints = 1,

        IronThroneTrackPosition = 6,
        FiefdomTrackPosition = 2,
        KingsCourtTrackPosition = 4
      };

      UpdateHouseSupplyLevel(house, 2);
      UpdateNumSpecialOrdersBasedOnKingsCourtPosition(house);
      return house;
    }

    private static HouseState CreateBaratheon()
    {
      HouseState house = new HouseState
      {
        Type = HouseType.Baratheon,
        PowerTokens = GameConstants.StartingPowerTokens,
        VictoryPoints = 1,

        IronThroneTrackPosition = 1,
        FiefdomTrackPosition = 6,
        KingsCourtTrackPosition = 6
      };

      UpdateHouseSupplyLevel(house, 2);
      UpdateNumSpecialOrdersBasedOnKingsCourtPosition(house);
      return house;
    }

    private static HouseState CreateArryn()
    {
      HouseState house = new HouseState
      {
        Type = HouseType.Arryn,
        PowerTokens = GameConstants.StartingPowerTokens,
        VictoryPoints = 2,

        IronThroneTrackPosition = 7,
        FiefdomTrackPosition = 4,
        KingsCourtTrackPosition = 5
      };

      UpdateHouseSupplyLevel(house, 1);
      UpdateNumSpecialOrdersBasedOnKingsCourtPosition(house);
      return house;
    }

    private static HouseState CreateTargaryen()
    {
      HouseState house = new HouseState
      {
        Type = HouseType.Targaryen,
        PowerTokens = GameConstants.StartingPowerTokens,
        VictoryPoints = 1,

        IronThroneTrackPosition = 8,
        FiefdomTrackPosition = 8,
        KingsCourtTrackPosition = 8
      };

      UpdateHouseSupplyLevel(house, 4);
      UpdateNumSpecialOrdersBasedOnKingsCourtPosition(house);
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
