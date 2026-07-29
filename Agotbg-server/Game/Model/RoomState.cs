namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the complete state of a game room.
  /// </summary>
  public class RoomState
  {
    public string RoomId { get; set; } = string.Empty;
    public byte MaxPlayers { get; set; } = 3;

    public Dictionary<string, PlayerState> Players { get; set; } = [];
    public Dictionary<HouseType, VassalState> Vassals { get; set; } = [];
    public RoundState Round { get; set; } = new RoundState();
    public WildingState Wilding { get; set; } = new WildingState();
    public InfluenceState Influence { get; set; } = new InfluenceState();

    public bool UseVassals { get; set; } = false;
    public bool IsGameStarted { get; set; } = false;
    public bool IsGameFinished { get; set; } = false;
    public HouseType? Winner { get; set; } = null;
  }
}
