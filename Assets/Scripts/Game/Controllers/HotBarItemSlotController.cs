using SpiritWorld.Inventories.Items;
using UnityEngine;

namespace SpiritWorld.Controllers {
  public class HotBarItemSlotController : MonoBehaviour {

    /// <summary>
    /// Enlarged/selected icon size
    /// </summary>
    const float LargeSize = 75;

    /// <summary>
    /// Enlarged/selected icon size
    /// </summary>
    const float DefaultSize = 50;

    /// <summary>
    /// Max distance at which faded items can be seen still
    /// </summary>
    const int MaxVisibleDistance = 4;

    /// <summary>
    /// if this the selected item
    /// </summary>
    public bool isSelected {
      get;
      private set;
    }

    /// <summary>
    /// This's transform
    /// </summary>
    RectTransform rectTransform =>  _rectTransform ?? (_rectTransform = icon.GetComponent<RectTransform>());
    RectTransform _rectTransform;

    /// <summary>
    /// The canvas group
    /// </summary>
    CanvasGroup canvasGroup => _canvasGroup ?? (_canvasGroup = GetComponent<CanvasGroup>());
    CanvasGroup _canvasGroup;

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
      rectTransform.sizeDelta = new Vector2(LargeSize, LargeSize);
    }

    /// <summary>
    /// Mark this item unselected
    /// </summary>
    public void markUnselected() {
      isSelected = false;
      rectTransform.sizeDelta = new Vector2(DefaultSize, DefaultSize);
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
        canvasGroup.alpha = 1 - distanceFromSelectedItem / MaxVisibleDistance;
      }
    }
  }
}