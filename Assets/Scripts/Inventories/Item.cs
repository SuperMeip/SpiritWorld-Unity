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
    /// Make a new item/stack of item
    /// </summary>
    public Item(Type type, byte quantity = 1) {
      this.type = type;
      this.quantity = quantity;
    }
  }
}