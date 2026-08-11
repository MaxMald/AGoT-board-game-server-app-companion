using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Represents the round phase where the results of the wildlings bidding are presented
  /// to the players.
  /// </summary>
  ///
  /// <remarks>
  /// Possible transitions from this phase:
  /// <list type="bullet">
  ///   <item><see cref="RoundPhaseType.Westeros"/></item>
  /// </list>
  /// </remarks>
  public class RpWildlingsBiddingPresentation : ARoundPhase
  {
    /// <inheritdoc />
    public override RoundPhaseType Type => RoundPhaseType.WildlingsBiddingPresentation;

    /// <summary>
    /// Creates a new instance of the <see cref="RpWildlingsBiddingPresentation"/> class.
    /// </summary>
    ///
    /// <param name="gameStateService">The game state service.</param>
    /// <param name="houseStateService">The house state service.</param>
    /// <param name="wildlingsStateService">The wildlings state service.</param>
    public RpWildlingsBiddingPresentation(
      IGameStateService gameStateService,
      IHouseStateService houseStateService,
      IWildlingsStateService wildlingsStateService
    ) : base(gameStateService, houseStateService)
    {
      m_wildlingsStateServices = wildlingsStateService;
    }

    /// <inheritdoc />
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      m_wildlingsStateServices.ClearBiddingProperties(gameState.Wildlings);
      gameState.CurrentPhase = RoundPhaseType.Westeros;
      return Result.SUCCESS();
    }

    /// <inheritdoc />
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      return commandType == RoundPhaseCommandType.Resolve;
    }

    private IWildlingsStateService m_wildlingsStateServices;
  }
}
