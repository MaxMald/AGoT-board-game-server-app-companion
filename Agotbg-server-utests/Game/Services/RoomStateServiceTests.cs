using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Utilities;
using Moq;

namespace Agotbg.Server.Utests.Game.Services
{
  internal class RoomStateServiceTests
  {
    [SetUp]
    public void Setup()
    {
      InfluenceTrackBiddingStateService = new Mock<IInfluenceTrackBiddingStateService>();
      InfluenceTrackService = new Mock<IInfluenceTrackService>();
      WildlingsStateService = new Mock<IWildlingsStateService>();
      VassalAssignmentStateService = new Mock<IVassalAssignmentStateService>();
      DragonTokensStateService = new Mock<IDragonTokensStateService>();
      FireMadeFleshStateService = new Mock<IFireMadeFleshStateService>();

      RSS = new RoomStateService(
        InfluenceTrackService.Object,
        WildlingsStateService.Object,
        VassalAssignmentStateService.Object,
        InfluenceTrackBiddingStateService.Object,
        DragonTokensStateService.Object,
        FireMadeFleshStateService.Object
      );
    }

    [Test]
    public void AddNewPlayerDescriptor_ShouldFail_WhenRoomStatusIsNotPreparingGame()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];

      // room status is set to InProgress to simulate that the game has already started
      state.RoomStatus = RoomStatus.InProgress;

      // Act
      Result result = RSS.AddNewPlayerDescriptor(
        state,
        "player1",
        "Player One"
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(state.PlayersDescriptors.ContainsKey("player1"), Is.False);
    }

    [Test]
    public void AddNewPlayerDescriptor_ShouldFail_WhenRoomIsFull()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      CreatePlayer(state, "player1", "Player One");
      CreatePlayer(state, "player2", "Player Two");

      // Act
      Result result = RSS.AddNewPlayerDescriptor(
        state,
        "player3",
        "Player Three"
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(state.PlayersDescriptors.ContainsKey("player3"), Is.False);
    }

    [Test]
    public void AddNewPlayerDescriptor_ShouldFaile_WhenPlayerIdAlreadyExists()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      CreatePlayer(state, "player1", "Player One");

      // Act
      Result result = RSS.AddNewPlayerDescriptor(
        state,
        "player1",
        "Another Player One"
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void AddNewPlayerDescriptor_ShouldSucceed_WhenValidScenario()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];

