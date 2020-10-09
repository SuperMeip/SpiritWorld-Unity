using System.Collections.Generic;

namespace SpiritWorld.World.Terrain.Constructs {
  public partial struct TileStruct {

    /// <summary>
    /// What types of placement slots are allowed for this construct
    /// </summary>
    public enum PlacementSlotTypes {
      Center, // can be placed in the center of the tile
      Inner,  // can be placed between the center and an edge or vert 
      Border  // can be placed along the edges(center of edge) or a vert
    }

    public abstract class Type {

      /// <summary>
      /// The id of this feature type
      /// </summary>
      public short Id {
        get;
      }

      /// <summary>
      /// The id of this feature type
      /// </summary>
      public string Name {
        get;
      }

      /// <summary>
      /// By default, we allow all 3, but any type can override this
      /// </summary>
      public PlacementSlotTypes[] placementSlotTypes {
        get;
        protected set;
      } = new PlacementSlotTypes[] {
        PlacementSlotTypes.Center,
        PlacementSlotTypes.Inner,
        PlacementSlotTypes.Border
      };

      /// <summary>
      /// For making new types
      /// </summary>
      /// <param name="id"></param>
      protected Type(short id, string name) {
        Id = id;
        Name = name;

        // on creation, add the singleton to the all types list.
        Types.Add(this);
      }
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
      static SortedDictionary<short, Type> all
        = new SortedDictionary<short, Type>();

      /// <summary>
      /// Get a block by it's type id
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public static Type Get(short id) {
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