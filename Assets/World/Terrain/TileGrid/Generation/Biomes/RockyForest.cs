using SpiritWorld.World.Terrain.Features;
using SpiritWorld.World.Terrain.Generation.Noise;
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
        Clouds,
        Count // # of layers
      }

      /// <summary>
      /// set up this biome type
      /// </summary>
      internal RockyForest() : base(1, (int)NoiseLayers.Count) { }


      /// <summary>
      /// Generate a forest with some rocky areas
      /// </summary>
      /// <returns></returns>
      public override (Tile tile, FeaturesByLayer features) generateAt(Coordinate axialKey, FastNoise[] noiseLayers) {
        /// get the tile type and height
        float heightNoise = noiseLayers[(int)NoiseLayers.Height].GetPerlinFractal(axialKey.x * 20, axialKey.z * 20);
        float tileTypeNoise = noiseLayers[(int)NoiseLayers.Terrain].GetPerlinFractal(axialKey.x * 10, axialKey.z * 10);
        int scaledHeightValue = Mathf.Max((int)heightNoise.scale(20, 1), 7);
        Tile.Type tileType =
          scaledHeightValue == 7
            ? Tile.Types.Water
            : tileTypeNoise > 0 && scaledHeightValue > 10
              ? Tile.Types.Rocky
              : Tile.Types.Grass;

        /// check for features
        FeaturesByLayer features = null;
        // trees
        if (tileType == Tile.Types.Grass) {
          float forestNoise = noiseLayers[(int)NoiseLayers.Forest].GetCellular(axialKey.x * 20, axialKey.z * 20);
          if (forestNoise >= 0) {
            features = new FeaturesByLayer {{
               TileFeature.Types.ConniferTrio.Layer,
               new TileFeature(TileFeature.Types.ConniferTrio)
             }};
          }
        }

        // rocks
        float cloudNoise = noiseLayers[(int)NoiseLayers.Clouds].GetCellular(axialKey.x * 50, axialKey.z * 50);
        if ((tileType == Tile.Types.Rocky || tileType == Tile.Types.Grass) && (features == null || !features.ContainsKey(TileFeature.Layer.Decoration))) {
          float rockNoise = noiseLayers[(int)NoiseLayers.Forest].GetCellular(axialKey.x * 35, axialKey.z * 40);
          if (rockNoise >= 0) {
          int rockMode = (int)cloudNoise.scale(0, 3);
            if (features == null) {
              features = new FeaturesByLayer {{
               TileFeature.Types.RockPile.Layer,
               new TileFeature(TileFeature.Types.RockPile, rockMode)
             }};
            } else {
              features.Add(
                TileFeature.Types.RockPile.Layer,
                new TileFeature(TileFeature.Types.RockPile, rockMode)
              );
            }
          }
        }

        // clouds
        if (cloudNoise >= 0.7f) {
          int cloudMode = (int)cloudNoise.scale(0, 3);
          if (features == null) {
            features = new FeaturesByLayer {{
               TileFeature.Types.WhiteClouds.Layer,
               new TileFeature(TileFeature.Types.WhiteClouds, cloudMode)
             }};
          } else {
            features.Add(
              TileFeature.Types.WhiteClouds.Layer,
              new TileFeature(TileFeature.Types.WhiteClouds, cloudMode)
            );
          }
        }

        return (
          new Tile(
            tileType,
            axialKey,
            scaledHeightValue
          ),
          features
        );
      }
    }
  }
}