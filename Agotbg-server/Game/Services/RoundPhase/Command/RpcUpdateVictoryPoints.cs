namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Represents a command to update a player's victory points.
  /// </summary>
  public class RpcUpdateVictoryPoints : IRoundPhaseCommand
  {
    /// <inheritdoc/>
    public RoundPhaseCommandType Type => RoundPhaseCommandType.UpdateVictoryPoints;

    /// <summary>
    /// The ID of the player whose victory points are being updated.
    /// </summary>
    public string PlayerId { get; }

    /// <summary>
    /// The new number of victory points.
    /// </summary>
    public byte NewVictoryPoints { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    ///
    /// <param name="playerId">The ID of the player whose victory points are being
    /// updated.</param>
    /// <param name="newVictoryPoints">The new number of victory points.</param>
    public RpcUpdateVictoryPoints(string playerId, byte newVictoryPoints)
    {
      PlayerId = playerId;
      NewVictoryPoints = newVictoryPoints;
    }
  }
}
