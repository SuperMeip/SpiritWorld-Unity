using SpiritWorld.World.Terrain.Generation.Noise;
using System.Collections.Generic;

namespace SpiritWorld.World.Terrain.TileGrid.Generation {
  public partial struct Biome {

    /// <summary>
    /// Singleton class for biome type data
    /// </summary>
    public abstract class Type {

      /// <summary>
      /// The id of this feature type
      /// </summary>
      public byte Id {
        get;
      }

      /// <summary>
      /// How many layers of noise this biome needs to be generated
      /// </summary>
      public int NoiseLayerCount {
        get;
      }

      /// <summary>
      /// For making new types
      /// </summary>
      /// <param name="id"></param>
      protected Type(byte id, int noiseLayers = 1) {
        Id = id;
        NoiseLayerCount = noiseLayers;

        // on creation, add the singleton to the all types list.
        Types.Add(this);
      }

      /// <summary>
      /// Generate the tile at the given axial key location
      /// </summary>
      /// <param name="axialKey"></param>
      /// <returns></returns>
      public abstract Tile generateAt(Coordinate axialKey, FastNoise[] noiseLayers);
    }

    /// <summary>
    /// Tile type singleton constants
    /// </summary>
    public static partial class Types {

      /// <summary>
      /// All registered block types as an ordered array
      /// </summary>
      public static Type[] All {
        get {
          Type[] types = new Type[all.Count];
          all.Values.CopyTo(types, 0);
          return types;
        }
      }

      /// <summary>
      /// The dictionary of type values
      /// </summary>
      static SortedDictionary<byte, Type> all 
        = new SortedDictionary<byte, Type>();

      /// <summary>
      /// Get a block by it's type id
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public static Type Get(byte id) {
        return all[id];
      }

      /// <summary>
      /// Add a type to the list of all types
      /// </summary>
      /// <param name="type"></param>
      internal static void Add(Type type) {
        if (all.ContainsKey(type.Id)) {
          throw new System.Exception("Attempted to register a new biome type with an existing type's Id");
        } else {
          all.Add(type.Id, type);
        }
      }
    }
  }
}
