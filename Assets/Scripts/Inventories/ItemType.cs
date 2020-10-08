using System.Collections.Generic;

namespace SpiritWorld.Inventories.Items {
  public partial class Item {

    /// <summary>
    /// The class pattern for a item type
    /// </summary>
    public abstract class Type {

      /// <summary>
      /// The id of this tile type
      /// </summary>
      public short Id {
        get;
      }

      /// <summary>
      /// Name of the tile type
      /// </summary>
      public string Name {
        get;
      }

      /// <summary>
      /// How many of this item can you hold in one slot
      /// </summary>
      public byte StackSize {
        get;
      }

      /// <summary>
      /// For making new types
      /// </summary>
      /// <param name="id"></param>
      protected Type(short id, string name, byte stackSize = 100) {
        Id = id;
        Name = name;
        StackSize = stackSize;

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
          throw new System.Exception("Attempted to register a new item type with an existing type's Id");
        } else {
          all.Add(type.Id, type);
        }
      }
    }
  }
}
