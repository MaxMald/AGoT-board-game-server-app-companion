namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Describes the selection of a vassal house, including its type and the associated
  /// order token set type.
  /// </summary>
  public class VassalHouseSelectionDescriptor
  {
    /// <summary>
    /// The type of the vassal house being selected.
    /// </summary>
    public HouseType HouseType { get; set; }

    /// <summary>
    /// The type of vassal order token set associated with the selected house.
    /// </summary>
    public VassalOrderTokenSetType VassalOrderTokenSetType { get; set; }
  }
}
