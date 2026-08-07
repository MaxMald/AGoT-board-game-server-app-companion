namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to start a preemptive raid phase.
  /// </summary>
  public class RpcStartPreemptiveRaid : IRoundPhaseCommand
  {
    public RoundPhaseCommandType Type => RoundPhaseCommandType.StartPreemptiveRaid;
  }
}
