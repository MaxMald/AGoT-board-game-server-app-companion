using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides services for creating and managing house states, including initialization,
  /// resource updates, track positions, and vassalage relationships.
  /// </summary>
  public class HouseStateService : IHouseStateService
  {
    /// <inheritdoc/>
    public void UpdatePowerTokens(HouseState house, byte newPowerTokens)
    {
      house.PowerTokens = Math.Min(newPowerTokens, GameConstants.MaximumPowerTokens);
    }

    /// <inheritdoc/>
    public void UpdateHouseSupplyLevel(HouseState house, byte newSupplyLevel)
    {
      house.SupplyLevel = Math.Min(newSupplyLevel, GameConstants.MaximumSupplyLevel);
    }

    /// <inheritdoc/>
    public Result MakeVassalageRelationship(HouseState commander, HouseState vassal)
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

    /// <inheritdoc/>
    public Result BreakVassalageStatus(HouseState commander, HouseState vassal)
    {
      if (!vassal.IsVassal)
        return Result.FAILURE("The house is not a vassal.");

      if (vassal.CommanderHouse != commander.Type)
        return Result.FAILURE("The specified commander does not command this vassal.");

      if (!commander.VassalHouseTypes.Contains(vassal.Type))
        return Result.FAILURE("The specified commander does not have this vassal in its vassal list.");

      vassal.CommanderHouse = HouseType.Undefined;
      commander.VassalHouseTypes.Remove(vassal.Type);

      return Result.SUCCESS();
    }

    /// <inheritdoc/>
    public void ClearVassalageProperties(HouseState house)
    {
      house.CommanderHouse = HouseType.Undefined;
      house.VassalHouseTypes.Clear();
    }

    /// <inheritdoc/>
    public Result UpdateIronBankLoanInterest(HouseState house, byte newLoanInterest)
    {
      if (house.IsVassal)
        return Result.FAILURE("Vassal houses cannot have Iron Bank loan interest.");

      house.IronBankLoanInterest = newLoanInterest;
      return Result.SUCCESS();
    }

    /// <inheritdoc/>
    public Result SubmitPowerTokensBid(HouseState house, byte bid)
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

    /// <inheritdoc/>
    public void CancelPowerTokensBid(HouseState house)
    {
      if (!house.HasBidPowerTokens)
        return;

      house.PowerTokens += house.PowerTokensBid;
      house.PowerTokensBid = 0;
      house.HasBidPowerTokens = false;
    }

    /// <inheritdoc/>
    public void ClearSubmittedPowerTokenBid(HouseState house)
    {
      house.PowerTokensBid = 0;
      house.HasBidPowerTokens = false;
    }

    /// <inheritdoc/>
    public void UpdateVictoryPoints(HouseState house, byte newVictoryPoints)
    {
      house.VictoryPoints = Math.Min(newVictoryPoints, GameConstants.NumVictoryPointsToWin);
    }

    /// <inheritdoc/>
    public Result TransferPowerTokens(HouseState from, HouseState to, byte amount)
    {
      if (to.IsVassal)
        return Result.FAILURE("Vassal houses cannot receive power tokens.");

      if (from.IsVassal)
        return Result.FAILURE("Vassal houses cannot transfer power tokens.");

      if (from.PowerTokens < amount)
        return Result.FAILURE("Insufficient power tokens to transfer.");

      byte newPowerTokens = (byte)(to.PowerTokens + amount);
      if (newPowerTokens > GameConstants.MaximumPowerTokens)
        return Result.FAILURE("Transfer would exceed the maximum power tokens.");

      from.PowerTokens -= amount;
      to.PowerTokens = newPowerTokens;

      return Result.SUCCESS();
    }

    /// <inheritdoc/>
    public Result SetHouseAsDefeated(HouseState house)
    {
      if (house.IsVassal)
        return Result.FAILURE("Vassal houses cannot be set as defeated.");

      house.IsDefeated = true;
      house.SupplyLevel = 0;
      house.VictoryPoints = 0;
      house.PowerTokens = 0;

      return Result.SUCCESS();
    }

    /// <inheritdoc/>
    public void PillageHouse(HouseState saboteur, HouseState sabotaged)
    {
      if (!saboteur.IsVassal && saboteur.PowerTokens < GameConstants.MaximumPowerTokens)
        saboteur.PowerTokens += 1;

      if (sabotaged.PowerTokens > 0)
        sabotaged.PowerTokens -= 1;
    }
  }
}
