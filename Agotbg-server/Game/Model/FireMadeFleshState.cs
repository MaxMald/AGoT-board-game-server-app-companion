namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the state of the "Fire Made Flesh" card in the game.
  /// </summary>
  ///
  /// <remarks>
  /// <para>
  /// During this event, the Targaryen player has the option to take a dragon token from
  /// the Round track to raise the Dragon's strength. However the Targaryen player may
  /// choose to revive a destroyed dragon miniature, which is not handle by the game
  /// state.
  /// </para>
  /// <para>
  /// The <see cref="PositionOfDesiredDragonToken"/> property indicates the position of
  /// the desired dragon token on the round track, while the <see
  /// cref="PlayersWantsDragonToken"/> property indicates if the Targaryen player wants a
  /// dragon token placed on the round track. It would be false if the Targaryen player
  /// chooses to revive a destroyed dragon miniature instead.
  /// </para>
  /// <para>
  /// The <see cref="IsCompleted"/> property indicates if the "Fire Made Flesh" card
  /// has been completed and resolved.
  /// </para>
  /// </remarks>
  public class FireMadeFleshState
  {
    /// <summary>
    /// Indicates the position of the desired dragon token on the round track.
    /// </summary>
    public byte PositionOfDesiredDragonToken { get; set; }

    /// <summary>
    /// Indicates if the Targaryen player wants a dragon token placed on the round track.
    /// </summary>
    public bool PlayersWantsDragonToken { get; set; }

    /// <summary>
    /// Indicates if the "Fire Made Flesh" card has been completed and resolved.
    /// </summary>
    public bool IsCompleted { get; set; } = false;
  }
}
