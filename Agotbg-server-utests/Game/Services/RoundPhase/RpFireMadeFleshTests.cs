using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Game.Services.RoundPhase;
using Agotbg.Server.Game.Services.RoundPhase.Command;
using Agotbg.Server.Utilities;
using Moq;

namespace Agotbg.Server.Utests.Game.Services.RoundPhase
{
  internal class RpFireMadeFleshTests : ARoundPhaseTest
  {
    [SetUp]
    public void Setup()
    {
      GameStateServiceMock = new Mock<IGameStateService>();
      HouseStateServiceMock = new Mock<IHouseStateService>();
      DragonTokensStateServiceMock = new Mock<IDragonTokensStateService>();
      FireMadeFleshStateServiceMock = new Mock<IFireMadeFleshStateService>();

      RPFireMadeFlesh = new RpFireMadeFlesh(
        GameStateServiceMock.Object,
        HouseStateServiceMock.Object,
        DragonTokensStateServiceMock.Object,
        FireMadeFleshStateServiceMock.Object
      );
    }

    [Test]
    public void ExecuteResolveFireMadeFlesh_ShouldFail_WhenStateIsAlreadyCompleted()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);

      gameState.FireMadeFleshState.IsCompleted = true;

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolveFireMadeFlesh("Targaryen", 1, true)
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void ExecuteResolveFireMadeFlesh_ShouldFail_WhenPlayerIsNotTargaryen()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);

      gameState.FireMadeFleshState.IsCompleted = false;

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolveFireMadeFlesh("Stark", 1, true)
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(gameState.FireMadeFleshState.IsCompleted, Is.False);
    }

    [Test]
    public void ExecuteResolveFireMadeFlesh_ShouldFailAndMoveToWesteros_WhenThereIsNoTargaryenPlayer()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.FireMadeFleshState.IsCompleted = false;

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolveFireMadeFlesh("Lannister", 1, true)
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(gameState.CurrentPhase, Is.EqualTo(RoundPhaseType.Westeros));
    }

    [Test]
    public void ExecuteResolveFireMadeFlesh_ShouldUpdateFireMadeFleshState_WhenPlayerWantsDragonToken()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);

      gameState.FireMadeFleshState.IsCompleted = false;
      gameState.DragonTokensState.AvailableDragonTokenPositions.Add(2);
      gameState.DragonTokensState.AvailableDragonTokenPositions.Add(4);

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolveFireMadeFlesh("Targaryen", 2, true)
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(gameState.FireMadeFleshState.IsCompleted, Is.True);
      Assert.That(gameState.FireMadeFleshState.PlayersWantsDragonToken, Is.True);
      Assert.That(gameState.FireMadeFleshState.PositionOfDesiredDragonToken, Is.EqualTo(2));
    }

    [Test]
    public void ExecuteResolveFireMadeFlesh_ShouldFail_WhenPlayerWantsDragonTokenButPositionIsInvalid()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);

      gameState.FireMadeFleshState.IsCompleted = false;
      gameState.DragonTokensState.AvailableDragonTokenPositions.Add(2);
      gameState.DragonTokensState.AvailableDragonTokenPositions.Add(4);

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolveFireMadeFlesh("Targaryen", 3, true)
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(gameState.FireMadeFleshState.IsCompleted, Is.False);
    }

    [Test]
    public void ExecuteResolveFireMadeFlesh_ShouldUpdateFireMadeFleshState_WhenPlayerDoesNotWantDragonToken()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);

      gameState.FireMadeFleshState.IsCompleted = false;

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolveFireMadeFlesh("Targaryen", 0, false)
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(gameState.FireMadeFleshState.IsCompleted, Is.True);
      Assert.That(gameState.FireMadeFleshState.PlayersWantsDragonToken, Is.False);
    }

    [Test]
    public void ExecuteResolve_ShouldFailAndMoveToWesteros_WhenTargaryenPlayerIsNotFound()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.FireMadeFleshState.IsCompleted = false;

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolve("Lannister")
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(gameState.CurrentPhase, Is.EqualTo(RoundPhaseType.Westeros));
    }

    [Test]
    public void ExecuteResolve_ShouldFail_WhenFireMadeFleshIsNotCompleted()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);

      gameState.FireMadeFleshState.IsCompleted = false;

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolve("Targaryen")
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void ExecuteResolve_ShouldFail_WhenPlayerIdIsNotTargaryenOrHoster()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);
      AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.FireMadeFleshState.IsCompleted = true;

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolve("Lannister")
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void ExecuteResolve_ShouldSucceed_WhenPlayerIdIsTargaryen_AndStateIsCompleted()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);
      AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.FireMadeFleshState.PlayersWantsDragonToken = false;
      gameState.FireMadeFleshState.IsCompleted = true;

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolve("Targaryen")
      );

      // Assert
      Assert.That(result.Success, Is.True);
    }

    [Test]
    public void ExecuteResolve_ShouldSucceed_WhenPlayerIdIsHoster_AndStateIsCompleted()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);
      AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.FireMadeFleshState.PlayersWantsDragonToken = false;
      gameState.FireMadeFleshState.IsCompleted = true;

      GameStateServiceMock.Setup(
        gss => gss.IsHoster(
          It.IsAny<GameState>(),
          It.Is<string>(s => s == "Stark")
        )
      ).Returns(true);

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolve("Stark")
      );

      // Assert
      Assert.That(result.Success, Is.True);
    }

    [Test]
    public void ExecuteResolve_ShouldMoveToWesteros_WhenDoesNotDesirePowerToken()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);
      AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.FireMadeFleshState.PlayersWantsDragonToken = false;
      gameState.FireMadeFleshState.IsCompleted = true;

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolve("Targaryen")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(gameState.CurrentPhase, Is.EqualTo(RoundPhaseType.Westeros));
    }

    [Test]
    public void ExecuteResolve_ShouldCall_TakeDragonToken_WhenDesiresDragonToken()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);
      AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.FireMadeFleshState.PlayersWantsDragonToken = true;
      gameState.FireMadeFleshState.PositionOfDesiredDragonToken = 2;
      gameState.FireMadeFleshState.IsCompleted = true;

      DragonTokensStateServiceMock.Setup(
        dtss => dtss.TakeDragonToken(
          It.IsAny<DragonTokensState>(),
          It.Is<byte>(b => b == 2)
        ))
        .Returns(Result.SUCCESS()
      );

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolve("Targaryen")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      DragonTokensStateServiceMock.Verify(
        dts => dts.TakeDragonToken(gameState.DragonTokensState, 2),
        Times.Once
      );
    }

    [Test]
    public void ExecuteResolve_ShouldNotCall_TakeDragonToken_WhenDoesNotDesireDragonToken()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);
      AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.FireMadeFleshState.PlayersWantsDragonToken = false;
      gameState.FireMadeFleshState.IsCompleted = true;

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolve("Targaryen")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      DragonTokensStateServiceMock.Verify(
        dts => dts.TakeDragonToken(
          It.IsAny<DragonTokensState>(),
          It.IsAny<byte>()
        ),
        Times.Never
      );
    }

    [Test]
    public void ExecuteResolve_ShouldMoveToWesteros_WhenTakeDragonTokensSucceeded()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);
      AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.FireMadeFleshState.PlayersWantsDragonToken = true;
      gameState.FireMadeFleshState.PositionOfDesiredDragonToken = 2;
      gameState.FireMadeFleshState.IsCompleted = true;

      DragonTokensStateServiceMock.Setup(
        dtss => dtss.TakeDragonToken(
          It.IsAny<DragonTokensState>(),
          It.Is<byte>(b => b == 2)
        ))
        .Returns(Result.SUCCESS()
      );

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolve("Targaryen")
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(gameState.CurrentPhase, Is.EqualTo(RoundPhaseType.Westeros));
    }

    [Test]
    public void ExecuteResolve_ShouldNotMoveToWesteros_WhenTakeDragonTokensFailed()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);
      AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.CurrentPhase = RoundPhaseType.FireMadeFlesh;
      gameState.FireMadeFleshState.PlayersWantsDragonToken = true;
      gameState.FireMadeFleshState.PositionOfDesiredDragonToken = 2;
      gameState.FireMadeFleshState.IsCompleted = true;

      DragonTokensStateServiceMock.Setup(
        dtss => dtss.TakeDragonToken(
          It.IsAny<DragonTokensState>(),
          It.Is<byte>(b => b == 2)
        ))
        .Returns(Result.FAILURE("Failed to take dragon token.")
      );

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolve("Targaryen")
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(gameState.CurrentPhase, Is.EqualTo(RoundPhaseType.FireMadeFlesh));
    }

    [Test]
    public void ExecuteResolve_ShouldResetFireMadeFleshState_WhenTakeDragonTokensFailed()
    {
      // Arrange
      GameState gameState = CreateGameStateWithHoster("Stark", HouseType.Stark);
      AddPlayerState(gameState, "Targaryen", HouseType.Targaryen);
      AddPlayerState(gameState, "Lannister", HouseType.Lannister);

      gameState.FireMadeFleshState.PlayersWantsDragonToken = true;
      gameState.FireMadeFleshState.PositionOfDesiredDragonToken = 2;
      gameState.FireMadeFleshState.IsCompleted = true;

      DragonTokensStateServiceMock.Setup(
        dtss => dtss.TakeDragonToken(
          It.IsAny<DragonTokensState>(),
          It.Is<byte>(b => b == 2)
        ))
        .Returns(Result.FAILURE("Failed to take dragon token.")
      );

      // Act
      Result result = RPFireMadeFlesh.Execute(
        gameState,
        new RpcResolve("Targaryen")
      );

      // Assert
      Assert.That(result.Success, Is.False);
      FireMadeFleshStateServiceMock.Verify(
        fmfss => fmfss.Prepare(gameState.FireMadeFleshState),
        Times.Once
      );
    }

    private RpFireMadeFlesh RPFireMadeFlesh { get; set; }
    private Mock<IGameStateService> GameStateServiceMock { get; set; }
    private Mock<IHouseStateService> HouseStateServiceMock { get; set; }
    private Mock<IDragonTokensStateService> DragonTokensStateServiceMock { get; set; }
    private Mock<IFireMadeFleshStateService> FireMadeFleshStateServiceMock { get; set; }
  }
}
