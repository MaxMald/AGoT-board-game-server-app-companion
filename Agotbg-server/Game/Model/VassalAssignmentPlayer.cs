namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents a player participating in the vassal assignment phase.
  /// </summary>
  public class VassalAssignmentPlayer
  {
    /// <summary>
    /// The ID of the player participating in the vassal assignment phase.
    /// </summary>
    public string PlayerId { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the next player in the turn order for the vassal assignment phase. An
    /// empty string indicates that there is no next player, which may occur if the
    /// current player is the last in the turn order.
    /// </summary>
    public string NextPlayerId { get; set; } = string.Empty;

    /// <summary>
    /// The status of the player during the vassal assignment phase, indicating whether
    /// they are waiting, selecting, or done.
    /// </summary>
    public VassalSelectionPlayerStatus Status { get; set; }

    /// <summary>
    /// The list of Vassal Order Token Set types that the player possesses. This list can
    /// include any combination of Star, Circle, Triangle, or Square types, and is used
    /// to determine the player's available number of vassal order tokens during the
    /// assignment phase.
    /// </summary>
    public List<VassalOrderTokenSetType> PossesedOrderTokenSets { get; set; } = [];

    /// <summary>
    /// The list vassal houses that the player has selected during the vassal assignment
    /// phase.
    /// </summary>
    public List<VassalHouseSelectionDescriptor> SelectedVassalHouses { get; set; } = [];
  }
}
