using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Represents the round phase where a player resolves a tie group in the influence
  /// track bidding phase of the game.
  /// </summary>
  ///
  /// <remarks>
  /// Possible transitions from this phase include:
  /// <list type="bullet">
  ///   <item><see cref="RoundPhaseType.InfluenceTrackBiddingPresentation"/></item>
  /// </list>
  /// </remarks>
  public class RpInfluenceTrackBiddingTieResolution : ARoundPhase
  {
    /// <inheritdoc />
    public override RoundPhaseType Type => RoundPhaseType.InfluenceTrackBiddingTieResolution;

    /// <summary>
    /// Creates a new instance of the <see cref="RpInfluenceTrackBiddingTieResolution"/>
    /// class.
    /// </summary>
    ///
    /// <param name="gameStateService">The game state service.</param>
    /// <param name="houseStateService">The house state service.</param>
    /// <param name="influenceTrackBiddingStateService">The influence track bidding state
    /// service.</param>
    public RpInfluenceTrackBiddingTieResolution(
      IGameStateService gameStateService,
      IHouseStateService houseStateService,
      IInfluenceTrackBiddingStateService influenceTrackBiddingStateService
    ) : base(gameStateService, houseStateService)
    {
      m_influenceTrackBiddingStateService = influenceTrackBiddingStateService;
    }

    /// <inheritdoc />
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcResolveInfluenceTieGroup resolveTieGroupCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for round phase {Type}");

      string senderPlayerId = resolveTieGroupCommand.PlayerId;
      string validPlayerId;

      try
      { 
        validPlayerId = m_gameStateService.GetPlayerIdThatHoldsTheIronThroneToken(gameState);
      }
      catch (InvalidOperationException ex)
      {
        return Result.FAILURE($"Failed to get the player with the Iron Throne token: {ex.Message}");
      }

      if (senderPlayerId != validPlayerId)
        return Result.FAILURE($"Player {senderPlayerId} is not allowed to resolve the influence tie group. Only player {validPlayerId} can do that.");

      Result result = m_influenceTrackBiddingStateService.ResolveTieGroup(
        gameState.InfluenceTrackBiddingState,
        resolveTieGroupCommand.TiedGroupBreaker
      );

      if (!result.Success)
        return result;

      if (!m_influenceTrackBiddingStateService.HasTiedGroups(gameState.InfluenceTrackBiddingState))
        gameState.CurrentPhase = RoundPhaseType.InfluenceTrackBiddingPresentation;

      return Result.SUCCESS();
    }

    /// <inheritdoc />
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      return commandType == RoundPhaseCommandType.ResolveInfluenceTieGroup;
    }

    /// <summary>
    /// Reference to the influence track bidding state service, which is used to manage
    /// the state of influence track bidding in the game.
    /// </summary>
    private IInfluenceTrackBiddingStateService m_influenceTrackBiddingStateService;
  }
}
