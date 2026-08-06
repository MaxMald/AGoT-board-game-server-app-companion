namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Enumerates the possible statuses of a player during the vassal assignment phase.
  /// </summary>
  public enum VassalSelectionPlayerStatus : byte
  {
    Waiting,
    Selecting,
    Done
  }
}
