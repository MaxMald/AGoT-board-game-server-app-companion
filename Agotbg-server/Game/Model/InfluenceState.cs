namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the three influence tracks: Iron Throne, Fiefdoms, and King's Court
  /// </summary>
  public class InfluenceState
  {
    public List<HouseType> IronThroneTrack { get; set; } = [];
    public List<HouseType> FiefdomsTrack { get; set; } = [];
    public List<HouseType> KingsCourtTrack { get; set; } = [];
  }
}
