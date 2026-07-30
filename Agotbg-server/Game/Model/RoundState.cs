namespace Agotbg.Server.Game.Model
{
  public class RoundState
  {
    public byte RoundNumber { get; set; } = 1;
    public RoundPhaseType CurrentPhase { get; set; } = RoundPhaseType.Setup;
  }
}
