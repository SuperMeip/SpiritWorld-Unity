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
    /// The icon currently being shown
    /// </summary>
    ItemIconController icon;

    /// <summary>
    /// Set the item on this slot
    /// </summary>
    /// <param name="item"></param>
    public void setItem(Item item) {
      if (item != null) {
        gameObject.SetActive(true);
        icon = ItemIconController.Make(item, transform);
      }
    }

    /// <summary>
    /// mark this item selected
    /// </summary>
    public void markSelected() {
      isSelected = true;
      icon.resize(LargeSize);
    }

    /// <summary>
    /// Mark this item unselected
    /// </summary>
    public void markUnselected() {
      isSelected = false;
      icon.resize(DefaultSize);
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