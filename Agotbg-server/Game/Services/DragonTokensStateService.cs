using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides utility methods for managing the state of dragon tokens in the game.
  /// </summary>
  public class DragonTokensStateService : IDragonTokensStateService
  {
    /// <inheritdoc/>
    public void Initialize(DragonTokensState state)
    {
      state.AvailableDragonTokenPositions.Clear();
      state.AvailableDragonTokenPositions.Add(2);
      state.AvailableDragonTokenPositions.Add(4);
      state.AvailableDragonTokenPositions.Add(6);
      state.AvailableDragonTokenPositions.Add(8);
      state.AvailableDragonTokenPositions.Add(10);
      state.DragonTokensTaken = 0;
    }

    /// <inheritdoc/>
    public Result TakeDragonToken(DragonTokensState state, byte position)
    {
      if (!state.AvailableDragonTokenPositions.Contains(position))
        return Result.FAILURE($"Dragon token at position {position} is not available.");

      state.AvailableDragonTokenPositions.Remove(position);
      state.DragonTokensTaken++;
      return Result.SUCCESS();
    }

    /// <inheritdoc/>
    public void PrepareForNextRound(DragonTokensState state, byte nextRound)
    {
      if (state.AvailableDragonTokenPositions.Contains(nextRound))
      {
        state.AvailableDragonTokenPositions.Remove(nextRound);
        state.DragonTokensTaken++;
      }
    }
  }
}
