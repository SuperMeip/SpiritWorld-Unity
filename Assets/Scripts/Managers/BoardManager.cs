using SpiritWorld.Controllers;
using SpiritWorld.Events;
using SpiritWorld.World.Terrain.Features;
using SpiritWorld.World.Terrain.TileGrid;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpiritWorld.Managers {
  public class BoardManager : MonoBehaviour, IObserver {

    /// <summary>
    /// How often to update the active chunks in seconds 
    /// </summary>
    public float chunkUpdateTickSeconds = 1;

    /// <summary>
    /// The chunk controllers we have available to use
    /// </summary>
    GridController[] chunkControllerPool;

    /// <summary>
    /// The tile board that is currently acitve
    /// </summary>
    public TileBoard activeBoard {
      get;
      private set;
    }

    /// <summary>
    /// The tick timer
    /// </summary>
    float tickTimer = 0;

    /// <summary>
    /// The controller of the local player
    /// </summary>
    LocalPlayerMovementController localPlayerController;

    /// <summary>
    /// Controllers currently being used, indexed by which chunk they're being used for
    /// </summary>
    Dictionary<Coordinate, GridController> inUseControllers
      = new Dictionary<Coordinate, GridController>();

    /// <summary>
    /// Set up the grid chunk controller pool
    /// </summary>
    void Awake() {
      chunkControllerPool = gameObject.GetComponentsInChildren<GridController>(true);
      localPlayerController = GameObject.FindWithTag("Local Player").GetComponent<LocalPlayerMovementController>();
    }

    /// <summary>
    /// Poll player location
    /// </summary>
    void Update() {
      if (tickTimer >= chunkUpdateTickSeconds) {
        checkAndUpdateChunksAroundLocalPlayer();
        tickTimer = 0;
      } else {
        tickTimer += Time.deltaTime;
      }
    }

    /// <summary>
    /// Set the active board this tileboard is managing.
    /// </summary>
    /// <param name="newBoard"></param>
    public void setBoard(TileBoard newBoard) {
      clear();
      activeBoard = newBoard;
    }

    /// <summary>
    /// Load the board around a given tile's X/Z in the world.
    /// </summary>
    /// <param name="worldLocation"></param>
    public void loadBoardAround(Coordinate worldLocation) {
      if (activeBoard != null) {
        /// figure out which board chunks we want to load and load them
        // collect the locations of the board chunks we need to load based on the centered world location
        Coordinate[] chunkLocationsToLoad = getLiveChunksAround(worldLocation);

        // assign controllers to the board chunks
        int index = 0;
        foreach (Coordinate chunkLocationKey in chunkLocationsToLoad) {
          // currently chunks load on assignment
          inUseControllers[chunkLocationKey] = chunkControllerPool[index];
          chunkControllerPool[index++].updateGridTo(activeBoard[chunkLocationKey], chunkLocationKey);
        }
      }
    }

    /// <summary>
    /// clear the active board and the controllers being used of their chunks
    /// </summary>
    void clear() {
      activeBoard = null;
      foreach (GridController controller in chunkControllerPool) {
        controller.clear();
      }
    }

    /// <summary>
    /// used for polling player location and updating the loaded chunks
    /// </summary>
    void checkAndUpdateChunksAroundLocalPlayer() {
      Coordinate[] chunkLocationsThatShouldBeLoaded = getLiveChunksAround(localPlayerController.transform.position);
      // if the chunks that should be loaded don't match the current in use chunk controllers
      IEnumerable<Coordinate> chunksToLoad = chunkLocationsThatShouldBeLoaded.Except(inUseControllers.Keys.ToArray());
      IEnumerable<Coordinate> chunksToUnload = inUseControllers.Keys.ToArray().Except(chunkLocationsThatShouldBeLoaded);
      foreach (Coordinate chunkKey in chunksToUnload) {
        inUseControllers[chunkKey].clear();
        inUseControllers.Remove(chunkKey);
      }
      foreach (Coordinate chunkKeyToLoad in chunksToLoad) {
        // currently chunks load on assignment
        inUseControllers[chunkKeyToLoad] = chunkControllerPool[getUnusedControllerIndex()];
        inUseControllers[chunkKeyToLoad].updateGridTo(activeBoard[chunkKeyToLoad], chunkKeyToLoad);
      }
    }

    /// <summary>
    /// Get an empty controller index
    /// </summary>
    /// <returns></returns>
    int getUnusedControllerIndex() {
      int index = 0;
      foreach (GridController controller in chunkControllerPool) {
        if (!controller.isInUse) {
          return index;
        }
        index++;
      }

      return -1;
    }

    /// <summary>
    /// Get the board locations of the chunks that should be visible to a player at the given worldlocation
    /// </summary>
    /// <returns>The board locations (x/z of the grid in the tileboard)</returns>
    Coordinate[] getLiveChunksAround(Vector3 worldLocation) {
      Tile currentTile = activeBoard.get(worldLocation);
      Coordinate currentChunkLocationKey = activeBoard.getChunkKeyFor(currentTile);
      Vector3 inChunkTileLocation = currentTile.localLocation;
      /*
        worldLocation.x % RectangularBoard.ChunkWorldOffset.x,
        0,
        worldLocation.z % RectangularBoard.ChunkWorldOffset.z
      );*/
      // set up and add the current chunk location
      List<Coordinate> chunkLocationsToLoad = new List<Coordinate> {
        currentChunkLocationKey
      };

      // if we're close to the east of the chunk
      if (inChunkTileLocation.x > RectangularBoard.ChunkWorldCenter.x) {
        chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.East.Offset);
        // if we're also close to the north of the chunk
        if (inChunkTileLocation.z > RectangularBoard.ChunkWorldCenter.z) {
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.North.Offset);
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.North.Offset + Directions.East.Offset);
          // if we're also close to the south of the chunk
        } else if (inChunkTileLocation.z < RectangularBoard.ChunkWorldCenter.z) {
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.South.Offset);
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.South.Offset + Directions.East.Offset);
        }
        // if we're close to the west of the chunk
      } else if (inChunkTileLocation.x < RectangularBoard.ChunkWorldCenter.x) {
        chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.West.Offset);
        // if we're also close to the north of the chunk
        if (inChunkTileLocation.z > RectangularBoard.ChunkWorldCenter.z) {
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.North.Offset);
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.North.Offset + Directions.West.Offset);
          // if we're also close to the south of the chunk
        } else if (inChunkTileLocation.z < RectangularBoard.ChunkWorldCenter.z) {
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.South.Offset);
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.South.Offset + Directions.West.Offset);
        }
        // if we're only close to the north of the chunk
      } else if (inChunkTileLocation.z > RectangularBoard.ChunkWorldCenter.z) {
        chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.North.Offset);
        // if we're only close to the south of the chunk
      } else if (inChunkTileLocation.z < RectangularBoard.ChunkWorldCenter.z) {
        chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.South.Offset);
      }

      return chunkLocationsToLoad.ToArray();
    }

    /// <summary>
    /// Get the board locations of the chunks that should be visible to a player at the given worldlocation
    /// </summary>
    /// <returns>The board locations (x/z of the grid in the tileboard)</returns>
    /*Coordinate[] getLiveChunksAround(Vector3 worldLocation) {
      Coordinate currentChunkLocationKey = RectangularBoard.ChunkLocationFromWorldPosition(worldLocation);
      List<Coordinate> chunkLocationsToLoad = new List<Coordinate>();
      (currentChunkLocationKey - ActiveChunkBuffer)
        .until((currentChunkLocationKey + ActiveChunkBuffer + 1), (chunkKey) => {
          chunkLocationsToLoad.Add(chunkKey);
        });

      return chunkLocationsToLoad.ToArray();
    }*/

    /// <summary>
    /// Listen for events
    /// </summary>
    /// <param name="event"></param>
    public void notifyOf(IEvent @event) {
      switch (@event) {
        case TileSelectionManager.TileFeatureUpdated tsetfmc:
          updateFeature(tsetfmc.updatedTile, tsetfmc.updatedFeature);
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Update the mode for a feature on a given tile.
    /// It needs to be on the same layer as the one being updated.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="featureLayer"></param>
    void updateFeature(Tile tile, TileFeature feature) {
      if (inUseControllers.TryGetValue(tile.parentChunkKey, out GridController chunkController)) {
        chunkController.updateFeatureModel(tile, feature);
      }
    }
  }
}
