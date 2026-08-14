
using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;

namespace Agotbg.Server.Utests.Game.Services.RoundPhase
{
  internal abstract class ARoundPhaseTest
  {
    protected static GameState CreateGameStateWithHoster(
      string hosterPlayerId,
      HouseType hosterHouseType
    )
    {
      GameState gameState = new()
      {
        HosterPlayerId = hosterPlayerId,
        Winner = null,
        IsGameOver = false
      };

      gameState.Players.Add(
        hosterPlayerId,
        CreatePlayerState(hosterPlayerId, hosterHouseType)
      );

      return gameState;
    }

    protected static PlayerState AddPlayerState(
      GameState gamestate,
      string playerId,
      HouseType houseType
    )
    {
      PlayerState playerState = CreatePlayerState(playerId, houseType);
      gamestate.Players.Add(playerId, playerState);
      return playerState;
    }

    protected static HouseState AddVassal(
      GameState gameState,
      HouseType vassal
    )
    {
      HouseState houseState = CreateVassal(vassal);
      gameState.Vassals.Add(vassal, houseState);
      return houseState;
    }

    protected static PlayerState CreatePlayerState(string playerId, HouseType houseType)
    {
      PlayerState playerState = new()
      {
        PlayerId = playerId,
        HouseState = HouseStateFactory.Create(houseType)
      };
      return playerState;
    }

    protected static HouseState CreateVassal(HouseType houseType)
    {
      return HouseStateFactory.CreateVassal(houseType);
    }
  }
}
