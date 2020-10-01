using SpiritWorld.World.Terrain.Generation.Noise;
using SpiritWorld.World.Terrain.TileGrid.Features;
using System.Collections.Generic;
using UnityEngine;

namespace SpiritWorld.World.Terrain.TileGrid.Generation {

  /// <summary>
  /// Extensions for the Grass tile type
  /// </summary>
  public partial struct Biome {

    /// <summary>
    /// Tile type singleton constants
    /// </summary>
    public static partial class Types {
      public static Type RockyForest = new RockyForest();
    }

    /// <summary>
    /// Empty tile type
    /// </summary>
    public class RockyForest : Type {

      /// <summary>
      /// Types of noise this biome needs for gen
      /// </summary>
      enum NoiseLayers {
        Height,
        Terrain,
        Forest,
        Count // # of layers
      }

      /// <summary>
      /// set up this biome type
      /// </summary>
      internal RockyForest() : base(1, (int)NoiseLayers.Count) {}

      /// <summary>
      /// Generate a forest with some rocky areas
      /// </summary>
      /// <param name="axialKey"></param>
      /// <param name="noiseLayers"></param>
      /// <returns></returns>
      public override Tile generateAt(Coordinate axialKey, FastNoise[] noiseLayers) {
        // get the tile type and height
        float heightNoise = noiseLayers[(int)NoiseLayers.Height].GetPerlinFractal(axialKey.x * 20, axialKey.z * 20);
        float tileTypeNoise = noiseLayers[(int)NoiseLayers.Terrain].GetPerlinFractal(axialKey.x * 10, axialKey.z * 10);
        int scaledHeightValue = Mathf.Max((int)heightNoise.scale(20, 1), 7);
        Tile.Type tileType =
          scaledHeightValue == 7
            ? Tile.Types.Water
            : tileTypeNoise > 0 && scaledHeightValue > 10
              ? Tile.Types.Rocky
              : Tile.Types.Grass;

        // check for features
        Dictionary<TileFeature.Layer, TileFeature> features = null;
        if (tileType == Tile.Types.Grass) {
          float forestNoise = noiseLayers[(int)NoiseLayers.Forest].GetCellular(axialKey.x, axialKey.z);
          if (forestNoise >= 0) {
            features = new Dictionary<TileFeature.Layer, TileFeature> {{
              TileFeature.Types.ConniferTrio.Layer,
              new TileFeature(TileFeature.Types.ConniferTrio)
            }};
          }
        }

        return new Tile(
          tileType,
          axialKey,
          scaledHeightValue,
          features
        );
      }
    }
  }
}