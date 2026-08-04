namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to resolve the number of Wildling icons during a round phase.
  /// </summary>
  public class RpcResolveWildlingIcons : IRoundPhaseCommand
  {
    /// <inheritdoc/>
    public RoundPhaseCommandType Type => RoundPhaseCommandType.ResolveWildlingIcons;

    /// <summary>
    /// The number of wildings icons to resolve.
    /// </summary>
    public byte NumWildlingIcons { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="numWildlingIcons">The number of wildling icons to resolve.</param>
    public RpcResolveWildlingIcons(byte numWildlingIcons)
    {
      NumWildlingIcons = numWildlingIcons;
    }
  }
}
