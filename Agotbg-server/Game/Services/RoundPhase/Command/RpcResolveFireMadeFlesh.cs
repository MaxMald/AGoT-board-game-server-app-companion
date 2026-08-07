namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to resolve the "Fire Made Flesh" phase in the game.
  /// </summary>
  public class RpcResolveFireMadeFlesh : IRoundPhaseCommand
  {
    /// <inheritdoc/>
    public RoundPhaseCommandType Type => RoundPhaseCommandType.ResolveFireMadeFlesh;

    /// <summary>
    /// The identifier of the player who is resolving the "Fire Made Flesh" phase.
    /// </summary>
    public string PlayerId { get; }

    /// <summary>
    /// Gets or sets the position of the desired dragon token.
    /// </summary>
    public int PositionOfDesiredDragonToken { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the player wants the dragon token.
    /// </summary>
    public bool PlayerWantsDragonToken { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="playerId">The identifier of the player who is resolving the "Fire
    /// Made Flesh" phase.</param>
    /// <param name="positionOfDesiredDragonToken">The position of the desired dragon
    /// token.</param>
    /// <param name="playerWantsDragonToken">A value indicating whether the player wants
    /// the dragon token.</param>
    public RpcResolveFireMadeFlesh(
      string playerId,
      int positionOfDesiredDragonToken,
      bool playerWantsDragonToken
    )
    {
      PlayerId = playerId;
      PositionOfDesiredDragonToken = positionOfDesiredDragonToken;
      PlayerWantsDragonToken = playerWantsDragonToken;
    }
  }
}
