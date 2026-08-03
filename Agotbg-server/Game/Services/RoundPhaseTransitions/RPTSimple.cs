using Agotbg.Server.Game.Model;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhaseTransitions
{
  /// <summary>
  /// A simple implementation of the IRoundPhaseTransition interface that transitions to
  /// a specified round phase.
  ///
  /// This transition is used when a specific next phase is predetermined and does not
  /// require any additional logic or conditions to be met.
  /// </summary>
  public class RPTSimple : IRoundPhaseTransition
  {
    public RoundPhaseType To { get; }

    public RPTSimple(RoundPhaseType to) => To = to;

    public Result Execute(GameState state)
    {
      state.CurrentPhase = To;
      return Result.SUCCESS();
    }
  }
}
