using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to resolve a tie group in the influence track bidding phase of
  /// the game.
  /// </summary>
  public class RpcResolveInfluenceTieGroup : IRoundPhaseCommand
  {
    /// <inheritdoc />
    public RoundPhaseCommandType Type => RoundPhaseCommandType.ResolveInfluenceTieGroup;

    /// <summary>
    /// The ID of the player who wants to resolve the tie group. This is used to ensure
    /// that only the player who is allowed to break the tie can do so.
    /// </summary>
    public string PlayerId { get; }

    /// <summary>
    /// The tied group breaker that will be used to resolve the tie group.
    /// </summary>
    public InfluenceTrackTiedGroupBreaker TiedGroupBreaker { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="playerId">The ID of the player who wants to resolve the tie
    /// group.</param>
    /// <param name="tiedGroupBreaker">The tied group breaker that will be used to
    /// resolve the tie group.</param>
    public RpcResolveInfluenceTieGroup(
      string playerId,
      InfluenceTrackTiedGroupBreaker tiedGroupBreaker
    )
    {
      PlayerId = playerId;
      TiedGroupBreaker = tiedGroupBreaker;
    }
  }
}
