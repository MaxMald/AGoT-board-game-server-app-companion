using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Provides services for processing influence track bidding, determining house
  /// positions, and resolving tied bets.
  /// </summary>
  public class InfluenceTrackBiddingStateService : IInfluenceTrackBiddingStateService
  {
    /// <inheritdoc/>
    public void Initialize(InfluenceTrackBiddingState state)
    {
      Clear(state);
    }

    /// <inheritdoc/>
    public void Prepare(
      InfluenceTrackBiddingState state,
      InfluenceTrackType trackType
    )
    {
      Clear(state);
      state.InfluenceTrackType = trackType;
    }

    /// <inheritdoc/>
    public void ProcessBetsAndDeterminePositions(InfluenceTrackBiddingState state)
    {
      ApplyTargaryenGifts(state);

      List<HouseBet> bets = state.HouseBets.ToList();
      RemoveTargaryenBet(bets);

      state.TiedGroups.Clear();
      state.HouseInfluencePositions.Clear();
      byte trackPosition = 1;

      while (bets.Count > 0)
      {
        byte maxBetAmount = bets.Max(bet => bet.BetAmount);
        List<HouseBet> betsWithMaxAmount = bets.Select(bet => bet)
                                               .Where(bet => bet.BetAmount == maxBetAmount)
                                               .ToList();

        bets.RemoveAll(bet => bet.BetAmount == maxBetAmount);

        if (betsWithMaxAmount.Count() == 1)
        {
          HouseBet houseBet = betsWithMaxAmount.First();
          betsWithMaxAmount.Clear();

          HouseInfluencePositionItem positionItem = new()
          {
            HouseType = houseBet.HouseType,
            InfluencePosition = trackPosition
          };

          state.HouseInfluencePositions.Add(positionItem);
          ++trackPosition;
        }
        else
        {
          InfluenceTrackTiedGroup tiedGroup = new()
          {
            StartingPosition = trackPosition
          };

          foreach (HouseBet houseBet in betsWithMaxAmount)
          {
            tiedGroup.TiedHouses.Add(houseBet.HouseType);
          }

          state.TiedGroups.Add(tiedGroup);
          trackPosition += (byte)betsWithMaxAmount.Count;
        }
      }

      state.HouseBets.Clear();
    }

    /// <inheritdoc/>
    public bool HasTiedGroups(InfluenceTrackBiddingState state)
    {
      return state.TiedGroups.Count > 0;
    }

    /// <inheritdoc/>
    public Result ResolveTieGroup(
      InfluenceTrackBiddingState state,
      InfluenceTrackTiedGroupBreaker tiedGroupBreaker
    )
    {
      byte startingPosition = tiedGroupBreaker.StartingPosition;

      InfluenceTrackTiedGroup? tiedGroup = state.TiedGroups.FirstOrDefault(
        group => group.StartingPosition == startingPosition
      );

      if ( tiedGroup == null )
        return Result.FAILURE($"No tied group found at position {startingPosition}");

      List<HouseType> tiedHouses = tiedGroup.TiedHouses.ToList();
      List<HouseType> tieBreakerHouses = tiedGroupBreaker.HouseOrderedByPriority.ToList();

      bool areEqual = tiedHouses.OrderBy(x => x)
                                .SequenceEqual(tieBreakerHouses.OrderBy(x => x));

      if (!areEqual)
        return Result.FAILURE("The provided tie breaker houses do not match the tied group houses.");

      List<HouseType> housesOrderedByPriority = tiedGroupBreaker.HouseOrderedByPriority;

      for (int i = 0; i < housesOrderedByPriority.Count; i++)
      {
        HouseType house = housesOrderedByPriority[i];
        byte position = (byte)(startingPosition + i);

        HouseInfluencePositionItem positionItem = new()
        {
          HouseType = house,
          InfluencePosition = position
        };

        state.HouseInfluencePositions.Add(positionItem);
      }

      state.TiedGroups.Remove(tiedGroup);
      return Result.SUCCESS();
    }

    /// <inheritdoc/>
    public void Clear(InfluenceTrackBiddingState state)
    {
      state.InfluenceTrackType = InfluenceTrackType.None;
      state.TargaryenPowerTokenGifts.Clear();
      state.HouseBets.Clear();
      state.TiedGroups.Clear();
      state.HouseInfluencePositions.Clear();
    }

    /// <summary>
    /// Applies Targaryen's power token gifts to the corresponding house bets, increasing
    /// their bet amounts. Clears all gifts after application.
    /// </summary>
    /// 
    /// <param name="state">The influence track bidding state containing gifts and
    /// bets.</param>
    private static void ApplyTargaryenGifts(InfluenceTrackBiddingState state)
    {
      foreach (PowerTokenGift gift in state.TargaryenPowerTokenGifts)
      {
        HouseBet? houseBet = state.HouseBets.FirstOrDefault(
          bet => bet.HouseType == gift.Receiver
        );

        if (houseBet != null)
          houseBet.BetAmount += gift.Amount;
      }
      state.TargaryenPowerTokenGifts.Clear();
    }

    /// <summary>
    /// Removes Targaryen house from the list of bets, as Targaryen follows special
    /// positioning rules and does not participate in standard bidding.
    /// </summary>
    /// 
    /// <param name="houseBets">The list of house bets to filter.</param>
    private static void RemoveTargaryenBet(List<HouseBet> houseBets)
    {
      houseBets.RemoveAll(bet => bet.HouseType == HouseType.Targaryen);
    }
  }
}
