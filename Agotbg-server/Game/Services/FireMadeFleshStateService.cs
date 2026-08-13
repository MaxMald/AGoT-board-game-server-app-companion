using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides utility methods for managing the state of the "Fire Made Flesh" card in
  /// the game.
  /// </summary>
  public class FireMadeFleshStateService : IFireMadeFleshStateService
  {
    /// <inheritdoc/>
    public void Initialize(FireMadeFleshState state)
    {
      state.PositionOfDesiredDragonToken = 0;
      state.PlayersWantsDragonToken = false;
      state.IsCompleted = false;
    }

    /// <inheritdoc />
    public void Prepare(FireMadeFleshState state)
    {
      state.PositionOfDesiredDragonToken = 0;
      state.PlayersWantsDragonToken = false;
      state.IsCompleted = false;
    }
  }
}
