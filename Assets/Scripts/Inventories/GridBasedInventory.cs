﻿using SpiritWorld.Inventories.Items;
using System;
using System.Collections.Generic;

namespace SpiritWorld.Inventories {

  /// <summary>
  /// A player inventory represented by a square grid
  /// </summary>
  public abstract class GridBasedInventory : StackedInventory, IGridInventory {

    /// <summary>
    /// Value for empty grid slots
    /// </summary>
    public const int EmptyGridSlot = -1;

    /// <summary>
    /// The dimensions of this pack
    /// </summary>
    public (int x, int y) dimensions {
      get;
    }

    /// <summary>
    /// The grid used to hold item info.
    /// Each X,Y represents a tile in the grid, and each int is and 
    /// index in the inventory stacks list.
    /// </summary>
    protected readonly int[][] stackSlotGrid;

    /// <summary>
    /// Make a new grid pack of the given capacity
    /// </summary>
    /// <param name="dimensions"></param>
    public GridBasedInventory((int x, int y) dimensions) : base(dimensions.x * dimensions.y) {
      this.dimensions = dimensions;
      stackSlotGrid = new int[dimensions.x][];
      for (int x = 0; x < dimensions.x; x++) {
        stackSlotGrid[x] = new int[dimensions.y];
        stackSlotGrid[x].Populate(EmptyGridSlot);
      }
    }

    /// <summary>
    /// Make a new empty grid with one dimension (a bar or column)
    /// </summary>
    /// <param name="dimensions"></param>
    public GridBasedInventory(int x) : base(x) {
      dimensions = (x, 1);
      stackSlotGrid = new int[dimensions.x][];
      for (int i = 0; i < x; i++) {
        stackSlotGrid[i] = new int[] { EmptyGridSlot };
      }
    }

    /// <summary>
    /// Try to add an item to a specific place in this inventory
    /// </summary>
    /// <param name="item"></param>
    /// <param name="location"></param>
    /// <param name="successfullyAddedItem"></param>
    /// <returns></returns>
    public abstract Item tryToAdd(Item item, Coordinate location, out Item successfullyAddedItem);

    /// <summary>
    /// Try to add an item to a specific place in this inventory
    /// </summary>
    /// <param name="item"></param>
    /// <param name="location"></param>
    /// <param name="successfullyAddedItem"></param>
    /// <param name="modifiedStackPivots"></param>
    /// <returns></returns>
    public abstract Item tryToAdd(Item item, out Item successfullyAddedItem, out Coordinate[] modifiedStackPivots);

    /// <summary>
    /// override shortcut
    /// </summary>
    /// <param name="item"></param>
    /// <param name="successfullyAddedItem"></param>
    /// <returns></returns>
    public override Item tryToAdd(Item item, out Item successfullyAddedItem) {
      return tryToAdd(item, out successfullyAddedItem, out _);
    }

    /// <summary>
    /// Try to remove an item from the given grid slot
    /// </summary>
    /// <param name="itemLocation"></param>
    /// <returns></returns>
    public virtual Item removeAt(Coordinate pivotLocation) {
      Item removedItem = getItemStackAt(pivotLocation, out int stackId);
      if (stackId != EmptyGridSlot) {
        clearStack(stackId);
      }
      stackSlotGrid[pivotLocation.x][pivotLocation.y] = EmptyGridSlot;

      return removedItem;
    }

    /// <summary>
    /// try to empty another inventory into this shaped one with important deets added
    /// </summary>
    /// <param name="inventory"></param>
    /// <param name="succesfullyAddedUpItems"></param>
    /// <param name="modifiedPivots"></param>
    /// <returns></returns>
    public Item[] tryToLoot(IInventory inventory, out Item[] succesfullyAddedUpItems, out Coordinate[] modifiedPivots) {
      List<Item> itemsSuccessfullyAdded = new List<Item>();
      List<Coordinate> pivotsModified = new List<Coordinate>();
      List<Item> leftovers = new List<Item>();
      // for each item in the emptied other inventory
      foreach (Item item in inventory.empty()) {
        // try to add each one to the inventory and collect the changes
        Item leftoverItemStack = tryToAdd(item, out Item addedItems, out Coordinate[] updatedPivots);
        if (leftoverItemStack != null) {
          leftovers.Add(leftoverItemStack);
        }
        if (addedItems != null) {
          itemsSuccessfullyAdded.Add(addedItems);
        }
        pivotsModified.AddRange(updatedPivots);
      }

      succesfullyAddedUpItems = itemsSuccessfullyAdded.ToArray();
      modifiedPivots = pivotsModified.ToArray();
      return leftovers.ToArray();
    }

