using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Game.Services.RoundPhase;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;
using Moq;

namespace Agotbg.Server.Utests.Game.Services.RoundPhase
{
  internal class RpInfluenceTrackBiddingTests : ARoundPhaseTest
  {
    [SetUp]
    public void Setup()
    {
      GameStateServiceMock = new Mock<IGameStateService>();
      HouseStateServiceMock = new Mock<IHouseStateService>();
      InfluenceTrackBiddingStateServiceMock = new Mock<IInfluenceTrackBiddingStateService>();
      InfluenceTrackServiceMock = new Mock<IInfluenceTrackService>();

      RPInfluenceTrackBidding = new RpInfluenceTrackBidding(
        GameStateServiceMock.Object,
        HouseStateServiceMock.Object,
        InfluenceTrackBiddingStateServiceMock.Object,
        InfluenceTrackServiceMock.Object
      );
    }

    [Test]
    public void ExecuteCancelPowerTokenBid_ShouldCall_CancelPowerTokensBid()
    {
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.InfluenceTrackBiddingState.InfluenceTrackType
        = InfluenceTrackType.IronThrone;

      // Act
      Result result = RPInfluenceTrackBidding!.Execute(
        gameState,
        new RpcCancelPowerTokensBid("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);

      HouseStateServiceMock.Verify(
        hss => hss.CancelPowerTokensBid(
          It.Is<HouseState>(hs => hs.Type == HouseType.Stark)
        ),
        Times.Once
      );
    }

    [Test]
    public void ExecuteUpdatePowerTokensBid_ShouldCall_SubmitPowerTokensBid()
    {
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      PlayerState starkPS = gameState.Players["Stark"];
      AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.InfluenceTrackBiddingState.InfluenceTrackType
        = InfluenceTrackType.IronThrone;

      starkPS.HouseState.PowerTokens = 5;

      GameStateServiceMock.Setup(
        gss => gss.GetPlayerState(
          It.IsAny<GameState>(),
          It.Is<string>(s => s == "Stark")
        )
      ).Returns(starkPS);

      HouseStateServiceMock.Setup(
        hss => hss.SubmitPowerTokensBid(
          It.IsAny<HouseState>(),
          It.IsAny<byte>()
        )
      ).Returns(Result.SUCCESS());

      // Act
      Result result = RPInfluenceTrackBidding.Execute(
        gameState,
        new RpcUpdatePowerTokensBid("Stark", 5)
      );

      // Assert
      Assert.That(result.Success, Is.True);
      HouseStateServiceMock.Verify(
        hss => hss.SubmitPowerTokensBid(
          It.Is<HouseState>(hs => hs.Type == HouseType.Stark),
          It.Is<byte>(b => b == 5)
        ),
        Times.Once
      );
    }

    [Test]
    public void ExecuteResolve_ShouldFail_WhenNotHoster()
    {
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.InfluenceTrackBiddingState.InfluenceTrackType
        = InfluenceTrackType.IronThrone;

      GameStateServiceMock.Setup(
        gss => gss.IsHoster(
          It.IsAny<GameState>(),
          It.Is<string>(s => s == "Stark")
        )
      ).Returns(true);

      GameStateServiceMock.Setup(
        gss => gss.HaveAllPlayersSubmittedTheirBids(
          It.IsAny<GameState>()
        )
      ).Returns(true);

      GameStateServiceMock.Setup(
        gss => gss.CreateHouseBets(
          It.IsAny<GameState>()
        )
      ).Returns(
        new List<HouseBet>()
        {
          CreateHouseBet(HouseType.Stark, 5),
          CreateHouseBet(HouseType.Lannister, 3)
        }
      );

      // Act
      Result result = RPInfluenceTrackBidding.Execute(
        gameState,
        new RpcResolve("Lannister")
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void ExecuteResolve_ShouldFail_WhenNotAllPlayersHaveSubmittedBids()
    {
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.InfluenceTrackBiddingState.InfluenceTrackType
        = InfluenceTrackType.IronThrone;

      GameStateServiceMock.Setup(
        gss => gss.IsHoster(
          It.IsAny<GameState>(),
          It.Is<string>(s => s == "Stark")
        )
      ).Returns(true);

      GameStateServiceMock.Setup(
        gss => gss.HaveAllPlayersSubmittedTheirBids(
          It.IsAny<GameState>()
        )
      ).Returns(false);

      GameStateServiceMock.Setup(
        gss => gss.CreateHouseBets(
          It.IsAny<GameState>()
        )
      ).Returns(
        new List<HouseBet>()
        {
          CreateHouseBet(HouseType.Stark, 5)
        }
      );

      // Act
      Result result = RPInfluenceTrackBidding.Execute(
        gameState,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void ExecuteResolve_ShouldSucceed_WhenHosterAndAllPlayersHaveSubmittedBids()
    {
      // Arrange
      GameState gameState = PrepareScenarioWithBiddingsSubmitted_Stark_Lannister(5, 3);

      // Act
      Result result = RPInfluenceTrackBidding.Execute(
        gameState,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
    }

    [Test]
    public void ExecuteResolve_ShouldCall_ClearAllHouseSubmittedBids_WhenSuccessful()
    {
      GameState gameState = PrepareScenarioWithBiddingsSubmitted_Stark_Lannister(5, 3);

      // Act
      Result result = RPInfluenceTrackBidding.Execute(
        gameState,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      GameStateServiceMock.Verify(
        gss => gss.ClearAllHousesSubmittedBids(
          It.Is<GameState>(gs => gs == gameState)
        ),
        Times.Once
      );
    }

    [Test]
    public void ExecuteResolve_ShouldFail_WhenInfluenceTrackTypeIsNone()
    {
      // Arrange
      GameState gameState = PrepareScenarioWithBiddingsSubmitted_Stark_Lannister(1, 2);

      gameState.InfluenceTrackBiddingState.InfluenceTrackType
        = InfluenceTrackType.None;

      // Act
      var result = RPInfluenceTrackBidding.Execute(
        gameState,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void ExecuteResolve_ShouldUpdateInfluenceTrackBets_WhenSuccessful()
    {
      // Arrange
      GameState gameState = PrepareScenarioWithBiddingsSubmitted_Stark_Lannister(5, 3);
      PlayerState starkPS = gameState.Players["Stark"];
      PlayerState lannisterPS = gameState.Players["Lannister"];

      // Act
      Result result = RPInfluenceTrackBidding.Execute(
        gameState,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);

      List<HouseBet> expectedBets = new List<HouseBet>()
      {
        CreateHouseBet(HouseType.Stark, 5),
        CreateHouseBet(HouseType.Lannister, 3)
      };

      List<HouseBet> actualBets = gameState
        .InfluenceTrackBiddingState
        .HouseBets
        .ToList();

      expectedBets.Sort((a, b) => a.HouseType.CompareTo(b.HouseType));
      actualBets.Sort((a, b) => a.HouseType.CompareTo(b.HouseType));

      Assert.That(actualBets.Count, Is.EqualTo(expectedBets.Count));
      Assert.That(actualBets[0].HouseType, Is.EqualTo(expectedBets[0].HouseType));
      Assert.That(actualBets[0].BetAmount, Is.EqualTo(expectedBets[0].BetAmount));
      Assert.That(actualBets[1].HouseType, Is.EqualTo(expectedBets[1].HouseType));
      Assert.That(actualBets[1].BetAmount, Is.EqualTo(expectedBets[1].BetAmount));
    }

    [Test]
    public void ExecuteResolve_ShouldNotMoveToTargaryenResolution_WhenTargaryenIsNotPresent()
    {
      // Arrange
      GameState gameState = PrepareScenarioWithBiddingsSubmitted_Stark_Lannister(5, 3);
      PlayerState starkPS = gameState.Players["Stark"];
      PlayerState lannisterPS = gameState.Players["Lannister"];

      // Act
      Result result = RPInfluenceTrackBidding.Execute(
        gameState,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(gameState.CurrentPhase, Is.Not.EqualTo(RoundPhaseType.InfluenceTrackBiddingTargaryenResolution));
    }

    [Test]
    public void ExecuteResolve_ShouldNotMoveToTargaryenResolution_WhenTargaryenDidNotBet()
    {
      // Arrange
      GameState gameState = PrepareScenarioWithBiddingsSubmitted_Stark_Lannister_Targaryen(5, 3, 0);
      
      // Act
      Result result = RPInfluenceTrackBidding.Execute(
        gameState,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(gameState.CurrentPhase, Is.Not.EqualTo(RoundPhaseType.InfluenceTrackBiddingTargaryenResolution));
    }

    [Test]
    public void ExeucteResolve_ShouldMoveToTargaryenResolution_WhenTargaryenDidBet()
    {
      // Arrange
      GameState gameState = PrepareScenarioWithBiddingsSubmitted_Stark_Lannister_Targaryen(5, 3, 2);

      // Act
      Result result = RPInfluenceTrackBidding.Execute(
        gameState,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(gameState.CurrentPhase, Is.EqualTo(RoundPhaseType.InfluenceTrackBiddingTargaryenResolution));
    }

    [Test]
    public void ExecuteResolve_ShouldMoveToTieResolution_WhenThereIsATie()
    {
      // Arrange
      GameState gameState = PrepareScenarioWithBiddingsSubmitted_Stark_Lannister(5, 5);

      InfluenceTrackBiddingStateServiceMock.Setup(
        it => it.HasTiedGroups(
          It.IsAny<InfluenceTrackBiddingState>()
        )
      ).Returns(true);

      // Act
      Result result = RPInfluenceTrackBidding.Execute(
        gameState,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(gameState.CurrentPhase, Is.EqualTo(RoundPhaseType.InfluenceTrackBiddingTieResolution));
    }

    [Test]
    public void ExeucteResolve_ShouldCall_ProcessBetsAndDeterminePositions_WhenSuccessful()
    {
      // Arrange
      GameState gameState = PrepareScenarioWithBiddingsSubmitted_Stark_Lannister(5, 3);

      // Act
      Result result = RPInfluenceTrackBidding.Execute(
        gameState,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      InfluenceTrackBiddingStateServiceMock.Verify(
        it => it.ProcessBetsAndDeterminePositions(
          It.Is<InfluenceTrackBiddingState>(it => it == gameState.InfluenceTrackBiddingState)
        ),
        Times.Once
      );
    }

    [Test]
    public void ExecuteResolve_ShouldCall_UpdateInfluenceTrack_WhenSuccessful()
    {
      // Arrange
      GameState gameState = PrepareScenarioWithBiddingsSubmitted_Stark_Lannister(5, 3);

      // Act
      Result result = RPInfluenceTrackBidding.Execute(
        gameState,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      InfluenceTrackServiceMock.Verify(
        it => it.UpdateInfluenceTrackPositions(
          It.IsAny<List<HouseState>>(),
          It.IsAny<List<HouseInfluencePositionItem>>(),
          It.IsAny<InfluenceTrackType>()
        ),
        Times.Once
      );
    }

    [Test]
    public void ExecuteResolve_ShouldMoveToPresentationPhase_WhenSuccessful()
    {
      // Arrange
      GameState gameState = PrepareScenarioWithBiddingsSubmitted_Stark_Lannister(5, 3);

      // Act
      Result result = RPInfluenceTrackBidding.Execute(
        gameState,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(gameState.CurrentPhase, Is.EqualTo(RoundPhaseType.InfluenceTrackBiddingPresentation));
    }

    private static HouseBet CreateHouseBet(HouseType houseType, byte betAmount)
    {
      return new HouseBet()
      {
        HouseType = houseType,
        BetAmount = betAmount
      };
    }

    /// <summary>
    /// Creates and prepare a game state with two players (Stark and Lannister).
    ///
    /// <list type="bullet">
    ///   <item>Sets Stark as the hoster.</item>
    ///   <item>Sets power token amounts for bidding.</item>
    ///   <item>Setup necessary mocks for the game state service to simulate that all
    ///   players have submitted their bids and to create house bets.</item>
    ///   <item>Sets the influence track type to IronThrone.</item>
    /// </list>
    /// </summary>
    ///
    /// <param name="starkBetAmount">The bet amount of the stark player.</param>
    /// <param name="lannisterBetAmount">The bet amount of the lannister player.</param>
    ///
    /// <returns>The prepared game state.</returns>
    private GameState PrepareScenarioWithBiddingsSubmitted_Stark_Lannister(
      byte starkBetAmount,
      byte lannisterBetAmount
    )
    {
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      PlayerState starkPS = gameState.Players["Stark"];
      PlayerState lannisterPS = AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.InfluenceTrackBiddingState.InfluenceTrackType
        = InfluenceTrackType.IronThrone;

      starkPS.HouseState.PowerTokens = starkBetAmount;
      lannisterPS.HouseState.PowerTokens = lannisterBetAmount;

      GameStateServiceMock.Setup(
        gss => gss.IsHoster(
          It.IsAny<GameState>(),
          It.Is<string>(s => s == "Stark")
        )).Returns(true);

      GameStateServiceMock.Setup(
        gss => gss.HaveAllPlayersSubmittedTheirBids(
          It.IsAny<GameState>()
        )).Returns(true);

      GameStateServiceMock.Setup(
        gss => gss.CreateHouseBets(
          It.IsAny<GameState>()
        )).Returns(
          new List<HouseBet>()
          {
            new HouseBet() { HouseType = HouseType.Stark, BetAmount = starkBetAmount },
            new HouseBet() { HouseType = HouseType.Lannister, BetAmount = lannisterBetAmount }
          }
        );

      return gameState;
    }

    /// <summary>
    /// Creates and prepare a game state with three players (Stark, Lannister, and
    /// Targaryen).
    ///
    /// <list type="bullet">
    ///   <item>Sets Stark as the hoster.</item>
    ///   <item>Sets power token amounts for bidding.</item>
    ///   <item>Setup necessary mocks for the game state service to simulate that all
    ///   players have submitted their bids and to create house bets.</item>
    ///   <item>Sets the influence track type to IronThrone.</item>
    /// </list>
    /// </summary>
    ///
    /// <param name="starkBetAmount">The bet amount of the stark player.</param>
    /// <param name="lannisterBetAmount">The bet amount of the lannister player.</param>
    /// <param name="targaryenBetAmount">The bet amount of the targaryen player.</param>
    ///
    /// <returns>The prepared game state.</returns>
    private GameState PrepareScenarioWithBiddingsSubmitted_Stark_Lannister_Targaryen(
      byte starkBetAmount,
      byte lannisterBetAmount,
      byte targaryenBetAmount
    )
    {
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      PlayerState starkPS = gameState.Players["Stark"];
      PlayerState lannisterPS = AddPlayerState(gameState, "Lannister", HouseType.Lannister);
      PlayerState targaryenPS = AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);

      gameState.InfluenceTrackBiddingState.InfluenceTrackType
        = InfluenceTrackType.IronThrone;

      starkPS.HouseState.PowerTokens = starkBetAmount;
      lannisterPS.HouseState.PowerTokens = lannisterBetAmount;
      targaryenPS.HouseState.PowerTokens = targaryenBetAmount;

      GameStateServiceMock.Setup(
        gss => gss.IsHoster(
          It.IsAny<GameState>(),
          It.Is<string>(s => s == "Stark")
        )).Returns(true);

      GameStateServiceMock.Setup(
        gss => gss.HaveAllPlayersSubmittedTheirBids(
          It.IsAny<GameState>()
        )).Returns(true);

      GameStateServiceMock.Setup(
        gss => gss.CreateHouseBets(
          It.IsAny<GameState>()
        )).Returns(
          new List<HouseBet>()
          {
            new HouseBet() { HouseType = HouseType.Stark, BetAmount = starkBetAmount },
            new HouseBet() { HouseType = HouseType.Lannister, BetAmount = lannisterBetAmount },
            new HouseBet() { HouseType = HouseType.Targaryen, BetAmount = targaryenBetAmount }
          }
        );

      return gameState;
    }

    RpInfluenceTrackBidding RPInfluenceTrackBidding { get; set; }
    Mock<IGameStateService> GameStateServiceMock { get; set; }
    Mock<IHouseStateService> HouseStateServiceMock { get; set; }
    Mock<IInfluenceTrackBiddingStateService> InfluenceTrackBiddingStateServiceMock { get; set; }
    Mock<IInfluenceTrackService> InfluenceTrackServiceMock { get; set; }
  }
}
