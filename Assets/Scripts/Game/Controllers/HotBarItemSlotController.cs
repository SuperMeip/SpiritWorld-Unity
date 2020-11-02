using SpiritWorld.Inventories.Items;
using SpiritWorld.World.Entities.Creatures;
using System;
using UnityEngine;

namespace SpiritWorld.Game.Controllers {
  public class HotBarItemSlotController : MonoBehaviour {

    #region Constants

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
    /// The base color
    /// </summary>
    public Color BaseColor
      = new Color (0, 131, 190);

    /// <summary>
    /// The selected item color
    /// </summary>
    public Color SelectedColor
      = new Color(0, 131, 255);

    /// <summary>
    /// The hover color
    /// </summary>
    public Color HoverColor
      = new Color(80, 131, 30);

    #endregion

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
    /// if this is the current slot we're hovering over with another item
    /// </summary>
    public bool isHoverFocus {
      get;
      private set;
    }

    /// <summary>
    /// If this slot is actie and being used to display an item
    /// </summary>
    public bool isInUse
      => icon != null;

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
        icon = item == null ? ItemIconController.Make(null, transform) : ItemIconController.Make(
          item,
          transform,
          true,
          true,
          barSlotIndex,
          (barSlotIndex, 0),
          World.Entities.Creatures.Player.InventoryTypes.HotBar
        );
      }
      this.barSlotIndex = barSlotIndex;
      this.barSlotCount = barSlotCount;
      updateLocation();
    }

    /// <summary>
    /// remove the item displayed and make this empty
    /// </summary>
    public void removeDisplayedItem() {
      if (icon != null) {
        changeIcon();
      }
    }

    /// <summary>
    /// update the displayed item or it's data
    /// </summary>
    /// <param name="newItem"></param>
    public void updateDisplayedItemTo(int barSlotIndex, int barSlotCount, Item newItem = null) {
      // if the items are the same, just update the stack count
      if (newItem != null) {
        if (newItem.Equals(icon.item)) {
          icon.updateStackCount();
        } else {
          changeIcon(newItem);
        }
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
      icon.setBGColor(SelectedColor);
    }

    /// <summary>
    /// Mark this item unselected
    /// </summary>
    public void markUnselected() {
      isSelected = false;
      icon.resize(DefaultSize);
      icon.setBGColor(BaseColor);
    }

    /// <summary>
    /// mark this item selected
    /// </summary>
    public void markHoverTarget() {
      isHoverFocus = true;
      icon.resize(LargeSize);
      icon.setBGColor(HoverColor);
    }

    /// <summary>
    /// Mark this item unselected
    /// </summary>
    public void unmarkHoverTarget() {
      isHoverFocus = false;
      icon.resize(isSelected ? LargeSize : DefaultSize);
      icon.setBGColor(BaseColor);
    }

    /// <summary>
    /// Set the fade for the distance from the selected item
    /// </summary>
    /// <param name="distanceFromSelectedItem"></param>
    public void updateFadeDistance(int distanceFromSelectedItem) {
      float alpha = distanceFromSelectedItem >= MaxVisibleDistance
        ? 0
        : 1f - (float)distanceFromSelectedItem / MaxVisibleDistance;
      updateOpacity(alpha);
    }

    /// <summary>
    /// Change the icon to a new item or no item
    /// </summary>
    /// <param name="newItem"></param>
    void changeIcon(Item newItem = null) {
      float currentAlpha = icon.currentOpacity;
      Destroy(icon.gameObject);
      icon = newItem == null
        ? ItemIconController.Make(null, transform)
        : ItemIconController.Make(
          newItem,
          transform,
          true,
          true,
          barSlotIndex,
          (barSlotIndex, 0), Player.InventoryTypes.HotBar
        );
      if (isSelected) {
        markSelected();
      }
      updateOpacity(currentAlpha);
    }

    /// <summary>
    /// Update the opacity
    /// </summary>
    /// <param name="alpha"></param>
    void updateOpacity(float alpha) {
      if (alpha <= 0) {
        gameObject.SetActive(false);
      } else {
        if (!gameObject.activeSelf) {
          gameObject.SetActive(true);
        }
        icon.setOpacity(alpha);
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