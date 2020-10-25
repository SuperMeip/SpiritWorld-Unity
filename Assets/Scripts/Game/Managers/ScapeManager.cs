using SpiritWorld.World;
using SpiritWorld.World.Entities.Creatures;
using SpiritWorld.World.Terrain.TileGrid;
using SpiritWorld.World.Terrain.TileGrid.Generation;
using UnityEngine;

namespace SpiritWorld.Managers {
  public class ScapeManager : MonoBehaviour {

    /// <summary>
    /// The manager of the active board for this worldscape
    /// </summary>
    public BoardManager boardManager;

    /// <summary>
    /// The active player
    /// </summary>
    public LocalPlayerManager localPlayerController;

    // set up our scape
    void Start() {
      // set up the local player and notification managers
      Player localPlayer = new Player("Meep");
      Universe.LocalPlayerManager = localPlayerController;
      localPlayerController.setPlayer(localPlayer);

      // subscribe this to all channels
      Universe.EventSystem.subscribeToAll(Universe.LocalPlayerManager.NotificationsManager);

      // listen for inventory changes
      Universe.EventSystem.subscribe(
        Universe.LocalPlayerManager.PackGridController,
        Events.WorldScapeEventSystem.Channels.LocalPlayerUpdates
      );
      Universe.EventSystem.subscribe(
        Universe.LocalPlayerManager.ItemHotBarController,
        Events.WorldScapeEventSystem.Channels.LocalPlayerUpdates
      );

      // set up the scape, 
      WorldScape testScape = new WorldScape();
      Biome testForest = new Biome(Biome.Types.RockyForest, 1234);
      testScape.addBoard(new RectangularBoard(testForest));
      Universe.CurrentScape = testScape;

      // set up our active board manager. This will manage the visible board for the scape
      Universe.ActiveBoardManager = boardManager;
      Universe.EventSystem.subscribe(boardManager, Events.WorldScapeEventSystem.Channels.TileUpdates);
      boardManager.setBoard(testScape.mainBoard);

      // load the board
      testScape.mainBoard.populate((-10, -10), (11, 11));
      boardManager.loadBoardAround(localPlayerController.gameObject.transform.position);
    }
  }
}
