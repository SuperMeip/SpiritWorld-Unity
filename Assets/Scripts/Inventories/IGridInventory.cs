using SpiritWorld.Inventories.Items;

namespace SpiritWorld.Inventories {
  public interface IGridInventory : IInventory {

    /// <summary>
    /// Add items to the inventory at the given grid location
    /// </summary>
    /// <param name="successfullyAddedItems">Returns succesfully added item stack for reporting</param>
    /// <returns>leftovers that don't fit</returns>
    Item tryToAdd(Item item, Coordinate location, out Item successfullyAddedItem);

    /// <summary>
    /// Add items to the inventory at the given grid location
    /// </summary>
    /// <param name="successfullyAddedItems">Returns succesfully added item stack for reporting</param>
    /// <param name="modifiedStackPivots">Returns which stacks were modified via the pivot value</param>
    /// <returns>leftovers that don't fit</returns>
    Item tryToAdd(Item item, out Item successfullyAddedItem, out Coordinate[] modifiedStackPivots);

    /// <summary>
    /// remove the stack at the given grid location from the inventory
    /// </summary>
    /// <returns>removed items</returns>
    Item[] removeAt(Coordinate itemLocation);
  }
}