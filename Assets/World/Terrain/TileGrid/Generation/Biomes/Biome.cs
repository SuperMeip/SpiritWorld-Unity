using SpiritWorld.World.Terrain.Generation.Noise;

namespace SpiritWorld.World.Terrain.TileGrid.Generation {
  public partial struct Biome {

    /// <summary>
    /// The type data for this feature
    /// </summary>
    public Type type {
      get;
    }

    /// <summary>
    /// The seed used to generate this biome's noise
    /// </summary>
    public int seed {
      get;
    }

    /// <summary>
    /// noise layers for this biome
    /// </summary>
    FastNoise[] noiseLayers;

    /// <summary>
    /// Make a new biome of the given type
    /// </summary>
    /// <param name="biomeType"></param>
    /// <param name="seed"></param>
    public Biome(Type biomeType, int seed) {
      type = biomeType;
      this.seed = seed;

      // get all needed layers of noise for the biome
      noiseLayers = new FastNoise[type.NoiseLayerCount];
      for(int i = 0; i < type.NoiseLayerCount; i++) {
        noiseLayers[i] = new FastNoise(seed + i);
      }
    }

    /// <summary>
    /// Generate the tile at an axial point using the biome type
    /// </summary>
    /// <param name="axialKey"></param>
    /// <returns></returns>
    public Tile generateAt(Coordinate axialKey) {
      return type.generateAt(axialKey, noiseLayers);
    }
  }
}
