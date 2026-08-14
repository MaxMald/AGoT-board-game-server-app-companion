using Agotbg.Server.Game.Model;
using Agotbg.Server.Game.Services.Interfaces;
using Agotbg.Server.Utilities;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Implements the <see cref="IRoomStateService"/> interface to manage the state of a
  /// room in the game.
  /// </summary>
  public class RoomStateService : IRoomStateService
  {
    /// <summary>
    /// Instantiates a new <see cref="RoomStateService"/> object with its required
    /// dependencies.
    /// </summary>
    ///
    /// <param name="influenceTrackService">The service responsible for managing the
    /// influence track state.</param>
    /// <param name="wildlingsStateService">The service responsible for managing the
    /// wildlings state.</param>
    /// <param name="vassalAssignmentStateService">The service responsible for managing
    /// the vassal assignment state.</param>
    /// <param name="influenceTrackBiddingStateService">The service responsible for
    /// managing the influence track bidding state.</param>
    /// <param name="dragonTokensStateService">The service responsible for managing the
    /// dragon tokens state.</param>
    /// <param name="fireMadeFleshStateService">The service responsible for managing the
    /// Fire Made Flesh state.</param>
    public RoomStateService(
      IInfluenceTrackService influenceTrackService,
      IWildlingsStateService wildlingsStateService,
      IVassalAssignmentStateService vassalAssignmentStateService,
      IInfluenceTrackBiddingStateService influenceTrackBiddingStateService,
      IDragonTokensStateService dragonTokensStateService,
      IFireMadeFleshStateService fireMadeFleshStateService)
    {
      InfluenceTrackService = influenceTrackService;
      WildlingsStateService = wildlingsStateService;
      VassalAssignmentStateService = vassalAssignmentStateService;
      InfluenceTrackBiddingStateService = influenceTrackBiddingStateService;
      DragonTokensStateService = dragonTokensStateService;
      FireMadeFleshStateService = fireMadeFleshStateService;
    }

    /// <inheritdoc/>
    public Result AddNewPlayerDescriptor(
      RoomState room,
      string playerId,
      string playerName
    )
    {
      if (room.RoomStatus != RoomStatus.PreparingGame)
        return Result.FAILURE("Cannot add new players after the game has started.");

      if (room.PlayersDescriptors.Count >= room.MaxPlayers)
        return Result.FAILURE($"Cannot add new player. Maximum number of players ({room.MaxPlayers}) reached.");

      if (room.PlayersDescriptors.ContainsKey(playerId))
        return Result.FAILURE($"Player ID '{playerId}' is already in use.");

      playerName = playerName.Trim();
      Result result = Helpers.IsValidPlayerName(playerName);
      if (!result.Success)
        return result;

      PlayerDescriptor playerDescriptor = new()
      {
        PlayerId = playerId,
        Name = playerName,
        HouseType = HouseType.Undefined
      };

      room.PlayersDescriptors.Add(playerId, playerDescriptor);
      return Result.SUCCESS();
    }

    /// <inheritdoc/>
    public Result RemovePlayerDescriptor(RoomState room, string playerId)
    {
      if (room.RoomStatus != RoomStatus.PreparingGame)
        return Result.FAILURE("Cannot remove players after the game has started.");

      if (room.HosterPlayerId == playerId)
        return Result.FAILURE("Cannot remove the hoster player from the room.");

      if (!room.PlayersDescriptors.ContainsKey(playerId))
        return Result.FAILURE($"Player ID '{playerId}' does not exist.");

      room.PlayersDescriptors.Remove(playerId);

      return Result.SUCCESS();
    }

    /// <inheritdoc/>
    public Result ModifyPlayerDescriptorHouse(
      RoomState room,
      string playerId,
      HouseType newHouse
    )
    {
      if (room.RoomStatus != RoomStatus.PreparingGame)
        return Result.FAILURE("Cannot modify player house after the game has started.");

      if (!room.PlayersDescriptors.ContainsKey(playerId))
        return Result.FAILURE($"Player ID '{playerId}' does not exist.");

      PlayerDescriptor playerDescriptor = room.PlayersDescriptors[playerId];
      playerDescriptor.HouseType = newHouse;

      return Result.SUCCESS();
    }

    /// <inheritdoc/>
    public Result ModifyMaxNumberOfPlayers(RoomState room, byte newMaxPlayers)
    {
      if (room.RoomStatus != RoomStatus.PreparingGame)
        return Result.FAILURE("Cannot modify max number of players after the game has started.");

      if (newMaxPlayers < GameConstants.MinPlayers || newMaxPlayers > GameConstants.MaxPlayers)
        return Result.FAILURE($"Max number of players must be between {GameConstants.MinPlayers} and {GameConstants.MaxPlayers}.");

      room.MaxPlayers = newMaxPlayers;
      return Result.SUCCESS();
    }

    /// <inheritdoc/>
    public Result CreateGame(RoomState roomState)
    {
      if (roomState.RoomStatus != RoomStatus.PreparingGame)
        return Result.FAILURE($"Cannot initialize game for room '{roomState.RoomId}' because the room is not in PreparingGame status.");

      if (roomState.GameState != null)
        return Result.FAILURE($"Cannot initialize game for room '{roomState.RoomId}' because the game state is already initialized.");

      List<PlayerDescriptor> playersDescriptors = roomState
        .PlayersDescriptors
        .Values
        .ToList();

      int numPlayers = playersDescriptors.Count;
      if (numPlayers > roomState.MaxPlayers)
        return Result.FAILURE($"Too many players to start the game. Current Number of players: {numPlayers}. Maximum allowed is {roomState.MaxPlayers}.");

      if (numPlayers < GameConstants.MinPlayers)
        return Result.FAILURE($"Not enough players to start the game. Minimum required is {GameConstants.MinPlayers}.");

      List<HouseType> selectedHouses = [];
      foreach (PlayerDescriptor playerDescriptor in playersDescriptors)
      {
        if (playerDescriptor.HouseType == HouseType.Undefined)
          return Result.FAILURE($"Player '{playerDescriptor.Name}' has not selected a house.");

        if (playerDescriptor.HouseType == HouseType.Targaryen && numPlayers < 4)
          return Result.FAILURE("Targaryen house can only be selected if there are at least 4 players.");

        if (selectedHouses.Contains(playerDescriptor.HouseType))
          return Result.FAILURE($"House '{playerDescriptor.HouseType}' has been selected by multiple players.");

        selectedHouses.Add(playerDescriptor.HouseType);
      }

      GameState gameState = new();

      try
      {
        // Create player and vassal houses based on the selected houses and the remaining
        // houses
        CreatePlayerHouses(gameState, playersDescriptors);
        CreateVassalHouses(gameState);

        // Initialize the game state sub-states using their services
        List<HouseState> allHouses = GetAllHouseStates(gameState);
        InfluenceTrackService.Initialize(allHouses);
        WildlingsStateService.Initialize(gameState.Wildlings);
        VassalAssignmentStateService.Initialize(gameState.VassalAssignmentState);
        InfluenceTrackBiddingStateService.Initialize(gameState.InfluenceTrackBiddingState);
        DragonTokensStateService.Initialize(gameState.DragonTokensState);
        FireMadeFleshStateService.Initialize(gameState.FireMadeFleshState);
      }
      catch (Exception e)
      {
        return Result.FAILURE($"Failed to initialize game state for room '{roomState.RoomId}': {e.Message}");
      }

      gameState.CurrentPhase = RoundPhaseType.Setup;
      gameState.CurrentRound = GameConstants.StartingRoundNumber;
      gameState.HosterPlayerId = roomState.HosterPlayerId;
      roomState.GameState = gameState;
      return Result.SUCCESS();
    }

    private IInfluenceTrackService InfluenceTrackService { get; }
    private IVassalAssignmentStateService VassalAssignmentStateService { get; }
    private IWildlingsStateService WildlingsStateService { get; }
    private IInfluenceTrackBiddingStateService InfluenceTrackBiddingStateService { get; }
    private IDragonTokensStateService DragonTokensStateService { get; }
    private IFireMadeFleshStateService FireMadeFleshStateService { get; }

    private static void CreatePlayerHouses(
      GameState gameState,
      List<PlayerDescriptor> playersDescriptors
    )
    {
      foreach (var playerDescriptor in playersDescriptors)
      {
        HouseState houseState = HouseStateFactory.Create(playerDescriptor.HouseType);
        PlayerState playerState = new PlayerState()
        {
          PlayerId = playerDescriptor.PlayerId,
          HouseState = houseState
        };

        gameState.Players[playerDescriptor.PlayerId] = playerState;
      }
    }

    private static void CreateVassalHouses(GameState gameState)
    {
      for (byte i = 0; i < (byte)HouseType.Count; ++i)
      {
        HouseType houseType = (HouseType)i;
        if (houseType == HouseType.Undefined || houseType == HouseType.Targaryen)
          continue; // Skip undefined type. Targaryen cannot be a vassal house

        if (gameState.Players.Values.Any(p => p.HouseState.Type == houseType))
          continue; // Skip if the house is already taken by a player

        if (gameState.Vassals.ContainsKey(houseType))
          continue; // Skip if the house is already added as a vassal

        gameState.Vassals[houseType] = HouseStateFactory.CreateVassal(houseType);
      }
    }

    private static List<HouseState> GetAllHouseStates(GameState gameState)
    {
      List<HouseState> allHouses = new();
      foreach (var player in gameState.Players.Values)
        allHouses.Add(player.HouseState);

      foreach (var vassal in gameState.Vassals.Values)
        allHouses.Add(vassal);
      return allHouses;
    }
  }
}
