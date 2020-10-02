using System.Collections.Generic;

/// <summary>
/// Collection for features by layer for simplifications sake
/// </summary>
namespace SpiritWorld.World.Terrain.Features {
  public class FeaturesByLayer : Dictionary<TileFeature.Layer, TileFeature> {}
}
