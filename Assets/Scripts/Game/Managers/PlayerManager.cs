using SpiritWorld.Inventories.Items;
using SpiritWorld.World.Entities.Creatures;
using SpiritWorld.Events;
using UnityEngine;
using SpiritWorld.Inventories;
using System;
using System.Collections.Generic;

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
    /// try to add an item to an inventory at the given spot.
    /// </summary>
    /// <param name="item">The item to try to add</param>
    /// <param name="inventoryType">The inventoy to add to</param>
    /// <param name="pivotLocation">the place in the inventory we're adding to</param>
    /// <param name="succesfullyAddedItem">the item stack that was added successfully</param>
    /// <returns>Any leftovers or replaced items</returns>
    public virtual Item tryToAddToInventoryAt(Item item, Player.InventoryTypes inventoryType, Coordinate pivotLocation, out Item succesfullyAddedItem) {
      succesfullyAddedItem = null;
      switch (inventoryType) {
        case Player.InventoryTypes.GridPack:
          return player.packInventory.tryToAdd(item, pivotLocation, out succesfullyAddedItem);
        case Player.InventoryTypes.HotBar:
          return player.hotBarInventory.tryToAdd(item, pivotLocation, out succesfullyAddedItem);
        default: return null;
      }
    }

    /// <summary>
    /// Try to remove an item from the player inventory
    /// </summary>
    /// <param name="inventoryType"></param>
    /// <param name="pivotLocation"></param>
    /// <returns></returns>
    public virtual Item removeFromInventoryAt(Player.InventoryTypes inventoryType, Coordinate pivotLocation) {
      Item removedItem;
      switch (inventoryType) {
        case Player.InventoryTypes.GridPack:
          removedItem = player.packInventory.removeAt(pivotLocation);
          break;
        case Player.InventoryTypes.HotBar:
          removedItem = player.hotBarInventory.removeAt(pivotLocation);
          break;
        default: return null;
      }

      return removedItem;
    }

    /// <summary>
    /// Try to take all the items from the given inventory
    /// </summary>
    /// <param name="inventory"></param>
    /// <returns></returns>
    public virtual Item[] tryToLoot(IInventory inventory, out Item[] succesfullyAddedUpItems) {
      return tryToLoot(inventory, out succesfullyAddedUpItems, out _);
    }

    /// <summary>
    /// try to loot with pivot updates
    /// </summary>
    /// <param name="inventory"></param>
    /// <param name="succesfullyAddedUpItems"></param>
    /// <param name="modifiedPivots"></param>
    /// <returns></returns>
    protected Item[] tryToLoot(IInventory inventory, out Item[] succesfullyAddedUpItems, out (Coordinate, Player.InventoryTypes)[] modifiedPivots) {
      List<(Coordinate, Player.InventoryTypes)> updatedPivots = new List<(Coordinate, Player.InventoryTypes)>();
      BasicInventory leftovers = new BasicInventory();
      List<Item> sucessfullyAddedStacks = new List<Item>();
      foreach (Item item in inventory.empty()) {
        // first put all the matches in the hot bar
        if (player.hotBarInventory.search(existingItem => existingItem != null && existingItem.canStackWith(item)).Length > 0) {
          Item leftoverStack = player.hotBarInventory.tryToAdd(item, out Item succesfullyAddedStack, out Coordinate[] changedPivots);
          if (leftoverStack != null) {
            leftovers.tryToAdd(leftoverStack, out _);
          }
          if (succesfullyAddedStack != null) {
            sucessfullyAddedStacks.Add(succesfullyAddedStack);
          }
          foreach(Coordinate pivot in changedPivots) {
            updatedPivots.Add((pivot, Player.InventoryTypes.HotBar));
          }
          // then put any that don't match the bar but the match in the pack in the pack.
        } else if (player.packInventory.search(existingItem => existingItem != null && existingItem.canStackWith(item)).Length > 0) {
          Item leftoverStack = player.packInventory.tryToAdd(item, out Item succesfullyAddedStack, out Coordinate[] changedPivots);
          if (leftoverStack != null) {
            leftovers.tryToAdd(leftoverStack, out _);
          }
          if (succesfullyAddedStack != null) {
            sucessfullyAddedStacks.Add(succesfullyAddedStack);
          }
          foreach (Coordinate pivot in changedPivots) {
            updatedPivots.Add((pivot, Player.InventoryTypes.GridPack));
          }
          // if it doesn't match either inventory, add it to the leftovers
        } else {
          leftovers.tryToAdd(item, out _);
        }
      }

      // loot the remaining inventory items into the pack first, and then into the item bar
      Item[] remainingLeftovers = player.packInventory.tryToLoot(leftovers, out Item[] succesfullyAddedItems, out Coordinate[] changedPackPivots);
      foreach(Coordinate pivot in changedPackPivots) {
        updatedPivots.Add((pivot, Player.InventoryTypes.GridPack));
      }
      sucessfullyAddedStacks.AddRange(succesfullyAddedItems);
      if (remainingLeftovers.Length > 0) {
        remainingLeftovers = player.hotBarInventory.tryToLoot(leftovers, out succesfullyAddedItems, out Coordinate[] changedBarPivots);
        foreach (Coordinate pivot in changedBarPivots) {
          updatedPivots.Add((pivot, Player.InventoryTypes.HotBar));
        }
        sucessfullyAddedStacks.AddRange(succesfullyAddedItems);
      }

      modifiedPivots = updatedPivots.ToArray();
      succesfullyAddedUpItems = sucessfullyAddedStacks.ToArray();
      return remainingLeftovers;
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
    public struct LocalPlayerInventoryItemsUpdatedEvent : IEvent {
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
      public LocalPlayerInventoryItemsUpdatedEvent(Coordinate[] modifiedPivots, Player.InventoryTypes updatedInventoryType) {
        this.modifiedPivots = modifiedPivots;
        this.updatedInventoryType = updatedInventoryType;
      }
    }

    /// <summary>
    /// Event for adding items to their pack
    /// </summary>
    public struct LocalPlayerInventoryItemsRemovedEvent : IEvent {
      public string name
        => "Player removed item";

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
      public LocalPlayerInventoryItemsRemovedEvent(Coordinate[] modifiedPivots, Player.InventoryTypes updatedInventoryType) {
        this.modifiedPivots = modifiedPivots;
        this.updatedInventoryType = updatedInventoryType;
      }
    }
  }
}
