namespace Agotbg.Server.Game.Model
{
  public class PlayerState
  {
    public string PlayerId { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public HouseState HouseState { get; set; } = new HouseState();
  }
}
