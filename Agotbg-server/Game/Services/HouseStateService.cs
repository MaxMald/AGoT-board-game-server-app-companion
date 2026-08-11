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
    /// Updates the power token of a house.
    /// </summary>
    /// 
    /// <param name="house">The house state to update.</param>
    /// <param name="newPowerTokens">The new power token value to set.</param>
    public static void UpdatePowerTokens(HouseState house, byte newPowerTokens)
    {
      house.PowerTokens = Math.Min(newPowerTokens, GameConstants.MaximumPowerTokens);
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
    /// Establishes a vassalage relationship between two houses.
    /// </summary>
    /// 
    /// <param name="commander">The house that will command the vassal.</param>
    /// <param name="vassal">The house that will become a vassal.</param>
    /// 
    /// <returns>A Result indicating success, or failure with an error message if
    /// validation fails.</returns>
    public static Result MakeVassalageRelationship(HouseState commander, HouseState vassal)
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
    /// Clears the vassalage properties of a house, resetting its commander house to
    /// undefined and clearing its list of vassal house types.
    /// </summary>
    ///
    /// <remarks>
    /// This method clears the vassalage properties, either for a commander house or a
    /// vassal house. It does not update the vassalage properties of any other houses
    /// that may be related to this house. To safely remove a vassalage relationship, use
    /// the <see cref="BreakVassalageStatus"/> method to ensure that the vassalage
    /// relationships are properly managed.
    /// </remarks>
    ///
    /// <param name="house">The house state for which to clear the vassalage
    /// properties.</param>
    public static void ClearVassalageProperties(HouseState house)
    {
      house.CommanderHouse = HouseType.Undefined;
      house.VassalHouseTypes.Clear();
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
    /// <param name="bid">The bid amount to set.</param>
    ///
    /// <returns>A Result indicating success, or failure the bid exceeds available power
    /// tokens.</returns>
    public static Result SubmitPowerTokensBid(HouseState house, byte bid)
    {
      if (house.HasBidPowerTokens)
        return Result.FAILURE("House has already submitted a power tokens bid.");

      if (bid > house.PowerTokens)
        return Result.FAILURE("Bid cannot exceed available power tokens.");

      house.PowerTokens -= bid;
      house.PowerTokensBid = bid;
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
      if (!house.HasBidPowerTokens)
        return;

      house.PowerTokens += house.PowerTokensBid;
      house.PowerTokensBid = 0;
      house.HasBidPowerTokens = false;
    }

    /// <summary>
    /// Clears the power tokens bid for a house, resetting the bid amount to zero and
    /// indicating that the house has not bid any power tokens. This method does not
    /// refund the bid amount to the house's available power tokens.
    /// </summary>
    ///
    /// <param name="house">The house state for which to clear the power tokens
    /// bid.</param>
    public static void ClearSubmittedPowerTokenBid(HouseState house)
    {
      house.PowerTokensBid = 0;
      house.HasBidPowerTokens = false;
    }

    /// <summary>
    /// Undoes the resolution of a bid by restoring the previous bid amount to the
    /// house's
    /// </summary>
    /// 
    /// <param name="house">The house state for which to undo the bid resolution.</param>
    /// <param name="previousBid">The previous bid amount to restore.</param>
    public static void UndoBidResolution(HouseState house, byte previousBid)
    {
      house.PowerTokens += previousBid;
      house.PowerTokensBid = previousBid;
      house.HasBidPowerTokens = true;
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
  }
}
