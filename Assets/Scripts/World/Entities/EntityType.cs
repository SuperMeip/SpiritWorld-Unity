using System.Collections.Generic;

namespace SpiritWorld.World.Entities {
  /// <summary>
  /// Extension for entity types
  /// </summary>
  public partial class Entity {

    /// <summary>
    /// Used for unique entity IDs
    /// </summary>
    public abstract class Type {

      /// <summary>
      /// The unique id of this entity
      /// </summary>
      public int Id {
        get;
        private set;
      }

      /// <summary>
      /// The name of this type of entity
      /// </summary>
      public string Name {
        get;
        private set;
      }

      /// <summary>
      /// Base constructor
      /// </summary>
      /// <param name="id"></param>
      /// <param name="name"></param>
      protected Type(int id, string name) {
        Id = id;
        Name = name;

        // add to the singleton constants
        Types.Add(this);
      }
    }

    /// <summary>
    /// Entity type singleton constants
    /// </summary>
    public static partial class Types {

      /// <summary>
      /// All registered types as an ordered array
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
      static SortedDictionary<int, Type> all 
        = new SortedDictionary<int, Type>();

      /// <summary>
      /// Get a type by it's id
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public static Type Get(int id) {
        return all[id];
      }

      /// <summary>
      /// Add a type to the list of all types
      /// </summary>
      /// <param name="type"></param>
      internal static void Add(Type type) {
        if (all.ContainsKey(type.Id)) {
          throw new System.Exception("Attempted to register a new Entity type with an existing type's Id");
        } else {
          all.Add(type.Id, type);
        }
      }
    }
  }
}