using SpiritWorld.Inventories.Items;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorld.Inventories {

  /// <summary>
  /// A linear bar of items, capable of expanding slots like folders
  /// </summary>
  public class ItemBar : GridBasedInventory {
    /// <summary>
    /// The current size of the bar / # of active bar slots. set by the player usually
    /// </summary>
    public int activeBarSlotCount {
      get;
      private set;
    }

    /// <summary>
    /// Count the in use stacks
    /// </summary>
    public int usedBarSlotCount
      => stackSlotGrid.Count(barSlot => barSlot[0] != EmptyGridSlot);

    /// <summary>
    /// how many slots are in the item bar
    /// </summary>
    public int barSize
      => dimensions.x;

    /// <summary>
    /// How many available pocket slots there are
    /// </summary>
    public int pocketSlotCount
      => barSize - activeBarSlotCount;

    /// <summary>
    /// How many expandable slots can one bar slot hold
    /// </summary>
    int maxSlotDepth {
      get;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="size"></param>
    public ItemBar(int slotCount, int maxSlotDepth = 1, Item[] items = null) : base(slotCount) {
      this.maxSlotDepth = maxSlotDepth;
      activeBarSlotCount = barSize / 2;
      if (items != null) {
        int slotIndex = 0;
        foreach(Item item in items) {
          tryToAdd(item, (slotIndex++, 0), out _);
        }
      }
    }

    /// <summary>
    /// Check the item at the given index of the item bar without changing it
    /// NOT USED TO REMOVE
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Item getItemAt(int index) {
      return getItemStackAt((index, 0));
    }

    /// <summary>
    /// try to add or remove an available slot from the bar, and add it to the pocket size
    /// </summary>
    /// <returns>the items moved to the pockets or bar depending on size change direction</returns>
    public Item[] incrementBarSize(bool increase = true) {
      int slotCountModifier = increase ? 1 : -1;
      int originalBarSize = activeBarSlotCount;
      activeBarSlotCount += slotCountModifier;

      // if we're adding slots to the pockets
      if (slotCountModifier < 0 && activeBarSlotCount > 1) {
        List<Item> itemsMovedToPockets = new List<Item>();
        for (int index = originalBarSize - 1; index > activeBarSlotCount; index--) {
          Item currentItem = getItemStackAt((index, 0));
          if (currentItem != null) {
            itemsMovedToPockets.Add(currentItem);
          }
        }

        return itemsMovedToPockets.ToArray();
      // if we're adding slots to the bar
      } else if (slotCountModifier > 0 && activeBarSlotCount < barSize) {
        List<Item> itemsMovedToBar = new List<Item>();
        for (int index = originalBarSize - 1; index < activeBarSlotCount; index++) {
          Item currentItem = getItemStackAt((index, 0));
          if (currentItem != null) {
            itemsMovedToBar.Add(currentItem);
          }
        }

        return itemsMovedToBar.ToArray();
      // if we can't go down or up anymore, reset and return
      } else {
        activeBarSlotCount = originalBarSize;
      }

      return null;
    }

    /// <summary>
    /// Get if the location is in the pockets
    /// </summary>
    /// <param name="gridLocation"></param>
    /// <returns></returns>
    public bool isInPockets(Coordinate gridLocation) {
      return gridLocation.x >= activeBarSlotCount;
    }

    /// <summary>
    /// Try to put an item in the first free pocket slot
    /// </summary>
    /// <returns></returns>
    public Item tryToAddToPockets(Item item, out Item successfullyAddedItem, out Coordinate[] modifiedStackPivots) {
      return tryToAdd(item, out successfullyAddedItem, out modifiedStackPivots, true);
    }

    /// <summary>
    /// Try to add an item to the bar at the given slot.
    /// TODO: doesn't work for pockets, add functionality
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <param name="barAndDeepSlot">bar slot, and depth slot to add it to (0 base)</param>
    /// <param name="successfullyAddedItem">The item that was added successfully if there was one</param>
    /// <returns>Any items replaced or removed from the slot in order to place the new one</returns>
    public override Item tryToAdd(Item item, Coordinate barAndDeepSlot, out Item successfullyAddedItem) {
      (int barSlot, int deepSlot) = barAndDeepSlot;

      /// if it's within the current bar
      if (barSlot < activeBarSlotCount) {
        // if it's the main bar slot or we've expanded this far already into the deep spots, just swap out the item.
        if (deepSlot == 0 || stackSlotGrid[barSlot].Length > deepSlot) {
          successfullyAddedItem = tryToSwapOut(barAndDeepSlot, item, out Item oldItem) ? item : null;
          return oldItem;
        // if we need to expand by one
        } else if (deepSlot < maxSlotDepth) {
          addStack(item, (barSlot, deepSlot));
          successfullyAddedItem = item;
          return null;
        }
      }

      /// failed to add the item, return it as leftover
      successfullyAddedItem = null;
      return item;
    }

    /// <summary>
    /// try to add an item stack
    /// </summary>
    /// <param name="item"></param>
    /// <param name="successfullyAddedItems"></param>
    /// <param name="modifiedStackPivots"></param>
    /// <returns>leftovers, null, or the un-added item</returns>
    public override Item tryToAdd(Item item, out Item successfullyAddedItems, out Coordinate[] modifiedStackPivots) {
      return tryToAdd(item, out successfullyAddedItems, out modifiedStackPivots);
    }

    /// <summary>
    /// try to add an item stack, return false if it doesn't fit.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="successfullyAddedItems"></param>
    /// <returns>leftovers or null</returns>
    Item tryToAdd(Item item, out Item successfullyAddedItems, out Coordinate[] modifiedStackPivots, bool addToPocketSlot = false) {
      int barSlotIndex = 0;
      int firstEmptySlot = EmptyGridSlot;
      Item itemsLeftToAdd = item;
      List<Coordinate> modifiedPivots = new List<Coordinate>();
      successfullyAddedItems = null;
      foreach (int[] barSlotStack in stackSlotGrid) {
        /// stop if we've run out of active bar
        if (!addToPocketSlot && barSlotIndex >= activeBarSlotCount) {
          break;
        }
        // if this slot is empty, mark it for if we dont' find an existing stack
        if (barSlotStack[0] == EmptyGridSlot) {
          if (firstEmptySlot == EmptyGridSlot
            // if we're not looking for a pocket slot, or we are looking for a pocket slot and we're passed the active bar slots
            && (!addToPocketSlot || (addToPocketSlot && barSlotIndex >= activeBarSlotCount))
          ) {
            firstEmptySlot = barSlotIndex;
          }
          // if the slot is full but matches the item
        } else if (!stacks[barSlotStack[0]].isFull && stacks[barSlotStack[0]].canStackWith(itemsLeftToAdd)) {
          Coordinate slotLocation = (barSlotIndex, 0);
          modifiedPivots.Add(slotLocation);
          itemsLeftToAdd = addToStack(itemsLeftToAdd, slotLocation, out Item itemsAdded);
          if (itemsAdded != null) {
            successfullyAddedItems = successfullyAddedItems == null
              ? itemsAdded
              : successfullyAddedItems.addToStack(itemsAdded, out _);
          }
        }

        // stop when we run out
        if (itemsLeftToAdd == null) {
          break;
        }

        barSlotIndex++;
      }

      // if we have leftovers and found an empty slot stick it there
      if (firstEmptySlot != EmptyGridSlot) {
        Coordinate slotLocation = (firstEmptySlot, 0);
        modifiedPivots.Add(slotLocation);
        addStack(itemsLeftToAdd, slotLocation);
        successfullyAddedItems = item;

        itemsLeftToAdd = null;
      }

      modifiedStackPivots = modifiedPivots.ToArray();
      return itemsLeftToAdd;
    }

    /// <summary>
    /// Remove the item from the inventory at
    /// </summary>
    /// <param name="itemLocation"></param>
    /// <returns></returns>
    public override Item[] removeAt(Coordinate itemLocation) {
      throw new System.NotImplementedException();
    }
  }
}