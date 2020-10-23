using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorld.Inventories.Items {
  public partial class Item {

    /// <summary>
    /// The class pattern for a item type
    /// </summary>
    [System.Serializable]
    public abstract class Type : IEquatable<Type> {

      /// <summary>
      /// The shape blocks used to make up the item shape
      /// </summary>
      public enum ShapeBlocks {
        Empty,
        Pivot,
        Solid
      }

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
      /// The shape of this item in a shaped inventory,
      /// determined by a 3D grid with 0,0 being bottom left
      /// O is used to mark the center/pivot of the shaped icon, X is used for other parts. All other chars are ignored.
      /// </summary>
      public ShapeBlocks[,] Shape {
        get;
        protected set;
      } = new ShapeBlocks[,] {
        {ShapeBlocks.Pivot} 
      };

      /// <summary>
      /// Get the center of the shape
      /// </summary>
      public Coordinate ShapePivot {
        get {
          for (int x = 0; x < Shape.GetLength(0); x++) {
            for (int y = 0; y < Shape.GetLength(1); y++) {
              if (Shape[x,y] == ShapeBlocks.Pivot) {
                return (x, y);
              }
            }
          }

          return (0, 0);
        }
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

      /// <summary>
      /// eqwuality
      /// </summary>
      /// <param name="other"></param>
      /// <returns></returns>
      public bool Equals(Type other) {
        return Id == other.Id;
      }

      /// <summary>
      /// hash code
      /// </summary>
      /// <returns></returns>
      public override int GetHashCode() {
        return Id;
      }

      /// <summary>
      /// Item basic deets
      /// </summary>
      /// <returns></returns>
      public override string ToString() {
        return $"I.t[{Name}]";
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
