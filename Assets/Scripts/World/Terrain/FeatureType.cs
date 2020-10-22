using Newtonsoft.Json.Linq;
using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using System;
using System.Collections.Generic;

namespace SpiritWorld.World.Terrain.Features {
 
  public partial struct TileFeature {

    /// <summary>
    /// Singleton class for tile feature type data
    /// </summary>
    public abstract class Type : IEquatable<Type> {

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
      /// The time it takes to use up one unit of this feature
      /// TODO: this should be on the base
      /// </summary>
      public float TimeToUse {
        get;
      } = 2.0f;

      /// <summary>
      /// How many times this resource can be interacted with before it's used up
      /// </summary>
      public int NumberOfUses {
        get;
      } = UnlimitedInteractions;

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
      /// Drops for this tile type if there are any.
      /// Indexed by Moded first, then by tool type, then by the level of the tool.
      /// </summary>
      public SortedDictionary<Tool.Type, DropChanceCollection[]>[] Drops {
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
        bool isInteractive = false,
        int numberOfUses = UnlimitedInteractions,
        float useTime = 2.0f
      ) {
        Id = id;
        Name = name;
        Layer = layer;
        IsInteractive = isInteractive;
        NumberOfModes = numberOfUses + 1;
        TimeToUse = useTime;
        NumberOfUses = numberOfUses;

        // on creation, add the singleton to the all types list.
        Types.Add(this);
      }

      /// <summary>
      /// Try to 'use'/interact with this tile
      /// </summary>
      /// <returns></returns>
      public bool TryToUseOnce(float totalTimeInteractedWithForSoFar) {
        return totalTimeInteractedWithForSoFar >= TimeToUse;
      }

      /// <summary>
      /// equality
      /// </summary>
      /// <param name="other"></param>
      /// <returns></returns>
      public bool Equals(Type other) {
        return Id == other.Id;
      }

      /// <summary>
      /// Check if the given tool can break this feature at the given mode
      /// </summary>
      public bool CanBeMinedBy(ITool tool, int mode = 0) {
        var modeData = getModeSpecificData(mode);
        return modeData.ContainsKey(Tool.Type.Any) || modeData.ContainsKey(tool.ToolType);
      }

      /// <summary>
      /// Check if the given tool can break this feature at the given mode
      /// </summary>
      public Tool.Requirement[] GetToolRequirements(int mode = 0) {
        SortedDictionary<Tool.Type, DropChanceCollection[]> modeData = getModeSpecificData(mode);
        Tool.Requirement[] toolRequirements = new Tool.Requirement[modeData.Keys.Count];
        int toolTypeIndex = 0;
        // foreach tool, find the minimum mode we get drops back for
        foreach(KeyValuePair<Tool.Type, DropChanceCollection[]> keyValyePair in modeData) {
          (Tool.Type toolType, DropChanceCollection[] dropChanceCollectionsByToolLevel) = keyValyePair;
          int minimumToolLevelNeeded = 0;
          foreach(DropChanceCollection dropChanceCollection in dropChanceCollectionsByToolLevel) {
            if (dropChanceCollection != null) {
              break;
            }
          }

          toolRequirements[toolTypeIndex++] = new Tool.Requirement(toolType, minimumToolLevelNeeded);
        }

        return toolRequirements;
      }

      /// <summary>
      /// Get a random drop inventory for this type of tile when it's used up. Null if it has none
      /// </summary>
      public IInventory GetRandomDrops(ITool toolUsed, int mode = 0) {
        SortedDictionary<Tool.Type, DropChanceCollection[]> modeData = getModeSpecificData(mode);
        DropChanceCollection potentialDrops = null;
        // first try to get the drop collection specific to the tool being used if there is one
        if (modeData.TryGetValue(toolUsed.ToolType, out DropChanceCollection[] dropsByToolLevel)) {
          // if this mode's drops are empty, return null
          if (dropsByToolLevel.Length == 0) {
            return null;
          }
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
      /// Import the drops from a json
      /// </summary>
      /// <param name="dropJSON"></param>
      /// <returns></returns>
      protected static SortedDictionary<Tool.Type, DropChanceCollection[]>[] ParseDropDataJSON(string dropJSON) {
        List<SortedDictionary<Tool.Type, DropChanceCollection[]>> dropDataByMode = new List<SortedDictionary<Tool.Type, DropChanceCollection[]>>();
        JArray dropDataByModeJSON = JArray.Parse(dropJSON);
        foreach (JArray modeDropData in dropDataByModeJSON.Children<JArray>()) {
          SortedDictionary<Tool.Type, DropChanceCollection[]> modeData = new SortedDictionary<Tool.Type, DropChanceCollection[]>();
          dropDataByMode.Add(modeData);
          foreach(JObject toolDropData in modeDropData.Children<JObject>()) {
            List<DropChanceCollection> dropChanceCollectionsByToolLevel = new List<DropChanceCollection>();
            foreach(JArray dropPosibilitiesByToolLevel in toolDropData["DropPossibilities"].Values<JArray>()) {
              DropChanceCollection dropChanceCollection = new DropChanceCollection();
              dropChanceCollectionsByToolLevel.Add(dropChanceCollection);
              foreach (JObject dropPosibility in dropPosibilitiesByToolLevel.Values<JObject>()) {
                dropChanceCollection.add(
                  dropPosibility["Weight"].Value<int>(),
                  new BasicInventory(dropPosibility["Items"].Value<string>())
                );
              }
            }

            modeData.Add(
              (Tool.Type)Enum.Parse(typeof(Tool.Type),toolDropData["Tool"].Value<string>()),
              dropChanceCollectionsByToolLevel.ToArray()
            );
          }
        }

        return dropDataByMode.ToArray();
      }

      /// <summary>
      /// Helper func to get data for a mode's drops and tools
      /// </summary>
      /// <param name="mode"></param>
      /// <returns></returns>
      SortedDictionary<Tool.Type, DropChanceCollection[]> getModeSpecificData(int mode) {
        if (Drops == null) {
          return null;
        }

        return mode >= Drops.Length
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
      /// TODO: investigate why this is called twice
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