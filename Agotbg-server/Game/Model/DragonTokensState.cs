namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the state of dragon tokens, including their available positions on the
  /// board and the count taken by the Targaryen player.
  /// </summary>
  public class DragonTokensState
  {
    /// <summary>
    /// List of positions of the current available dragon tokens on the board.
    /// </summary>
    public List<byte> AvailableDragonTokenPositions { get; set; } = [];

    /// <summary>
    /// Indicates the number of dragons tokens taken by the Targaryen player during the
    /// current round.
    /// </summary>
    public byte DragonTokensTaken { get; set; } = 0;
  }
}
