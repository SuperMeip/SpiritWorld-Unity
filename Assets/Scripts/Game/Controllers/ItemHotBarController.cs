using SpiritWorld.Events;
using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using SpiritWorld.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpiritWorld.Game.Controllers {
  public class ItemHotBarController : MonoBehaviour, IObserver {

    #region Constants

    /// <summary>
    /// The item bar camera
    /// </summary>
    public Camera UICamera;

    #endregion

    /// <summary>
    /// The height of an item slot
    /// </summary>
    const float ItemSlotHeight 
      = HotBarItemSlotController.LargeSize;

    /// <summary>
    /// The unit height of the item bar per slot
    /// </summary>
    const float ItemBarUnitHeight 
      = ItemSlotHeight + HotBarItemSlotController.DefaultSize / 2;

    /// <summary>
    /// Prefab for a slot in the item hot bar
    /// </summary>
    public GameObject HotBarSlotPrefab;

    /// <summary>
    /// The currently selected item of the local player via their hot bar.
    /// </summary>
    public Item selectedItem 
      => getSelectedItem();

    /// <summary>
    /// The index in the item bar that's currently selected
    /// </summary>
    int currentlySelectedItemIndex = 0;

    /// <summary>
    /// The item slot controllers for each item slot
    /// </summary>
    List<HotBarItemSlotController> itemSlotControllers
      = new List<HotBarItemSlotController>();

    /// <summary>
    /// This's transform
    /// </summary>
    RectTransform rectTransform
      => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());
    RectTransform _rectTransform;

    /// <summary>
    /// The inventory this manages for the local player
    /// </summary>
    ItemBar barInventory
    => Universe.LocalPlayer.hotBarInventory;

    /// <summary>
    /// Test bar
    /// </summary>
    public static ItemBar TestStartingItemBar
      = new ItemBar(8, 1, new Item[] {
        new Item(Item.Types.AutoToolShortcut),
        new Item(Item.Types.Spapple, 2),
        new Item(Item.Types.Iron, 2)
    });

    #region Initialization

    /// <summary>
    /// Populate the bar with our initial items, hide ones too far away.
    /// </summary>
    void Start() {
      // build the background of the bar based on the number of items to show
      currentlySelectedItemIndex = 0;
      updateBarSlotCount(barInventory.activeBarSlotCount);
      /// initialize all the slots
      for (int currentItemIndex = 0; currentItemIndex < barInventory.activeBarSlotCount; currentItemIndex++) {
        addItemSlotFor(currentItemIndex);
      }
    }

    #endregion

    #region Update Loop

    // Update is called once per frame
    void Update() {
      scroll();
    }

    /// <summary>
    /// Check if we should scroll and do so
    /// </summary>
    void scroll() {
      if (!ItemInventoryDragController.AnItemIsBeingDragged && !Input.GetButton("Rotate Camera Lock")) {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        int previousSelectedItemIndex = currentlySelectedItemIndex;
        // data value change and actual movement of the bar
        if (scrollDelta > 0) {
          tryToScrollUp();
        } else if (scrollDelta < 0) {
          tryToScrollDown();
        }

        /// visual changes to item slots
        if (currentlySelectedItemIndex != previousSelectedItemIndex) {
          int itemSlotControllerIndex = 0;
          foreach (HotBarItemSlotController itemSlotController in itemSlotControllers) {
            if (itemSlotController.isInUse) {
              // size
              if (itemSlotControllerIndex == currentlySelectedItemIndex) {
                itemSlotController.markSelected();
              } else if (itemSlotController.isSelected) {
                itemSlotController.markUnselected();
              }

              // fade
              itemSlotController.updateFadeDistance(Math.Abs(itemSlotControllerIndex++ - currentlySelectedItemIndex));
            }
          }
        }
      }
    }

    /// <summary>
    /// Try scrolling down
    /// </summary>
    void tryToScrollDown() {
      if (currentlySelectedItemIndex > 0) {
        currentlySelectedItemIndex--;
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y - ItemSlotHeight);
      }
    }

    /// <summary>
    /// try scrolling up
    /// </summary>
    void tryToScrollUp() {
      if (currentlySelectedItemIndex < barInventory.usedBarSlotCount - 1) {
        currentlySelectedItemIndex++;
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + ItemSlotHeight);
      }
    }

    #endregion

    #region Modify and Control Slots

    /// <summary>
    /// add an item slot from the bottom
    /// </summary>
    void incrementBarSlotsFromBottom() {
      updateBarSlotCount(barInventory.activeBarSlotCount);
      // arange the existing ones
      for(int currentItemIndex = 0; currentItemIndex < barInventory.activeBarSlotCount - 1; currentItemIndex++) {
         HotBarItemSlotController slotController = itemSlotControllers[currentItemIndex];
        slotController.updateDisplayedItemTo(currentItemIndex, barInventory.activeBarSlotCount);
      }
      // add the new one at the bottom
      addItemSlotFor(barInventory.activeBarSlotCount - 1);
    }

    /// <summary>
    /// Stretch the item bar and space it around the given index, used for inserting items in the bar
    /// </summary>
    /// <param name="spaceIndex"></param>
    void spaceItemsAroundIndex(int spaceIndex) {
      updateBarSlotCount(barInventory.activeBarSlotCount + 1);
      for (int currentItemIndex = 0; currentItemIndex < barInventory.activeBarSlotCount; currentItemIndex++) {
          HotBarItemSlotController slotController = itemSlotControllers[currentItemIndex];
        if (currentItemIndex > spaceIndex) {
          slotController.updateDisplayedItemTo(currentItemIndex, barInventory.activeBarSlotCount + 1);
        } else {
          slotController.updateDisplayedItemTo(currentItemIndex + 1, barInventory.activeBarSlotCount + 1);
        }
      }
    }

    /// <summary>
    /// Realign the items, fixing any stretching etc
    /// </summary>
    /// <param name="spaceIndex"></param>
    void realignItemSlots() {
      updateBarSlotCount(barInventory.activeBarSlotCount);
      for (int currentItemIndex = 0; currentItemIndex < barInventory.activeBarSlotCount; currentItemIndex++) {
        HotBarItemSlotController slotController = itemSlotControllers[currentItemIndex];
        slotController.updateDisplayedItemTo(currentItemIndex, barInventory.activeBarSlotCount);
      }
    }

    /// <summary>
    /// resize the bar for a new slot count
    /// </summary>
    /// <param name="newSlotCount"></param>
    void updateBarSlotCount(int newSlotCount) {
      rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (newSlotCount - 1) * ItemBarUnitHeight);
    }

    /// <summary>
    /// Add a slot for the current index.
    /// </summary>
    /// <param name="currentItemIndex"></param>
    void addItemSlotFor(int currentItemIndex) {
      itemSlotControllers.Add(Instantiate(HotBarSlotPrefab, transform).GetComponent<HotBarItemSlotController>());
      HotBarItemSlotController slotController = itemSlotControllers[currentItemIndex];
      Item item = barInventory.getItemAt(currentItemIndex);
      if (item != null) {
        slotController.setDisplayedItem(item, currentItemIndex, barInventory.activeBarSlotCount);
        if (currentItemIndex == currentlySelectedItemIndex) {
          slotController.markSelected();
        } else {
          slotController.markUnselected();
        }

        slotController.updateFadeDistance(Math.Abs(currentItemIndex - currentlySelectedItemIndex));
      }
    }

    /// <summary>
    /// Try to get the item slot controller at the given slot
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="itemSlotController"></param>
    /// <returns></returns>
    bool tryToGetSlot(int slotIndex, out HotBarItemSlotController itemSlotController) {
      itemSlotController = slotIndex < itemSlotControllers.Count
        ? itemSlotControllers[slotIndex]
        : null;
      return itemSlotController == null;
    }

    /// <summary>
    /// Get the item the player is currently selecting. This takes into account shortcuts.
    /// </summary>
    /// <returns></returns>
    Item getSelectedItem() {
      Item selectedItem = barInventory.getItemAt(currentlySelectedItemIndex);
      // if the item is a shortcut, use it to find the item we want from the inventory
      if (selectedItem is IHotBarItemShortcut itemBarUtility) {
        selectedItem = itemBarUtility.tryToFindMatchIn(
          Universe.LocalPlayer.inventories,
          (Universe.LocalPlayerManager.TileSelectionManager.selectedTile, Universe.LocalPlayerManager.TileSelectionManager.selectedTileFeatures),
          out Item matchingItem
        ) ? matchingItem : null;
      }

      return selectedItem;
    }

    #endregion

    #region IObserver

    /// <summary>
    /// Receive notifications
    /// </summary>
    /// <param name="event"></param>
    public void notifyOf(IEvent @event) {
      switch (@event) {
        case PlayerManager.PackInventoryItemsUpdatedEvent pcPIIUE:
          if (pcPIIUE.updatedInventoryType == World.Entities.Creatures.Player.InventoryTypes.HotBar) {
            foreach (Coordinate updatedItemPivot in pcPIIUE.modifiedPivots) {
              // this is for hot bar, not pockets
              if (!barInventory.isInPockets(updatedItemPivot)) {
                // if it's an empty slot, just toss it in
                if (updatedItemPivot.x >= itemSlotControllers.Count) {
                  incrementBarSlotsFromBottom();
                }
                HotBarItemSlotController slotController = itemSlotControllers[updatedItemPivot.x];
                // if the slot is being used, we need to update it
                if (slotController.isInUse) {
                  itemSlotControllers[updatedItemPivot.x].updateDisplayedItemTo(updatedItemPivot.x, barInventory.activeBarSlotCount, barInventory.getItemAt(updatedItemPivot.x));
                  // if the slot isn't being used, lets nab it and set it.
                } else {
                  slotController.setDisplayedItem(barInventory.getItemAt(updatedItemPivot.x), updatedItemPivot.x, barInventory.activeBarSlotCount);
                  slotController.updateFadeDistance(Math.Abs(updatedItemPivot.x - currentlySelectedItemIndex));
                  slotController.markUnselected();
                }
              }
            }
          }
          break;
        default:
          break;
      }
    }

    #endregion
  }
}