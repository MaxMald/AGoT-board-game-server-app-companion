namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Enumerates the types of order restrictions that can be applied to a game state.
  /// </summary>
  public enum OrderRestrictionType : byte
  {
    None,
    NoDefenseOrder,
    NoMarchPlusOneOrder,
    NoRaidOrder,
    NoConsolidatePowerOrder,
    NoSupportOrder
  }
}
