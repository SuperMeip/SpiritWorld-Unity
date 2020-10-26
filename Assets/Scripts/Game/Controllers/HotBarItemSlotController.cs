using SpiritWorld.Inventories.Items;
using SpiritWorld.World.Entities.Creatures;
using UnityEngine;

namespace SpiritWorld.Game.Controllers {
  public class HotBarItemSlotController : MonoBehaviour {

    /// <summary>
    /// Enlarged/selected icon size
    /// </summary>
    public const float LargeSize = 75;

    /// <summary>
    /// Enlarged/selected icon size
    /// </summary>
    public const float DefaultSize 
      = ItemIconController.DefaultIconDiameter;

    /// <summary>
    /// Max distance at which faded items can be seen still
    /// </summary>
    const float MaxVisibleDistance = 4;

    /// <summary>
    /// if this the selected item
    /// </summary>
    public bool isSelected {
      get;
      private set;
    }

    /// <summary>
    /// The index of this in the bar
    /// </summary>
    public int barSlotIndex {
      get;
      private set;
    }

    /// <summary>
    /// If this slot is actie and being used to display an item
    /// </summary>
    public bool isInUse
      => icon?.item != null;

    /// <summary>
    /// This's transform
    /// </summary>
    RectTransform rectTransform 
      => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());
    RectTransform _rectTransform;

    /// <summary>
    /// The icon currently being shown
    /// </summary>
    ItemIconController icon;

    /// <summary>
    /// The number of slots this is adjusted for
    /// </summary>
    int barSlotCount;

    /// <summary>
    /// Set the item on this slot
    /// </summary>
    /// <param name="item"></param>
    public void setDisplayedItem(Item item, int barSlotIndex, int barSlotCount) {
      if (item != null) {
        gameObject.SetActive(true);
        icon = ItemIconController.Make(
          item,
          transform,
          true,
          true,
          barSlotIndex,
          World.Entities.Creatures.Player.InventoryTypes.HotBar
        );
      }
      this.barSlotIndex = barSlotIndex;
      this.barSlotCount = barSlotCount;
      updateLocation();
    }

    /// <summary>
    /// update the displayed item or it's data
    /// </summary>
    /// <param name="newItem"></param>
    internal void updateDisplayedItemTo(int barSlotIndex, int barSlotCount, Item newItem = null) {
      // if the items are the same, just update the stack count
      if (newItem != null && newItem.Equals(icon.item)) {
        icon.updateStackCount();
      } else {
        Destroy(icon.gameObject);
        icon = ItemIconController.Make(newItem, transform, true, true, barSlotIndex, Player.InventoryTypes.HotBar);
      }
      if (barSlotIndex != this.barSlotIndex || barSlotCount != this.barSlotCount) {
        this.barSlotIndex = barSlotIndex;
        this.barSlotCount = barSlotCount;
        updateLocation();
      }
    }

    /// <summary>
    /// mark this item selected
    /// </summary>
    public void markSelected() {
      isSelected = true;
      icon.resize(LargeSize);
      icon.setBGColor(new Color(0, 131, 255));
    }

    /// <summary>
    /// Mark this item unselected
    /// </summary>
    public void markUnselected() {
      isSelected = false;
      icon.resize(DefaultSize);
      icon.setBGColor();
    }

    /// <summary>
    /// Set the fade for the distance from the selected item
    /// </summary>
    /// <param name="distanceFromSelectedItem"></param>
    public void updateFadeDistance(int distanceFromSelectedItem) {
      if (distanceFromSelectedItem >= MaxVisibleDistance) {
        gameObject.SetActive(false);
      } else {
        if (!gameObject.activeSelf) {
          gameObject.SetActive(true);
        }
        icon.setOpacity(1f - (float)distanceFromSelectedItem / MaxVisibleDistance);
      }
    }

    /// <summary>
    /// Set the location based on the index and bar size
    /// </summary>
    /// <param name="index"></param>
    /// <param name="barSize"></param>
    void updateLocation() {
      float newYAnchor = 1 - (float)barSlotIndex / (float)barSlotCount;
      rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, newYAnchor);
      rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, newYAnchor);
      rectTransform.anchoredPosition = new Vector3(0, 0, rectTransform.localPosition.z);
    }
  }
}