namespace Agotbg.Server.Game.Services.RoundPhase.Command
{
  /// <summary>
  /// Enumerates the different types of commands that can be issued during a round phase.
  /// </summary>
  public enum RoundPhaseCommandType : byte
  {
    None,
    Resolve,
    ResolveAndMoveTo,
    ResolveWithWinner,
    ResolveTieBySupplyLevelOrIronThronePosition,
    ResolveWildlingIcons,
    ModifyPowerTokens,
    TransferPowerTokens,
    UpdateSupplyLevel,
    UpdateVassalSupplyLevel,
    UpdateVictoryPoints,
    UpdatePowerTokensBid,
    CancelPowerTokensBid,
    UpdateIronBankLoanInterest,
    Pillage,
    PillageVassal,
    MakeVassalageStatus,
    BreakVassalageStatus
  }
}
