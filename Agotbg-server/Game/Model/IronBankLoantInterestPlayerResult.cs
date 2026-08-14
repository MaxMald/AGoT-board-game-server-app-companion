namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the result of an interest payment attempt for a player during the Iron
  /// Bank Loan Interest Resolution phase.
  /// </summary>
  public class IronBankLoantInterestPlayerResult
  {
    /// <summary>
    /// The id of the player which this interest state belongs to.
    /// </summary>
    public string PlayerId { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if this result has been resolved. If a player has no loans, this result
    /// will be unresolved and will not be considered in the Iron Bank Loan Interest
    /// Resolution phase.
    /// </summary>
    public bool Resolved { get; set; } = false;

    /// <summary>
    /// Indicates if the last interest payment attempt for this player was successful.
    /// </summary>
    public bool Succeeded { get; set; } = false;

    /// <summary>
    /// Indicates the interest amount that the player needs to pay.
    /// </summary>
    public byte InterestAmount { get; set; } = 0;

    /// <summary>
    /// Indicates the actual amount of interest that the player paid. This can be less
    /// than the interest amount if the player has insufficient power tokens.
    /// </summary>
    public byte InterestPaid { get; set; } = 0;
  }
}
