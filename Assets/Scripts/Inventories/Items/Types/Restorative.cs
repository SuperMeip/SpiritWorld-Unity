using SpiritWorld.Stats;
using System.Collections.Generic;

namespace SpiritWorld.Inventories.Items {

  /// <summary>
  /// A consumable that restores a depletable stat of some kind
  /// </summary>
  public partial class Restorative : UseableItem {
    public Restorative(Type type, byte quantity = 1, int? usesRemaining = null) 
      : base(type, quantity, usesRemaining) { }

    /// <summary>
    /// Restoritive type const
    /// </summary>
    public new abstract class Type : UseableItem.Type {

      /// <summary>
      /// What stat is being restored.
      /// </summary>
      public Stat.Type StatToRestore {
        get;
      }

      /// <summary>
      /// How many points it restores
      /// </summary>
      public int RestoritivePower {
        get;
      }

      /// <summary>
      /// The time it takes to restore the value, 0 is instant.
      /// </summary>
      public float RestoreTime {
        get;
      }

      /// <summary>
      /// For making new types
      /// </summary>
      /// <param name="id"></param>
      protected Type(
        byte id,
        string name,
        Stat.Type statToRestore,
        int restoritivePower = 1,
        short numberOfUses = 1,
        float restoreTime = 1.0f
      ) : base(id, name, numberOfUses) {
        StatToRestore = statToRestore;
        RestoritivePower = restoritivePower;
        RestoreTime = restoreTime;

        // on creation, add the singleton to the all types list.
        Types.Add(this);
      }
    }

    /// <summary>
    /// Tile type singleton constants
    /// </summary>
    public new static partial class Types {

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
      public static Type Get(byte id) {
        return all[id];
      }

      /// <summary>
      /// Add a type to the list of all types
      /// </summary>
      /// <param name="type"></param>
      internal static void Add(Type type) {
        if (all.ContainsKey(type.Id)) {
          throw new System.Exception("Attempted to register a new Restoritive item type with an existing type's Id");
        } else {
          all.Add(type.Id, type);
        }
      }
    }
  }
}
