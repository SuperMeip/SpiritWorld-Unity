using SpiritWorld.World.Terrain.Features;
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
    /// Override for missing grids
    /// </summary>
    /// <param name="chunkBoardLocationKey"></param>
    /// <returns></returns>
    public new HexGrid this[Coordinate chunkBoardLocationKey] 
      => TryGetValue(chunkBoardLocationKey, out HexGrid grid) ? grid : null;

    /// <summary>
    /// Get a tile at the world position in this board.
    /// Also can return the key of the chunk it's in
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public abstract Tile get(Vector3 worldPosition, out Coordinate containingChunkKey);

    /// <summary>
    /// Get a tile at the world position in this board.
    /// Also can return the key of the chunk it's in
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Tile get(Vector3 worldPosition) {
      return get(worldPosition, out _);
    }

    /// <summary>
    /// Get the features for the given tile
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public FeaturesByLayer getFeaturesFor(Tile tile) {
      return this[tile.parentChunkKey]?.getTileFeatures(tile.axialKey);
    }

    /// <summary>
    /// Get the grid containing the tile at the given world position
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public abstract Coordinate getChunkKeyFor(Tile tile);

    /// <summary>
    /// Populate a range of new grids.
    /// </summary>
    /// <param name="start">inclusive</param>
    /// <param name="end">exclusive</param>
    /// <param name="biome"></param>
    public void populate(Coordinate start, Coordinate end, Biome biome) {
      start.until(end, chunkLocation => createNewGrid(chunkLocation, biome));
    }

    /// <summary>
    /// Create a new grid at the location in this tileboard
    /// </summary>
    /// <param name="gridWorldLocation"></param>
    /// <param name="biome"></param>
    public abstract void createNewGrid(Coordinate gridWorldLocation, Biome biome);

    /// <summary>
    /// Set or update a tile and a feature on the tileboard
    /// </summary>
    /// <param name="selectedTile"></param>
    /// <param name="resource"></param>
    internal abstract void update(Tile selectedTile, TileFeature resource);
  }
}