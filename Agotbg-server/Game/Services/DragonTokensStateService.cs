using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides utility methods for managing the state of dragon tokens in the game.
  /// </summary>
  public static class DragonTokensStateService
  {
    /// <summary>
    /// Initialize the dragon tokens state by setting up the available positions and
    /// resetting the count of taken tokens.
    /// </summary>
    /// 
    /// <param name="state">The dragon tokens state to initialize.</param>
    public static void Initialize(DragonTokensState state)
    {
      state.AvailableDragonTokenPositions.Clear();
      state.AvailableDragonTokenPositions.Add(2);
      state.AvailableDragonTokenPositions.Add(4);
      state.AvailableDragonTokenPositions.Add(6);
      state.AvailableDragonTokenPositions.Add(8);
      state.AvailableDragonTokenPositions.Add(10);
      state.DragonTokensTaken = 0;
    }

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
    public static Result TakeDragonToken(DragonTokensState state, byte position)
    {
      if (!state.AvailableDragonTokenPositions.Contains(position))
        return Result.FAILURE($"Dragon token at position {position} is not available.");

      state.AvailableDragonTokenPositions.Remove(position);
      state.DragonTokensTaken++;
      return Result.SUCCESS();
    }
  }
}
