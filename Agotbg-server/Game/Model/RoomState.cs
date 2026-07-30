namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the complete state of a game room.
  /// </summary>
  public class RoomState
  {
    public string RoomId { get; set; } = string.Empty;
    public string HosterPlayerId { get; set; } = string.Empty;
    public byte MaxPlayers { get; set; } = 3;
    public RoomStatus RoomStatus { get; set; } = RoomStatus.PreparingGame;
    public List<PlayerDescriptor> PlayersDescriptors { get; set; } = [];

    public Dictionary<string, PlayerState> Players { get; set; } = [];
    public Dictionary<HouseType, HouseState> Vassals { get; set; } = [];
    public RoundState Round { get; set; } = new RoundState();
    public WildingState Wilding { get; set; } = new WildingState();
    public InfluenceState Influence { get; set; } = new InfluenceState();
    public HouseType? Winner { get; set; } = null;
  }
}
