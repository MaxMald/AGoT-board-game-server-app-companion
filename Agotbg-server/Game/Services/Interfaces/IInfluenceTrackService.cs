using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services.Interfaces
{
  /// <summary>
  /// Defines the contract for managing influence tracks in the game.
  /// </summary>
  public interface IInfluenceTrackService
  {
    /// <summary>
    /// Initializes all three influence tracks for the given houses based on their
    /// starting positions defined in the game rules.
    /// </summary>
    ///
    /// <param name="houses">The list of houses to position on the influence
    /// tracks.</param>
    public void Initialize(List<HouseState> houses);

    /// <summary>
    /// Moves a specified house to a new position on the given influence track. This
    /// shifts other houses accordingly to maintain the order. The new position is
    /// 1-based, with 1 being the highest position on the track.
    /// </summary>
    ///
    /// <param name="houses">The list of houses.</param>
    /// <param name="houseType">The type of the house to move.</param>
    /// <param name="trackType">The type of the influence track.</param>
    /// <param name="newPosition">The new 1-based position for the house.</param>
    ///
    /// <exception cref="Exception"/>
    /// <exception cref="ArgumentException"/>
    public void MoveInfluenceTrackPositionForHouse(
      List<HouseState> houses,
      HouseType houseType,
      InfluenceTrackType trackType,
      byte newPosition
    );

    /// <summary>
    /// Updates the influence track positions for the provided houses based on the given
    /// list of house influence position items. Each item specifies a house and its new
    /// position on the specified influence track. The method ensures that the houses are
    /// updated correctly according to the provided positions.
    /// </summary>
    ///
    /// <param name="houses">The list of houses to update.</param>
    /// <param name="houseInfluencePositions">The list of house influence position
    /// items.</param>
    /// <param name="trackType">The type of the influence track.</param>
    ///
    /// <exception cref="Exception">Thrown when a house in the provided list is not
    /// found.</exception>
    public void UpdateInfluenceTrackPositions(
      List<HouseState> houses,
      List<HouseInfluencePositionItem> houseInfluencePositions,
      InfluenceTrackType trackType
    );
  }
}
