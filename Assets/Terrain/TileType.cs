using System.Collections.Generic;

/// <summary>
/// Extentions for tile type singletons
/// </summary>
public partial struct Tile {

  /// <summary>
  /// The class pattern for a tile type
  /// </summary>
  public abstract class Type {

    /// <summary>
    /// The id of this tile type
    /// </summary>
    public byte Id {
      get;
    }

    /// <summary>
    /// For making new types
    /// </summary>
    /// <param name="id"></param>
    protected Type(byte id) {
      Id = id;

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
    static SortedDictionary<byte, Type> all = new SortedDictionary<byte, Type>();

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
        throw new System.Exception("Attempted to register a new Tile type with an existing type's Id");
      } else {
        all.Add(type.Id, type);
      }
    }
  }
}
