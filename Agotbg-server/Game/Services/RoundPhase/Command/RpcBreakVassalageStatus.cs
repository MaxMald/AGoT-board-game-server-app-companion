using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to break a vassalage status between two houses during a round
  /// phase in the game.
  /// </summary>
  public class RpcBreakVassalageStatus : IRoundPhaseCommand
  {
    /// <inheritdoc />
    public RoundPhaseCommandType Type => RoundPhaseCommandType.BreakVassalageStatus;

    /// <summary>
    /// The ID of the commander player who is breaking the vassalage status.
    /// </summary>
    public string CommanderPlayerId { get; }

    /// <summary>
    /// The vassal house type that is being broken from the commander's house.
    /// </summary>
    public HouseType VassalHouseType { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="commanderPlayerId">The ID of the commander player who is breaking the
    /// vassalage status.</param>
    /// <param name="vassalHouseType">The vassal house type that is being broken from the
    /// commander's house.</param>
    public RpcBreakVassalageStatus(string commanderPlayerId, HouseType vassalHouseType)
    {
      CommanderPlayerId = commanderPlayerId;
      VassalHouseType = vassalHouseType;
    }
  }
}
