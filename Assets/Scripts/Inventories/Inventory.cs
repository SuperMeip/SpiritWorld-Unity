using SpiritWorld.Inventories.Items;
using System.Collections.Generic;

namespace SpiritWorld.Inventories {

  /// <summary>
  /// basic inventory class for storing items
  /// </summary>
  public class Inventory : Dictionary<Item.Type, Item> {

    /// <summary>
    /// override for nulls
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public new Item this[Item.Type type]
      => TryGetValue(type, out Item item) ? item : null;
  }
}
