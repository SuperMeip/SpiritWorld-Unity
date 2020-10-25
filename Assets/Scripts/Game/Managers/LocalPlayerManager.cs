using SpiritWorld.Game.Controllers;
using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;

namespace SpiritWorld.Managers {

  /// <summary>
  /// The local player controller
  /// </summary>
  public class LocalPlayerManager : PlayerManager {

    #region consts
    ///// SET VIA UNITY

    /// <summary>
    /// The local player's item hot bar controller.
    /// </summary>
    public ItemHotBarController ItemHotBarController;

    /// <summary>
    /// The local player's pack grid inventory controller
    /// </summary>
    public PlayerPackGridController PackGridController;

    /// <summary>
    /// The local player's notification manager
    /// </summary>
    public LocalNotificationsManager NotificationsManager;

    /// <summary>
    /// Manages tile selection for the local player
    /// </summary>
    public TileSelectionManager TileSelectionManager;

    /// <summary>
    /// The movement controller for the local player
    /// </summary>
    public LocalPlayerMotionController MotionController;

    ///// ////////////////
    #endregion

    /// <summary>
    /// Try to pick up the given item stack
    /// </summary>
    /// <param name="itemStack"></param>
    /// <returns></returns>
    public override Item tryToPickUp(Item itemStack, out Item succesfullyPickedUpItem) {
      Item leftovers = player.packInventory.tryToAdd(itemStack, out succesfullyPickedUpItem, out Coordinate[] modifiedPivots);

      // update local notifications
      if (succesfullyPickedUpItem != null) {
        Universe.EventSystem.notifyChannelOf(
          new PlayerObtainItemEvent(player, succesfullyPickedUpItem),
          Events.WorldScapeEventSystem.Channels.LocalPlayerUpdates
        );
      }

      // update local grid inventory if needed
      if (modifiedPivots.Length > 0) {
        Universe.EventSystem.notifyChannelOf(
          new PackInventoryItemsUpdatedEvent(modifiedPivots),
          Events.WorldScapeEventSystem.Channels.LocalPlayerUpdates
        );
      }

      return leftovers;
    }

    /// <summary>
    /// Try to take all the items from the given inventory
    /// </summary>
    /// <param name="inventory"></param>
    /// <returns></returns>
    public override Item[] tryToEmpty(IInventory inventory, out Item[] succesfullyAddedUpItems) {
      Item[] leftovers = base.tryToEmpty(inventory, out succesfullyAddedUpItems);

      // send the local player picked up items notification
      foreach (Item item in succesfullyAddedUpItems) {
        Universe.EventSystem.notifyChannelOf(
          new PlayerObtainItemEvent(player, item),
          Events.WorldScapeEventSystem.Channels.LocalPlayerUpdates
        );
      }

      return leftovers;
    }
  }
}