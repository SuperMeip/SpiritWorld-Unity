using System;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid {

  /// <summary>
  /// Tiles stores by hex axial coordinates.
  /// </summary>
  Dictionary<Coordinate, Tile> tiles;

  /// <summary>
  /// Make a new grid of tiles
  /// </summary>
  public HexGrid() {
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
  /// Set the hex tile using axial hex coodinates
  /// </summary>
  /// <param name="location">The 2D in world location of the tile (X, Z)</param>
  /// <param name="tile"></param>
  public void set(Vector2 location, Tile.Type tileType, byte height = 0) {
    Tile newTile = new Tile(location, tileType, height);

    tiles[newTile.axialKey] = newTile;
  }

  /// <summary>
  /// Do something on each tile (in no particular order)
  /// </summary>
  /// <param name="action">A function taking the world coordinate location of the hexagon as well as the tile</param>
  public void forEach(Action<Coordinate, Tile> action) {
    foreach (KeyValuePair<Coordinate, Tile> entry in tiles) {
      action(entry.Key, entry.Value);
    }
  }
}
