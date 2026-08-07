using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to assign vassal houses to a player during the vassal
  /// assignment phase of the game.
  /// </summary>
  public class RpcAssignVassalHouses : IRoundPhaseCommand
  {
    /// <inheritdoc />
    public RoundPhaseCommandType Type => RoundPhaseCommandType.AssignVassalHouses;

    /// <summary>
    /// The ID of the player to whom the vassal houses are being assigned.
    /// </summary>
    public string PlayerId { get; } = string.Empty;

    /// <summary>
    /// The houses to be assigned to the player.
    /// </summary>
    public List<HouseType> HouseTypes { get; } = [];

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="playerId">The ID of the player to whom the vassal houses are being
    /// assigned.</param>
    /// <param name="houseTypes">The houses to be assigned to the player.</param>
    public RpcAssignVassalHouses(string playerId, List<HouseType> houseTypes)
    {
      PlayerId = playerId;
      HouseTypes = houseTypes;
    }
  }
}
