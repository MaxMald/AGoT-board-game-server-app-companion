using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;
using Agotbg.Server.Game.Services.RoundPhaseTransitions;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Utests.Game.Services.RoundPhaseTransitions
{
  internal class RPTActionToEndOfRoundTests
  {
    private GameState m_gameState = new();

    [SetUp]
    public void Setup()
    {
      m_gameState = new GameState
      {
        CurrentRound = 1,
        CurrentPhase = RoundPhaseType.Action
      };

      PlayerState starkPlayer = new()
      {
        PlayerId = "Stark",
        HouseState = HouseStateService.Create(HouseType.Stark)
      };

      PlayerState lannisterPlayer = new()
      {
        PlayerId = "Lannister",
        HouseState = HouseStateService.Create(HouseType.Lannister)
      };

      PlayerState greyjoyPlayer = new()
      {
        PlayerId = "Greyjoy",
        HouseState = HouseStateService.Create(HouseType.Greyjoy)
      };

      m_gameState.Players.Add("Stark", starkPlayer);
      m_gameState.Players.Add("Lannister", lannisterPlayer);
      m_gameState.Players.Add("Greyjoy", greyjoyPlayer);
    }

    [Test]
    public void Execute_ShouldTransitionToEndOfRound()
    {
      var transition = new RPTActionToEndOfRound();

      // Act/Arrange/Assert
      for (int i = 1; i <= GameConstants.NumRounds; i++)
      {
        // Arrange
        m_gameState.CurrentRound = (byte)(i);

        // Act
        Result result = transition.Execute(m_gameState);

        //Assert
        Assert.That(result.Success);
        Assert.That(m_gameState.CurrentPhase, Is.EqualTo(RoundPhaseType.EndOfRound));
      }
    }

    [Test]
    public void Execute_ShouldUpdateRoundNumber_WhenCurrentRoundIsNotLast()
    {
      // Arrange
      var transition = new RPTActionToEndOfRound();

      // Act/Arrange/Assert
      for (int i = 1; i < GameConstants.NumRounds; i++)
      {
        // Arrange
        m_gameState.CurrentRound = (byte)(i);

        // Act
        transition.Execute(m_gameState);

        //Assert
        Assert.That(m_gameState.CurrentRound, Is.EqualTo((byte)(i + 1)));
      }
    }

    [Test]
    public void Execute_ShouldNotEndGame_WhenCurrentRoundIsNotLast()
    {
      // Arrange
      var transition = new RPTActionToEndOfRound();

      // Act/Arrange/Assert
      for (int i = 1; i < GameConstants.NumRounds; i++)
      {
        // Arrange
        m_gameState.CurrentRound = (byte)(i);

        // Act
        transition.Execute(m_gameState);

        //Assert
        Assert.That(m_gameState.IsGameOver, Is.False);
      }
    }

    [Test]
    public void Execute_ShouldEndGame_WhenCurrentRoundIsLast()
    {
      // Arrange
      var transition = new RPTActionToEndOfRound();
      m_gameState.CurrentRound = GameConstants.NumRounds;

      // Act
      transition.Execute(m_gameState);

      //Assert
      Assert.That(m_gameState.IsGameOver, Is.True);
    }

    [Test]
    public void Execute_ShouldNotSelectWinner_WhenCurrentRoundIsNotLast()
    {
      // Arrange
      var transition = new RPTActionToEndOfRound();

      // Act/Arrange/Assert
      for (int i = 1; i < GameConstants.NumRounds; i++)
      {
        // Arrange
        m_gameState.CurrentRound = (byte)(i);

        // Act
        transition.Execute(m_gameState);

        //Assert
        Assert.That(m_gameState.Winner, Is.Null);
      }
    }

    [Test]
    public void Execute_ShouldSelectWinner_WhenCurrentRoundIsLast()
    {
      // Arrange
      var transition = new RPTActionToEndOfRound();
      m_gameState.CurrentRound = GameConstants.NumRounds;

      // Act
      transition.Execute(m_gameState);

      //Assert
      Assert.That(m_gameState.Winner, Is.Not.Null);
    }

    [Test]
    public void Execute_ShouldSelectWinnerWithHighestVictoryPoints_WhenCurrentRoundIsLast()
    {
      // Arrange
      var transition = new RPTActionToEndOfRound();
      m_gameState.CurrentRound = GameConstants.NumRounds;

      // Set victory points for players
      m_gameState.Players["Stark"].HouseState.VictoryPoints = 5;
      m_gameState.Players["Lannister"].HouseState.VictoryPoints = 7;
      m_gameState.Players["Greyjoy"].HouseState.VictoryPoints = 6;

      // Act
      transition.Execute(m_gameState);

      //Assert
      Assert.That(m_gameState.Winner, Is.EqualTo(HouseType.Lannister));
    }

    [Test]
    public void Execute_ShouldSelectWinnerWithLowestIronThronePosition_WhenTiesAndCurrentRoundIsLast()
    {
      // Arrange
      var transition = new RPTActionToEndOfRound();
      m_gameState.CurrentRound = GameConstants.NumRounds;

      // Set Iron Throne positions for players (lowest is better)
      m_gameState.Players["Stark"].HouseState.IronThroneTrackPosition = 3;
      m_gameState.Players["Lannister"].HouseState.IronThroneTrackPosition = 2;
      m_gameState.Players["Greyjoy"].HouseState.IronThroneTrackPosition = 1;

      // Set victory points for players (tie between all players)
      m_gameState.Players["Stark"].HouseState.VictoryPoints = 6;
      m_gameState.Players["Lannister"].HouseState.VictoryPoints = 6;
      m_gameState.Players["Greyjoy"].HouseState.VictoryPoints = 5;

      // Act
      transition.Execute(m_gameState);

      // Assert
      // Lanisters have the lowest Iron Throne position among the tied players, so
      // they should be selected as the winner.
      Assert.That(m_gameState.Winner, Is.EqualTo(HouseType.Lannister));
    }

    [Test]
    public void Execute_ShouldNotSelectTargaryenAsWinner_WhenCurrentRoundIsLast()
    {
      // Arrange
      var transition = new RPTActionToEndOfRound();
      m_gameState.CurrentRound = GameConstants.NumRounds;

      // Add Targaryen player with highest victory points
      PlayerState targaryenPlayer = new()
      {
        PlayerId = "Targaryen",
        HouseState = HouseStateService.Create(HouseType.Targaryen)
      };
      m_gameState.Players.Add("Targaryen", targaryenPlayer);

      // Set victory points for players
      m_gameState.Players["Stark"].HouseState.VictoryPoints = 5;
      m_gameState.Players["Lannister"].HouseState.VictoryPoints = 7;
      m_gameState.Players["Greyjoy"].HouseState.VictoryPoints = 6;
      m_gameState.Players["Targaryen"].HouseState.VictoryPoints = 10; // Targaryen has the highest victory points

      // Act
      transition.Execute(m_gameState);

      // Assert that Targaryen should not be selected as the winner, even though they
      // have the highest victory points. Lannister should be selected as the winner
      // instead.
      Assert.That(m_gameState.Winner, Is.EqualTo(HouseType.Lannister));
    }

    [TestCase(2, 1)]  // Round 2 → Dragon strength 1
    [TestCase(4, 2)]  // Round 4 → Dragon strength 2
    [TestCase(6, 3)]  // Round 6 → Dragon strength 3
    [TestCase(8, 4)]  // Round 8 → Dragon strength 4
    [TestCase(10, 5)] // Round 10 → Dragon strength 5
    public void Execute_ShouldUpdateTargaryenDragonStrength_OnEvenRounds(byte round, byte expectedStrength)
    {
      // Arrange
      var state = CreateGameStateWith3Westerosi1Targaryen();
      state.CurrentRound = (byte)(round - 1);

      var transition = new RPTActionToEndOfRound();

      // Act
      transition.Execute(state);

      // Assert
      Assert.That(state.Players["player1"].HouseState.DragonStrength, Is.EqualTo(expectedStrength));
    }

    [TestCase(1)]
    [TestCase(3)]
    [TestCase(5)]
    [TestCase(7)]
    [TestCase(9)]
    public void Execute_ShouldNotUpdateTargaryenDragonStrength_OnOddRounds(byte round)
    {
      // Arrange
      var state = CreateGameStateWith3Westerosi1Targaryen();
      state.Players["Targaryen"].HouseState.DragonStrength = 2; // Set initial dragon strength
      state.CurrentRound = (byte)(round - 1);

      var transition = new RPTActionToEndOfRound();

      // Act
      transition.Execute(state);

      // Assert
      Assert.That(state.Players["Targaryen"].HouseState.DragonStrength, Is.EqualTo(2), "Dragon strength should not change on odd rounds");
    }

    private GameState CreateGameStateWith3Westerosi1Targaryen()
    {
      var state = new GameState
      {
        CurrentRound = 1,
        CurrentPhase = RoundPhaseType.Action
      };

      PlayerState starkPlayer = new()
      {
        PlayerId = "Stark",
        HouseState = HouseStateService.Create(HouseType.Stark)
      };

      PlayerState lannisterPlayer = new()
      {
        PlayerId = "Lannister",
        HouseState = HouseStateService.Create(HouseType.Lannister)
      };

      PlayerState greyjoyPlayer = new()
      {
        PlayerId = "Greyjoy",
        HouseState = HouseStateService.Create(HouseType.Greyjoy)
      };

      PlayerState targaryenPlayer = new()
      {
        PlayerId = "Targaryen",
        HouseState = HouseStateService.Create(HouseType.Targaryen)
      };

      state.Players.Add("Stark", starkPlayer);
      state.Players.Add("Lannister", lannisterPlayer);
      state.Players.Add("Greyjoy", greyjoyPlayer);
      state.Players.Add("Targaryen", targaryenPlayer);
      return state;
    }
  }
}
