using SpiritWorld.Game.Controllers;
using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using SpiritWorld.World.Entities.Creatures;
using System.Collections.Generic;
using UnityEngine;

namespace SpiritWorld.Managers {

  /// <summary>
  /// The local player controller
  /// </summary>
  public class LocalPlayerManager : PlayerManager {

    #region Constants
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
          new PackInventoryItemsUpdatedEvent(modifiedPivots, Player.InventoryTypes.GridPack),
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
    public override Item[] tryToLoot(IInventory inventory, out Item[] succesfullyAddedUpItems) {
      List<Item> itemsSuccessfullyAdded = new List<Item>();
      Coordinate[] modifiedHotBarPivots;
      Coordinate[] modifiedGridPackPivots = new Coordinate[0];
      // try hot bar first
      Item[] leftovers = player.hotBarInventory.tryToLoot(inventory, out Item[] addedItems, out modifiedHotBarPivots);
      itemsSuccessfullyAdded.AddRange(addedItems);
      // then try pack
      if (leftovers.Length > 0) {
        leftovers = player.packInventory.tryToLoot(inventory, out addedItems, out modifiedGridPackPivots);
        itemsSuccessfullyAdded.AddRange(addedItems);
      }

      // send the local player picked up items notification
      succesfullyAddedUpItems = itemsSuccessfullyAdded.ToArray();
      foreach (Item item in succesfullyAddedUpItems) {
        Universe.EventSystem.notifyChannelOf(
          new PlayerObtainItemEvent(player, item),
          Events.WorldScapeEventSystem.Channels.LocalPlayerUpdates
        );
      }


      // update local hot bar inventory if needed
      if (modifiedHotBarPivots.Length > 0) {
        Universe.EventSystem.notifyChannelOf(
          new PackInventoryItemsUpdatedEvent(modifiedHotBarPivots, Player.InventoryTypes.HotBar),
          Events.WorldScapeEventSystem.Channels.LocalPlayerUpdates
        );
      }
      // update local grid inventory if needed
      if (modifiedGridPackPivots.Length > 0) {
        Universe.EventSystem.notifyChannelOf(
          new PackInventoryItemsUpdatedEvent(modifiedGridPackPivots, Player.InventoryTypes.GridPack),
          Events.WorldScapeEventSystem.Channels.LocalPlayerUpdates
        );
      }

      return leftovers;
    }
  }
}