namespace Agotbg.Server.Game.Model
{
  public class HouseState
  {
    public HouseType Type { get; set; } = HouseType.Undefined;
    public byte PowerTokens { get; set; } = 0;
    public byte SupplyLevel { get; set; } = 0;
    public byte VictoryPoints { get; set; } = 0;
    public bool IsDefeated { get; set; } = false;
  }
}
