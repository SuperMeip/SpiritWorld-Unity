using SpiritWorld.World.Terrain.TileGrid.Features;
using System.Collections.Generic;
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
    /// The features on this tile
    /// </summary>
    Dictionary<TileFeature.Layer, TileFeature> featuresByLayer;

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
      get => Hexagon.WorldLocationToAxialKey(worldLocation);
    }

    /// <summary>
    /// Create a new tile using an axial key
    /// </summary>
    public Tile(
      Type type = null,
      Coordinate axialKey = default,
      int height = 0,
      Dictionary<TileFeature.Layer, TileFeature> featues = null
    ) {
      Vector3 worldLocationXZ = Hexagon.AxialKeyToWorldLocation(axialKey);
      worldLocation = new Vector3(worldLocationXZ.x, height, worldLocationXZ.z);
      featuresByLayer = featues;

      this.type = type ?? Types.Empty;
    }

    /// <summary>
    /// Make a new tile of the desired type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="height"></param>
    public Tile(Coordinate worldLocation2D = default, Type type = null, int height = 0) {
      worldLocation = new Vector3(worldLocation2D.x, height, worldLocation2D.z);
      featuresByLayer = null;

      this.type = type ?? Types.Empty;
    }
  }
}