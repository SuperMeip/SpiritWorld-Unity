using SpiritWorld.Inventories.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorld.Inventories {

  /// <summary>
  /// basic inventory class for storing basic items in single stacks.
  /// for stuff like drops
  /// </summary>
  public class BasicInventory : Dictionary<Item.Type, Item>, IInventory {

    /// <summary>
    /// the basic inventory
    /// </summary>
    public BasicInventory() {}

    /// <summary>
    /// Create a basic inventory from a list in the format:
    /// ITemId:quantity,ItemId,quantity
    /// </summary>
    /// <param name="v"></param>
    public BasicInventory(string inventoryString) {
      string[] itemStacks = inventoryString.Split(',');
      foreach(string itemString in itemStacks) {
        tryToAdd(new Item(itemString), out _);
      }
    }

    /// <summary>
    /// Return the first available stack of the given item
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public new Item this[Item.Type type] {
      get => TryGetValue(type, out Item item) ? item : null;
      private set => base[type] = value;
    }

    /// <summary>
    /// Generic add an item to the inventory
    /// </summary>
    /// <param name="item"></param>
    public virtual Item tryToAdd(Item item, out Item successfullyAddedItem) {
      Item existingItem = this[item.type];
      if (existingItem != null) {
        return existingItem.addToStack(item, out successfullyAddedItem);
      } else {
        this[item.type] = item;
        successfullyAddedItem = item;

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


    /// <summary>
    /// Empty this inventory into another one
    /// TODO: make sure this deletes items that hit 0 from this inventory
    /// </summary>
    /// <returns>leftovers that don't fit</returns>
    public Item[] emptyInto(IInventory otherInventory, out Item[] successfullyAddedItems) {
      List<Item> addedItems = new List<Item>();
      List<Item> leftovers = new List<Item>();
      foreach(Item stack in Values) {
        leftovers.Add(otherInventory.tryToAdd(stack, out Item addedItem));
        // TODO remove emptied stacks
        if (addedItem != null) {
          addedItems.Add(addedItem);
        }
      }

      successfullyAddedItems = addedItems.ToArray();
      return leftovers.ToArray();
    }

    /// <summary>
    /// Make a copy of this inventory
    /// </summary>
    /// <returns></returns>
    public IInventory copy() {
      BasicInventory copy = new BasicInventory();
      foreach(Item stack in Values) {
        copy.Add(stack.type, stack.copy());
      }

      return copy;
    }

    /// <summary>
    /// Inventory Contents
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
      string text = "I{";
      foreach(Item item in Values) {
        text += $"[{item.type.Name}:{item.quantity}], ";
      }
      text.Trim(new char[] {' ', ','});
      return text + "}";
    }

    /// <summary>
    /// find the given items
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public Item[] search(Func<Item, bool> query) {
      List<Item> matches = new List<Item>();
      foreach(Item item in Values) {
        if (query(item)) {
          matches.Add(item);
        }
      }

      return matches.ToArray();
    }
  }
}
