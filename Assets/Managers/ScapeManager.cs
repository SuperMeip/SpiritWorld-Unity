using SpiritWorld.Controllers;
using SpiritWorld.World;
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
    public PlayerController player;

    // set up our scape
    void Start() {
      WorldScape testScape = new WorldScape();
      Universe.CurrentScape = testScape;
      Biome testForest = new Biome(Biome.Types.RockyForest, 1234);
      boardManager.setBoard(testScape.mainBoard);
      testScape.mainBoard.populate((-10, -10), (11, 11), testForest);
      boardManager.loadBoardAround(player.gameObject.transform.position);
    }
  }
}
