﻿using SpiritWorld.Stats;
using System.Collections.Generic;

namespace SpiritWorld.Inventories.Items {

  /// <summary>
  /// An item that can be used to change the terrain and do thigns in the overworld.
  /// These will all show up in the hot-bar
  /// TODO: should this be an interface instead? What's different between tool and useable? 
  /// I think each resource should need to know the tool types that can mine it, and their level, maybe make those into the type?
  /// </summary>
  public partial class Weapon : UseableItem {

    /// <summary>
    /// Make a new usable item
    /// </summary>
    /// <param name="type"></param>
    /// <param name="quantity"></param>
    /// <param name="usesRemaining"></param>
    public Weapon(Type type, byte quantity = 1, int? usesRemaining = null) : base(type, quantity) {
      this.usesRemaining = usesRemaining ?? type.NumberOfUses;
    }

    /// <summary>
    /// The class pattern for a tool type
    /// </summary>
    public new abstract class Type : UseableItem.Type {

      /// <summary>
      /// The weapon's stats
      /// </summary>
      public abstract WeaponStats WeaponStats {
        get;
      }

      /// <summary>
      /// For making new types
      /// </summary>
      /// <param name="id"></param>
      protected Type(
        short id,
        string name,
        byte stackSize = 1
        // they always have unlimited uses, they can be used to initiate combat on the main board
      ) : base(id, name, UnlimitedUses, stackSize) {

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
      public static Type Get(short id) {
        return all[id];
      }

      /// <summary>
      /// Add a type to the list of all types
      /// </summary>
      /// <param name="type"></param>
      internal static void Add(Type type) {
        if (all.ContainsKey(type.Id)) {
          throw new System.Exception("Attempted to register a new useable item type with an existing type's Id");
        } else {
          all.Add(type.Id, type);
        }
      }
    }
  }
}