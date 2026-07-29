namespace Agotbg.Server.Game.Model
{
  public class VassalState
  {
    public HouseType House { get; set; } = HouseType.Undefined;
    public HouseType CommanderHouse { get; set; } = HouseType.Undefined;
    public byte SupplyLevel { get; set; } = 0;
  }
}
