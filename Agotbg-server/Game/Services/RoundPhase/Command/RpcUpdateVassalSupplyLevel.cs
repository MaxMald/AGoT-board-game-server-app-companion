using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to update the supply level of a vassal house during a round
  /// phase in the game.
  /// </summary>
  public class RpcUpdateVassalSupplyLevel : IRoundPhaseCommand
  {
    /// <inheritdoc />
    public RoundPhaseCommandType Type => RoundPhaseCommandType.UpdateVassalSupplyLevel;

    /// <summary>
    /// The vassal house type.
    /// </summary>
    public HouseType VassalHouseType { get; set; }

    /// <summary>
    /// The new supply level.
    /// </summary>
    public byte NewSupplyLevel { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// 
    /// <param name="vassalHouseType">The vassal house type.</param>
    /// <param name="newSupplyLevel">The new supply level.</param>
    public RpcUpdateVassalSupplyLevel(HouseType vassalHouseType, byte newSupplyLevel)
    {
      VassalHouseType = vassalHouseType;
      NewSupplyLevel = newSupplyLevel;
    }
  }
}
