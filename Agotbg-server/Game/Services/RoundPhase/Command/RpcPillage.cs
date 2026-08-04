namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to pillage a power token from one player to another during a
  /// round phase in the game.
  /// </summary>
  public class RpcPillage : IRoundPhaseCommand
  {
    /// <inheritdoc />
    public RoundPhaseCommandType Type => RoundPhaseCommandType.Pillage;

    /// <summary>
    /// The ID of the saboteur player.
    /// </summary>
    public string SaboteurPlayerId { get; }

    /// <summary>
    /// The ID of the sabotaged player.
    /// </summary>
    public string SabotagedPlayerId { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="saboteurPlayerId">The ID of the saboteur player.</param>
    /// <param name="sabotagedPlayerId">The ID of the sabotaged player.</param>
    /// <param name="pillageAmount">The amount of power tokens to pillage.</param>
    public RpcPillage(
      string saboteurPlayerId,
      string sabotagedPlayerId,
      byte pillageAmount
    )
    {
      SaboteurPlayerId = saboteurPlayerId;
      SabotagedPlayerId = saboteurPlayerId;
    }
  }
}