    /// <summary>
    /// Try to swap an item at a given location with another
    /// </summary>
    /// <returns>leftovers or the replaced item</returns>
    protected virtual Item tryToSwapOut(Coordinate location, Item newItem, out Item succesfullyAddedItem) {
      succesfullyAddedItem = newItem;

      // gather old item info and clear it out
      Item oldItem = getItemStackAt(location);
      Item leftovers = oldItem;
      if (oldItem != null && oldItem.canStackWith(newItem)) {
        leftovers = newItem.addToStack(oldItem, out succesfullyAddedItem);
      }
      int stackId = stackSlotGrid[location.x][location.y];
      if (stackId != EmptyGridSlot) {
        clearStack(stackId);
      }

      // add the new item stack at the same ID, to preserve grid shapes, etc
      stackId = addNewStack(newItem, stackId == EmptyGridSlot ? (int?)null : stackId);
      addItemStackToSlot(stackId, location);

      return leftovers;
    }

    /// <summary>
    /// If the grid location is within this item grid
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    protected bool isWithinGrid(Coordinate gridLocation) {
      return gridLocation.isWithin((0, 0), dimensions);
    }

    /// <summary>
    /// Get the item stack stored at the given grid spot.
    /// NOT USED TO REMOVE
    /// </summary>
    /// <returns></returns>
    public Item getItemStackAt(Coordinate location) {
      int stackId = stackSlotGrid[location.x][location.y];
      return stackId != EmptyGridSlot 
        ? stacks[stackId] 
        : null;
    }

    /// <summary>
    /// Get the item stack stored at the given grid spot
    /// NOT USED TO REMOVE
    /// </summary>
    /// <returns></returns>
    public Item getItemStackAt(Coordinate location, out int stackId) {
      stackId = stackSlotGrid[location.x][location.y];
      return stackId != EmptyGridSlot 
        ? stacks[stackId] 
        : null;
    }

    /// <summary>
    /// Add the stack to the given slot
    /// </summary>
    protected int addStack(Item stack, Coordinate slot) {
      int itemStackId = addNewStack(stack);
      addItemStackToSlot(itemStackId, slot);

      return itemStackId;
    }

    /// <summary>
    /// add items to a stack at a given location
    /// </summary>
    /// <returns>leftovers or null</returns>
    protected Item addToStack(Item stack, Coordinate slot, out Item succesfullyAddedItems) {
      Item currentStack = getItemStackAt(slot);
      return currentStack.addToStack(stack, out succesfullyAddedItems);
    }

    /// <summary>
    /// add a new slot to an expandable list.
    /// </summary>
    /// <param name="item1"></param>
    /// <param name="item2"></param>
    protected void addItemStackToSlot(int itemStackId, Coordinate slot) {
      int[] oldItemYList = stackSlotGrid[slot.x];
      int[] newItemYList;
      // if this is an extendable list in the depth dimension, extend it.
      if (slot.y >= oldItemYList.Length && slot.y < dimensions.y) {
        newItemYList = new int[oldItemYList.Length + 1];
        stackSlotGrid[slot.x].CopyTo(newItemYList, 0);
        newItemYList[oldItemYList.Length] = itemStackId;
      // if not just add the stack where we want
      } else {
        newItemYList = oldItemYList;
        newItemYList[slot.y] = itemStackId;
      }

      stackSlotGrid[slot.x] = newItemYList;
    }
  }
}
