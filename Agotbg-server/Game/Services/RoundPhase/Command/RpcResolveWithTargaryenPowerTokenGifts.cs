using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to resolve the Targaryen power token gifts during a bidding
  /// phase.
  /// </summary>
  public class RpcResolveWithTargaryenPowerTokenGifts : IRoundPhaseCommand
  {
    /// <inheritdoc />
    public RoundPhaseCommandType Type => RoundPhaseCommandType.ResolveWithTargaryenPowerTokenGifts;

    /// <summary>
    /// The list of power tokens gifts.
    /// </summary>
    public List<PowerTokenGift> PowerTokenGifts { get; } = [];

    /// <summary>
    /// Constructor.
    /// </summary>
    /// 
    /// <param name="powerTokenGifts">The list of power tokens gifts.</param>
    public RpcResolveWithTargaryenPowerTokenGifts(List<PowerTokenGift> powerTokenGifts)
    {
      PowerTokenGifts = powerTokenGifts;
    }
  }
}
