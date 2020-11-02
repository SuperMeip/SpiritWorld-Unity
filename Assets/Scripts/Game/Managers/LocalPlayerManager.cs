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
          new LocalPlayerInventoryItemsUpdatedEvent(modifiedPivots, Player.InventoryTypes.GridPack),
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
      Item[] leftovers = base.tryToLoot(inventory, out succesfullyAddedUpItems, out (Coordinate, Player.InventoryTypes)[] modifiedPivots);

      // send the local player picked up items notification
      foreach (Item item in succesfullyAddedUpItems) {
        Universe.EventSystem.notifyChannelOf(
          new PlayerObtainItemEvent(player, item),
          Events.WorldScapeEventSystem.Channels.LocalPlayerUpdates
        );
      }


      // update the local inventory uis if need be
      List<Coordinate> modifiedHotBarPivots = new List<Coordinate>();
      List<Coordinate> modifiedGridPackPivots = new List<Coordinate>();
      foreach ((Coordinate pivot, Player.InventoryTypes inventoryType) in modifiedPivots) {
        if (inventoryType == Player.InventoryTypes.GridPack) {
          modifiedGridPackPivots.Add(pivot);
        } else if (inventoryType == Player.InventoryTypes.HotBar) {
          modifiedHotBarPivots.Add(pivot);
        }
      }
      if (modifiedHotBarPivots.Count > 0) {
        Universe.EventSystem.notifyChannelOf(
          new LocalPlayerInventoryItemsUpdatedEvent(modifiedHotBarPivots.ToArray(), Player.InventoryTypes.HotBar),
          Events.WorldScapeEventSystem.Channels.LocalPlayerUpdates
        );
      }
      if (modifiedGridPackPivots.Count > 0) {
        Universe.EventSystem.notifyChannelOf(
          new LocalPlayerInventoryItemsUpdatedEvent(modifiedGridPackPivots.ToArray(), Player.InventoryTypes.GridPack),
          Events.WorldScapeEventSystem.Channels.LocalPlayerUpdates
        );
      }

      return leftovers;
    }

    /// <summary>
    /// override for adding item to player inventory
    /// TODO this should be a swap function
    /// </summary>
    /// <param name="item"></param>
    /// <param name="inventoryType"></param>
    /// <param name="location"></param>
    /// <param name="succesfullyAddedItem"></param>
    /// <returns></returns>
    public override Item tryToAddToInventoryAt(Item item, Player.InventoryTypes inventoryType, Coordinate location, out Item succesfullyAddedItem) {
      Item leftovers = base.tryToAddToInventoryAt(item, inventoryType, location, out succesfullyAddedItem);
      if (succesfullyAddedItem != null) {
        if (inventoryType == Player.InventoryTypes.HotBar) {
          Universe.EventSystem.notifyChannelOf(
            new LocalPlayerInventoryItemsUpdatedEvent(new Coordinate[] { location }, Player.InventoryTypes.HotBar),
            Events.WorldScapeEventSystem.Channels.LocalPlayerUpdates
          );
        } else if (inventoryType == Player.InventoryTypes.GridPack) {
          if (item.quantity > 0) {
            Universe.EventSystem.notifyChannelOf(
              new LocalPlayerInventoryItemsUpdatedEvent(new Coordinate[] { location }, Player.InventoryTypes.GridPack),
              Events.WorldScapeEventSystem.Channels.LocalPlayerUpdates
            );
          // if we stacked it, remove one stack and update the other.
          } else {
            player.packInventory.getItemStackAt(location, out Coordinate pivot);
            Universe.EventSystem.notifyChannelOf(
              new LocalPlayerInventoryItemsUpdatedEvent(new Coordinate[] { pivot }, Player.InventoryTypes.GridPack),
              Events.WorldScapeEventSystem.Channels.LocalPlayerUpdates
            );
          }
        }
      }

      return leftovers;
    }

    /// <summary>
    /// override for update alerts
    /// </summary>
    /// <param name="inventoryType"></param>
    /// <param name="pivotLocation"></param>
    /// <returns></returns>
    public override Item removeFromInventoryAt(Player.InventoryTypes inventoryType, Coordinate pivotLocation) {
      Item removedItem = base.removeFromInventoryAt(inventoryType, pivotLocation);
      if (removedItem != null) {
        Universe.EventSystem.notifyChannelOf(
          new LocalPlayerInventoryItemsRemovedEvent(new Coordinate[] { pivotLocation }, inventoryType),
          Events.WorldScapeEventSystem.Channels.LocalPlayerUpdates
        );
      }

      return removedItem;
    }
  }
}