using SpiritWorld.Inventories.Items;
using System;
using System.Collections.Generic;

namespace SpiritWorld.Inventories {
  public class ShapedPack : GridBasedInventory {

    /// <summary>
    /// The pivot locations for each stack
    /// </summary>
    readonly Dictionary<int, Coordinate> stackPivotLocations 
      = new Dictionary<int, Coordinate>();

    /// <summary>
    /// Make a shaped item pack of the given dimensions
    /// </summary>
    /// <param name="dimensions"></param>
    public ShapedPack((int x, int y) dimensions) : base(dimensions) {}

    /// <summary>
    /// Make a shaped item pack of the given dimensions with starting items
    /// </summary>
    /// <param name="dimensions"></param>
    public ShapedPack((int x, int y) dimensions, (Item, Coordinate)[] initialValues) : base(dimensions) {
      foreach(var (item, addLocation) in initialValues) {
        tryToAdd(item, addLocation, out _);
      }
    }

    /// <summary>
    /// Try to add an item to the grid, centered on the given location
    /// </summary>
    /// <param name="item">The item we're trying to add</param>
    /// <param name="location">the place the center/pivot of the shaped item icon is placed on the grid</param>
    /// <param name="successfullyAddedItem">Any items sucessfully added</param>
    /// <returns>Leftovers/items not added or returned.</returns>
    public override Item tryToAdd(Item item, Coordinate location, out Item successfullyAddedItem) {
      Item leftoverStack = null;
      successfullyAddedItem = null;

      // check the placed center location stacks first
      Item pivotStack = getItemStackAt(location);
      // if it's null, check to see if the whole shape worth of spaces are empty and add it if so
      if (pivotStack == null) {
        bool allStacksAreEmpty = true;
        // check the other spaces around the pivot and make sure they're also empty.
        Coordinate itemPivot = item.type.ShapePivot;
        Coordinate.Zero.until((item.type.Shape.GetLength(0), item.type.Shape.GetLength(1)), offset => {
          // from bottom left => right top
          Coordinate currentGridLocation = location + (offset - itemPivot);
          // make sure the grid location exists
          if (!currentGridLocation.isWithin(Coordinate.Zero, dimensions)) {
            leftoverStack = item;
            allStacksAreEmpty = false;
            return false;
          }
          // we can skip the middle one (pivot type) too because we already checked it.
          if (item.type.Shape[offset.x, offset.z] == Item.Type.ShapeBlocks.Solid) {
            Item blockStack = getItemStackAt(currentGridLocation);
            // if the blockstack is occupied, abort.
            if (blockStack != null) {
              leftoverStack = item;
              allStacksAreEmpty = false;
              return false;
            }
          }

          return true;
        });

        /// if all the stacks were empty, add the shaped item
        if (allStacksAreEmpty) {
          successfullyAddedItem = item;
          addShapedItemStack(item, location);
        }
      /// if it can stack, stack em
      } else if (item.canStackWith(pivotStack)) {
        leftoverStack = pivotStack.addToStack(item, out successfullyAddedItem);
      /// try to swap it for the other item if they're the same shape
      } else if (item.type.Shape == pivotStack.type.Shape) {
        successfullyAddedItem = tryToSwapOut(location, item, out leftoverStack) 
          ? item 
          : null;
      }

      return leftoverStack;
    }

