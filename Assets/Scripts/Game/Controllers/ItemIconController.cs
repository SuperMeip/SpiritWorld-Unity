using SpiritWorld.Inventories.Items;
using UnityEngine;

namespace SpiritWorld.Controllers {
  public class ItemIconController : MonoBehaviour {

    /// <summary>
    /// Renderers used to modify model based icons
    /// </summary>
    Renderer[] itemModelRenderers;

    /// <summary>
    /// The alpha of this item icon atm.
    /// </summary>
    float currentAlpha = 1.0f;

    /// <summary>
    /// The canvas group
    /// </summary>
    CanvasGroup canvasGroup 
      => _canvasGroup ?? (_canvasGroup = GetComponent<CanvasGroup>());
    CanvasGroup _canvasGroup;

    /// <summary>
    /// Make a new item icon for the given item
    /// </summary>
    /// <returns></returns>
    public static ItemIconController Make(Item item, Transform parent = null) {
      GameObject icon = parent != null
        ? Instantiate(ItemDataMapper.ItemIconPrefab, parent)
        : Instantiate(ItemDataMapper.ItemIconPrefab);

      Sprite itemSprite = ItemDataMapper.GetIconFor(item);

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
        Renderer[] itemModelRenderers = itemModel.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in itemModelRenderers) {
          renderer.material.shader = ItemDataMapper.ItemUIShader;
        }
      }

      iconScaler.SetActive(true);
      return icon.GetComponent<ItemIconController>();
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
          renderer.material.color = color;
        }
      }
    }
  }
}