      // Act
      Result result = RSS.AddNewPlayerDescriptor(
        state,
        "player1",
        "Player One"
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.PlayersDescriptors.ContainsKey("player1"), Is.True);
    }

    [Test]
    public void AddNewPlayerDescriptor_ShouldTrimPlayerName_WhenSuccessful()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];

      // Act
      Result result = RSS.AddNewPlayerDescriptor(
        state,
        "player1",
        "   Player One   "
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.PlayersDescriptors.ContainsKey("player1"), Is.True);
      Assert.That(state.PlayersDescriptors["player1"].Name, Is.EqualTo("Player One"));
    }

    [Test]
    public void AddNewPlayerDescriptor_ShouldFail_WhenPlayerNameIsEmpty()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];

      // Act
      Result result = RSS.AddNewPlayerDescriptor(
        state,
        "player1",
        "   "
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(state.PlayersDescriptors.ContainsKey("player1"), Is.False);
    }

    [TestCase('@')]
    [TestCase('#')]
    [TestCase('$')]
    [TestCase('%')]
    [TestCase('^')]
    [TestCase('&')]
    [TestCase('*')]
    [TestCase('(')]
    [TestCase(')')]
    public void AddNewPlayerDescriptor_ShouldFai_WhenPlayerNameHasInvalidCharacters(
      char invalidChar
    )
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];

      // Act
      Result result = RSS.AddNewPlayerDescriptor(
        state,
        "player1",
        $"Player{invalidChar}One"
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(state.PlayersDescriptors.ContainsKey("player1"), Is.False);
    }

    public void AddNewPlayerDescriptor_ShouldFail_WhenPlayerNameExceedsMaxLength()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];

      // Act
      Result result = RSS.AddNewPlayerDescriptor(
        state,
        "player1",
        new string('A', Helpers.MaxPlayerNameLength + 1)
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(state.PlayersDescriptors.ContainsKey("player1"), Is.False);
    }

    [Test]
    public void RemovePlayerDescriptor_ShouldFail_WhenPlayerIdIsHoster()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");

      // Act
      Result result = RSS.RemovePlayerDescriptor(
        state,
        hosterPlayer.PlayerId
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(state.PlayersDescriptors.ContainsKey(hosterPlayer.PlayerId), Is.True);
    }

    [Test]
    public void RemovePlayerDescriptor_ShouldFail_WhenRoomStatusIsNotPreparingGame()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");

      // room status is set to InProgress to simulate that the game has already started
      state.RoomStatus = RoomStatus.InProgress;

      // Act
      Result result = RSS.RemovePlayerDescriptor(
        state,
        playerOne.PlayerId
      );

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(state.PlayersDescriptors.ContainsKey(playerOne.PlayerId), Is.True);
    }

    [Test]
    public void RemovePlayerDescriptor_ShouldFail_WhenPlayerWithIdNotFound()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");

      // Act
      Result result = RSS.RemovePlayerDescriptor(
        state,
        "nonExistentPlayer"
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void RemovePlayerDescriptor_ShouldSucceed_WhenPlayerWithIdFound()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");

      // Act
      Result result = RSS.RemovePlayerDescriptor(
        state,
        playerOne.PlayerId
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.PlayersDescriptors.ContainsKey(playerOne.PlayerId), Is.False);
    }

    [Test]
    public void ModifyPlayerDescriptorHouse_ShouldFail_WhenRoomStatusIsNotPreparingGame()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");

      // room status is set to InProgress to simulate that the game has already started
      state.RoomStatus = RoomStatus.InProgress;

      // Act
      Result result = RSS.ModifyPlayerDescriptorHouse(
        state,
        playerOne.PlayerId,
        HouseType.Greyjoy
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void ModifyPlayerDescriptorHouse_ShouldFail_WhenPlayerWithIdNotFound()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");

      // Act
      Result result = RSS.ModifyPlayerDescriptorHouse(
        state,
        "nonExistentPlayer",
        HouseType.Greyjoy
      );

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void ModifyPlayerDescriptorHouse_ShouldSucceed_WhenPlayerWithIdFound()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");

      // Act
      Result result = RSS.ModifyPlayerDescriptorHouse(
        state,
        playerOne.PlayerId,
        HouseType.Greyjoy
      );

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(playerOne.HouseType, Is.EqualTo(HouseType.Greyjoy));
    }

    [Test]
    public void ModifyMaxNumberOfPlayers_ShouldFail_WhenRoomStatusIsNotPreparingGame()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);

      // room status is set to InProgress to simulate that the game has already started
      state.RoomStatus = RoomStatus.InProgress;

      // Act
      Result result = RSS.ModifyMaxNumberOfPlayers(state, 4);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(state.MaxPlayers, Is.EqualTo(3));
    }

    [Test]
    public void ModifyMaxNumberOfPlayers_ShouldFail_WhenDesiredValueIsLassMinimumDefinedByGameRules()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);

      // Act - The game rules specify a minimum of 3 players to play Game of Thrones the
      // Board Game, specified by the <see cref="GameConstants.MinPlayers"/> constant
      // (which is 3). Therefore, the room cannot be configured with a maximum of players
      // less than 3.
      Result result = RSS.ModifyMaxNumberOfPlayers(state, 2);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(state.MaxPlayers, Is.EqualTo(3));
    }

    [Test]
    public void ModifyMaxNumberOfPlayers_ShouldFail_WhenDesiredValueGreaterThanMaximumDefinedByGameRules()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);

      // Act - The game rules specify a maximum of 8 players to play Game of Thrones the
      // Board Game, specified by the <see cref="GameConstants.MaxPlayers"/> constant
      // (which is 8). Therefore, the room cannot be configured with a maximum of players
      // greater than 8.
      Result result = RSS.ModifyMaxNumberOfPlayers(state, 9);

      // Assert
      Assert.That(result.Success, Is.False);
      Assert.That(state.MaxPlayers, Is.EqualTo(3));
    }

    [Test]
    public void ModifyMaxNumberOfPlayers_ShouldSucceed_WhenDesiredValueIsValid()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);

      // Act
      Result result = RSS.ModifyMaxNumberOfPlayers(state, 5);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.MaxPlayers, Is.EqualTo(5));
    }

    [Test]
    public void CreateGame_ShouldFail_WhenRoomStatusIsNotPreparingGame()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      CreatePlayer(state, "player1", "Player One");
      CreatePlayer(state, "player2", "Player Two");

      // room status is set to InProgress to simulate that the game has already started
      state.RoomStatus = RoomStatus.InProgress;

      // Act
      Result result = RSS.CreateGame(state);

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void CreateGame_ShouldFail_WhenNumberOfPlayersIsGreaterThanMaxPlayers()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      CreatePlayer(state, "player1", "Player One");
      CreatePlayer(state, "player2", "Player Two");

      // Since the hoster is considered a player, the addition of the "player3" exceeds
      // the max players limit
      CreatePlayer(state, "player3", "Player Three"); 

      // Act
      Result result = RSS.CreateGame(state);

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void CreateGame_ShouldFail_WhenNumberOfPlayersIsLessThanMinimumRequiredByGameRules()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(2);
      CreatePlayer(state, "player1", "Player One");

      // The hoster configurated the room with a max of 2 players. Then the hoster and
      // "player1" are present to player. However, the game rules especify a minimum to
      // player Game of Thrones the Board Game, specified by the <see
      // cref="GameConstants.MinPlayers"/> constant (which is 3). Therefore, the game
      // cannot be started with only 2 players.

      // Act
      Result result = RSS.CreateGame(state);

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void CreateGame_ShouldFail__WhenPlayerHasUndefinedHouse()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");
      PlayerDescriptor playerTwo = CreatePlayer(state, "player2", "Player Two");

      hosterPlayer.HouseType = HouseType.Arryn;
      hosterPlayer.HouseType = HouseType.Greyjoy;
      hosterPlayer.HouseType = HouseType.Undefined;

      // Act
      Result result = RSS.CreateGame(state);

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void CreateGame_ShouldFail_WhenPlayerSelectedTargaryen_AndNumberOfPlayersAreLessThanFour()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");
      PlayerDescriptor playerTwo = CreatePlayer(state, "player2", "Player Two");

      hosterPlayer.HouseType = HouseType.Targaryen;
      playerOne.HouseType = HouseType.Greyjoy;
      playerTwo.HouseType = HouseType.Arryn;

      // The Targaryen house is only allowed to be selected when there are 4 or more
      // players in the game. Since the room is configured for a maximum of 3 players,
      // the game cannot be started with Targaryen selected.

      // Act
      Result result = RSS.CreateGame(state);

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void CreateGame_ShouldFail_WhenPlayersHaveDuplicateHouses()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");
      PlayerDescriptor playerTwo = CreatePlayer(state, "player2", "Player Two");

      hosterPlayer.HouseType = HouseType.Arryn;
      playerOne.HouseType = HouseType.Greyjoy;
      playerTwo.HouseType = HouseType.Greyjoy;

      // Act
      Result result = RSS.CreateGame(state);

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void CreateGame_ShouldFail_WhenAlreadyHasInstanceOfGameState()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");
      PlayerDescriptor playerTwo = CreatePlayer(state, "player2", "Player Two");

      hosterPlayer.HouseType = HouseType.Arryn;
      playerOne.HouseType = HouseType.Greyjoy;
      playerTwo.HouseType = HouseType.Lannister;

      // Simulate that the game has already been created by assigning a non-null
      // instance of GameState to the RoomState.
      state.GameState = new GameState();

      // Act
      Result result = RSS.CreateGame(state);

      // Assert
      Assert.That(result.Success, Is.False);
    }

    [Test]
    public void CreateGame_ShouldCreateGameState_WhenSuccessful()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");
      PlayerDescriptor playerTwo = CreatePlayer(state, "player2", "Player Two");

      hosterPlayer.HouseType = HouseType.Arryn;
      playerOne.HouseType = HouseType.Greyjoy;
      playerTwo.HouseType = HouseType.Lannister;

      // Act
      Result result = RSS.CreateGame(state);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.GameState, Is.Not.Null);
    }

    [Test]
    public void CreateGame_ShouldCreatePlayerStatesForPlayers_WhenSuccessful()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");
      PlayerDescriptor playerTwo = CreatePlayer(state, "player2", "Player Two");

      hosterPlayer.HouseType = HouseType.Arryn;
      playerOne.HouseType = HouseType.Greyjoy;
      playerTwo.HouseType = HouseType.Lannister;

      // Act
      Result result = RSS.CreateGame(state);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.GameState, Is.Not.Null);
      Assert.That(state.GameState.Players.Count, Is.EqualTo(3));
      Assert.That(state.GameState.Players, Does.ContainKey(hosterPlayer.PlayerId));
      Assert.That(state.GameState.Players, Does.ContainKey(playerOne.PlayerId));
      Assert.That(state.GameState.Players, Does.ContainKey(playerTwo.PlayerId));
    }

    [Test]
    public void CreateGame_ShouldAssignHousesToPlayerStates_WhenSuccessful()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");
      PlayerDescriptor playerTwo = CreatePlayer(state, "player2", "Player Two");

      hosterPlayer.HouseType = HouseType.Arryn;
      playerOne.HouseType = HouseType.Greyjoy;
      playerTwo.HouseType = HouseType.Lannister;

      // Act
      Result result = RSS.CreateGame(state);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.GameState, Is.Not.Null);
      Assert.That(state.GameState.Players[hosterPlayer.PlayerId].HouseState.Type, Is.EqualTo(HouseType.Arryn));
      Assert.That(state.GameState.Players[playerOne.PlayerId].HouseState.Type, Is.EqualTo(HouseType.Greyjoy));
      Assert.That(state.GameState.Players[playerTwo.PlayerId].HouseState.Type, Is.EqualTo(HouseType.Lannister));
    }

    [Test]
    public void CreateGame_ShouldInitializeGameState_WhenSuccessful()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");
      PlayerDescriptor playerTwo = CreatePlayer(state, "player2", "Player Two");

      hosterPlayer.HouseType = HouseType.Arryn;
      playerOne.HouseType = HouseType.Greyjoy;
      playerTwo.HouseType = HouseType.Lannister;

      // Act
      Result result = RSS.CreateGame(state);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.GameState, Is.Not.Null);
      Assert.That(state.GameState.CurrentRound, Is.EqualTo(GameConstants.StartingRoundNumber));
      Assert.That(state.GameState.CurrentPhase, Is.EqualTo(RoundPhaseType.Setup));
      Assert.That(state.GameState.Winner, Is.Null);
      Assert.That(state.GameState.IsGameOver, Is.False);
      Assert.That(state.GameState.HosterPlayerId, Is.EqualTo(state.HosterPlayerId));
    }

    [Test]
    public void CreateGame_ShouldCallInitializeMethodsOfGameStateServices_WhenSuccessful()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");
      PlayerDescriptor playerTwo = CreatePlayer(state, "player2", "Player Two");

      hosterPlayer.HouseType = HouseType.Arryn;
      playerOne.HouseType = HouseType.Greyjoy;
      playerTwo.HouseType = HouseType.Lannister;

      // Act
      Result result = RSS.CreateGame(state);

      // Assert
      Assert.That(result.Success, Is.True);

      InfluenceTrackService.Verify(s => s.Initialize(It.IsAny<List<HouseState>>()), Times.Once);
      WildlingsStateService.Verify(s => s.Initialize(It.IsAny<WildlingsState>()), Times.Once);
      VassalAssignmentStateService.Verify(s => s.Initialize(It.IsAny<VassalAssignmentState>()), Times.Once);
      InfluenceTrackBiddingStateService.Verify(s => s.Initialize(It.IsAny<InfluenceTrackBiddingState>()), Times.Once);
      DragonTokensStateService.Verify(s => s.Initialize(It.IsAny<DragonTokensState>()), Times.Once);
      FireMadeFleshStateService.Verify(s => s.Initialize(It.IsAny<FireMadeFleshState>()), Times.Once);
    }

    [Test]
    public void CreateGame_ShouldSucceed_WhenPlayerSelectedTargaryenAndNumberOfPlayersAreFourOrMore()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(4);
      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");
      PlayerDescriptor playerTwo = CreatePlayer(state, "player2", "Player Two");
      PlayerDescriptor playerThree = CreatePlayer(state, "player3", "Player Three");

      hosterPlayer.HouseType = HouseType.Targaryen;
      playerOne.HouseType = HouseType.Greyjoy;
      playerTwo.HouseType = HouseType.Arryn;
      playerThree.HouseType = HouseType.Lannister;

      // Act
      Result result = RSS.CreateGame(state);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.GameState, Is.Not.Null);
      Assert.That(state.GameState.Players[hosterPlayer.PlayerId].HouseState.Type, Is.EqualTo(HouseType.Targaryen));
    }

    [Test]
    public void CreateGame_ShouldCreateVassalHouses_WhenSuccessful()
    {
      // Arrange
      RoomState state = CreateRoomWithHosterOnly(3);

      PlayerDescriptor hosterPlayer = state.PlayersDescriptors[state.HosterPlayerId];
      PlayerDescriptor playerOne = CreatePlayer(state, "player1", "Player One");
      PlayerDescriptor playerTwo = CreatePlayer(state, "player2", "Player Two");

      hosterPlayer.HouseType = HouseType.Arryn;
      playerOne.HouseType = HouseType.Greyjoy;
      playerTwo.HouseType = HouseType.Lannister;

      // Act
      Result result = RSS.CreateGame(state);

      // Assert
      Assert.That(result.Success, Is.True);
      Assert.That(state.GameState, Is.Not.Null);

      // The expected vassal houses are Stark, Tyrell, Baratheon, and Martell. These
      // houses are not selected by any player and should be available as vassals in the
      // game state.
      //
      // Game rules says that Targaryen house cannot be a vassal house.
      List<HouseType> expectedVassalHouses = new()
      {
        HouseType.Stark,
        HouseType.Tyrell,
        HouseType.Baratheon,
        HouseType.Martell
      };

      List<HouseType> actualVassalHouses = state.GameState
                                                .Vassals
                                                .Select(v => v.Value.Type).ToList();

      Assert.That(actualVassalHouses, Is.EquivalentTo(expectedVassalHouses));
    }

    private Mock<IInfluenceTrackService> InfluenceTrackService { get; set; }
    private Mock<IWildlingsStateService> WildlingsStateService { get; set; }
    private Mock<IVassalAssignmentStateService> VassalAssignmentStateService { get; set; }
    private Mock<IInfluenceTrackBiddingStateService> InfluenceTrackBiddingStateService { get; set; }
    private Mock<IDragonTokensStateService> DragonTokensStateService { get; set; }
    private Mock<IFireMadeFleshStateService> FireMadeFleshStateService { get; set; }
    private RoomStateService RSS { get; set; }

    private static RoomState CreateRoomWithHosterOnly(byte maxPlayers = 3)
    {
      return new RoomState()
      {
        RoomId = "testRoom",
        HosterPlayerId = "hosterPlayer",
        RoomStatus = RoomStatus.PreparingGame,
        PlayersDescriptors = new Dictionary<string, PlayerDescriptor>()
        {
          {
            "hosterPlayer",
            new PlayerDescriptor()
            {
              PlayerId = "hosterPlayer",
              Name = "Hoster Player"
            }
          }
        },
        MaxPlayers = maxPlayers,
      };
    }

    private static PlayerDescriptor CreatePlayer(
      RoomState room,
      string playerId,
      string playerName
    )
    {
      PlayerDescriptor playerDescriptor = new()
      {
        PlayerId = playerId,
        Name = playerName,
        HouseType = HouseType.Undefined
      };

      room.PlayersDescriptors.Add(playerId, playerDescriptor);
      return playerDescriptor;
    }
  }
}