    /// <summary>
    /// Try to add the given items to the first free spots
    /// </summary>
    /// <param name="item"></param>
    /// <param name="successfullyAddedItems"></param>
    /// <returns></returns>
    public override Item tryToAdd(Item item, out Item successfullyAddedItems, out Coordinate[] modifiedStackPivots) {
      Item itemsLeftToAdd = item;
      Item itemsSuccessfullyAdded = null;
      List<Coordinate> modifiedPivots = new List<Coordinate>();
      // first go through and try to stack everything we can
      forEach((pivot, existingItemStack, stackId) => {
        if (!existingItemStack.isFull && existingItemStack.canStackWith(itemsLeftToAdd)) {
          modifiedPivots.Add(pivot);
          itemsLeftToAdd = existingItemStack.addToStack(itemsLeftToAdd, out Item itemsAdded);
          if (itemsAdded != null) {
            itemsSuccessfullyAdded = itemsSuccessfullyAdded == null
              ? itemsAdded
              : itemsSuccessfullyAdded.addToStack(itemsAdded, out _);
          }
        }

        /// stop if it's empty
        if (itemsLeftToAdd == null) {
          return false;
        }

        return true;
      });

      // next try to find an open spot this fits in if we still need to
      if (itemsLeftToAdd != null && itemsLeftToAdd.quantity > 0) {
        Coordinate.Zero.until(dimensions, gridSlotLocation => {
          int initalStackSize = itemsLeftToAdd.quantity;
          if (getItemStackAt(gridSlotLocation) == null) {
            itemsLeftToAdd = tryToAdd(itemsLeftToAdd, gridSlotLocation, out Item itemsAdded);
            if (itemsAdded != null) {
              itemsSuccessfullyAdded = itemsSuccessfullyAdded == null
                ? itemsAdded
                : itemsSuccessfullyAdded.addToStack(itemsAdded, out _);
            }
          }
          if (initalStackSize != itemsLeftToAdd?.quantity) {
            modifiedPivots.Add(gridSlotLocation);
          }
          if (itemsLeftToAdd == null) {
            return false;
          }

          return true;
        });
      }

      modifiedStackPivots = modifiedPivots.ToArray();
      successfullyAddedItems = itemsSuccessfullyAdded;
      return itemsLeftToAdd;
    }

    /// <summary>
    /// Remove the item at the given coordinate location
    /// </summary>
    /// <param name="itemLocation"></param>
    /// <returns></returns>
    public override Item[] removeAt(Coordinate itemLocation) {
      throw new System.NotImplementedException();
    }

    /// <summary>
    /// Do something on each item stack given it's pivot location
    /// </summary>
    /// <param name="action">item pivot location, item stack, stack id</param>
    public void forEach(Action<Coordinate, Item, int> action) {
      for(int stackId = 0; stackId < stacks.Count; stackId++) {
        if (stacks[stackId] is Item stack && stack != null) {
          action(stackPivotLocations[stackId], stack, stackId);
        }
      }
    }

    /// <summary>
    /// Do something on each item stack given it's pivot location
    /// </summary>
    /// <param name="action">item pivot location, item stack, stack id</param>
    public void forEach(Func<Coordinate, Item, int, bool> action) {
      for(int stackId = 0; stackId < stacks.Count; stackId++) {
        if (stacks[stackId] is Item stack && stack != null) {
          if (!action(stackPivotLocations[stackId], stack, stackId))
            return;
        }
      }
    }

    /// <summary>
    /// Add a shaped item at the given pivot location
    /// </summary>
    /// <param name="item"></param>
    /// <param name="location"></param>
    void addShapedItemStack(Item item, Coordinate location) {
      int? stackId = null;
      Coordinate itemPivot = item.type.ShapePivot;
      Coordinate.Zero.until((item.type.Shape.GetLength(0), item.type.Shape.GetLength(1)), offset => {
        // from bottom left => right top
        // we can skip the middle one (pivot type) too because we already checked it.
        Coordinate currentGridLocation = location + (offset - itemPivot);
        if (item.type.Shape[offset.x, offset.z] != Item.Type.ShapeBlocks.Empty) {
          // add the item to a new stack first, then use that stack id to populate the slot grid
          if (stackId == null) {
            stackId = addStack(item, currentGridLocation);
          } else {
            addItemStackToSlot((int)stackId, currentGridLocation);
          }
        }
      });

      // add the pivot
      stackPivotLocations.Add((int)stackId, location);
    }
  }
}
