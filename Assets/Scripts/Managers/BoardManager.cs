using SpiritWorld.Controllers;
using SpiritWorld.Events;
using SpiritWorld.World.Terrain.Features;
using SpiritWorld.World.Terrain.TileGrid;
using System.Collections.Generic;
using UnityEngine;

namespace SpiritWorld.Managers {
  public class BoardManager : MonoBehaviour, IObserver {

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
    /// Controllers currently being used, indexed by which chunk they're being used for
    /// </summary>
    Dictionary<Coordinate, GridController> inUseControllers
      = new Dictionary<Coordinate, GridController>();

    /// <summary>
    /// Set up the grid chunk controller pool
    /// </summary>
    void Awake() {
      chunkControllerPool = gameObject.GetComponentsInChildren<GridController>(true);
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
        foreach(Coordinate chunkLocationKey in chunkLocationsToLoad) {
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
      foreach(GridController controller in chunkControllerPool) {
        controller.clear();
      }
    }

    /// <summary>
    /// Get the board locations of the chunks that should be visible to a player at the given worldlocation
    /// </summary>
    /// <returns>The board locations (x/z of the grid in the tileboard)</returns>
    Coordinate[] getLiveChunksAround(Vector3 worldLocation) {
      Coordinate currentChunkLocationKey = RectangularBoard.ChunkLocationFromWorldPosition(worldLocation);
      Vector3 inChunkTileLocation = new Vector3(
        worldLocation.x % RectangularBoard.ChunkWorldOffset.x,
        0,
        worldLocation.z % RectangularBoard.ChunkWorldOffset.z
      );
      // set up and add the current chunk location
      List<Coordinate> chunkLocationsToLoad = new List<Coordinate> {
        currentChunkLocationKey
      };

      // if we're close to the east of the chunk
      if (inChunkTileLocation.x > (RectangularBoard.ChunkWorldCenter.x + RectangularBoard.ChunkWorldCenter.x / 2)) {
        chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.East.Offset);
        // if we're also close to the north of the chunk
        if (inChunkTileLocation.z > (RectangularBoard.ChunkWorldCenter.y + RectangularBoard.ChunkWorldCenter.y / 2)) {
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.North.Offset);
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.North.Offset + Directions.East.Offset);
        // if we're also close to the south of the chunk
        } else if (inChunkTileLocation.y < (RectangularBoard.ChunkWorldCenter.y - RectangularBoard.ChunkWorldCenter.z / 2)) {
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.South.Offset);
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.South.Offset + Directions.East.Offset);
        }
      // if we're close to the west of the chunk
      } else if (inChunkTileLocation.x < (RectangularBoard.ChunkWorldCenter.x - RectangularBoard.ChunkWorldCenter.x / 2)) {
        chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.West.Offset);
        // if we're also close to the north of the chunk
        if (inChunkTileLocation.z > (RectangularBoard.ChunkWorldCenter.z + RectangularBoard.ChunkWorldCenter.z / 2)) {
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.North.Offset);
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.North.Offset + Directions.West.Offset);
          // if we're also close to the south of the chunk
        } else if (inChunkTileLocation.z < (RectangularBoard.ChunkWorldCenter.z - RectangularBoard.ChunkWorldCenter.z / 2)) {
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.South.Offset);
          chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.South.Offset + Directions.West.Offset);
        }
      // if we're only close to the north of the chunk
      } else if (inChunkTileLocation.z > (RectangularBoard.ChunkWorldCenter.z + RectangularBoard.ChunkWorldCenter.z / 2)) {
        chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.North.Offset);
      // if we're only close to the south of the chunk
      } else if (inChunkTileLocation.z < (RectangularBoard.ChunkWorldCenter.z - RectangularBoard.ChunkWorldCenter.z / 2)) {
        chunkLocationsToLoad.Add(currentChunkLocationKey + Directions.South.Offset);
      }

      return chunkLocationsToLoad.ToArray();
    }

    /// <summary>
    /// Listen for events
    /// </summary>
    /// <param name="event"></param>
    public void notifyOf(IEvent @event) {
      switch (@event) {
        case TileSelectionManager.TileFeatureModeChanged tsetfmc:
          updateModeForFeature(tsetfmc.updatedTile, tsetfmc.updatedFeature);
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Update the mode for a feature on a given tile
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="featureLayer"></param>
    void updateModeForFeature(Tile tile, TileFeature feature) {
      if (inUseControllers.TryGetValue(tile.parentChunkKey, out GridController chunkController)) {
        chunkController.updateFeatureModel(tile, feature);
      }
    }
  }
}
