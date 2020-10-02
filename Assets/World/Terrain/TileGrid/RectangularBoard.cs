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
    /// The size of a chunk in square game units
    /// </summary>
    public static Vector3 ChunkWorldOffset = new Vector3(54, 0, 62.35f);

    /// <summary>
    /// Make a new board of rectangular shape
    /// </summary>
    public RectangularBoard() : base(Shapes.Rectangle) {}

    /// <summary>
    /// get the chunk a world location is in
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public override HexGrid getGridChunkFor(Vector3 worldPosition) {
      Coordinate chunkLocation = (
        (int)(worldPosition.x / ChunkWorldOffset.x),
        (int)(worldPosition.z / ChunkWorldOffset.z)
      );
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
        newGrid.set(biome.generateAt(axialKey));
      });

      Add(gridWorldLocation, newGrid);
    }
  }
}
