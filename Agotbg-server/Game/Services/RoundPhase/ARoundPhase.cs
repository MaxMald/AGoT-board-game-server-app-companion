using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// Base class for round phases that validates and executes commands specific to each
  /// phase type.
  /// </summary>
  public abstract class ARoundPhase : IRoundPhase
  {
    /// <inheritdoc/>
    public abstract RoundPhaseType Type { get; }

    /// <summary>
    /// Protected constructor for the ARoundPhase class, initializing the game state and
    /// house state services.
    /// </summary>
    /// 
    /// <param name="gameStateService">The game state service.</param>
    /// <param name="houseStateService">The house state service.</param>
    protected ARoundPhase(
      IGameStateService gameStateService,
      IHouseStateService houseStateService
    )
    {
      m_gameStateService = gameStateService;
      m_houseStateService = houseStateService;
    }

    /// <summary>
    /// Executes the given command for the current round phase, if the command type is
    /// valid for this phase.
    /// </summary>
    ///
    /// <param name="gameState">The current state of the game.</param>
    /// <param name="command">The command to execute.</param>
    /// 
    /// <returns>The result of executing the command.</returns>
    public Result Execute(GameState gameState, IRoundPhaseCommand command)
    {
      if (!IsValidCommandType(command.Type))
      {
        return Result.FAILURE(
          $"Invalid command type {command.Type} for round phase {Type}."
        );
      }

      return ExecuteDerived(gameState, command);
    }

    protected IGameStateService m_gameStateService;
    protected IHouseStateService m_houseStateService;
    protected List<HouseState> m_houseStates = [];

    /// <summary>
    /// Executes the command with derived class-specific logic.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// <param name="command">The round phase command to execute.</param>
    /// 
    /// <returns>The result of the command execution.</returns>
    protected abstract Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    );

    /// <summary>
    /// Determines whether the specified command type is valid for this round phase.
    /// </summary>
    ///
    /// <param name="commandType">The command type to validate.</param>
    /// 
    /// <returns>true if the command type is valid; otherwise, false.</returns>
    protected abstract bool IsValidCommandType(RoundPhaseCommandType commandType);

    /// <summary>
    /// Executes the <see cref="RpcModifyPowerTokens"/> command to modify the power
    /// tokens of a player.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// <param name="command">The round phase command to execute.</param>
    /// 
    /// <returns>The result of the command execution.</returns>
    protected Result ExecuteModifyPowerTokens(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcModifyPowerTokens modifyPowerTokensCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for modifying power tokens.");

      PlayerState? playerState = m_gameStateService.GetPlayerState(
        gameState,
        modifyPowerTokensCommand.PlayerId
      );

      if (playerState == null)
        return Result.FAILURE($"Player with ID {modifyPowerTokensCommand.PlayerId} not found.");

      short newPowerTokens = (short)(playerState.HouseState.PowerTokens + modifyPowerTokensCommand.Delta);
      newPowerTokens = Math.Min((short)255, Math.Max((short)0, newPowerTokens));

      m_houseStateService.UpdatePowerTokens(
        playerState.HouseState,
        (byte)newPowerTokens
      );

      return Result.SUCCESS();
    }

    /// <summary>
    /// Executes the <see cref="RpcTransferPowerTokens"/> command to transfer power
    /// tokens from one player to another.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// <param name="command">The round phase command to execute.</param>
    /// 
    /// <returns>The result of the command execution.</returns>
    protected Result ExecuteTransferPowerTokens(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcTransferPowerTokens transferPowerTokensCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for transferring power tokens.");

      PlayerState? fromPlayerState = m_gameStateService.GetPlayerState(
        gameState,
        transferPowerTokensCommand.FromPlayerId
      );

      if (fromPlayerState == null)
        return Result.FAILURE($"From player with ID {transferPowerTokensCommand.FromPlayerId} not found.");

      PlayerState? toPlayerState = m_gameStateService.GetPlayerState(
        gameState,
        transferPowerTokensCommand.ToPlayerId
      );

      if (toPlayerState == null)
        return Result.FAILURE($"To player with ID {transferPowerTokensCommand.ToPlayerId} not found.");

      return m_houseStateService.TransferPowerTokens(
        fromPlayerState.HouseState,
        toPlayerState.HouseState,
        transferPowerTokensCommand.Amount
      );
    }

    /// <summary>
    /// Executes the <see cref="RpcUpdateSupplyLevel"/> command to update the supply
    /// level of a player's house.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// <param name="command">The round phase command to execute.</param>
    /// 
    /// <returns>The result of the command execution.</returns>
    protected Result ExecuteUpdateSupplyLevel(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcUpdateSupplyLevel updateSupplyLevelCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for updating supply level.");

      PlayerState? playerState = m_gameStateService.GetPlayerState(
        gameState,
        updateSupplyLevelCommand.PlayerId
      );

      if (playerState == null)
        return Result.FAILURE($"Player with ID {updateSupplyLevelCommand.PlayerId} not found.");

      m_houseStateService.UpdateHouseSupplyLevel(
        playerState.HouseState,
        updateSupplyLevelCommand.NewSupplyLevel
      );

      return Result.SUCCESS();
    }

    /// <summary>
    /// Executes the <see cref="RpcUpdateVassalSupplyLevel"/> command to update the
    /// supply level of a vassal house.
    /// </summary>
    /// 
    /// <param name="gameState">The current game state.</param>
    /// <param name="command">The round phase command to execute.</param>
    /// 
    /// <returns>The result of the command execution.</returns>
    protected Result ExecuteUpdateVassalSupplyLevel(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcUpdateVassalSupplyLevel updateVassalSupplyLevelCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for updating vassal supply level.");

      HouseState? vassalHouseState = m_gameStateService.GetVassalHouseState(
        gameState,
        updateVassalSupplyLevelCommand.VassalHouseType
      );

      if (vassalHouseState == null)
        return Result.FAILURE($"Vassal house with type {updateVassalSupplyLevelCommand.VassalHouseType} not found.");

      m_houseStateService.UpdateHouseSupplyLevel(
        vassalHouseState,
        updateVassalSupplyLevelCommand.NewSupplyLevel
      );

      return Result.SUCCESS();
    }
    
    /// <summary>
    /// Executes the <see cref="RpcUpdateVictoryPoints"/> command to update the victory
    /// points of a player's house.
    /// </summary>
    ///
    /// <param name="gameState">The current game state.</param>
    /// <param name="command">The round phase command to execute.</param>
    ///
    /// <returns>The result of the command execution.</returns>
    protected Result ExecuteUpdateVictoryPoints(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcUpdateVictoryPoints updateVictoryPointsCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for updating victory points.");

      PlayerState? playerState = m_gameStateService.GetPlayerState(
        gameState,
        updateVictoryPointsCommand.PlayerId
      );

      if (playerState == null)
        return Result.FAILURE($"Player with ID {updateVictoryPointsCommand.PlayerId} not found.");

      m_houseStateService.UpdateVictoryPoints(
        playerState.HouseState,
        updateVictoryPointsCommand.NewVictoryPoints
      );

      return Result.SUCCESS();
    }

    /// <summary>
    /// Executes the <see cref="RpcUpdateIronBankLoanInterest"/> command to update the
    /// Iron Bank loan interest of a player's house.
    /// </summary>
    ///
    /// <param name="gameState">The current game state.</param>
    /// <param name="command">The round phase command to execute.</param>
    ///
    /// <returns>The result of the command execution.</returns>
    public Result ExecuteUpdateIronBankLoanInterest(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if (command is not RpcUpdateIronBankLoanInterest updateIronBankLoanInterestCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for updating Iron Bank loan interest.");

      PlayerState? playerState = m_gameStateService.GetPlayerState(
        gameState,
        updateIronBankLoanInterestCommand.PlayerId
      );

      if (playerState == null)
        return Result.FAILURE($"Player with ID {updateIronBankLoanInterestCommand.PlayerId} not found.");


      return m_houseStateService.UpdateIronBankLoanInterest(
        playerState.HouseState,
        updateIronBankLoanInterestCommand.NewInterest
      );
    }

    /// <summary>
    /// Executes the <see cref="RpcMoveInfluenceTrackPositionForHouse"/> command to move
    /// the influence track position for a house.
    /// </summary>
    ///
    /// <param name="gameState">The current game state.</param>
    /// <param name="command">The round phase command to execute.</param>
    ///
    /// <returns>The result of the command execution.</returns>
    public Result ExecuteMoveInfluenceTrackPositionForHouse(
      GameState gameState,
      IRoundPhaseCommand command,
      IInfluenceTrackService influenceTrackService
    )
    {
      if (command is not RpcMoveInfluenceTrackPositionForHouse moveInfluenceTrackPositionCommand)
        return Result.FAILURE($"Invalid command type {command.Type} for moving influence track position for house.");

      try
      {
        m_gameStateService.GetAllHouseStates(gameState, m_houseStates);
        influenceTrackService.MoveInfluenceTrackPositionForHouse(
          m_houseStates,
          moveInfluenceTrackPositionCommand.HouseType,
          moveInfluenceTrackPositionCommand.InfluenceTrackType,
          moveInfluenceTrackPositionCommand.NewPosition
        );
      }
      catch (Exception e)
      {
        return Result.FAILURE($"Error moving influence track position for house: {e.Message}");
      }
      return Result.SUCCESS();
    }
  }
}
