using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides services for managing the Iron Bank loan interest state, including
  /// initialization, resolution of interest payments, and clearing of the state.
  /// </summary>
  public class IronBankInterestPaymentStateService : IIronBankInterestPaymentStateService
  {
    /// <inheritdoc />
    public void Initialize(
      IronBankInterestPaymentState state,
      List<PlayerState> players
    )
    {
      foreach (PlayerState pa in players)
      {
        state.PlayerResults.Add(new IronBankLoantInterestPlayerResult
        {
          PlayerId = pa.PlayerId,
          Resolved = false,
          Succeeded = false,
          InterestAmount = 0,
          InterestPaid = 0
        });
      }
    }

    /// <inheritdoc />
    public bool HasAnyResolvedInterestPayment(IronBankInterestPaymentState state)
    {
      return state.PlayerResults.Any(pr => pr.Resolved);
    }

    /// <inheritdoc />
    public Result ResolvePlayerInterestPayment(
      IronBankInterestPaymentState state,
      PlayerState playerState
    )
    {
      IronBankLoantInterestPlayerResult? playerResult = state.PlayerResults
        .FirstOrDefault(pr => pr.PlayerId == playerState.PlayerId);

      if (playerResult == null)
        return Result.FAILURE($"Player result not found for player {playerState.PlayerId}");

      if (playerResult.Resolved)
        return Result.FAILURE($"Player {playerState.PlayerId} has already resolved their interest payment");

      byte interestAmount = playerState.HouseState.IronBankLoanInterest;
      if (interestAmount == 0)
        return Result.SUCCESS();

      playerResult.InterestAmount = interestAmount;
      playerResult.Resolved = true;

      if (playerState.HouseState.PowerTokens >= interestAmount)
      {
        playerState.HouseState.PowerTokens -= interestAmount;
        playerResult.InterestPaid = interestAmount;
        playerResult.Succeeded = true;
      }
      else
      {
        playerResult.InterestPaid = playerState.HouseState.PowerTokens;
        playerState.HouseState.PowerTokens = 0;
        playerResult.Succeeded = false;
      }

      return Result.SUCCESS();
    }

    /// <inheritdoc />
    public void Clear(IronBankInterestPaymentState state)
    {
      foreach (IronBankLoantInterestPlayerResult playerResult in state.PlayerResults)
      {
        playerResult.Resolved = false;
        playerResult.Succeeded = false;
        playerResult.InterestAmount = 0;
        playerResult.InterestPaid = 0;
      }
    }
  }
}
