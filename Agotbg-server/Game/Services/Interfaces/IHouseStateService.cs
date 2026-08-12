using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.Interfaces
{
  /// <summary>
  /// Provides operations for managing house state, power tokens, vassalage
  /// relationships, supply levels, bidding, and defeat mechanics.
  /// </summary>
  public interface IHouseStateService
  {
    /// <summary>
    /// Updates the power token of a house.
    /// </summary>
    /// 
    /// <param name="house">The house state to update.</param>
    /// <param name="newPowerTokens">The new power token value to set.</param>
    public void UpdatePowerTokens(HouseState house, byte newPowerTokens);

    /// <summary>
    /// Updates the supply level for a house, capping it at the maximum allowed value,
    /// and recalculates the maximum armies.
    /// </summary>
    /// 
    /// <param name="house">The house state to update.</param>
    /// <param name="newSupplyLevel">The new supply level value to set.</param>
    public void UpdateHouseSupplyLevel(HouseState house, byte newSupplyLevel);

    /// <summary>
    /// Establishes a vassalage relationship between two houses.
    /// </summary>
    /// 
    /// <param name="commander">The house that will command the vassal.</param>
    /// <param name="vassal">The house that will become a vassal.</param>
    /// 
    /// <returns>A Result indicating success, or failure with an error message if
    /// validation fails.</returns>
    public Result MakeVassalageRelationship(HouseState commander, HouseState vassal);

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
    public Result BreakVassalageStatus(HouseState commander, HouseState vassal);

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
    public void ClearVassalageProperties(HouseState house);

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
    public Result UpdateIronBankLoanInterest(HouseState house, byte newLoanInterest);

    /// <summary>
    /// Updates the power tokens bid
    /// </summary>
    ///
    /// <param name="house">The house state to update.</param>
    /// <param name="bid">The bid amount to set.</param>
    ///
    /// <returns>A Result indicating success, or failure the bid exceeds available power
    /// tokens.</returns>
    public Result SubmitPowerTokensBid(HouseState house, byte bid);

    /// <summary>
    /// Cancels the power tokens bid for a house, resetting the bid amount to zero and
    /// indicating that the house has not bid any power tokens.
    /// </summary>
    /// 
    /// <param name="house">The house state for which to cancel the power tokens bid.</param>
    public void CancelPowerTokensBid(HouseState house);

    /// <summary>
    /// Clears the power tokens bid for a house, resetting the bid amount to zero and
    /// indicating that the house has not bid any power tokens. This method does not
    /// refund the bid amount to the house's available power tokens.
    /// </summary>
    ///
    /// <param name="house">The house state for which to clear the power tokens
    /// bid.</param>
    public void ClearSubmittedPowerTokenBid(HouseState house);

    /// <summary>
    /// Updates the num of victory points for a house, ensuring that the new value is
    /// within valid bounds.
    /// </summary>
    /// 
    /// <param name="house">The house state for which to update the victory points.</param>
    /// <param name="newVictoryPoints">The new number of victory points.</param>
    public void UpdateVictoryPoints(HouseState house, byte newVictoryPoints);

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
    public Result TransferPowerTokens(HouseState from, HouseState to, byte amount);

    /// <summary>
    /// Set a house as defeated, resetting its supply level, maximum armies, victory
    /// points, and power tokens.
    /// </summary>
    /// 
    /// <param name="house">The house to set as defeated.</param>
    /// 
    /// <returns>A Result indicating success, or failure with an error message if
    /// validation fails.</returns>
    public Result SetHouseAsDefeated(HouseState house);

    /// <summary>
    /// Adds one power token to the saboteur house and removes one power token from the
    /// sabotaged house (if any).
    /// </summary>
    /// 
    /// <param name="saboteur">The house performing the pillage.</param>
    /// <param name="sabotaged">The house being pillaged.</param>
    public void PillageHouse(HouseState saboteur, HouseState sabotaged);
  }
}
