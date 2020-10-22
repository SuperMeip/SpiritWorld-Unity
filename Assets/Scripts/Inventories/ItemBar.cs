using SpiritWorld.Inventories.Items;
using System;
using System.Linq;

namespace SpiritWorld.Inventories {

  /// <summary>
  /// A linear bar of items, capable of expanding slots like folders
  /// </summary>
  public class ItemBar : GridBasedInventory {

    /// <summary>
    /// Count the in use stacks
    /// </summary>
    public int usedBarSlotCount
      => stackSlotGrid.Count(barSlot => barSlot[0] != EmptyGridSlot);

    /// <summary>
    /// How many expandable slots can one bar slot hold
    /// </summary>
    int maxSlotDepth {
      get;
    }

    /// <summary>
    /// how many slots are in the item bar
    /// </summary>
    public int barSize
      => dimensions.x;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="size"></param>
    public ItemBar(int slotCount, int maxSlotDepth = 1, Item[] items = null) : base(slotCount) {
      this.maxSlotDepth = maxSlotDepth;
      if (items != null) {
        int slotIndex = 0;
        foreach(Item item in items) {
          tryToAdd(item, (slotIndex++, 0), out _);
        }
      }
    }

    /// <summary>
    /// Check the item at the given index of the item bar without changing it
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Item getItemAt(int index) {
      return getItemStackAt((index, 0));
    }

    /// <summary>
    /// Try to add an item to the bar at the given slot
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <param name="barAndDeepSlot">bar slot, and depth slot to add it to (0 base)</param>
    /// <param name="successfullyAddedItem">The item that was added successfully if there was one</param>
    /// <returns>Any items replaced or removed from the slot in order to place the new one</returns>
    public override Item tryToAdd(Item item, Coordinate barAndDeepSlot, out Item successfullyAddedItem) {
      (int barSlot, int deepSlot) = barAndDeepSlot;
      /// if it's within the bar
      if (barSlot < barSize) {
        // if it's the main bar slot or we've expanded this far already into the deep spots, just swap out the item.
        if (deepSlot == 0 || stackSlotGrid[barSlot].Length > deepSlot) {
          successfullyAddedItem = tryToSwapOut(barAndDeepSlot, item, out Item oldItem) ? item : null;
          return oldItem;
          // if we need to expand by one
        } else if (deepSlot < maxSlotDepth) {
          int itemStackId = addNewStack(item);
          addDeepSlot(itemStackId, barSlot);
          successfullyAddedItem = item;
          return null;
        }
      }

      /// failed to add the item, return it as leftover
      successfullyAddedItem = null;
      return item;
    }

    public override Item[] removeAt(Coordinate itemLocation) {
      throw new System.NotImplementedException();
    }

    /// <summary>
    /// add a new slot to an expandable list.
    /// </summary>
    /// <param name="item1"></param>
    /// <param name="item2"></param>
    void addDeepSlot(int itemStackId, int barSlot) {
      int[] oldItemBarDepthCollection = stackSlotGrid[barSlot];
      int[] newItemBarDepthCollection = new int[oldItemBarDepthCollection.Length + 1];
      stackSlotGrid[barSlot].CopyTo(newItemBarDepthCollection, 0);
      newItemBarDepthCollection[oldItemBarDepthCollection.Length] = itemStackId;
    }
  }
}