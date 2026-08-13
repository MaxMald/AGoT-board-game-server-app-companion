using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services;
using Agotbg.Server.Game.Services.Interfaces;
using Moq;

namespace Agotbg.Server.Utests.Game.Services.VassalAssignmentStateServiceTests
{
  internal class AVassalAssignmetStateServiceTest
  {
    protected Mock<IGameStateService> GameStateService { get; private set; }
    protected VassalAssignmentStateServices VASS { get; private set; }

    [SetUp]
    public void Setup()
    {
      GameStateService = new Mock<IGameStateService>();
      VASS = new VassalAssignmentStateServices(GameStateService.Object);
    }

    protected VassalAssignmentPlayer CreateVassalAssignmentPlayer(
     string id,
     string nexId = "",
     int numOrderTokenSets = 0
    )
    {
      VassalAssignmentPlayer vaPlayer = new()
      {
        PlayerId = id,
        NextPlayerId = nexId
      };

      numOrderTokenSets = Math.Clamp(numOrderTokenSets, 0, (int)VassalOrderTokenSetType.MaximumNumberOfSets);
      while (numOrderTokenSets > 0)
      {
        vaPlayer.PossesedOrderTokenSets.Add(VassalOrderTokenSetType.Star);
        numOrderTokenSets--;
      }

      return vaPlayer;
    }

    protected VassalAssignmentPlayer GetVassalAssignmentPlayer(string id, List<VassalAssignmentPlayer> players)
    {
      VassalAssignmentPlayer? player = players.FirstOrDefault(p => p.PlayerId == id);
      if (player == null)
        throw new ArgumentException("Player with the given ID does not exist in the provided list.", nameof(id));
      return player;
    }

    protected void AssertHasSelectedVassalHouse(
      VassalAssignmentPlayer vaPlayer,
      HouseType house
    )
    {
      foreach (VassalHouseSelectionDescriptor selection in vaPlayer.SelectedVassalHouses)
      {
        if (selection.HouseType == house && selection.VassalOrderTokenSetType != VassalOrderTokenSetType.None)
          return;
      }
      throw new Exception($"Player {vaPlayer.PlayerId} does not have a selected vassal house of type {house}.");
    }
  }
}
