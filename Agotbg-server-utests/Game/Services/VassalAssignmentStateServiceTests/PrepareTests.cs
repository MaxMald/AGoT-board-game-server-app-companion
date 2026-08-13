
using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;

namespace Agotbg.Server.Utests.Game.Services.VassalAssignmentStateServiceTests
{
  internal class PrepareTests : AVassalAssignmetStateServiceTest
  {
    [Test]
    public void Prepare_ShouldStartWithPlayerWithHighestIronTrackPosition()
    {
      // Arrange
      GameState gameState = new()
      {
        Players = new Dictionary<string, PlayerState>
        {
          { "Stark", CreatePlayerState("Stark", HouseType.Stark) },
          { "Lannister", CreatePlayerState("Lannister", HouseType.Lannister) },
          { "Baratheon", CreatePlayerState("Baratheon", HouseType.Baratheon) }
        },

        Vassals = new Dictionary<HouseType, HouseState>
        {
          { HouseType.Greyjoy, HouseStateFactory.Create(HouseType.Greyjoy) },
          { HouseType.Arryn, HouseStateFactory.Create(HouseType.Arryn) },
          { HouseType.Tyrell, HouseStateFactory.Create(HouseType.Tyrell) }
        }
      };

      GameStateService
        .Setup(x => x.GetPlayersInTurnOrder(gameState))
        .Returns(new List<PlayerState>
        {
          gameState.Players["Lannister"],
          gameState.Players["Stark"],
          gameState.Players["Baratheon"]
        });

      // Act
      VASS.Prepare(gameState);

      // Assert
      Assert.That(gameState.VassalAssignmentState.IsCompleted, Is.False);
      Assert.That(gameState.VassalAssignmentState.CurrentPlayerID, Is.EqualTo("Lannister"));
    }

    [Test]
    public void Prepare_ShouldLinkVassalAssignmentPlayersInTurnOrder()
    {
      // Arrange
      GameState gameState = new()
      {
        Players = new Dictionary<string, PlayerState>
        {
          { "Stark", CreatePlayerState("Stark", HouseType.Stark) },
          { "Lannister", CreatePlayerState("Lannister", HouseType.Lannister) },
          { "Baratheon", CreatePlayerState("Baratheon", HouseType.Baratheon) }
        },

        Vassals = new Dictionary<HouseType, HouseState>
        {
          { HouseType.Greyjoy, HouseStateFactory.Create(HouseType.Greyjoy) },
          { HouseType.Arryn, HouseStateFactory.Create(HouseType.Arryn) },
          { HouseType.Tyrell, HouseStateFactory.Create(HouseType.Tyrell) }
        }
      };

      GameStateService
        .Setup(x => x.GetPlayersInTurnOrder(gameState))
        .Returns(new List<PlayerState>
        {
          gameState.Players["Lannister"],
          gameState.Players["Stark"],
          gameState.Players["Baratheon"]
        });

      // Act
      VASS.Prepare(gameState);

      // Assert
      VassalAssignmentState vassalAssignmentState = gameState.VassalAssignmentState;
      Assert.That(vassalAssignmentState.IsCompleted, Is.False);

      VassalAssignmentPlayer lannisterVAP = GetVassalAssignmentPlayer("Lannister", vassalAssignmentState.Players);
      VassalAssignmentPlayer starkVAP = GetVassalAssignmentPlayer("Stark", vassalAssignmentState.Players);
      VassalAssignmentPlayer baratheonVAP = GetVassalAssignmentPlayer("Baratheon", vassalAssignmentState.Players);

      Assert.That(lannisterVAP.NextPlayerId, Is.EqualTo("Stark"));
      Assert.That(starkVAP.NextPlayerId, Is.EqualTo("Baratheon"));
      Assert.That(baratheonVAP.NextPlayerId, Is.Empty);
    }

