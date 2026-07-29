namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the different phases of the game.
  /// </summary>
  public enum GamePhaseType : byte
  {
    Setup,
    Westeros,
    Planning,
    Action,
    EndOfRound
  }
}
