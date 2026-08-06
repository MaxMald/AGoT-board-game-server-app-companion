namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Enumerates the possible Vassal Order Token Set types. These are defined by the
  /// icons on the Vassal Order Tokens, which can be Star, Circle, Triangle, or Square.
  /// The 'None' value indicates that no specific set type is assigned.
  /// </summary>
  public enum VassalOrderTokenSetType : byte
  {
    None,
    Star,
    Circle,
    Triangle,
    Square
  }
}
