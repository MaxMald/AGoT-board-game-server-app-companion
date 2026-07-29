namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the rules and settings for the game.
  /// </summary>
  public class GameRules
  {
    public static readonly byte MaxPlayers = 8;
    public static readonly byte MinPlayers = 3;

    public byte StartingPowerTokens { get; set; } = 5;
    public byte NumRounds { get; set; } = 10;
    public byte NumCastlesToWin { get; set; } = 7;
    public byte WildingMaxStrength { get; set; } = 12;
    public byte WildingStartingStrength { get; set; } = 2;
    public byte WildingStrengthStep { get; set; } = 2;
  }
}
