using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services.RoundPhase
{
  /// <summary>
  /// <para>
  /// The Westeros Wildling Icons Resolution round phase is responsible for resolving the
  /// number of Wildling icons that have been revealed during the Westeros phase. This
  /// phase calculates the total strength of the Wildlings based on the number of icons
  /// and updates the game state accordingly. Depending on the total strength, the game
  /// may transition to either the Wildlings Bidding phase or to the Westeros phase.
  /// </para>
  /// <para>
  /// Possible transitions from this phase include:
  /// <list type="bullet">
  ///   <item>Wildlings Bidding</item>
  ///   <item>Westeros</item>
  /// </list>
  /// </para>
  /// </summary>
  public class RpWesterosWildlingIconsResolution : ARoundPhase
  {
    /// <inheritdoc/>
    public override RoundPhaseType Type => RoundPhaseType.WesterosWildlingIconsResolution;

    /// <inheritdoc/>
    protected override Result ExecuteDerived(
      GameState gameState,
      IRoundPhaseCommand command
    )
    {
      if ( command is not RpcResolveWildlingIcons resolveWildlingIconsCommand)
        return Result.FAILURE("Invalid command type for this round phase.");

      if (resolveWildlingIconsCommand.NumWildlingIcons == 0)
      {
        gameState.CurrentPhase = RoundPhaseType.Westeros;
        return Result.SUCCESS();
      }

      byte iconsStrength = (byte)(resolveWildlingIconsCommand.NumWildlingIcons * GameConstants.WildlingStrengthStep);
      byte totalStrength = (byte)(gameState.Wildlings.Strength + iconsStrength);

      if (totalStrength > GameConstants.WildlingMaxStrength)
        totalStrength = GameConstants.WildlingMaxStrength;

      gameState.Wildlings.Strength = totalStrength;
      if (gameState.Wildlings.Strength >= GameConstants.WildlingMaxStrength)
      {
        gameState.CurrentPhase = RoundPhaseType.WildlingsBidding;
        return Result.SUCCESS();
      }

      gameState.CurrentPhase = RoundPhaseType.Westeros;
      return Result.SUCCESS();
    }

    /// <inheritdoc/>
    protected override bool IsValidCommandType(RoundPhaseCommandType commandType)
    {
      return commandType == RoundPhaseCommandType.ResolveWildlingIcons;
    }
  }
}
