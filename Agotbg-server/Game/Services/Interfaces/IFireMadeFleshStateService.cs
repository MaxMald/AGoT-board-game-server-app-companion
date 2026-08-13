using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services.Interfaces
{
  /// <summary>
  /// Defines a service for managing the state of the "Fire Made Flesh" card in the game.
  /// </summary>
  public interface IFireMadeFleshStateService
  {
    /// <summary>
    /// Initialize the "Fire Made Flesh" state for a new game.
    /// </summary>
    ///
    /// <param name="state">The state of the "Fire Made Flesh" to
    /// initialize.</param>
    public void Initialize(FireMadeFleshState state);

    /// <summary>
    /// Prepares the "Fire Made Flesh" state for a new "Fire Made Flesh" game phase.
    /// </summary>
    /// 
    /// <param name="state">The state of the "Fire Made Flesh" card to prepare.</param>
    public void Prepare(FireMadeFleshState state);
  }
}
