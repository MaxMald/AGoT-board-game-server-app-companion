using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to make a vassalage status between two houses during a round
  /// phase in the game.
  /// </summary>
  public class RpcMakeVassalageStatus : IRoundPhaseCommand
  {
    /// <inheritdoc />
    public RoundPhaseCommandType Type => RoundPhaseCommandType.MakeVassalageStatus;

    /// <summary>
    /// The ID of the commander player who is making the vassalage status.
    /// </summary>
    public string CommanderPlayerId { get; }

    /// <summary>
    /// The vassal house type that is being made a vassal of the commander's house.
    /// </summary>
    public HouseType VassalHouseType { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="commanderPlayerId">The ID of the commander player who is making the
    /// vassalage status.</param>
    /// <param name="vassalHouseType">The vassal house type that is being made a vassal
    /// of the commander's house.</param>
    public RpcMakeVassalageStatus(string commanderPlayerId, HouseType vassalHouseType)
    {
      CommanderPlayerId = commanderPlayerId;
      VassalHouseType = vassalHouseType;
    }
  }
}
