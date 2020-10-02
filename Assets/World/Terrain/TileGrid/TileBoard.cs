using SpiritWorld.World.Terrain.TileGrid.Generation;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  collection for hex grids into one big board
///  Indexed by world coordinates
/// </summary>
namespace SpiritWorld.World.Terrain.TileGrid {
  public abstract class TileBoard : Dictionary<Coordinate, HexGrid> {

    /// <summary>
    /// Shape configurations for tile boards
    /// </summary>
    public enum Shapes {
      Rectangle
    }

    public Shapes shape {
      get;
    }

    /// <summary>
    /// Make a new tileboard
    /// </summary>
    /// <param name="shape"></param>
    protected TileBoard(Shapes shape) {
      this.shape = shape;
    }

    /// <summary>
    /// Get the grid containing the tile at the given world position
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public abstract HexGrid getGridChunkFor(Vector3 worldPosition);

    /// <summary>
    /// Create a new grid at the location in this tileboard
    /// </summary>
    /// <param name="gridWorldLocation"></param>
    /// <param name="biome"></param>
    public abstract void createNewGrid(Coordinate gridWorldLocation, Biome biome);
  }
}