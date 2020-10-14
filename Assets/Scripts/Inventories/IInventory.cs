using SpiritWorld.Inventories.Items;

namespace SpiritWorld.Inventories {
  public interface IInventory {

    /// <summary>
    /// Add items to the inventory
    /// </summary>
    /// <returns>Any items not added</returns>
    Item tryToAdd(Item item);

    /// <summary>
    /// remove items from the inventory
    /// </summary>
    /// <param name=""></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    Item[] remove(Item item, int? quantity = null);

    /// <summary>
    /// empty all the items from the inventory
    /// </summary>
    /// <returns></returns>
    Item[] empty();
  }
}
