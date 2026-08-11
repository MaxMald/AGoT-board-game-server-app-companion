using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.Interfaces
{
  /// <summary>
  /// Defines the interface for managing the state of dragon tokens in the game.
  /// </summary>
  public interface IDragonTokensStateService
  {
    /// <summary>
    /// Initialize the dragon tokens state by setting up the available positions and
    /// resetting the count of taken tokens.
    /// </summary>
    /// 
    /// <param name="state">The dragon tokens state to initialize.</param>
    public void Initialize(DragonTokensState state);

    /// <summary>
    /// Attempts to take a dragon token from the specified position. If the position is
    /// available, it removes the token from the available positions and increments the
    /// count of taken tokens.
    /// </summary>
    ///
    /// <param name="state">The dragon tokens state to update.</param>
    /// <param name="position">The position of the dragon token to take.</param>
    ///
    /// <returns>A Result indicating success or failure of the operation.</returns>
    public Result TakeDragonToken(DragonTokensState state, byte position);

    /// <summary>
    /// Prepares the dragon tokens state for the next round by checking if a token is
    /// available at the specified round position. If available, it removes the token and
    /// increments the count of taken tokens.
    /// </summary>
    ///
    /// <param name="state">The dragon tokens state to update.</param>
    ///
    /// <param name="nextRound">The position of the dragon token for the next
    /// round.</param>
    public void PrepareForNextRound(DragonTokensState state, byte nextRound);
  }
}
