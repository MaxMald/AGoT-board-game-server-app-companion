using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides utility methods for managing the state of the "Fire Made Flesh" card in
  /// the game.
  /// </summary>
  public static class FireMadeFleshStateService
  {
    /// <summary>
    /// Prepares the "Fire Made Flesh" state for a new "Fire Made Flesh" game phase.
    /// </summary>
    /// 
    /// <param name="state">The state of the "Fire Made Flesh" card to prepare.</param>
    public static void Prepare(FireMadeFleshState state)
    {
      state.PositionOfDesiredDragonToken = 0;
      state.PlayersWantsDragonToken = false;
      state.IsCompleted = false;
    }
  }
}
