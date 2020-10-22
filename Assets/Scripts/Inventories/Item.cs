using System;

namespace SpiritWorld.Inventories.Items {

  /// <summary>
  /// An item, or stack of multiple of the same items
  /// </summary>
  public partial class Item {

    /// <summary>
    /// The type of tile
    /// </summary>
    public Type type {
      get;
    }

    /// <summary>
    /// How many of this item are in this stack
    /// </summary>
    public byte quantity {
      get;
      protected set;
    } = 1;

    /// <summary>
    /// If this item stack is full
    /// </summary>
    public bool isFull
      => quantity >= type.StackSize;

    /// <summary>
    /// Make a new item/stack of item
    /// </summary>
    public Item(Type type, byte quantity = 1) {
      this.type = type;
      this.quantity = quantity;
    }

    /// <summary>
    /// ake an item from a simple json
    /// </summary>
    /// <param name="itemString"></param>
    public Item(string itemString) {
      string[] itemData = itemString.Split(':');
      type = Types.Get(short.Parse(itemData[0]));
      quantity = (byte)short.Parse(itemData[1]);
    }

    /// <summary>
    /// Add a nother item's count to this one's.
    /// This doesn't check canStackWith first.
    /// </summary>
    /// <param name="successfullyAddedItem">The item stack that was successfuly added, for reporting</param>
    /// <returns>the overflow items or null</returns>
    public Item addToStack(Item item, out Item successfullyAddedItem) {
      int leftoverStackCount = item.quantity + quantity - type.StackSize;
      successfullyAddedItem = item.copy();

      if (leftoverStackCount > 0) {
        quantity = type.StackSize;
        item.quantity = (byte)leftoverStackCount;
        successfullyAddedItem.quantity -= (byte)leftoverStackCount;

        return item;
      } else {
        quantity += item.quantity;
        item.quantity = 0;

        return null;
      }
    }


    /// <summary>
    /// This doesn't check stack size equality, this just checks if the items can stack
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool canStackWith(Item item) {
      return item.type == type;
    }

    /// <summary>
    /// remove the items from the stack and return them
    /// </summary>
    /// <param name="quantity"></param>
    /// <returns></returns>
    public Item removeFromStack(byte quantity) {
      if (quantity > 0) {
        byte amountToSubtract = Math.Max(quantity, this.quantity);
        this.quantity -= amountToSubtract;
        return copy(amountToSubtract);
      }

      /// return an empty stack
      return copy(0);
    }

    /// <summary>
    /// copy this item
    /// </summary>
    /// <param name="quantity"></param>
    /// <returns></returns>
    public virtual Item copy(byte? quantity = null) {
      return new Item(type, quantity ?? this.quantity);
    }

    /// <summary>
    /// Item basic deets
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
      return $"I[{type.Name}:{quantity}]";
    }
  }
}