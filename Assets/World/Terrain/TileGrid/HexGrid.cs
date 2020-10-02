using SpiritWorld.World.Terrain.Features;
using System;
using System.Collections.Generic;

namespace SpiritWorld.World.Terrain.TileGrid {

  public class HexGrid {

    /// <summary>
    /// Tiles stores by hex axial coordinates.
    /// </summary>
    Dictionary<Coordinate, Tile> tiles;

    /// <summary>
    /// Tile features stores by hex axial coordinates of the tile they're on, and then by the feature layer.
    /// </summary>
    Dictionary<Coordinate, FeaturesByLayer> features
      = new Dictionary<Coordinate, FeaturesByLayer>();

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
    /// Set a tile and it's features at once
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="features"></param>
    public void set(Tile tile, FeaturesByLayer features) {
      set(tile);
      if (features != null) {
        set(tile.axialKey, features);
      }
    }

    /// <summary>
    /// Set a tile and it's features at once
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="features"></param>
    public void set((Tile tile, FeaturesByLayer features) tileValues) {
      set(tileValues.tile, tileValues.features);
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
    /// Set a feature on a given tile
    /// </summary>
    /// <param name="tileAxialKey"></param>
    /// <param name="feature"></param>
    public void set(Coordinate tileAxialKey, TileFeature feature) {
      if (tiles.ContainsKey(tileAxialKey)) {
        if (features.ContainsKey(tileAxialKey)) {
          features[tileAxialKey].Add(feature.type.Layer, feature);
        } else {
          features[tileAxialKey] = new FeaturesByLayer {{
            feature.type.Layer,
            feature
          }};
        }
      } else {
        throw new IndexOutOfRangeException($"No tile found to be set at {tileAxialKey}. Cannot set a feature to an empty tile");
      }
    }

    /// <summary>
    /// Set a feature on a given tile
    /// </summary>
    /// <param name="tileAxialKey"></param>
    /// <param name="feature"></param>
    public void set(Coordinate tileAxialKey, FeaturesByLayer newFeatures) {
      if (tiles.ContainsKey(tileAxialKey)) {
        features[tileAxialKey] = newFeatures;
      } else {
        throw new IndexOutOfRangeException($"No tile found to be set at {tileAxialKey}. Cannot set a feature to an empty tile");
      }
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
      foreach (KeyValuePair<Coordinate, Tile> tile in tiles) {
        action(tile.Key, tile.Value);
      }
    }

    /// <summary>
    /// Do something on each tile with a feature
    /// Coordinate: axial coord of the hex
    /// tile: the tile type
    /// </summary>
    /// <param name="action">A function taking the world coordinate location of the hexagon as well as the tile</param>
    public void forEach(Action<Coordinate, Tile, FeaturesByLayer> action) {
      foreach (KeyValuePair<Coordinate, FeaturesByLayer> feature in features) {
        action(feature.Key, tiles[feature.Key], feature.Value);
      }
    }
  }
}
