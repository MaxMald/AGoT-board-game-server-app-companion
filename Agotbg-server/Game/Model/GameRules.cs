namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the rules and settings for the game.
  /// </summary>
  public static class GameRules
  {
    public static readonly byte MaxPlayers = 8;
    public static readonly byte MinPlayers = 3;
    public static byte StartingPowerTokens { get; set; } = 5;
    public static byte NumRounds { get; set; } = 10;
    public static byte NumVictoryPointsToWin { get; set; } = 7;
    public static byte WildingMaxStrength { get; set; } = 12;
    public static byte WildingStartingStrength { get; set; } = 2;
    public static byte WildingStrengthStep { get; set; } = 2;
    public static byte MaximumNumberOfVassals { get; set; } = 4;
  }
}
