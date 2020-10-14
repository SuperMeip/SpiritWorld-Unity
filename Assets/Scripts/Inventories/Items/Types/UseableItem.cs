using System.Collections.Generic;

namespace SpiritWorld.Inventories.Items {

  /// <summary>
  /// An item that has a general 'use' feature
  /// </summary>
  public partial class UseableItem : Item {

    /// <summary>
    /// How many uses this item has left in it.
    /// One should only be used up on a successful use of an item
    /// </summary>
    public int usesRemaining {
      get;
      protected set;
    }

    /// <summary>
    /// Make a new usable item
    /// </summary>
    /// <param name="type"></param>
    /// <param name="quantity"></param>
    /// <param name="usesRemaining"></param>
    public UseableItem(Type type, byte quantity = 1, int? usesRemaining = null) : base(type, quantity) {
      this.usesRemaining = usesRemaining ?? type.NumberOfUses;
    }

    /// <summary>
    /// These can only stack if they have the same uses remaining
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public override bool canStackWith(Item item) {
      return base.canStackWith(item) && (item as UseableItem).usesRemaining == usesRemaining;
    }

    /// <summary>
    /// override for copy for this' extra data
    /// </summary>
    /// <param name="quantity"></param>
    /// <returns></returns>
    protected override Item copy(byte? quantity = null) {
      UseableItem newItem = new UseableItem(type as Type, quantity ?? this.quantity); ;
      newItem.usesRemaining = usesRemaining;

      return newItem;
    }

    /// <summary>
    /// The class pattern for a item type
    /// </summary>
    public new abstract class Type : Item.Type , IUseableItem {

      /// <summary>
      /// Const for unlimited use #
      /// </summary>
      public const short UnlimitedUses = 0;

      /// <summary>
      /// How many times this item can be used before being used up
      /// 0 is unlimited.
      /// </summary>
      public short NumberOfUses {
        get;
      } = UnlimitedUses;

      /// <summary>
      /// For making new types
      /// </summary>
      /// <param name="id"></param>
      protected Type(short id, string name, short numberOfUses = UnlimitedUses, byte stackSize = 100) 
        : base (id, name, stackSize) {
          NumberOfUses = numberOfUses;

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
          throw new System.Exception("Attempted to register a new useable item type with an existing type's Id");
        } else {
          all.Add(type.Id, type);
        }
      }
    }
  }
}