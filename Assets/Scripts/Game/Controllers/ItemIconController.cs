using SpiritWorld.Inventories.Items;
using UnityEngine;

namespace SpiritWorld.Game.Controllers {
  public class ItemIconController : MonoBehaviour {

    /// <summary>
    /// Default diameter of an icon
    /// </summary>
    public const float DefaultIconDiameter = 50f;

    /// <summary>
    /// Default scale of a model icon
    /// </summary>
    const float DefaultModelScale = 200f;

    /// <summary>
    /// Renderers used to modify model based icons
    /// </summary>
    public Renderer[] itemModelRenderers;

    /// <summary>
    /// The model being used as an icon if there is one
    /// </summary>
    public GameObject itemModel;

    /// <summary>
    /// The canvas group
    /// </summary>
    CanvasGroup canvasGroup 
      => _canvasGroup ?? (_canvasGroup = GetComponent<CanvasGroup>());
    CanvasGroup _canvasGroup;

    /// <summary>
    /// This's transform
    /// </summary>
    RectTransform rectTransform 
      => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());
    RectTransform _rectTransform;

    /// <summary>
    /// Make a new item icon for the given item
    /// </summary>
    /// <returns></returns>
    public static ItemIconController Make(Item item, Transform parent = null) {
      GameObject icon = parent != null
        ? Instantiate(ItemDataMapper.ItemIconPrefab, parent)
        : Instantiate(ItemDataMapper.ItemIconPrefab);

      Sprite itemSprite = ItemDataMapper.GetIconFor(item);
      ItemIconController iconController = icon.GetComponent<ItemIconController>();

      // if we found a sprite
      GameObject iconScaler;
      if (itemSprite != null) {
        iconScaler = icon.transform.Find("Small Icon Scaler").gameObject;
        GameObject sprite = Instantiate(new GameObject(), iconScaler.transform);
        sprite.layer = 5;
        SpriteRenderer spriteRenderer = sprite.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemSprite;
      // if we didn't, use the object as an icon.
      } else {
        iconScaler = icon.transform.Find("Model Icon Scaler").gameObject;
        GameObject itemModel = Instantiate(ItemDataMapper.GetModelFor(item), iconScaler.transform);
        iconController.itemModel = iconScaler;
        iconController.itemModelRenderers = itemModel.GetComponentsInChildren<Renderer>();
      }

      iconScaler.SetActive(true);
      return iconController;
    }

    /// <summary>
    /// set the opacity of the item icon
    /// </summary>
    /// <param name="alpha"></param>
    public void setOpacity(float alpha = 1.0f) {
      canvasGroup.alpha = alpha;
      if (itemModelRenderers != null) {
        foreach (Renderer renderer in itemModelRenderers) {
          Color color = renderer.material.color;
          color.a = alpha;
          renderer.material.SetColor("_BaseColor", color);
        }
      }
    }

    /// <summary>
    /// Resize the icon. Base is set to the default.
    /// </summary>
    /// <param name="diameter"></param>
    public void resize(float diameter = 50f) {
      rectTransform.sizeDelta = new Vector2(diameter, diameter);
      if (itemModel != null) {
        float modelScale = (diameter / DefaultIconDiameter) * DefaultModelScale;
        itemModel.transform.localScale = new Vector3(modelScale, modelScale, modelScale);
      }
    }
  }
}