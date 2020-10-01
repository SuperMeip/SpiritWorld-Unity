using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpiritWorld.World.Terrain.TileGrid {

  public class TileBoard {

    /// <summary>
    /// Tiles stores by hex axial coordinates.
    /// </summary>
    Dictionary<Coordinate, Tile> tiles;

    /// <summary>
    /// Make a new grid of tiles
    /// </summary>
    public TileBoard() {
      tiles = new Dictionary<Coordinate, Tile>();
    }

    /// <summary>
    /// Get the tile at the given axial hex location
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public Tile get(Coordinate location) {
      Tile fetchedTile;
      tiles.TryGetValue(location, out fetchedTile);
      return fetchedTile;
    }

    /// <summary>
    /// Set the hex tile using it's world location's coodinates
    /// </summary>
    /// <param name="worldLocation">The 2D in world location of the tile (X, 0Z)</param>
    /// <param name="tile"></param>
    public void set(Coordinate worldLocation, Tile.Type tileType, byte height = 0) {
      Tile newTile = new Tile(worldLocation, tileType, height);
      set(newTile);
    }

    /// <summary>
    /// Set the preconfigured new tile
    /// </summary>
    /// <param name="newTile"></param>
    public void set(Tile newTile) {
      tiles[newTile.axialKey] = newTile;
    }

    /// <summary>
    /// Do something on each tile (in no particular order)
    /// Coordinate: axial coord of the hex
    /// tile: the tile type
    /// </summary>
    /// <param name="action">A function taking the world coordinate location of the hexagon as well as the tile</param>
    public void forEach(Action<Coordinate, Tile> action) {
      foreach (KeyValuePair<Coordinate, Tile> entry in tiles) {
        action(entry.Key, entry.Value);
      }
    }
  }
}
