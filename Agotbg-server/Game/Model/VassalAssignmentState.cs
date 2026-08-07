namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// The VassalAssignmentState class represents the state of the vassal assignment phase
  /// in the game. It contains information about the available vassal houses, the players
  /// participating in the phase, and the current player whose turn it is.
  /// </summary>
  public class VassalAssignmentState
  {
    /// <summary>
    /// List of available vassal houses that players can choose from during the vassal
    /// assignment phase.
    /// </summary>
    public List<HouseType> AvailableVassalHouses { get; set; } = [];

    /// <summary>
    /// List of players participating in the vassal assignment phase.
    /// </summary>
    public List<VassalAssignmentPlayer> Players { get; set; } = [];

    /// <summary>
    /// The ID of the current player whose turn it is during the vassal assignment phase.
    /// </summary>
    public string CurrentPlayerID { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the vassal assignment phase has been completed. If true, the
    /// phase is finished, and the game can proceed to the next phase. If false, the
    /// phase is still ongoing, and players can continue to assign vassal houses.
    /// </summary>
    public bool IsCompleted { get; set; } = false;
  }
}
