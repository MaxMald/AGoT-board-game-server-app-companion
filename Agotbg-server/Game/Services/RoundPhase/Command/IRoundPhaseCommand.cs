namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command that can be issued during a round phase.
  /// </summary>
  public interface IRoundPhaseCommand
  {
    /// <summary>
    /// The type of this round phase command.
    /// </summary>
    public RoundPhaseCommandType Type { get; }
  }
}
