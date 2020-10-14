using SpiritWorld.Inventories.Items;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorld.Inventories {

  /// <summary>
  /// basic inventory class for storing basic items in single stacks.
  /// for stuff like drops
  /// </summary>
  public class BasicInventory : Dictionary<Item.Type, Item>, IInventory {

    /// <summary>
    /// Return the first available stack of the given item
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public new Item this[Item.Type type] {
      get => TryGetValue(type, out Item item) ? item : null;
      private set => this[type] = value;
    }

    /// <summary>
    /// Generic add an item to the inventory
    /// </summary>
    /// <param name="item"></param>
    public virtual Item tryToAdd(Item item) {
      Item existingItem = this[item.type];
      if (existingItem != null) {
        return existingItem.addToStack(item);
      } else {
        this[item.type] = item;
        return null;
      }
    }

    /// <summary>
    /// Remove an item.
    /// Default removes all of the item.
    /// </summary>
    /// <return>returns the removed item stack or null if no items were removed</return>>
    public virtual Item[] remove(Item item, int? quantity = null) {
      Item existingItem = this[item.type];
      // if the item exists, remove and return what we want from the stack
      if (quantity != null && quantity > 0) {
        if (existingItem != null) {
          Item removedItems = existingItem.removeFromStack((byte)quantity);
          // remove the item if there's none left
          if (existingItem.quantity == 0) {
            Remove(item.type);
          }
          // return the removed items if there are any
          if (removedItems.quantity != 0) {
            return new Item[] { removedItems };
          }
          return null;
        // else we removed nothing, so return null
        } else return null;
      // if it's -1 or 0 return the whole stack
      } else if (quantity == null || quantity != 0) {
        var itemsRemoved = existingItem;
        Remove(item.type);

        return new Item[] { itemsRemoved };
      // if we're not returning anything return nothing
      } else {
        return null;
      }
    }

    /// <summary>
    /// Empty it
    /// </summary>
    /// <returns></returns>
    public Item[] empty() {
      return Values.ToArray();
    }
  }
}
