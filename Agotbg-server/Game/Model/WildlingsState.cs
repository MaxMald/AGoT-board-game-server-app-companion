namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Contains information related to the state of the Wildlings and the bidding process
  /// against them in the game.
  /// </summary>
  public class WildlingsState
  {
    /// <summary>
    /// Indicates the current strength of the wildlings.
    /// </summary>
    public byte Strength { get; set; } = 2;

    /// <summary>
    /// Indicates whether the Night Watch has won against the Wildlings.
    /// </summary>
    public bool NightWatchWins { get; set; } = false;

    /// <summary>
    /// Indicates the total bet amount the houses have placed against the Wildlings.
    /// </summary>
    public short TotalBetAmount { get; set; } = 0;

    /// <summary>
    /// Indicates the wildling strength at the start of the bidding phase.
    /// </summary>
    public short StrengthWhenBiddingStarted { get; set; } = 0;

    /// <summary>
    /// Indicates if the current wildling attack phase is resolving the "Preemptive Raid"
    /// event.
    /// </summary>
    public bool IsPreemptiveRaid { get; set; } = false;

    /// <summary>
    /// Contains the list of bets placed by houses when bidding against the Wildlings.
    /// </summary>
    public List<HouseBet> HouseBets { get; set; } = [];
  }
}
