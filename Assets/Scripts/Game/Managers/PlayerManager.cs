using SpiritWorld.Inventories.Items;
using SpiritWorld.World.Entities.Creatures;
using SpiritWorld.Events;
using UnityEngine;
using SpiritWorld.Inventories;
using System;

namespace SpiritWorld.Managers {

  /// <summary>
  /// Used to control players in the overworld
  /// </summary>
  public class PlayerManager : MonoBehaviour {

    /// <summary>
    /// The player this controller is for
    /// </summary>

    /// <summary>
    /// The currently selected tool of the user
    /// TODO: move this functionality to ItemHotBarController.selectedItem for the local player only
    /// </summary>
    public ITool selectedTool {
      get;
      protected set;
    } = Player.EmptyHand;

    /// <summary>
    /// The player being controled
    /// </summary>
    protected Player player;

    /// <summary>
    /// Set the player being controled
    /// </summary>
    /// <param name="player"></param>
    public void setPlayer(Player player) {
      this.player = player;
    }

    /// <summary>
    /// Try to pick up the given item stack
    /// </summary>
    /// <param name="itemStack"></param>
    /// <returns></returns>
    public virtual Item tryToPickUp(Item itemStack) {
      return tryToPickUp(itemStack, out _);
    }

    /// <summary>
    /// get for the player
    /// </summary>
    /// <returns></returns>
    public Player getPlayer() {
      return player;
    }

    /// <summary>
    /// Try to take all the items from the given inventory
    /// </summary>
    /// <param name="inventory"></param>
    /// <returns></returns>
    public virtual Item[] tryToEmpty(IInventory inventory) {
      return tryToLoot(inventory, out _);
    }

    /// <summary>
    /// Try to pick up the given item stack
    /// </summary>
    /// <param name="itemStack"></param>
    /// <returns></returns>
    public virtual Item tryToPickUp(Item itemStack, out Item succesfullyPickedUpItem) {
      return player.packInventory.tryToAdd(itemStack, out succesfullyPickedUpItem);
    }

    /// <summary>
    /// Try to take all the items from the given inventory
    /// </summary>
    /// <param name="inventory"></param>
    /// <returns></returns>
    public virtual Item[] tryToLoot(IInventory inventory, out Item[] succesfullyAddedUpItems) {
      Item[] barLeftovers = player.hotBarInventory.tryToLoot(inventory, out succesfullyAddedUpItems, out _);
      return barLeftovers.Length > 0 ? player.packInventory.tryToLoot(inventory, out succesfullyAddedUpItems, out _) : barLeftovers;
    }

    /// <summary>
    /// event for player picking up an item
    /// </summary>
    public struct PlayerObtainItemEvent : IEvent {
      public string name
        => "Player picked up item";

      /// <summary>
      /// The item that was picked up
      /// </summary>
      public Item item {
        get;
      }

      /// <summary>
      /// The player who picked it up
      /// </summary>
      public Player player {
        get;
      }

      /// <summary>
      /// Make an event of this kind
      /// </summary>
      /// <param name="player"></param>
      /// <param name="item"></param>
      public PlayerObtainItemEvent(Player player, Item item) {
        this.item = item;
        this.player = player;
      }
    }

    /// <summary>
    /// Event for adding items to their pack
    /// </summary>
    public struct PackInventoryItemsUpdatedEvent : IEvent {
      public string name
        => "Player picked up item";

      /// <summary>
      /// The item that was picked up
      /// </summary>
      public Coordinate[] modifiedPivots {
        get;
      }

      public Player.InventoryTypes updatedInventoryType {
        get;
      }

      /// <summary>
      /// Make an event of this kind
      /// </summary>
      /// <param name="player"></param>
      /// <param name="item"></param>
      public PackInventoryItemsUpdatedEvent(Coordinate[] modifiedPivots, Player.InventoryTypes updatedInventoryType) {
        this.modifiedPivots = modifiedPivots;
        this.updatedInventoryType = updatedInventoryType;
      }
    }
  }
}
