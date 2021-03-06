﻿using SpiritWorld.World.Terrain.Features;
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
      public override (Tile tile, FeaturesByLayer features) generateAt(Coordinate axialKey, FastNoise[] noiseLayers, Coordinate chunkKeyOffset = default) {
        /// get the tile type and height
        Coordinate noiseKey = axialKey + chunkKeyOffset * RectangularBoard.ChunkWorldOffset;
        float heightNoise = noiseLayers[(int)NoiseLayers.Height].GetPerlinFractal(noiseKey.x * 20, noiseKey.z * 20);
        float tileTypeNoise = noiseLayers[(int)NoiseLayers.Terrain].GetPerlinFractal(noiseKey.x * 10, noiseKey.z * 10);
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
          float forestNoise = noiseLayers[(int)NoiseLayers.Forest].GetCellular(noiseKey.x * 20, noiseKey.z * 10);
          if (forestNoise >= 0) {
            features = new FeaturesByLayer {{
               TileFeature.Types.ConniferTrio.Layer,
               new TileFeature(TileFeature.Types.ConniferTrio)
             }};
          }
        }

        // rocks
        float cloudNoise = noiseLayers[(int)NoiseLayers.Clouds].GetCellular(noiseKey.x * 50, noiseKey.z * 50);
        if ((tileType == Tile.Types.Rocky || tileType == Tile.Types.Grass) && (features == null || !features.ContainsKey(TileFeature.Layer.Decoration))) {
          bool hasResouce = features?.ContainsKey(TileFeature.Layer.Resource) ?? false;
          float rockNoise = noiseLayers[(int)NoiseLayers.Forest].GetCellular(noiseKey.x * 35, noiseKey.z * 40);
          if ((!hasResouce && tileType != Tile.Types.Grass && rockNoise >= 0) || rockNoise >= 0.5f) {
            TileFeature rockPile;
            if (hasResouce) {
              rockPile = new TileFeature(TileFeature.Types.DecorativeRocks);
            } else {
              int rockSize = (int)cloudNoise.scale(0, (tileType == Tile.Types.Grass) ? 2 : 4);
              if (rockSize == 3) {
                rockPile = new TileFeature(TileFeature.Types.IronVeinedRocks);
              } else {
                rockPile = new TileFeature(TileFeature.Types.RockPile);
                rockPile.setRemainingInteractions(rockSize);
              }
            }
            if (features == null) {
              features = new FeaturesByLayer {{
               rockPile.type.Layer,
               rockPile
             }};
            } else {
              features.Add(
                rockPile.type.Layer,
                rockPile
              );
            }
          }
        }

        // lilypads
        if (tileType == Tile.Types.Water) {
          float lillyNoise = noiseLayers[(int)NoiseLayers.Clouds].GetPerlinFractal(noiseKey.x * 25 + 10, noiseKey.z * 25 + 25);
          if (lillyNoise >= 0.3f) {
            features = new FeaturesByLayer {{
               TileFeature.Types.BloomingLilypads.Layer,
               new TileFeature(TileFeature.Types.BloomingLilypads)
             }};
          } else if (lillyNoise >= 0.1f) {
            features = new FeaturesByLayer {{
               TileFeature.Types.SmallLilypads.Layer,
               new TileFeature(TileFeature.Types.SmallLilypads)
             }};
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
            scaledHeightValue,
            chunkKeyOffset
          ),
          features
        );
      }
    }
  }
}