using SpiritWorld.Inventories.Items;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorld.Inventories {
  
  /// <summary>
  /// An inventory that allows multiple stacks of the same item, and limits stack size
  /// </summary>
  public class StackedInventory : IInventory {

    /// <summary>
    /// The limit of how many stacks this inventory can hold
    /// </summary>
    public int stackLimit {
      get;
    }

    /// <summary>
    /// Count the in use stacks
    /// </summary>
    public int usedStackCount 
      => stacks.Count(stack => stack != null);

    /// <summary>
    /// How many empty stack slots there are
    /// </summary>
    public int emptyStackCount
      => stackLimit - usedStackCount;

    /// <summary>
    /// Get if this pack is full
    /// </summary>
    public bool isFull
      => usedStackCount <= stackLimit;

    /// <summary>
    /// The item stacks by 'stack slot id'
    /// </summary>
    readonly List<Item> stacks
      = new List<Item>();

    /// <summary>
    /// the stack slot ids indexed by item types
    /// </summary>
    readonly Dictionary<Item.Type, List<int>> stackIdsByItemType 
      = new Dictionary<Item.Type, List<int>>();

    /// <summary>
    /// Make a new stacked inventory
    /// </summary>
    /// <param name="size"></param>
    public StackedInventory(int size) {
      stackLimit = size;
    }

    /// <summary>
    /// Add an item stack
    /// </summary>
    /// <param name="item"></param>
    public virtual Item tryToAdd(Item item) {
      if (stackIdsByItemType.TryGetValue(item.type, out List<int> stackSlotIds)) {
        Item itemToAdd = item;
        // go though each slot of this type and try to add the item to it
        foreach (int stackSlotId in stackSlotIds) {
          Item currentItemStack = stacks[stackSlotId];
          // if the current stack isn't full and matches the item completely, stack em
          if (!currentItemStack.isFull && currentItemStack.canStackWith(itemToAdd)) {
            Item leftoverStack = currentItemStack.addToStack(itemToAdd);
            // we still need to deal with the leftovers, loop again!
            itemToAdd = leftoverStack;
          }
        }

        // if we still have leftovers, add them to a new stack
        if (itemToAdd != null && !isFull) {
          addNewStack(itemToAdd);

          return null;
          // if there's no room return what doesn't fit
        } else {
          return itemToAdd;
        }
      // if there's no stacks of this type just add it
      } else if (!isFull) {
        addNewStack(item);

        return null;
      // if there's no room return what doesn't fit
      } else {
        return item;
      }
    }

    /// <summary>
    /// remove the given amount of the item of the given kind
    /// </summary>
    /// <param name="type"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    public virtual Item[] remove(Item item, int? quantity = null) {
      // if we have items of that type remove them
      if (stackIdsByItemType.TryGetValue(item.type, out List<int> stackSlotIds)) {
        // how many do we actually want to remove of this kind?
        int totalItemsToRemove = quantity ?? item.quantity;
        List<Item> removedStacks = new List<Item>();
        foreach (int stackSlotId in stackSlotIds) {
          Item currentItemStack = stacks[stackSlotId];
          // if the current stack isn't full and matches the item completely, stack em
          if (!currentItemStack.isFull && currentItemStack.canStackWith(item)) {
            // trim the removal count to max stack size
            byte stackRemoveCount = (byte)totalItemsToRemove;
            Item removedStack = currentItemStack.removeFromStack(stackRemoveCount);
            // trim the remaining count by what we removed
            totalItemsToRemove -= removedStack.quantity;
            removedStacks.Add(removedStack);
            // if we removed enough, return what we removed
            if (totalItemsToRemove <= 0) {
              return removedStacks.ToArray();
            }
          }
        }
      }

      // if we have no items of that type to remove, return nothing
      return null;
    }

    /// <summary>
    /// Empty the whole inventory
    /// </summary>
    /// <returns></returns>
    public virtual Item[] empty() {
      Item[] stackArray = stacks.ToArray();
      stacks.Clear();

      return stackArray;
    }

    /// <summary>
    /// Add a new item stack to the collections
    /// </summary>
    /// <param name="itemStack"></param>
    void addNewStack(Item itemStack) {
      /// add the stack
      int? freeStackId = getFirstFreeStackSlot();
      if (freeStackId != null) {
        stacks.Insert((int)freeStackId, itemStack);
      } else {
        stacks.Add(itemStack);
        freeStackId = stacks.Count - 1;
      }

      /// add to the index dic
      if (stackIdsByItemType.ContainsKey(itemStack.type)) {
        stackIdsByItemType[itemStack.type].Add((int)freeStackId);
      } else {
        stackIdsByItemType[itemStack.type] = new List<int> { (int)freeStackId };
      }
    }

    /// <summary>
    /// Get the first free slot, or null if there isn't one.
    /// </summary>
    /// <returns></returns>
    int? getFirstFreeStackSlot() {
      int index = 0;
      foreach(Item itemStack in stacks) {
        if (itemStack == null) {
          return index;
        }
        index++;
      }

      return null;
    }
  }
}