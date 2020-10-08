using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
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
      /// Drops for this tile type if there are any.
      /// Indexed by Moded first, then by tool type, then by the level of the tool.
      /// </summary>
      public Dictionary<Tool.Type, DropChanceCollection[]>[] Drops {
        get;
        protected set;
      } = null;

      /// <summary>
      /// For making new types
      /// </summary>
      /// <param name="id"></param>
      protected Type(
        byte id,
        string name,
        Layer layer,
        bool isInteractive = false
      ) {
        Id = id;
        Name = name;
        Layer = layer;
        IsInteractive = isInteractive;

        // on creation, add the singleton to the all types list.
        Types.Add(this);
      }

      /// <summary>
      /// Check if the given tool can break this feature at the given mode
      /// </summary>
      internal bool canBeMinedBy(ITool tool, int mode = 0) {
        var modeData = getModeSpecificData(mode);
        return modeData.ContainsKey(Tool.Type.Any) || modeData.ContainsKey(tool.ToolType);
      }

      /// <summary>
      /// Get a random drop inventory for this type of tile when it's used up. Null if it has none
      /// </summary>
      internal Inventory getRandomDrop(ITool toolUsed, int mode = 0) {
        Dictionary<Tool.Type, DropChanceCollection[]> modeData = getModeSpecificData(mode);
        DropChanceCollection potentialDrops = null;
        // first try to get the drop collection specific to the tool being used if there is one
        if (modeData.TryGetValue(toolUsed.ToolType, out DropChanceCollection[] dropsByToolLevel)) {
          potentialDrops = dropsByToolLevel.Length <= toolUsed.UpgradeLevel
            ? dropsByToolLevel[0]
            : dropsByToolLevel[toolUsed.UpgradeLevel];
        // if there isn't a specific entry for the tool type, try to find the entry for Any tool
        } else {
          if (modeData.TryGetValue(Tool.Type.Any, out DropChanceCollection[] genericDropsByToolLevel)) {
            potentialDrops = genericDropsByToolLevel.Length <= toolUsed.UpgradeLevel
              ? genericDropsByToolLevel[0]
              : genericDropsByToolLevel[toolUsed.UpgradeLevel];
          }
        }

        return potentialDrops?.getRandomDrop();
      }

      /// <summary>
      /// Helper func to get data for a mode's drops and tools
      /// </summary>
      /// <param name="mode"></param>
      /// <returns></returns>
      Dictionary<Tool.Type, DropChanceCollection[]> getModeSpecificData(int mode) {
        if (Drops == null) {
          return null;
        }
        return mode > Drops.Length
          ? Drops[0]
          : Drops[mode];
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