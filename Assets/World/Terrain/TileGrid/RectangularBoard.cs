using SpiritWorld.World.Terrain.TileGrid.Generation;
using UnityEngine;

namespace SpiritWorld.World.Terrain.TileGrid {

  /// <summary>
  /// A hex tile board with rectangular grid chunks
  /// </summary>
  public class RectangularBoard : TileBoard {
    /// <summary>
    /// A chunks diameter in hex tiles
    /// </summary>
    public const int ChunkDiameterInTiles = 36;
    /// <summary>
    /// A chunks diameter in hex tiles
    /// </summary>
    public const int ChunkHeightInTiles = 18;

    /// <summary>
    /// The size of a chunk in square game units
    /// </summary>
    public static Vector3 ChunkWorldOffset = new Vector3(54, 0, 62.35f);

    /// <summary>
    /// The size of a chunk in square game units
    /// </summary>
    public static Vector3 ChunkWorldCenter = ChunkWorldOffset / 2;

    /// <summary>
    /// Make a new board of rectangular shape
    /// </summary>
    public RectangularBoard() : base(Shapes.Rectangle) {}

    /// <summary>
    /// Get a tile at the world position in this board.
    /// </summary>
    public override Tile get(Vector3 worldPosition, out Coordinate containingChunkKey) {
      // get which chunk we hit
      containingChunkKey = ChunkLocationFromWorldPosition(worldPosition);
      HexGrid containingChunk = this[containingChunkKey];
      Vector3 chunkWorldOffset = ChunkWorldOffset * containingChunkKey;
      Vector3 localChunkPosition = worldPosition - chunkWorldOffset;
      // get the axial of the selected hex.
      Coordinate localHexAxialKey = Hexagon.WorldLocationToAxialKey(localChunkPosition);
      // adjust for chunk edges that don't fit together peftectly
      if (localHexAxialKey.x >= ChunkDiameterInTiles) {
        containingChunkKey += Directions.East.Offset;
        containingChunk = this[containingChunkKey];
        localHexAxialKey.x -= ChunkDiameterInTiles;
        localHexAxialKey.z += ChunkDiameterInTiles / 2;
      }
      Tile foundTile = containingChunk.get(localHexAxialKey);

      // this can happen if the tile is missing, but it can also happen if we're in the wrong chunk. Try moving up a chunk
      if (!foundTile.axialKey.Equals(localHexAxialKey)) {
        containingChunkKey += Directions.North.Offset;
        containingChunk = this[containingChunkKey];
        localHexAxialKey.z -= ChunkDiameterInTiles;
      }
      foundTile = containingChunk.get(localHexAxialKey);

      return foundTile;
    }

    /// <summary>
    /// get the chunk a world location is in
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public override HexGrid getGridChunkFor(Vector3 worldPosition) {
      Coordinate chunkLocation = ChunkLocationFromWorldPosition(worldPosition);
      return ContainsKey(chunkLocation) ? this[chunkLocation] : null;
    }

    /// <summary>
    /// Add a new rectangular chunk of grid to the tile board
    /// </summary>
    /// <param name="gridWorldLocation"></param>
    /// <param name="biome"></param>
    public override void createNewGrid(Coordinate gridWorldLocation, Biome biome) {
      HexGrid newGrid = new HexGrid();
      HexGridShaper.Rectangle((36, 36), axialKey => {
        newGrid.set(biome.generateAt(axialKey, (gridWorldLocation * ChunkDiameterInTiles)));
      });

      Add(gridWorldLocation, newGrid);
    }

    /// <summary>
    /// Get the board location of a chunk on a rectangular grid that contains the given world position
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public static Coordinate ChunkLocationFromWorldPosition(Vector3 worldPosition) {
      return (
        (int)Mathf.Floor(worldPosition.x / ChunkWorldOffset.x),
        (int)Mathf.Floor(worldPosition.z / ChunkWorldOffset.z)
      );
    }
  }
}
