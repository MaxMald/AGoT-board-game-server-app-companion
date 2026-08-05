using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Represents the round phase where Targaryen resolves their power token gifts after
  /// an influence track bidding.
  /// </summary>
  ///
  /// <remarks>
  /// Possible transitions from this phase:
  /// <list type="bullet">
  ///   <item><see cref="RoundPhaseType.InfluenceTrackBiddingTargaryenPresentation"/></item>
  /// </list>
  /// </remarks>
  public class RpInfluenceTrackBiddingTargaryenResolution : ARoundPhase
  {
    /// <inheritdoc />
    public override RoundPhaseType Type => RoundPhaseType.InfluenceTrackBiddingTargaryenResolution;

    /// <inheritdoc />
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command.Type == RoundPhaseCommandType.ResolveWithTargaryenPowerTokenGifts)
        return ExecuteResolveWithTargaryenPowerTokenGifts(gameState, command);
      else if (command.Type == RoundPhaseCommandType.Resolve)
        return ExecuteResolve(gameState);

      return Result.FAILURE($"Invalid command type {command.Type} for round phase {Type}");
    }

    /// <inheritdoc />
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      return commandType == RoundPhaseCommandType.ResolveWithTargaryenPowerTokenGifts ||
             commandType == RoundPhaseCommandType.Resolve;
    }

    private static Result ExecuteResolve(GameState gameState)
    {
      gameState.InfluenceTrackBiddingState.TargaryenPowerTokenGifts.Clear();
      gameState.CurrentPhase = RoundPhaseType.InfluenceTrackBiddingTargaryenPresentation;
      return Result.SUCCESS();
    }

    private static Result ExecuteResolveWithTargaryenPowerTokenGifts(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcResolveWithTargaryenPowerTokenGifts giftsCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for round phase {RoundPhaseType.InfluenceTrackBiddingTargaryenResolution}");

      HouseBet? targaryenBet = gameState.InfluenceTrackBiddingState.HouseBets
        .FirstOrDefault(bet => bet.HouseType == HouseType.Targaryen);

      if (targaryenBet == null)
        return Result.FAILURE("Targaryen has not placed a bet");

      byte totalPowerTokensGifted = 0;
      foreach (PowerTokenGift gift in giftsCommand.PowerTokenGifts)
        totalPowerTokensGifted += gift.Amount;

      if (totalPowerTokensGifted > targaryenBet.BetAmount)
        return Result.FAILURE("Targaryen has gifted more power tokens than they have bet");

      foreach (PowerTokenGift gift in giftsCommand.PowerTokenGifts)
      {
        bool receiverExists = gameState.InfluenceTrackBiddingState
                                        .HouseBets
                                        .Any(houseBet => houseBet.HouseType == gift.Receiver);

        if (!receiverExists)
          return Result.FAILURE($"Invalid receiver: {gift.Receiver} for power token gift");
      }

      gameState.InfluenceTrackBiddingState.TargaryenPowerTokenGifts.Clear();
      gameState.InfluenceTrackBiddingState.TargaryenPowerTokenGifts.AddRange(giftsCommand.PowerTokenGifts);
      gameState.CurrentPhase = RoundPhaseType.InfluenceTrackBiddingTargaryenPresentation;

      return Result.SUCCESS();
    }
  }
}
