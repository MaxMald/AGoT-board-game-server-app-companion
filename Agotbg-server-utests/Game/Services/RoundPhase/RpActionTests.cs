
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

    private RpAction RPAction { get; set; }
    private Mock<IGameStateService> GameStateService { get; set; }
    private Mock<IHouseStateService> HouseStateService { get; set; }
    private Mock<IDragonTokensStateService> DragonTokensStateService { get; set; }
    private Mock<IInfluenceTrackService> InfluenceTrackService { get; set; }
    private Mock<IIronBankInterestPaymentStateService> IronBankInterestPaymentStateService { get; set; }
  }
}