    [Test]
    public void Prepare_ShouldSetOrderTokenSetsToFirstPlayers_WhenAvailableVassalHousesAreLessNumOfPlayers()
    {
      // Arrange
      GameState gameState = new()
      {
        Players = new Dictionary<string, PlayerState>
        {
          { "Stark", CreatePlayerState("Stark", HouseType.Stark) },
          { "Lannister", CreatePlayerState("Lannister", HouseType.Lannister) },
          { "Baratheon", CreatePlayerState("Baratheon", HouseType.Baratheon) }
        },

        Vassals = new Dictionary<HouseType, HouseState>
        {
          { HouseType.Greyjoy, HouseStateFactory.Create(HouseType.Greyjoy) },
          { HouseType.Arryn, HouseStateFactory.Create(HouseType.Arryn) }
        }
      };

      GameStateService
        .Setup(x => x.GetPlayersInTurnOrder(gameState))
        .Returns(new List<PlayerState>
        {
          gameState.Players["Stark"],
          gameState.Players["Lannister"],
          gameState.Players["Baratheon"]
        });

      // Act
      VASS.Prepare(gameState);

      // Assert
      VassalAssignmentState vassalAssignmentState = gameState.VassalAssignmentState;
      Assert.That(vassalAssignmentState.IsCompleted, Is.False);

      VassalAssignmentPlayer lannisterVAP = GetVassalAssignmentPlayer("Lannister", vassalAssignmentState.Players);
      VassalAssignmentPlayer starkVAP = GetVassalAssignmentPlayer("Stark", vassalAssignmentState.Players);
      VassalAssignmentPlayer baratheonVAP = GetVassalAssignmentPlayer("Baratheon", vassalAssignmentState.Players);

      // Lannister and Stark should have 1 "order token set" each since there are only 2
      // available vassal houses
      Assert.That(lannisterVAP.PossesedOrderTokenSets.Count, Is.EqualTo(1));
      Assert.That(lannisterVAP.PossesedOrderTokenSets[0], Is.Not.EqualTo(VassalOrderTokenSetType.None));
      Assert.That(starkVAP.PossesedOrderTokenSets.Count, Is.EqualTo(1));
      Assert.That(starkVAP.PossesedOrderTokenSets[0], Is.Not.EqualTo(VassalOrderTokenSetType.None));

      // Baratheon should not have any "order tokens set" since there are only 2
      // available vassal houses
      Assert.That(baratheonVAP.PossesedOrderTokenSets.Count, Is.EqualTo(0));
    }

    [Test]
    public void Prepare_ShouldSetOrderTokenSetsToFirstPlayers_WhenAvailableVassalHousesAreGreaterThanMaxOrderTokenSets()
    {
      // Notes:
      //
      // Although this scenario is not possible in the actual game, it is still a valid
      // test case for the sake of testing the Prepare method.

      // Arrange
      GameState gameState = new()
      {
        Players = new Dictionary<string, PlayerState>
        {
          { "Stark", CreatePlayerState("Stark", HouseType.Stark) },
          { "Lannister", CreatePlayerState("Lannister", HouseType.Lannister) },
          { "Baratheon", CreatePlayerState("Baratheon", HouseType.Baratheon) },
          { "Targaryen", CreatePlayerState("Targaryen", HouseType.Targaryen) },
          { "Arryn", CreatePlayerState("Arryn", HouseType.Arryn) }
        },

        Vassals = new Dictionary<HouseType, HouseState>
        {
          { HouseType.Greyjoy, HouseStateFactory.Create(HouseType.Greyjoy) },
          { HouseType.Arryn, HouseStateFactory.Create(HouseType.Arryn) },
          { HouseType.Tyrell, HouseStateFactory.Create(HouseType.Tyrell) },
          { HouseType.Martell, HouseStateFactory.Create(HouseType.Martell) },
          { HouseType.Baratheon, HouseStateFactory.Create(HouseType.Baratheon) }
        }
      };

      GameStateService
        .Setup(x => x.GetPlayersInTurnOrder(gameState))
        .Returns(new List<PlayerState>
        {
          gameState.Players["Stark"],
          gameState.Players["Lannister"],
          gameState.Players["Baratheon"],
          gameState.Players["Arryn"],
          gameState.Players["Targaryen"]
        });

      // Act
      VASS.Prepare(gameState);

      // Assert
      VassalAssignmentState vassalAssignmentState = gameState.VassalAssignmentState;
      Assert.That(vassalAssignmentState.IsCompleted, Is.False);

      VassalAssignmentPlayer lannisterVAP = GetVassalAssignmentPlayer("Lannister", vassalAssignmentState.Players);
      VassalAssignmentPlayer starkVAP = GetVassalAssignmentPlayer("Stark", vassalAssignmentState.Players);
      VassalAssignmentPlayer baratheonVAP = GetVassalAssignmentPlayer("Baratheon", vassalAssignmentState.Players);
      VassalAssignmentPlayer targaryenVAP = GetVassalAssignmentPlayer("Targaryen", vassalAssignmentState.Players);
      VassalAssignmentPlayer arrynVAP = GetVassalAssignmentPlayer("Arryn", vassalAssignmentState.Players);

      Assert.That(lannisterVAP.PossesedOrderTokenSets.Count, Is.EqualTo(1));
      Assert.That(lannisterVAP.PossesedOrderTokenSets[0], Is.Not.EqualTo(VassalOrderTokenSetType.None));
      Assert.That(starkVAP.PossesedOrderTokenSets.Count, Is.EqualTo(1));
      Assert.That(starkVAP.PossesedOrderTokenSets[0], Is.Not.EqualTo(VassalOrderTokenSetType.None));
      Assert.That(baratheonVAP.PossesedOrderTokenSets.Count, Is.EqualTo(1));
      Assert.That(baratheonVAP.PossesedOrderTokenSets[0], Is.Not.EqualTo(VassalOrderTokenSetType.None));
      Assert.That(arrynVAP.PossesedOrderTokenSets.Count, Is.EqualTo(1));
      Assert.That(arrynVAP.PossesedOrderTokenSets[0], Is.Not.EqualTo(VassalOrderTokenSetType.None));
      Assert.That(targaryenVAP.PossesedOrderTokenSets.Count, Is.EqualTo(0));
    }

