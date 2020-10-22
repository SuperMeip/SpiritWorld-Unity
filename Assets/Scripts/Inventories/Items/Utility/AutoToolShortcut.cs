using SpiritWorld.World.Terrain.Features;
using SpiritWorld.World.Terrain.TileGrid;
using System.Linq;

namespace SpiritWorld.Inventories.Items {

  public partial class Item {

    public static partial class Types {
      /// <summary>
      /// Shortcut placeholder for the item bar that picks the best item to work on the selected tile
      /// </summary>
      public static Type AutoToolShortcut = new AutoToolShortcut();
    }

    class AutoToolShortcut : Type, IHotBarItemShortcut {
      internal AutoToolShortcut() : base(32323, "Auto Tool", 1) {}

      /// <summary>
      /// Try to get the best tool for the job, except for shovels on 0 remaining use features.
      /// </summary>
      public bool tryToFindMatchIn(IInventory[] inventories, (Tile tile, FeaturesByLayer features) selectedTileData, out Item match) {
        match = null;

        if (selectedTileData.features.TryGetValue(TileFeature.Layer.Resource, out TileFeature resource)) {
          Tool.Requirement[] validToolTypes = resource.type.GetToolRequirements(resource.mode);
          // for each tool type in order of importance, see if we have any tools that match minimum requirments
          //// if we don't for the best match, move on to the next best tool type
          foreach(Tool.Requirement minimumToolRequirement in validToolTypes) {
            // if any is first, or we've gotten down to any without finding a tool, return no tool found, we'll try to use an empty hand.
            if (minimumToolRequirement.ToolType == Tool.Type.Any) {
              return false;
            }

            // get all the items matching the tool min requirements in all inventories
            Item[] matchingTools = new Item[0];
            foreach (IInventory inventory in inventories) {
              matchingTools = matchingTools.Concat(inventory.search(item => item is ITool tool 
                && tool.ToolType == minimumToolRequirement.ToolType 
                && tool.UpgradeLevel >= minimumToolRequirement.MinimumLevel
              )).ToArray();
            }

            /// if we got matches, get the best tool type, else move on to the next tool type
            if (matchingTools.Length > 0) {
              match = matchingTools.Aggregate(
                (tool1, tool2) => (tool1 as ITool).UpgradeLevel > (tool2 as ITool).UpgradeLevel 
                    ? tool1 
                    : tool2
              );

              return true;
            }
          }
        }

        // never found any matching tools in inventories
        return false;
      }
    }
  }
}
