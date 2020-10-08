namespace SpiritWorld.Inventories.Items {

  /// <summary>
  /// An item with a use on the main board
  /// </summary>
  public interface IUseableItem {

    /// <summary>
    /// How many times this item can be used before being used up
    /// 0 is unlimited.
    /// </summary>
    short NumberOfUses {
      get;
    }
  }
}