using SpiritWorld.Events;
using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using SpiritWorld.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SpiritWorld.Game.Controllers {
  public class ItemHotBarController : MonoBehaviour, IObserver {

    #region Constants

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
    /// The item bar camera
    /// </summary>
    public Camera UICamera;

    /// <summary>
    /// The canvas for this
    /// </summary>
    public Canvas Canvas;

    /// <summary>
    /// Prefab for a slot in the item hot bar
    /// </summary>
    public GameObject HotBarSlotPrefab;

    #endregion

#if UNITY_EDITOR

    /// <summary>
    /// For testing expanding item slots on the bar
    /// </summary>
    public Vector2 testMouseLocation;

    /// <summary>
    /// For testing expanding item slots on the bar
    /// </summary>
    public bool testSlotIsExpanded;

#endif

    /// <summary>
    /// The target object for items to be dropped into for this inventory
    /// </summary>
    public GameObject dropTarget
      => transform.parent.gameObject;

    /// <summary>
    /// The currently selected item of the local player via their hot bar.
    /// </summary>
    public Item selectedItem
      => getSelectedItem();

    /// <summary>
    /// This's transform
    /// </summary>
    public RectTransform rectTransform
      => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());
    RectTransform _rectTransform;

    /// <summary>
    /// how many item slots are currently visible
    /// </summary>
    public int visibleItemSlots
      => itemSlotControllers.Select(c => c.isInUse).Count();

    /// <summary>
    /// The index in the item bar that's currently selected
    /// </summary>
    int currentlySelectedItemIndex = 0;

    /// <summary>
    /// The index in the item bar that's currently hovered over with another item
    /// </summary>
    int currentlyTargetedItemIndex = GridBasedInventory.EmptyGridSlot;

    /// <summary>
    /// The item slot controllers for each item slot
    /// </summary>
    List<HotBarItemSlotController> itemSlotControllers
      = new List<HotBarItemSlotController>();

    /// <summary>
    /// The inventory this manages for the local player
    /// </summary>
    ItemBar barInventory
    => Universe.LocalPlayer.hotBarInventory;

    /// <summary>
    /// the expanded bar slot if we have one atm
    /// </summary>
    int expandedBarSlot = GridBasedInventory.EmptyGridSlot;

    /// <summary>
    /// Test bar
    /// </summary>
    public static ItemBar TestStartingItemBar
      = new ItemBar(12, 1, new Item[] {
        new Item(Item.Types.AutoToolShortcut),
        new Item(Item.Types.Spapple, 2),
        new Item(Item.Types.Iron, 2),
        new Item(Item.Types.PineCone, 2),
        new Item(Item.Types.PineCone, 2),
        new Item(Item.Types.PineCone, 2),
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
      if (currentlySelectedItemIndex < barInventory.activeBarSlotCount - 1) {
        currentlySelectedItemIndex++;
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + ItemSlotHeight);
      }
    }

    #endregion

    #region Visual Changes

    public void hoverOver(Vector2 mousePosition) {
      if (TryToGetItemBarHoverPosition(mousePosition, out short barItemSlot, out float slotOffset) 
        && barItemSlot != ItemInventoryDragController.ItemBeingDragged?.stackId
      ) {
        // if the player is holding in the key to expand the item bar
        if (Input.GetButton("Expand Item Bar")) {
          // if there's currently a targeted item, we need to shrink it first.
          if (currentlyTargetedItemIndex != GridBasedInventory.EmptyGridSlot) {
            itemSlotControllers[currentlyTargetedItemIndex].unmarkHoverTarget();
            currentlyTargetedItemIndex = GridBasedInventory.EmptyGridSlot;
          // once it's shrunk, we can try to select the tile between
          } else {
            int newExpandedSlot = slotOffset > 0.50f ? barItemSlot + 1 : Math.Max(0, (int)barItemSlot);
            if (expandedBarSlot != newExpandedSlot) {
              if (expandedBarSlot != GridBasedInventory.EmptyGridSlot) {
                realignItemSlots();
              }
              expandedBarSlot = newExpandedSlot;
              spaceItemsAroundIndex(expandedBarSlot);
            }
          }
          // if they're not
        } else {
          // if there's an expanded slot, first close it
          if (expandedBarSlot != GridBasedInventory.EmptyGridSlot) {
            realignItemSlots();
            expandedBarSlot = GridBasedInventory.EmptyGridSlot;
          // once it's closed, just select the hovered item.
          } else {
            if (currentlyTargetedItemIndex != barItemSlot) {
              if (currentlyTargetedItemIndex != GridBasedInventory.EmptyGridSlot) {
                itemSlotControllers[currentlyTargetedItemIndex].unmarkHoverTarget();
              }
              currentlyTargetedItemIndex = barItemSlot;
              itemSlotControllers[barItemSlot].markHoverTarget();
            }
          }
        }
      // if we're not hovering
      } else {
        // close and disable anything we need to
        if (currentlyTargetedItemIndex != GridBasedInventory.EmptyGridSlot) {
          itemSlotControllers[currentlyTargetedItemIndex].unmarkHoverTarget();
          currentlyTargetedItemIndex = GridBasedInventory.EmptyGridSlot;
        }
        if (expandedBarSlot != GridBasedInventory.EmptyGridSlot) {
          realignItemSlots();
          expandedBarSlot = GridBasedInventory.EmptyGridSlot;
        }
      }
    }

    /// <summary>
    /// Expand a bar slot based on the mouse position
    /// </summary>
    /// <param name="mousePosition"></param>
    public void checkAndExpandBarSlotAroundMousePosition(Vector2 mousePosition) {
      if ((TryToGetItemBarHoverPosition(mousePosition, out short barItemSlot, out float slotOffset))) {
        // if we're hovering over an expanded slot, just do nothing
        if (barItemSlot != expandedBarSlot) {
          //int potentialBarSlot = slotOffset < 0.50f ? barItemSlot + 1 : Math.Max(0, barItemSlot - 1);
          //if (potentialBarSlot != expandedBarSlot) {
            // if we have no expanded slot yet and we've moused between two existing slots:
            if (expandedBarSlot == GridBasedInventory.EmptyGridSlot && (slotOffset > 0.80f || slotOffset < 0.20f)) {
              // if we have a target index, close it first
              if (currentlyTargetedItemIndex != GridBasedInventory.EmptyGridSlot) {
                itemSlotControllers[currentlyTargetedItemIndex].unmarkHoverTarget();
                currentlyTargetedItemIndex = GridBasedInventory.EmptyGridSlot;
              } else {
                expandedBarSlot = slotOffset > 0.80f ? barItemSlot + 1 : Math.Max(0, (int)barItemSlot);
                spaceItemsAroundIndex(expandedBarSlot);
              }
            // If we're on an item slot
            } else if (slotOffset < 0.8f && slotOffset > 0.20f) {
              // if there was a slot expanded, contract it.
              if (expandedBarSlot >= 0) {
                realignItemSlots();
                expandedBarSlot = GridBasedInventory.EmptyGridSlot;
              // if there's not, we set this tile as the one we're hovering over.
              } else {
                // if we have a target index, close it first
                if (currentlyTargetedItemIndex != GridBasedInventory.EmptyGridSlot) {
                  itemSlotControllers[currentlyTargetedItemIndex].unmarkHoverTarget();
                  currentlyTargetedItemIndex = GridBasedInventory.EmptyGridSlot;
                }
                // mark our current hover target
                currentlyTargetedItemIndex = barItemSlot;
                itemSlotControllers[barItemSlot].markHoverTarget();
              }
           // }
          }
        }
      // if we're off the item bar area, just realign the slots and close out any over targets.
      } else if (expandedBarSlot >= 0) {
        // if we have a target index, close it first
        if (currentlyTargetedItemIndex != GridBasedInventory.EmptyGridSlot) {
          itemSlotControllers[currentlyTargetedItemIndex].unmarkHoverTarget();
          currentlyTargetedItemIndex = GridBasedInventory.EmptyGridSlot;
        }
        realignItemSlots();
        expandedBarSlot = GridBasedInventory.EmptyGridSlot;
      }
    }

    /// <summary>
    /// Realign slot items
    /// </summary>
    public void resetExpandedBarSlots() {
      if (expandedBarSlot != GridBasedInventory.EmptyGridSlot) {
        realignItemSlots();
        expandedBarSlot = GridBasedInventory.EmptyGridSlot;
      }
      /*if (currentlyTargetedItemIndex != GridBasedInventory.EmptyGridSlot) {
        itemSlotControllers[currentlyTargetedItemIndex].unmarkHoverTarget();
        currentlyTargetedItemIndex = GridBasedInventory.EmptyGridSlot;
      }*/
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
      // can't space around the first slot, that would push the whole bar down.
      if (spaceIndex <= 0) {
        return;
      }
      updateBarSlotCount(barInventory.activeBarSlotCount + 1);
      if (spaceIndex >= barInventory.activeBarSlotCount) {
        // TODO: add an empty spot to just the bottom
        return;
      }
      int currentItemPosition = 0;
      for (int currentItemIndex = 0; currentItemIndex < barInventory.activeBarSlotCount; currentItemIndex++) {
        if (currentItemIndex == spaceIndex) {
          currentItemPosition++;
        }
        HotBarItemSlotController slotController = itemSlotControllers[currentItemIndex];
        slotController.updateDisplayedItemTo(currentItemPosition, barInventory.activeBarSlotCount + 1);
        currentItemPosition++;
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
    /// remove the item icon at the given slot
    /// </summary>
    /// <param name="slotIndex"></param>
    void removeItemAtSlot(int slotIndex) {
      if (tryToGetSlot(slotIndex, out HotBarItemSlotController slotController)) {
        slotController.removeDisplayedItem();
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

      return itemSlotController != null;
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

    /// <summary>
    /// try to get the item bar slot we're hovering over
    /// </summary>
    /// <returns></returns>
    public static bool TryToGetItemBarHoverPosition(Vector2 pointerPosition, out short barItemSlot, out float slotOffset) {
      if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
         Universe.LocalPlayerManager.ItemHotBarController.rectTransform,
         pointerPosition,
         Universe.LocalPlayerManager.ItemHotBarController.UICamera,
         out Vector2 localCursor
       )) {
        float barSlotHeight
          = Universe.LocalPlayerManager.ItemHotBarController.rectTransform.rect.height
            / Universe.LocalPlayerManager.ItemHotBarController.visibleItemSlots;

        // if we're above the first slot, we're not in a slot and just reset the position
        if ((localCursor.y <= barSlotHeight / 2) 
          && (localCursor.x > -(Universe.LocalPlayerManager.ItemHotBarController.rectTransform.rect.width / 2))
          && (localCursor.x < Universe.LocalPlayerManager.ItemHotBarController.rectTransform.rect.width / 2)
        ) {
          float barSlotUntrimmed = (-(localCursor.y - barSlotHeight / 2) / barSlotHeight);
          if (barSlotUntrimmed <= Universe.LocalPlayerManager.ItemHotBarController.visibleItemSlots + 1) {
            barItemSlot = (short)barSlotUntrimmed;
            slotOffset = barSlotUntrimmed - (float)Math.Truncate(barSlotUntrimmed);

            return barItemSlot >= 0;
          }
        }
      }

      barItemSlot = 0;
      slotOffset = 0;
      return false;
    }

    #region IObserver

    /// <summary>
    /// Receive notifications
    /// </summary>
    /// <param name="event"></param>
    public void notifyOf(IEvent @event) {
      switch (@event) {
        case PlayerManager.LocalPlayerInventoryItemsUpdatedEvent pcPIIUE:
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
        case PlayerManager.LocalPlayerInventoryItemsRemovedEvent pcPIIRE:
          if (pcPIIRE.updatedInventoryType == World.Entities.Creatures.Player.InventoryTypes.HotBar) {
            foreach (Coordinate updatedItemPivot in pcPIIRE.modifiedPivots) {
              // remove the item icons from the given slots
              if (!barInventory.isInPockets(updatedItemPivot)) {
                removeItemAtSlot(updatedItemPivot.x);
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

#if UNITY_EDITOR

  /// <summary>
  /// Make a button to load for testing
  /// </summary>
  [CustomEditor(typeof(ItemHotBarController))]
  public class ItemHotBarEditor : Editor {
    public override void OnInspectorGUI() {
      DrawDefaultInspector();

      ItemHotBarController hotBarController = (ItemHotBarController)target;
      if (GUILayout.Button(hotBarController.testSlotIsExpanded ? "Contract" : "Expand")) {
        if (hotBarController.testSlotIsExpanded) {
          hotBarController.testSlotIsExpanded = false;
          Universe.LocalPlayerManager.ItemHotBarController.resetExpandedBarSlots();
        } else {
          Universe.LocalPlayerManager.ItemHotBarController.checkAndExpandBarSlotAroundMousePosition(hotBarController.testMouseLocation);
          hotBarController.testSlotIsExpanded = true;
        }
      }
    }
  }

#endif
}