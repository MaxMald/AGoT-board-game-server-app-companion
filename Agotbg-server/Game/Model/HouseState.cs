namespace Agotbg.Server.Game.Model
{
  /// <summary>
  /// Represents the current state of a house in the game, including resources, positions
  /// on influence tracks, army composition limits, and victory points.
  /// </summary>
  public class HouseState
  {
    /// <summary>
    /// Indicates the type of house of this House State.
    /// </summary>
    public HouseType Type { get; set; } = HouseType.Undefined;

    /// <summary>
    /// Indicates the number of power tokens this house has.
    /// </summary>
    public byte PowerTokens { get; set; } = 0;

    /// <summary>
    /// Indicates the supply level of this house, which is used to determine the maximum
    /// number of armies.
    /// </summary>
    public byte SupplyLevel { get; set; } = 0;

    /// <summary>
    /// Indicates the amount of Victory points, either castles or influence tokens if the
    /// house is Targaryen.
    /// </summary>
    public byte VictoryPoints { get; set; } = 0;

    /// <summary>
    /// Indicates the amount of power tokens this house has bid during the current
    /// bidding phase. This value is reset to 0 at the end of each bidding phase.
    /// </summary>
    public byte PowerTokensBid { get; set; } = 0;

    /// <summary>
    /// Indicates if this house has placed a bid during the current bidding phase.
    /// </summary>
    public bool HasBidPowerTokens { get; set; } = false;

    /// <summary>
    /// Indicates that the house has been defeated and cannot longer play the game.
    /// </summary>
    public bool IsDefeated { get; set; } = false;

    /// <summary>
    /// Indicates the 1-based position of this house in the Iron Throne track. 1 is the
    /// highest position.
    /// </summary>
    public byte IronThroneTrackPosition { get; set; } = 0;

    /// <summary>
    /// Indicates the 1-based position of this house in the Fiefdom track. 1 is the
    /// highest position.
    /// </summary>
    public byte FiefdomTrackPosition { get; set; } = 0;

    /// <summary>
    /// Indicates the 1-based position of this house in the King's Court track. 1 is the
    /// highest position.
    /// </summary>
    public byte KingsCourtTrackPosition { get; set; } = 0;

    /// <summary>
    /// Indicates the loan interest amount this house has with the Iron Bank.
    /// </summary>
    public byte IronBankLoanInterest { get; set; } = 0;

    /// <summary>
    /// List of vassal house types commanded by this house.
    /// </summary>
    public List<HouseType> VassalHouseTypes { get; set; } = [];

    //////////////////////////////////////////////////////
    /// TARGARYEN HOUSE PROPERTIES

    /// <summary>
    /// Indicates the strength of the dragons for this house. This only applies to the
    /// Targaryen house.
    /// </summary>
    public byte DragonStrength { get; set; } = 0;

    //////////////////////////////////////////////////////
    /// VASSAL HOUSE PROPERTIES

    /// <summary>
    /// Indicates if this house is a vassal house.
    /// </summary>
    public bool IsVassal { get; set; } = false;

    /// <summary>
    /// Indicates the house that commands this house. This only applies to vassal houses,
    /// which are commanded by a player house.
    /// </summary>
    public HouseType CommanderHouse { get; set; } = HouseType.Undefined;
  }
}
