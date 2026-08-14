namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the state of the Iron Bank loan interest resolution phase, containing
  /// results for each player's interest payment attempt.
  /// </summary>
  public class IronBankInterestPaymentState
  {
    /// <summary>
    /// List of player results for the Iron Bank Loan Interest Resolution phase. Each
    /// entry represents the result of an interest payment attempt for a specific player.
    /// </summary>
    public List<IronBankLoantInterestPlayerResult> PlayerResults { get; set; } = [];
  }
}
