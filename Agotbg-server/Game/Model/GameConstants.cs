namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the rules and settings for the game.
  /// </summary>
  public static class GameConstants
  {
    public static readonly byte MaxPlayers = 8;
    public static readonly byte MinPlayers = 3;
    public static readonly byte StartingPowerTokens = 5;
    public static readonly byte StartingRoundNumber = 1;
    public static readonly byte NumRounds = 10;
    public static readonly byte NumVictoryPointsToWin = 7;
    public static readonly byte WildlingMaxStrength = 12;
    public static readonly byte WildingStartingStrength = 2;
    public static readonly byte WildlingStrengthStep = 2;
    public static readonly byte WildlingStrengthReduction = 4;
    public static readonly byte MaximumNumberOfVassals = 4;
    public static readonly byte MaximumSupplyLevel = 6;
    public static readonly byte MaximumDragonStrength = 5;
    public static readonly byte MaximumPowerTokens = 20; // TODO add rule to game logic
    public static readonly byte TargaryenInfluencePosition = 8;
  }
}