    [Test]
    public void Prepare_ShouldSetNumOrderTokens_WhenAvailableVassalHousesAreGreaterThanMaxOrderTokenSets()
    {
      // Arrange
      GameState gameState = new()
      {
        Players = new Dictionary<string, PlayerState>
        {
          { "Stark", CreatePlayerState("Stark", HouseType.Stark) },
          { "Lannister", CreatePlayerState("Lannister", HouseType.Lannister) },
          { "Targaryen", CreatePlayerState("Targaryen", HouseType.Targaryen) }
        },
        Vassals = new Dictionary<HouseType, HouseState>
        {
          { HouseType.Greyjoy, HouseStateFactory.Create(HouseType.Greyjoy) },
          { HouseType.Arryn, HouseStateFactory.Create(HouseType.Arryn) },
          { HouseType.Tyrell, HouseStateFactory.Create(HouseType.Tyrell) },
          { HouseType.Martell, HouseStateFactory.Create(HouseType.Martell) },
          { HouseType.Baratheon, HouseStateFactory.Create(HouseType.Baratheon) }
        }
      };

      GameStateService
        .Setup(x => x.GetPlayersInTurnOrder(gameState))
        .Returns(new List<PlayerState>
        {
          gameState.Players["Stark"],
          gameState.Players["Targaryen"],
          gameState.Players["Lannister"]
        });

      // Act
      VASS.Prepare(gameState);

      // Assert
      VassalAssignmentState vassalAssignmentState = gameState.VassalAssignmentState;
      VassalAssignmentPlayer lannisterVAP = GetVassalAssignmentPlayer("Lannister", vassalAssignmentState.Players);
      VassalAssignmentPlayer starkVAP = GetVassalAssignmentPlayer("Stark", vassalAssignmentState.Players);
      VassalAssignmentPlayer targaryenVAP = GetVassalAssignmentPlayer("Targaryen", vassalAssignmentState.Players);

      Assert.That(vassalAssignmentState.IsCompleted, Is.False);
      Assert.That(lannisterVAP.PossesedOrderTokenSets.Count, Is.EqualTo(1));
      Assert.That(lannisterVAP.PossesedOrderTokenSets[0], Is.Not.EqualTo(VassalOrderTokenSetType.None));
      Assert.That(starkVAP.PossesedOrderTokenSets.Count, Is.EqualTo(1));
      Assert.That(starkVAP.PossesedOrderTokenSets[0], Is.Not.EqualTo(VassalOrderTokenSetType.None));
      Assert.That(targaryenVAP.PossesedOrderTokenSets.Count, Is.EqualTo(1));
      Assert.That(targaryenVAP.PossesedOrderTokenSets[0], Is.Not.EqualTo(VassalOrderTokenSetType.None));
    }

    [Test]
    public void Prepare_ShouldSetAvailableVassalHouses_WhenVassalsAreAvailable()
    {
      // Arrange
      GameState gameState = new()
      {
        Players = new Dictionary<string, PlayerState>
        {
          { "Stark", CreatePlayerState("Stark", HouseType.Stark) },
          { "Lannister", CreatePlayerState("Lannister", HouseType.Lannister) }
        },
        Vassals = new Dictionary<HouseType, HouseState>
        {
          { HouseType.Greyjoy, HouseStateFactory.Create(HouseType.Greyjoy) },
          { HouseType.Arryn, HouseStateFactory.Create(HouseType.Arryn) }
        }
      };

      GameStateService
        .Setup(x => x.GetPlayersInTurnOrder(gameState))
        .Returns(new List<PlayerState>
        {
          gameState.Players["Stark"],
          gameState.Players["Lannister"]
        });

      // Act
      VASS.Prepare(gameState);

      // Assert
      VassalAssignmentState vassalAssignmentState = gameState.VassalAssignmentState;
      Assert.That(vassalAssignmentState.IsCompleted, Is.False);
      Assert.That(vassalAssignmentState.AvailableVassalHouses.Count, Is.EqualTo(2));
      Assert.That(vassalAssignmentState.AvailableVassalHouses, Does.Contain(HouseType.Greyjoy));
      Assert.That(vassalAssignmentState.AvailableVassalHouses, Does.Contain(HouseType.Arryn));
    }

