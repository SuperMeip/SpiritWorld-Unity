using SpiritWorld.Inventories.Items;
using System.Collections.Generic;
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
    /// how many slots are in the item bar
    /// </summary>
    public int barSize
      => dimensions.x;

    /// <summary>
    /// How many expandable slots can one bar slot hold
    /// </summary>
    int maxSlotDepth {
      get;
    }

    /// <summary>
    /// The current size of the bar / # of active bar slots. set by the player usually
    /// </summary>
    public int activeBarSlotCount {
      get;
      private set;
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
    /// try to add or remove an available slot from the bar
    /// </summary>
    /// <param name="slotCountModifier"></param>
    /// <returns></returns>
    public Item[] changeBarSize(int slotCountModifier = 1) {
      if (slotCountModifier < 0 && activeBarSlotCount > 0) {
        activeBarSlotCount--;
        return removeAt((activeBarSlotCount, 0));
      } else if (slotCountModifier > 0 && activeBarSlotCount < barSize) {
        activeBarSlotCount++;
      }

      return null;
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
    /// try to add an item stack, return false if it doesn't fit.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="successfullyAddedItems"></param>
    /// <returns>leftovers or null</returns>
    public override Item tryToAdd(Item item, out Item successfullyAddedItems, out Coordinate[] modifiedStackPivots) {
      int barSlotIndex = 0;
      int firstEmptyBarSlotIndex = EmptyGridSlot;
      Item itemsLeftToAdd = item;
      List<Coordinate> modifiedPivots = new List<Coordinate>();
      successfullyAddedItems = null;
      foreach(int[] barSlotStack in stackSlotGrid) {
        /// stop if we've run out of active bar
        if (barSlotIndex >= activeBarSlotCount) {
          break;
        }
        // if this slot is empty, mark it for if we dont' find an existing stack
        if (barSlotStack[0] == EmptyGridSlot) {
          if (firstEmptyBarSlotIndex == EmptyGridSlot) {
            firstEmptyBarSlotIndex = barSlotIndex;
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
      if (firstEmptyBarSlotIndex != EmptyGridSlot) {
        Coordinate slotLocation = (firstEmptyBarSlotIndex, 0);
        modifiedPivots.Add(slotLocation);
        addStack(itemsLeftToAdd, slotLocation);
        successfullyAddedItems = item;

        itemsLeftToAdd = null;
      }

      modifiedStackPivots = modifiedPivots.ToArray();
      return itemsLeftToAdd;
    }

    public override Item[] removeAt(Coordinate itemLocation) {
      throw new System.NotImplementedException();
    }
  }
}