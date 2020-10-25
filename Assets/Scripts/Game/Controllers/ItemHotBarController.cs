using SpiritWorld.Events;
using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using SpiritWorld.Managers;
using System;
using UnityEngine;

namespace SpiritWorld.Game.Controllers {
  public class ItemHotBarController : MonoBehaviour, IObserver {

    /// <summary>
    /// The height of an item slot
    /// </summary>
    const float ItemSlotHeight = 75;

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
    HotBarItemSlotController[] itemSlotControllers;

    /// <summary>
    /// The transform of the item bar
    /// </summary>
    RectTransform rectTransform;

    /// <summary>
    /// The inventory this manages for the local player
    /// </summary>
    ItemBar barInventory
#if UNITY_EDITOR
    => TestStartingItemBar;

    /// <summary>
    /// Test bar
    /// </summary>
    public static ItemBar TestStartingItemBar
      = new ItemBar(10, 1, new Item[] {
        new Item(Item.Types.AutoToolShortcut),
        new Item(Item.Types.Spapple, 2),
        new Item(Item.Types.WaterLily, 2),
        new Item(Item.Types.Spapple, 2),
        new Item(Item.Types.Spapple, 2),
        new Item(Item.Types.Iron, 2),
        new Item(Item.Types.Spapple, 2),
        new Item(Item.Types.Spapple, 2),
        new Item(Item.Types.Stone, 2),
        new Item(Item.Types.PineCone, 2),
        new Item(Item.Types.Spapple, 2)
    });
#else
    => Universe.LocalPlayer.hotBarInventory;
#endif

    /// <summary>
    /// Get the consts and associations
    /// </summary>
    private void Awake() {
      itemSlotControllers = GetComponentsInChildren<HotBarItemSlotController>(true);
      rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Populate the bar with our initial items, hide ones too far away.
    /// </summary>
    void Start() {
      currentlySelectedItemIndex = 0;

      /// initialize all the slots
      for (int currentItemIndex = 0; currentItemIndex < barInventory.barSize; currentItemIndex++) {
        HotBarItemSlotController itemController = itemSlotControllers[currentItemIndex];
        Item item = barInventory.getItemAt(currentItemIndex);
        if (item != null) {
          itemController.setItem(item, currentItemIndex);
          if (currentItemIndex == currentlySelectedItemIndex) {
            itemController.markSelected();
          } else {
            itemController.markUnselected();
          }

          itemController.setFadeDistance(Math.Abs(currentItemIndex - currentlySelectedItemIndex));
        }
      }
    }

    // Update is called once per frame
    void Update() {
      scroll();
    }

    /// <summary>
    /// Check if we should scroll and do so
    /// </summary>
    void scroll() {
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
        foreach(HotBarItemSlotController itemSlotController in itemSlotControllers) {
          // size
          if (itemSlotControllerIndex == currentlySelectedItemIndex) {
            itemSlotController.markSelected();
          } else if (itemSlotController.isSelected) {
            itemSlotController.markUnselected();
          }

          // fade
          itemSlotController.setFadeDistance(Math.Abs(itemSlotControllerIndex++ - currentlySelectedItemIndex));
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

    /// <summary>
    /// Not yet implimented
    /// </summary>
    /// <param name="event"></param>
    public void notifyOf(IEvent @event) {}
  }
}