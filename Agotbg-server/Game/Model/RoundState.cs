namespace Agotbg.Server.Game.Model
{
  public class RoundState
  {
    public byte RoundNumber { get; set; } = 1;
    public GamePhaseType CurrentPhase { get; set; } = GamePhaseType.Setup;
  }
}
