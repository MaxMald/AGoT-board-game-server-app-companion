using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Represents a sub-phase where the result of the Iron Bank interest payment
  /// resolution is presented to the players.
  /// </summary>
  public class RpIronBankInterestPaymentResolution : ARoundPhase
  {
    /// <inheritdoc/>
    public override RoundPhaseType Type => RoundPhaseType.IronBankInterestPaymentResolution;

    /// <summary>
    /// Instantiates a new instance of the <see
    /// cref="RpIronBankInterestPaymentResolution"/> class.
    /// </summary>
    ///
    /// <param name="gameStateService">The game state service.</param>
    /// <param name="houseStateService">The house state service.</param>
    /// <param name="ironBankIterestPaymentStateService">The iron bank interest payment
    /// state service.</param>
    public RpIronBankInterestPaymentResolution(
      IGameStateService gameStateService,
      IHouseStateService houseStateService,
      IIronBankInterestPaymentStateService ironBankIterestPaymentStateService
    ) : base(gameStateService, houseStateService)
    {
      IronBankInterestPaymentStateService = ironBankIterestPaymentStateService;
    }

    /// <inheritdoc/>
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcResolve resolveCmd)
        return Result.FAILURE("Invalid command type for this phase.");

      if (!m_gameStateService.IsHoster(gameState, resolveCmd.PlayerId))
        return Result.FAILURE("Only the hoster can resolve this phase.");

      IronBankInterestPaymentStateService.Clear(gameState.IronBankLoanInterestState);
      return Result.SUCCESS();
    }

    /// <inheritdoc/>
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      return commandType == RoundPhaseCommandType.Resolve;
    }

    private IIronBankInterestPaymentStateService IronBankInterestPaymentStateService { get; }
  }
}
