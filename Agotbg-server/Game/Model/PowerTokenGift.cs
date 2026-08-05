namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents a gift of power tokens from one house to another in the game.
  /// </summary>
  public class PowerTokenGift
  {
    /// <summary>
    /// The receiver house.
    /// </summary>
    public HouseType Receiver;

    /// <summary>
    /// The amount of power tokens to gift.
    /// </summary>
    public byte Amount;
  }
}
