using SpiritWorld.World.Terrain.Features;
using SpiritWorld.World.Terrain.TileGrid;

namespace SpiritWorld.Inventories.Items {

  /// <summary>
  /// An item type that only exists in the item bar inventory to be used as a utility interface
  /// </summary>
  interface IHotBarItemShortcut {

    /// <summary>
    /// Try to find an item that matches this shortcut utility
    /// </summary>
    /// <param name="inventories"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    bool tryToFindMatchIn(IInventory[] inventories, (Tile tile, FeaturesByLayer features) selectedTileData, out Item match);
  }
}
