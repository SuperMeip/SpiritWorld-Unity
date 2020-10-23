using SpiritWorld.Inventories.Items;
using System;

namespace SpiritWorld.Inventories {
  public class ShapedPack : GridBasedInventory {

    /// <summary>
    /// Make a shaped item pack of the given dimensions
    /// </summary>
    /// <param name="dimensions"></param>
    public ShapedPack((int x, int y) dimensions) : base(dimensions) {}

    /// <summary>
    /// Try to add an item to the grid, centered on the given location
    /// </summary>
    /// <param name="item"></param>
    /// <param name="location">the place the center/pivot of the shaped item icon is placed on the grid</param>
    /// <param name="successfullyAddedItem"></param>
    /// <returns></returns>
    public override Item tryToAdd(Item item, Coordinate location, out Item successfullyAddedItem) {
      Item leftoverStack = null;
      successfullyAddedItem = null;

      // check the placed center location stacks first
      Item pivotStack =  getItemStackAt(location);
      // if it's null, check to see if the whole shape worth of spaces are empty and add it if so
      if (pivotStack == null) {
        bool allStacksAreEmpty = true;
        // check the other spaces around the pivot and make sure they're also empty.
        Coordinate itemPivot = item.type.ShapePivot;
        Coordinate.Zero.until((item.type.Shape.GetLength(0), item.type.Shape.GetLength(1)), offset => {
          // from bottom left => right top
          // we can skip the middle one (pivot type) too because we already checked it.
          Coordinate currentGridLocation = location + (offset - itemPivot);
          if (item.type.Shape[offset.x, offset.z] == Item.Type.ShapeBlocks.Solid) {
            Item blockStack = getItemStackAt(currentGridLocation);
            // if the blockstat is null, abort.
            if (blockStack != null) {
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

      // if it can stack, stack em
      } else if (item.canStackWith(pivotStack)) {
        return pivotStack.addToStack(item, out successfullyAddedItem);
      // try to swap it for the other item if they're the same shape

      } else if (item.type.Shape == pivotStack.type.Shape) {
        successfullyAddedItem = tryToSwapOut(location, item, out leftoverStack) 
          ? item 
          : null;
      }

      return leftoverStack;
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
    }
  }
}
