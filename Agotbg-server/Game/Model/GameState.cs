namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the complete state of a game, including all players, vassals, rounds,
  /// wildlings, winner.
  /// </summary>
  public class GameState
  {
    /// <summary>
    /// Dictionary of all players in the game, keyed by player ID.
    /// </summary>
    public Dictionary<string, PlayerState> Players { get; set; } = [];

    /// <summary>
    /// Dictionary of all vassal houses in the game, keyed by house type.
    /// </summary>
    public Dictionary<HouseType, HouseState> Vassals { get; set; } = [];

    /// <summary>
    /// Indicates the current round number of the game.
    /// </summary>
    public byte CurrentRound { get; set; } = 1;

    /// <summary>
    /// Indicates the current phase of the round.
    /// </summary>
    public RoundPhaseType CurrentPhase { get; set; } = RoundPhaseType.Setup;

    /// <summary>
    /// The current wildling state of the game.
    /// </summary>
    public WildlingsState Wildlings { get; set; } = new WildlingsState();

    /// <summary>
    /// The current influence track bidding state of the game.
    /// </summary>
    public InfluenceTrackBiddingState InfluenceTrackBiddingState { get; set; } = new InfluenceTrackBiddingState();

    /// <summary>
    /// Represents the state of a vassal assignment phase for this game.
    /// </summary>
    public VassalAssignmentState VassalAssignmentState { get; set; } = new VassalAssignmentState();

    /// <summary>
    /// Represents the state of dragon tokens in the game, including their available
    /// positions and the count taken by the Targaryen player.
    /// </summary>
    public DragonTokensState DragonTokensState { get; set; } = new DragonTokensState();

    /// <summary>
    /// The house type of the winner. Null if the game has not ended yet.
    /// </summary>
    public HouseType? Winner { get; set; } = null;

    /// <summary>
    /// Indicates if the game has ended.
    /// </summary>
    public bool IsGameOver { get; set; } = false;
  }
}
