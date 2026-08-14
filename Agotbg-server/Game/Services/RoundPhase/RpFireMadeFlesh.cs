using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Represents the Fire Made Flesh round phase where the Targaryen player decides
  /// whether to take a dragon token or not.
  /// </summary>
  ///
  /// <remarks>
  /// Possible transitions from this phase:
  /// <list type="bullet">
  ///   <item><see cref="RoundPhaseType.Westeros"/></item>
  /// </list>
  /// </remarks>
  public class RpFireMadeFlesh : ARoundPhase
  {
    /// <inheritdoc/>
    public override RoundPhaseType Type => RoundPhaseType.FireMadeFlesh;

    /// <summary>
    /// Instantiates a new instance of the <see cref="RpFireMadeFlesh"/> class.
    /// </summary>
    ///
    /// <param name="gameStateService">The game state service.</param>
    /// <param name="houseStateService">The house state service.</param>
    /// <param name="dragonTokensStateService">The dragon tokens state service.</param>
    /// <param name="fireMadeFleshStateService">The Fire Made Flesh state
    /// service.</param>
    public RpFireMadeFlesh(
      IGameStateService gameStateService,
      IHouseStateService houseStateService,
      IDragonTokensStateService dragonTokensStateService,
      IFireMadeFleshStateService fireMadeFleshStateService
    ) : base(gameStateService, houseStateService)
    {
      m_dragonTokenStateService = dragonTokensStateService;
      m_fireMadeFleshStateService = fireMadeFleshStateService;
    }

    /// <inheritdoc/>
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command.Type == RoundPhaseCommandType.Resolve)
        return ExecuteResolve(gameState, command);
      else
        if (command.Type == RoundPhaseCommandType.ResolveFireMadeFlesh)
        return ExecuteResolveFireMadeFlesh(gameState, command);
      else
        return Result.FAILURE($"Invalid command type: {command.Type}");
    }

    /// <inheritdoc/>
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      return commandType == RoundPhaseCommandType.ResolveFireMadeFlesh ||
        commandType == RoundPhaseCommandType.Resolve;
    }

    /// <summary>
    /// A reference to the dragon tokens state service.
    /// </summary>
    private IDragonTokensStateService m_dragonTokenStateService;

    /// <summary>
    /// A reference to the fire made flesh state service.
    /// </summary>
    private IFireMadeFleshStateService m_fireMadeFleshStateService;

    /// <summary>
    /// Execute the resolution of the "Fire Made Flesh" phase, handling the player's
    /// decision regarding the dragon token.
    /// </summary>
    /// 
    /// <param name="gameState">The current state of the game.</param>
    /// <param name="command">The command to execute.</param>
    /// 
    /// <returns>The result of the command execution.</returns>
    private Result ExecuteResolveFireMadeFlesh(
      GameState gameState,
      IRoundPhaseCommand command
     )
    {
      if (command is not RpcResolveFireMadeFlesh resolveFmf)
        return Result.FAILURE($"Invalid command type: {command.Type}");

      if (gameState.FireMadeFleshState.IsCompleted)
        return Result.FAILURE("Fire Made Flesh has already been resolved.");

      PlayerState? targaryenPlayerState = gameState.Players
                                                    .FirstOrDefault(p => p.Value.HouseState.Type == HouseType.Targaryen)
                                                    .Value;

      FireMadeFleshState fmfState = gameState.FireMadeFleshState;
      if (targaryenPlayerState == null)
      {
        fmfState.IsCompleted = true;
        gameState.CurrentPhase = RoundPhaseType.Westeros;
        return Result.FAILURE("No Targaryen player found. Game phase has returned to Westeros.");
      }

      bool isValidPlayerId = targaryenPlayerState.PlayerId == resolveFmf.PlayerId;

      if (!isValidPlayerId)
        return Result.FAILURE($"Invalid player ID: {resolveFmf.PlayerId}. Only the Targaryen player can resolve Fire Made Flesh.");

      DragonTokensState dragonTokensState = gameState.DragonTokensState;

      if (resolveFmf.PlayerWantsDragonToken)
      {
        byte byteDesiredPosition = (byte)resolveFmf.PositionOfDesiredDragonToken;
        if (!dragonTokensState.AvailableDragonTokenPositions.Contains(byteDesiredPosition))
          return Result.FAILURE($"Invalid position of desired dragon token: {byteDesiredPosition}. The position must be one of the available dragon token positions.");

        fmfState.PositionOfDesiredDragonToken = byteDesiredPosition;
        fmfState.PlayersWantsDragonToken = true;
        fmfState.IsCompleted = true;
      } 
      else
      {
        fmfState.PlayersWantsDragonToken = false;
        fmfState.IsCompleted = true;
      }

      return Result.SUCCESS();
    }

    /// <summary>
    /// Executes the resolution of the "Fire Made Flesh" phase, taking into account the
    /// player's decision regarding the dragon token.
    /// </summary>
    /// 
    /// <param name="gameState">The current state of the game.</param>
    /// <param name="command">The command to execute.</param>
    /// 
    /// <returns>The result of the command execution.</returns>
    private Result ExecuteResolve(GameState gameState, IRoundPhaseCommand command)
    {
      if (command is not RpcResolve resolveCommand)
        return Result.FAILURE($"Invalid command type: {command.Type}");

      PlayerState? targaryenPlayerState = gameState.Players
                                                   .FirstOrDefault(p => p.Value.HouseState.Type == HouseType.Targaryen)
                                                   .Value;

      if (targaryenPlayerState == null)
      {
        gameState.FireMadeFleshState.IsCompleted = true;
        gameState.CurrentPhase = RoundPhaseType.Westeros;
        return Result.FAILURE("No Targaryen player found. Game phase has returned to Westeros.");
      }

      bool isValidPlayerId = targaryenPlayerState.PlayerId == resolveCommand.PlayerId
        || m_gameStateService.IsHoster(gameState, resolveCommand.PlayerId);

      if (!isValidPlayerId)
        return Result.FAILURE($"Invalid player ID: {resolveCommand.PlayerId}. Only the Targaryen player can resolve Fire Made Flesh.");

      FireMadeFleshState fmfState = gameState.FireMadeFleshState;
      if (!fmfState.IsCompleted)
        return Result.FAILURE("Fire Made Flesh has not been resolved yet.");

      if (!fmfState.PlayersWantsDragonToken)
      {
        gameState.CurrentPhase = RoundPhaseType.Westeros;
        return Result.SUCCESS();
      }

      Result result = m_dragonTokenStateService.TakeDragonToken
      (
        gameState.DragonTokensState,
        fmfState.PositionOfDesiredDragonToken
      );

      if (!result.Success)
      {
        m_fireMadeFleshStateService.Prepare(fmfState);
        return Result.FAILURE($"Failed to take dragon token: {result.Message}. The Fire Made Flesh phase has been reset.");
      }

      gameState.CurrentPhase = RoundPhaseType.Westeros;
      return Result.SUCCESS();
    }
  }
}
