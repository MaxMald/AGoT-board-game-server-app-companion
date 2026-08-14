
using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Game.Services.RoundPhase;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;
using Moq;
using System.Resources;

namespace Agotbg.Server.Utests.Game.Services.RoundPhase
{
  internal class RpActionTests : ARoundPhaseTest
  {
    [SetUp]
    public void Setup()
    {
      GameStateService = new Mock<IGameStateService>();
      HouseStateService = new Mock<IHouseStateService>();
      DragonTokensStateService = new Mock<IDragonTokensStateService>();
      InfluenceTrackService = new Mock<IInfluenceTrackService>();
      IronBankInterestPaymentStateService = new Mock<IIronBankInterestPaymentStateService>();

      RPAction = new RpAction(
        GameStateService.Object,
        HouseStateService.Object,
        DragonTokensStateService.Object,
        InfluenceTrackService.Object,
        IronBankInterestPaymentStateService.Object
      );
    }

    [Test]
    public void ExecuteResolve_ShouldFail_WhenPlayerIdIsNotHoster()
    {
      // Arrange
      GameState state = CreateGameStateWithHoster("HosterPlayerId", HouseType.Stark);
      PlayerState hosterPS = state.Players["HosterPlayerId"];

      GameStateService
        .Setup(
          gss => gss.IsHoster(
            It.IsAny<GameState>(),
            It.Is<string>(id => id == "NonHosterPlayerId")
          ))
        .Returns(false);

      // Act
      Result result = RPAction.Execute(
        state,
        new RpcResolve("NonHosterPlayerId")
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void ExecuteResolve_ShouldIncrementRound_WhenIsNotLastRound()
    {
      // Arrange
      GameState state = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(state, "Lannister", HouseType.Lannister);
      AddPlayerState(state, "Martell", HouseType.Martell);
      AddPlayerState(state, "Arryn", HouseType.Arryn);

      GameStateService
        .Setup(
          gss => gss.IsHoster(
            It.IsAny<GameState>(),
            It.Is<string>(id => id == "Stark")
          ))
        .Returns(true);

      state.CurrentRound = 3;

      GameStateService
        .Setup(gss => gss.IsLastRound(It.IsAny<GameState>()))
        .Returns(false);

      // Act
      Result result = RPAction.Execute(
        state,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.CurrentRound, Is.EqualTo(4));
    }

    [Test]
    public void ExecuteResolve_ShouldCall_PrepareForNextRound_WhenIsNotLastRoundAndHasTargaryenPlayer()
    {
      // Arrange
      GameState state = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(state, "Lannister", HouseType.Lannister);
      AddPlayerState(state, "Martell", HouseType.Martell);
      AddPlayerState(state, "Arryn", HouseType.Arryn);
      AddPlayerState(state, "Targaryen", HouseType.Targaryen);

      GameStateService
        .Setup(
          gss => gss.IsHoster(
            It.IsAny<GameState>(),
            It.Is<string>(id => id == "Stark")
          ))
        .Returns(true);

      GameStateService
        .Setup(gss => gss.IsLastRound(It.IsAny<GameState>()))
        .Returns(false);

      IronBankInterestPaymentStateService
        .Setup(ibipss => ibipss.HasAnyResolvedInterestPayment(It.IsAny<IronBankInterestPaymentState>()))
        .Returns(false);

      state.CurrentRound = 3;

      // Act
      Result result = RPAction.Execute(
        state,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);

      DragonTokensStateService.Verify(
        dtss => dtss.PrepareForNextRound(
          It.IsAny<DragonTokensState>(),
          It.Is<byte>(b => b == 4) // Next round number should be 4
        ),
        Times.Once
      );
    }

    [Test]
    public void ExecuteResolve_ShouldMoveToWinnerTieResolution_WhenIsLastRoundAndHasTiedPlayers()
    {
      // Arrange
      GameState state = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(state, "Lannister", HouseType.Lannister);
      AddPlayerState(state, "Martell", HouseType.Martell);

      state.CurrentRound = GameConstants.NumRounds; // Last round

      GameStateService
        .Setup(
          gss => gss.IsHoster(
            It.IsAny<GameState>(),
            It.Is<string>(id => id == "Stark")
          ))
        .Returns(true);

      GameStateService
        .Setup(gss => gss.IsLastRound(It.IsAny<GameState>()))
        .Returns(true);

      GameStateService
        .Setup(gss => gss.HasTiedPlayersByVictoryPoints(It.IsAny<GameState>()))
        .Returns(true);

      // Act
      Result result = RPAction.Execute(
        state,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.CurrentPhase, Is.EqualTo(RoundPhaseType.WinnerTieResolution));
    }

    [Test]
    public void ExecuteResolve_ShouldMoveToGameOver_WhenIsLastRoundAndNoTiedPlayers()
    {
      // Arrange
      GameState state = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(state, "Lannister", HouseType.Lannister);
      AddPlayerState(state, "Martell", HouseType.Martell);

      state.CurrentRound = GameConstants.NumRounds; // Last round

      GameStateService
        .Setup(
          gss => gss.IsHoster(
            It.IsAny<GameState>(),
            It.Is<string>(id => id == "Stark")
          ))
        .Returns(true);

      GameStateService
        .Setup(gss => gss.IsLastRound(It.IsAny<GameState>()))
        .Returns(true);

      GameStateService
        .Setup(gss => gss.HasTiedPlayersByVictoryPoints(It.IsAny<GameState>()))
        .Returns(false);

      // Act
      Result result = RPAction.Execute(
        state,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.CurrentPhase, Is.EqualTo(RoundPhaseType.GameOver));
    }

    [Test]
    public void ExecuteResolve_ShouldSetWinner_WhenIsLastRoundAndNoTiedPlayers()
    {
      // Arrange
      GameState state = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(state, "Lannister", HouseType.Lannister);
      AddPlayerState(state, "Martell", HouseType.Martell);

      state.CurrentRound = GameConstants.NumRounds; // Last round
      state.Players["Stark"].HouseState.VictoryPoints = 9;
      state.Players["Lannister"].HouseState.VictoryPoints = 10; // Highest victory points
      state.Players["Martell"].HouseState.VictoryPoints = 6;

      GameStateService
        .Setup(
          gss => gss.IsHoster(
            It.IsAny<GameState>(),
            It.Is<string>(id => id == "Stark")
          ))
        .Returns(true);

      GameStateService
        .Setup(gss => gss.IsLastRound(It.IsAny<GameState>()))
        .Returns(true);

      GameStateService
        .Setup(gss => gss.HasTiedPlayersByVictoryPoints(It.IsAny<GameState>()))
        .Returns(false);

      // Act
      Result result = RPAction.Execute(
        state,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.Winner, Is.EqualTo(HouseType.Lannister));
    }

    [Test]
    public void ExecuteResolve_ShouldCall_ResolvePlayerInterestPayment_WhenIsNotLastRound()
    {
      // Arrange
      GameState state = CreateGameStateWithHoster("Stark", HouseType.Stark);
      PlayerState starkPS = state.Players["Stark"];
      PlayerState lannisterPS =  AddPlayerState(state, "Lannister", HouseType.Lannister);
      PlayerState martellPS = AddPlayerState(state, "Martell", HouseType.Martell);
      PlayerState arryPS = AddPlayerState(state, "Arryn", HouseType.Arryn);
      PlayerState targaryenPS = AddPlayerState(state, "Targaryen", HouseType.Targaryen);

      GameStateService
        .Setup(
          gss => gss.IsHoster(
            It.IsAny<GameState>(),
            It.Is<string>(id => id == "Stark")
          ))
        .Returns(true);

      GameStateService
        .Setup(gss => gss.IsLastRound(It.IsAny<GameState>()))
        .Returns(false);

      IronBankInterestPaymentStateService
        .Setup(ibipss => ibipss.HasAnyResolvedInterestPayment(It.IsAny<IronBankInterestPaymentState>()))
        .Returns(true);

      state.CurrentRound = 3;
      starkPS.HouseState.IronBankLoanInterest = 2;
      lannisterPS.HouseState.IronBankLoanInterest = 3;
      martellPS.HouseState.IronBankLoanInterest = 1;
      arryPS.HouseState.IronBankLoanInterest = 1;
      targaryenPS.HouseState.IronBankLoanInterest = 2;

      // Act
      Result result = RPAction.Execute(
        state,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);

      IronBankInterestPaymentStateService.Verify(
        ibipss => ibipss.ResolvePlayerInterestPayment(
          It.IsAny<IronBankInterestPaymentState>(),
          It.Is<PlayerState>(ps => ps == starkPS)
        ),
        Times.Once
      );

      IronBankInterestPaymentStateService.Verify(
        ibipss => ibipss.ResolvePlayerInterestPayment(
          It.IsAny<IronBankInterestPaymentState>(),
          It.Is<PlayerState>(ps => ps == lannisterPS)
        ),
        Times.Once
      );

      IronBankInterestPaymentStateService.Verify(
        ibipss => ibipss.ResolvePlayerInterestPayment(
          It.IsAny<IronBankInterestPaymentState>(),
          It.Is<PlayerState>(ps => ps == martellPS)
        ),
        Times.Once
      );

      IronBankInterestPaymentStateService.Verify(
        ibipss => ibipss.ResolvePlayerInterestPayment(
          It.IsAny<IronBankInterestPaymentState>(),
          It.Is<PlayerState>(ps => ps == arryPS)
        ),
        Times.Once
      );
        
      IronBankInterestPaymentStateService.Verify(
        ibipss => ibipss.ResolvePlayerInterestPayment(
          It.IsAny<IronBankInterestPaymentState>(),
          It.Is<PlayerState>(ps => ps == targaryenPS)
        ),
        Times.Once
      );
    }

    [Test]
    public void ExecuteResolve_ShouldCall_ResolvePlayerInterestPayemer_JustForPlayersWithInterest_WhenIsNotLastRound()
    {
      // Arrange
      GameState state = CreateGameStateWithHoster("Stark", HouseType.Stark);
      PlayerState starkPS = state.Players["Stark"];
      PlayerState lannisterPS = AddPlayerState(state, "Lannister", HouseType.Lannister);
      PlayerState martellPS = AddPlayerState(state, "Martell", HouseType.Martell);

      GameStateService
        .Setup(
          gss => gss.IsHoster(
            It.IsAny<GameState>(),
            It.Is<string>(id => id == "Stark")
          ))
        .Returns(true);

      GameStateService
        .Setup(gss => gss.IsLastRound(It.IsAny<GameState>()))
        .Returns(false);

      IronBankInterestPaymentStateService
        .Setup(ibipss => ibipss.HasAnyResolvedInterestPayment(It.IsAny<IronBankInterestPaymentState>()))
        .Returns(true);

      state.CurrentRound = 3;
      starkPS.HouseState.IronBankLoanInterest = 0; // No interest
      lannisterPS.HouseState.IronBankLoanInterest = 3; // Has interest
      martellPS.HouseState.IronBankLoanInterest = 0; // No interest

      // Act
      Result result = RPAction.Execute(
        state,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      IronBankInterestPaymentStateService.Verify(
        ibipss => ibipss.ResolvePlayerInterestPayment(
          It.IsAny<IronBankInterestPaymentState>(),
          It.Is<PlayerState>(ps => ps == starkPS)
        ),
        Times.Never
      );

      IronBankInterestPaymentStateService.Verify(
        ibipss => ibipss.ResolvePlayerInterestPayment(
          It.IsAny<IronBankInterestPaymentState>(),
          It.Is<PlayerState>(ps => ps == lannisterPS)
        ),
        Times.Once
      );

      IronBankInterestPaymentStateService.Verify(
        ibipss => ibipss.ResolvePlayerInterestPayment(
          It.IsAny<IronBankInterestPaymentState>(),
          It.Is<PlayerState>(ps => ps == martellPS)
        ),
        Times.Never
      );
    }

    [Test]
    public void ExecuteResolve_ShouldMoveToIronBankInterestPaymentResolution_WhenIsNotLastRoundAndHasResolvedInterestPayments()
    {
      // Arrange
      GameState state = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(state, "Lannister", HouseType.Lannister);
      AddPlayerState(state, "Martell", HouseType.Martell);

      GameStateService
        .Setup(
          gss => gss.IsHoster(
            It.IsAny<GameState>(),
            It.Is<string>(id => id == "Stark")
          ))
        .Returns(true);

      GameStateService
        .Setup(gss => gss.IsLastRound(It.IsAny<GameState>()))
        .Returns(false);

      IronBankInterestPaymentStateService
        .Setup(ibipss => ibipss.HasAnyResolvedInterestPayment(It.IsAny<IronBankInterestPaymentState>()))
        .Returns(true);

      state.CurrentRound = 3;

      // Act
      Result result = RPAction.Execute(
        state,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.CurrentPhase, Is.EqualTo(RoundPhaseType.IronBankInterestPaymentResolution));
    }

    [Test]
    public void ExecuteResolve_ShouldMoveToWesterosWildlingIconsResolution_WhenIsNotLastRoundAndNoResolvedInterestPayments()
    {
      // Arrange
      GameState state = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(state, "Lannister", HouseType.Lannister);
      AddPlayerState(state, "Martell", HouseType.Martell);
      GameStateService
        .Setup(
          gss => gss.IsHoster(
            It.IsAny<GameState>(),
            It.Is<string>(id => id == "Stark")
          ))
        .Returns(true);

      GameStateService
        .Setup(gss => gss.IsLastRound(It.IsAny<GameState>()))
        .Returns(false);

      IronBankInterestPaymentStateService
        .Setup(ibipss => ibipss.HasAnyResolvedInterestPayment(It.IsAny<IronBankInterestPaymentState>()))
        .Returns(false);

      state.CurrentRound = 3;

      // Act
      Result result = RPAction.Execute(
        state,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.CurrentPhase, Is.EqualTo(RoundPhaseType.WesterosWildlingIconsResolution));
    }

    private RpAction RPAction { get; set; }
    private Mock<IGameStateService> GameStateService { get; set; }
    private Mock<IHouseStateService> HouseStateService { get; set; }
    private Mock<IDragonTokensStateService> DragonTokensStateService { get; set; }
    private Mock<IInfluenceTrackService> InfluenceTrackService { get; set; }
    private Mock<IIronBankInterestPaymentStateService> IronBankInterestPaymentStateService { get; set; }
  }
}