    [Test]
    public void Prepare_ShouldSetAvailableVassalHouses_WhenNumberOfVassalsHousesAreGreaterThanMaxOrderTokenSets()
    {
      // Arrange
      GameState gameState = new()
      {
        Players = new Dictionary<string, PlayerState>
        {
          { "Stark", CreatePlayerState("Stark", HouseType.Stark) },
          { "Lannister", CreatePlayerState("Lannister", HouseType.Lannister) },
          { "Targaryen", CreatePlayerState("Targaryen", HouseType.Targaryen) }
        },
        Vassals = new Dictionary<HouseType, HouseState>
        {
          { HouseType.Greyjoy, HouseStateFactory.Create(HouseType.Greyjoy) },
          { HouseType.Arryn, HouseStateFactory.Create(HouseType.Arryn) },
          { HouseType.Tyrell, HouseStateFactory.Create(HouseType.Tyrell) },
          { HouseType.Martell, HouseStateFactory.Create(HouseType.Martell) },
          { HouseType.Baratheon, HouseStateFactory.Create(HouseType.Baratheon) }
        }
      };

      GameStateService
        .Setup(x => x.GetPlayersInTurnOrder(gameState))
        .Returns(new List<PlayerState>
        {
          gameState.Players["Stark"],
          gameState.Players["Targaryen"],
          gameState.Players["Lannister"]
        });

      // Act
      VASS.Prepare(gameState);

      // Assert
      VassalAssignmentState vassalAssignmentState = gameState.VassalAssignmentState;
      Assert.That(vassalAssignmentState.IsCompleted, Is.False);
      Assert.That(vassalAssignmentState.AvailableVassalHouses.Count, Is.EqualTo(5));
      Assert.That(vassalAssignmentState.AvailableVassalHouses, Does.Contain(HouseType.Greyjoy));
      Assert.That(vassalAssignmentState.AvailableVassalHouses, Does.Contain(HouseType.Arryn));
      Assert.That(vassalAssignmentState.AvailableVassalHouses, Does.Contain(HouseType.Tyrell));
      Assert.That(vassalAssignmentState.AvailableVassalHouses, Does.Contain(HouseType.Martell));
      Assert.That(vassalAssignmentState.AvailableVassalHouses, Does.Contain(HouseType.Baratheon));
    }

    [Test]
    public void Prepare_ShouldSetStateAsCompleted_WhenNoVassalHousesAreAvailable()
    {
      // Arrange
      GameState gameState = new()
      {
        Players = new Dictionary<string, PlayerState>
        {
          { "Stark", CreatePlayerState("Stark", HouseType.Stark) },
          { "Lannister", CreatePlayerState("Lannister", HouseType.Lannister) }
        },
        Vassals = new Dictionary<HouseType, HouseState>()
      };

      GameStateService
        .Setup(x => x.GetPlayersInTurnOrder(gameState))
        .Returns(new List<PlayerState>
        {
          gameState.Players["Stark"],
          gameState.Players["Lannister"]
        });

      // Act
      VASS.Prepare(gameState);

      // Assert
      VassalAssignmentState vassalAssignmentState = gameState.VassalAssignmentState;
      Assert.That(vassalAssignmentState.IsCompleted, Is.True);
      Assert.That(vassalAssignmentState.Players, Is.Empty);
      Assert.That(vassalAssignmentState.AvailableVassalHouses, Is.Empty);
    }

    [Test]
    public void Prepare_ShouldSetStateAsCompleted_WhenNoPlayersAreAvailable()
    {
      // Arrange
      GameState gameState = new()
      {
        Players = new Dictionary<string, PlayerState>(),
        Vassals = new Dictionary<HouseType, HouseState>
        {
          { HouseType.Greyjoy, HouseStateFactory.Create(HouseType.Greyjoy) },
          { HouseType.Arryn, HouseStateFactory.Create(HouseType.Arryn) }
        }
      };

      GameStateService
        .Setup(x => x.GetPlayersInTurnOrder(gameState))
        .Returns(new List<PlayerState>());

      // Act
      VASS.Prepare(gameState);

      // Assert
      VassalAssignmentState vassalAssignmentState = gameState.VassalAssignmentState;
      Assert.That(vassalAssignmentState.IsCompleted, Is.True);
      Assert.That(vassalAssignmentState.Players, Is.Empty);
      Assert.That(vassalAssignmentState.AvailableVassalHouses, Is.Empty);
    }

    private PlayerState CreatePlayerState(string id, HouseType houseType)
    {
      return new PlayerState()
      {
        PlayerId = id,
        HouseState = HouseStateFactory.Create(houseType)
      };
    }
  }
}
