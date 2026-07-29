namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the strength of the Wilding threat in the game.
  /// </summary>
  public class WildingState
  {
    public static readonly byte MaxStrength = 12;
    public byte Strength { get; set; } = 2;
  }
}
