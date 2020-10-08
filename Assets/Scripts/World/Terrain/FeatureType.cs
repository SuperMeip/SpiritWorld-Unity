using SpiritWorld.Inventories;
using System;
using System.Collections.Generic;

namespace SpiritWorld.World.Terrain.Features {
 
  public partial struct TileFeature {

    /// <summary>
    /// Singleton class for tile feature type data
    /// </summary>
    public abstract class Type {

      /// <summary>
      /// Constant for unlimited interacounts count
      /// </summary>
      public const int UnlimitedInteractions = -1;

      /// <summary>
      /// The id of this feature type
      /// </summary>
      public byte Id {
        get;
      }

      /// <summary>
      /// The name of the tile feature
      /// </summary>
      public string Name {
        get;
      }

      /// <summary>
      /// The layer that this feature type sits on
      /// </summary>
      public Layer Layer {
        get;
      }

      /// <summary>
      /// If this is an interactive/cliclable/usable feature
      /// </summary>
      public bool IsInteractive {
        get;
      }

      /// <summary>
      /// The way this tile type is rotated on placement
      /// </summary>
      public RotationType PlacementRotationType {
        get;
        protected set;
      } = RotationType.Random;

      /// <summary>
      /// How many modes/models this reouces has.
      /// </summary>
      public int NumberOfModes {
        get;
        protected set;
      } = 1;

      /// <summary>
      /// The drops, indexed by the mode they'll drop for.
      /// If there's only one item in the list we'll use that for all modes.
      /// If it's null there's no drops
      /// </summary>
      public DropChanceCollection[] DropsPerMode {
        get;
      } = null;

      /// <summary>
      /// For making new types
      /// </summary>
      /// <param name="id"></param>
      protected Type(
        byte id,
        string name,
        Layer layer,
        bool isInteractive = false,
        DropChanceCollection[] drops = null
      ) {
        Id = id;
        Name = name;
        Layer = layer;
        IsInteractive = isInteractive;
        DropsPerMode = drops;

        // on creation, add the singleton to the all types list.
        Types.Add(this);
      }

      /// <summary>
      /// Get a random drop inventory for this type of tile when it's used up. Null if it has none
      /// </summary>
      internal Inventory getRandomDrop(int mode = 0) {
        return DropsPerMode?[Math.Min(mode, DropsPerMode.Length - 1)].getRandomDrop();
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
          throw new System.Exception("Attempted to register a new Feature type with an existing type's Id");
        } else {
          all.Add(type.Id, type);
        }
      }
    }
  }
}