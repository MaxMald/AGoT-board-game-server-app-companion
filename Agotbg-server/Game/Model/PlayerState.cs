namespace Agotbg.Server.Game.Model
{
  public class PlayerState
  {
    /// <summary>
    /// The unique id of this player.
    /// </summary>
    public string PlayerId { get; set; } = string.Empty;

    /// <summary>
    /// The house state of this player.
    /// </summary>
    public HouseState HouseState { get; set; } = new HouseState();
  }
}
