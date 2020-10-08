using UnityEngine;

namespace SpiritWorld.World.Terrain.TileGrid {

  /// <summary>
  /// A tile of terrain on a gameboard
  /// </summary>
  public partial struct Tile {

    /// <summary>
    /// The type of tile
    /// </summary>
    public Type type {
      get;
    }

    /// <summary>
    /// The in world location of the center of this hex
    /// </summary>
    public Vector3 worldLocation {
      get;
    }

    /// <summary>
    /// The location of this block within the chunk with world coordinates
    /// </summary>
    public Vector3 localLocation {
      get => worldLocation - RectangularBoard.ChunkWorldOffset * parentChunkKey;
    }

    /// <summary>
    /// The X,Z of the parent chunk
    /// </summary>
    public Coordinate parentChunkKey {
      get;
    }

    /// <summary>
    /// The height of this tile
    /// </summary>
    public int height {
      get => (int)worldLocation.y;
    }

    /// <summary>
    /// The key used to store hexagons in the grid
    /// </summary>
    public Coordinate axialKey {
      get;
    }

    /// <summary>
    /// Create a new tile using an axial key
    /// </summary>
    public Tile(
      Type type = null,
      Coordinate axialKey = default,
      int height = 0,
      Coordinate parentChunkKey = default
    ) {
      this.axialKey = axialKey;
      this.parentChunkKey = parentChunkKey;
      Vector3 worldLocationXZ = Hexagon.AxialKeyToWorldLocation(axialKey) + RectangularBoard.ChunkWorldOffset * parentChunkKey;
      worldLocation = new Vector3(worldLocationXZ.x, height, worldLocationXZ.z);

      this.type = type ?? Types.Empty;
    }

    /// <summary>
    /// Make a new tile of the desired type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="height"></param>
    public Tile(Coordinate worldLocation2D = default, Type type = null, int height = 0, Coordinate parentChunkKey = default) {
      this.parentChunkKey = parentChunkKey;
      worldLocation = new Vector3(worldLocation2D.x, height, worldLocation2D.z);
      axialKey = Hexagon.WorldLocationToAxialKey(worldLocation - RectangularBoard.ChunkWorldOffset * parentChunkKey);
      worldLocation += RectangularBoard.ChunkWorldOffset * parentChunkKey;

      this.type = type ?? Types.Empty;
    }
  }
}