using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to move a house to a new position on a specified influence
  /// track.
  /// </summary>
  public class RpcMoveInfluenceTrackPositionForHouse : IRoundPhaseCommand
  {
    public RoundPhaseCommandType Type => RoundPhaseCommandType.MoveInfluenceTrackPositionForHouse;

    /// <summary>
    /// The type of the house to modify.
    /// </summary>
    public HouseType HouseType { get; }

    /// <summary>
    /// The type of the influence track to modify.
    /// </summary>
    public InfluenceTrackType InfluenceTrackType { get; }

    /// <summary>
    /// The new 1-based position for the house on the influence track.
    /// </summary>
    public byte NewPosition { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="house">The type of the house to modify.</param>
    /// <param name="influenceTrackType">The type of the influence track to
    /// modify.</param>
    /// <param name="newPosition">The new 1-based position for the house on the influence
    /// track.</param>
    public RpcMoveInfluenceTrackPositionForHouse(
      HouseType house,
      InfluenceTrackType influenceTrackType,
      byte newPosition
    )
    {
      HouseType = house;
      InfluenceTrackType = influenceTrackType;
      NewPosition = newPosition;
    }
  }
}
