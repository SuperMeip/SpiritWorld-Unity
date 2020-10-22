using SpiritWorld.Inventories.Items;
using System;

namespace SpiritWorld.Inventories {
  public interface IInventory {

    /// <summary>
    /// Add items to the inventory
    /// </summary>
    /// <param name="successfullyAddedItems">Returns succesfully added item stack for reporting</param>
    /// <returns>leftovers that don't fit</returns>
    Item tryToAdd(Item item, out Item successfullyAddedItem);

    /// <summary>
    /// remove items from the inventory
    /// </summary>
    /// <returns>removed items</returns>
    Item[] remove(Item item, int? quantity = null);

    /// <summary>
    /// empty all the items from the inventory
    /// </summary>
    /// <returns>the empties items</returns>
    Item[] empty();

    /// <summary>
    /// Empty this inventory into another one
    /// </summary>
    /// <param name="successfullyAddedItems">Returns succesfully added items for reporting</param>
    /// <returns>leftovers that don't fit</returns>
    Item[] emptyInto(IInventory otherInventory, out Item[] successfullyAddedItems);

    /// <summary>
    /// Copy the inventory into a new object
    /// </summary>
    /// <returns></returns>
    IInventory copy();

    /// <summary>
    /// Get the matching items from this inventory
    /// </summary>
    /// <returns>all items where the query is true</returns>
    Item[] search(Func<Item, bool> query);
  }
}
