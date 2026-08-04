namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to resolve a tie in the current round phase by supply level or
  /// Iron Throne position. This command is used to indicate that the players are ready
  /// to proceed to the next phase of the game after resolving a tie.
  /// </summary>
  public class RpcResolveTieBySupplyLevelOrIronThronePosition : IRoundPhaseCommand
  {
    /// <inheritdoc/>
    public RoundPhaseCommandType Type => RoundPhaseCommandType.ResolveTieBySupplyLevelOrIronThronePosition;
  }
}
