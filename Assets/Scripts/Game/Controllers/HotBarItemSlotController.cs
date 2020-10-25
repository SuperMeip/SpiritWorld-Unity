using SpiritWorld.Inventories.Items;
using UnityEngine;

namespace SpiritWorld.Game.Controllers {
  public class HotBarItemSlotController : MonoBehaviour {

    /// <summary>
    /// Enlarged/selected icon size
    /// </summary>
    const float LargeSize = 75;

    /// <summary>
    /// Enlarged/selected icon size
    /// </summary>
    const float DefaultSize 
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
    /// The icon currently being shown
    /// </summary>
    ItemIconController icon;

    /// <summary>
    /// Set the item on this slot
    /// </summary>
    /// <param name="item"></param>
    public void setDisplayedItem(Item item, int barSlotIndex) {
      if (item != null) {
        gameObject.SetActive(true);
        icon = ItemIconController.Make(item, transform);
      }
      this.barSlotIndex = barSlotIndex;
    }

    /// <summary>
    /// update the displayed item or it's data
    /// </summary>
    /// <param name="newItem"></param>
    internal void updateDisplayedItemTo(Item newItem) {
      // if the items are the same, just update the stack count
      if (newItem.Equals(icon.item)) {
        icon.updateStackCount();
      } else {
        icon = ItemIconController.Make(newItem, transform);
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
    public void setFadeDistance(int distanceFromSelectedItem) {
      if (distanceFromSelectedItem >= MaxVisibleDistance) {
        gameObject.SetActive(false);
      } else {
        if (!gameObject.activeSelf) {
          gameObject.SetActive(true);
        }
        icon.setOpacity(1f - (float)distanceFromSelectedItem / MaxVisibleDistance);
      }
    }
  }
}