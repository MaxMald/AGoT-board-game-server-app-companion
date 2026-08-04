namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents a bet placed by a house in the game.
  /// </summary>
  public class HouseBet
  {
    /// <summary>
    /// The type of the house placing the bet.
    /// </summary>
    public HouseType HouseType;

    /// <summary>
    /// The bet amount.
    /// </summary>
    public byte BetAmount;
  }
}
