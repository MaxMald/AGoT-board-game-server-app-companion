using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to pillage a vassal house.
  /// </summary>
  public class RpcPillageVassal : IRoundPhaseCommand
  {
    /// <inheritdoc />
    public RoundPhaseCommandType Type => RoundPhaseCommandType.PillageVassal;

    /// <summary>
    /// The ID of the saboteur player.
    /// </summary>
    public string SaboteurPlayerId { get; }

    /// <summary>
    /// The type of the vassal house.
    /// </summary>
    public HouseType SabotagedHouseType { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// 
    /// <param name="saboteurPlayerId">The ID of the saboteur player.</param>
    /// <param name="sabotagedHouseType">The type of the vassal house.</param>
    public RpcPillageVassal(
      string saboteurPlayerId,
      HouseType sabotagedHouseType
    )
    {
      SaboteurPlayerId = saboteurPlayerId;
      SabotagedHouseType = sabotagedHouseType;
    }
  }
}
