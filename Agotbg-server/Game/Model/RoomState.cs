namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the complete state of a game room.
  /// </summary>
  public class RoomState
  {
    public string RoomId { get; set; } = string.Empty;
    public byte NumPlayers { get; set; } = 6;
    public GameRules GameRules { get; set; } = new GameRules();

    public Dictionary<string, PlayerState> Players { get; set; } = [];
    public RoundState Round { get; set; } = new RoundState();
    public WildingState Wilding { get; set; } = new WildingState();
    public InfluenceState Influence { get; set; } = new InfluenceState();

    public bool IsGameStarted { get; set; } = false;
    public bool IsGameFinished { get; set; } = false;
    public HouseType? Winner { get; set; } = null;
  }
}
