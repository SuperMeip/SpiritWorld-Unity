using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using UnityEngine;
using UnityEngine.UI;

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
    /// The item this is for
    /// </summary>
    public Item item {
      get;
      private set;
    }

    /// <summary>
    /// If this is currently a shaped icon
    /// </summary>
    public bool isShaped {
      get;
      private set;
    }

    /// <summary>
    /// The id of the stack if this is being used in an inventory
    /// </summary>
    public int stackIndex {
      get;
      private set;
    }

    /// <summary>
    /// Renderers used to modify model based icons
    /// </summary>
    Renderer[] itemModelRenderers;

    /// <summary>
    /// default icon scaler. This is the item model if it's a model, or the small icon sprite's container.
    /// </summary>
    GameObject defaultIconScaler;

    /// <summary>
    /// The shaped icon scaler
    /// </summary>
    GameObject shapedIconScaler;

    /// <summary>
    /// The background image of the icon
    /// </summary>
    Image backgroundImage;

    /// <summary>
    /// The color this icon's BG was previously
    /// </summary>
    Color? previousBGColor; 

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
    /// The indicator object for stacksize
    /// </summary>
    RectTransform itemStackSizeIndicator;

    /// <summary>
    /// The text object for the stack size
    /// </summary>
    Text itemStackSizeIndicatorText;

    /// <summary>
    /// If this uses a model as it's icon
    /// </summary>
    bool hasAModelIcon 
      => itemModelRenderers != null;

    /// <summary>
    /// Make a new item icon for the given item
    /// </summary>
    /// <returns></returns>
    public static ItemIconController Make(
      Item item,
      Transform parent = null,
      int stackIndex = GridBasedInventory.EmptyGridSlot,
      bool loadShapedIcon = false
    ) {
      // make the icon under the given parent, or alone if we want
      GameObject icon = parent != null
        ? Instantiate(ItemDataMapper.ItemIconPrefab, parent)
        : Instantiate(ItemDataMapper.ItemIconPrefab);

      Sprite itemSprite = ItemDataMapper.GetIconFor(item);
      ItemIconController iconController = icon.GetComponent<ItemIconController>();
      iconController.item = item;
      iconController.stackIndex = stackIndex;
      iconController.backgroundImage = icon.transform.Find("Icon Background").GetComponent<Image>();

      /// if we found a sprite
      if (itemSprite != null) {
        /// load the regular icon
        iconController.defaultIconScaler = icon.transform.Find("Small Icon Scaler").gameObject;
        GameObject sprite = Instantiate(new GameObject(), iconController.defaultIconScaler.transform);
        sprite.layer = 5;
        SpriteRenderer spriteRenderer = sprite.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemSprite;
      // if we didn't, use the object as an icon.
      } else {
        iconController.defaultIconScaler = icon.transform.Find("Model Icon Scaler").gameObject;
        GameObject itemModel = Instantiate(ItemDataMapper.GetModelFor(item), iconController.defaultIconScaler.transform);
        iconController.itemModelRenderers = itemModel.GetComponentsInChildren<Renderer>();
      }

      /// if we're also loading the shaped icon:
      if (loadShapedIcon) {
        // get the shaped scaler
        iconController.shapedIconScaler = icon.transform.Find("Shaped Icon Scaler").gameObject;

        // make the prototype image object
        GameObject imageObject = new GameObject {
          layer = 5
        };
        imageObject.AddComponent<Image>();
        // resize the sprite according to it's shape size.
        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
        rectTransform.anchorMax = (item.type.ShapeSize - item.type.ShapePivot).Vec2;
        rectTransform.anchorMin = ((0, 0) - item.type.ShapePivot).Vec2;
        rectTransform.SetLTRB(0);

        // if we need a new icon for the shaped icon than the basic square icon, get it
        if (item.type.ShapeSize > (1, 1)) {
          Sprite shapedIcon = ItemDataMapper.GetIconFor(item, true);
          Image shapedIconImage = Instantiate(imageObject, iconController.shapedIconScaler.transform).GetComponent<Image>();
          shapedIconImage.sprite = shapedIcon;
        }

        // add the outline
        Image outline = Instantiate(imageObject, iconController.shapedIconScaler.transform).GetComponent<Image>();
        outline.sprite = ItemDataMapper.GetShapedOutlineFor(item);
        outline.color = new Color(1, 1, 0);
      }

      // set the correct icon scaler active
      iconController.defaultIconScaler.SetActive(true);

      /// set up the stack indicator
      if (item.type.StackSize > 1) {
        iconController.itemStackSizeIndicator = icon.transform.Find("Stack Quantity Indicator").GetComponent<RectTransform>();
        iconController.itemStackSizeIndicator.gameObject.SetActive(true);
        iconController.itemStackSizeIndicatorText = iconController.itemStackSizeIndicator.GetComponentInChildren<Text>();
        iconController.updateStackCount();
      }

      return iconController;
    }

    /// <summary>
    /// set the opacity of the item icon
    /// </summary>
    /// <param name="alpha"></param>
    public void setOpacity(float alpha = 1.0f) {
      canvasGroup.alpha = alpha;
      if (hasAModelIcon) {
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
      if (hasAModelIcon) {
        float modelScale = (diameter / DefaultIconDiameter) * DefaultModelScale;
        defaultIconScaler.transform.localScale = new Vector3(modelScale, modelScale, modelScale);
      }
    }

    /// <summary>
    /// Toggle between shaped and non-shaped icon
    /// </summary>
    /// <param name="setShaped"></param>
    public void setShaped(bool isShaped = true) {
      this.isShaped = isShaped;
      if (shapedIconScaler != null) {
        bool itemUsesSeperateShapedIcon
          = item.type.ShapeSize > (1, 1);

        /// toggle shaped on
        if (isShaped) {
          shapedIconScaler.SetActive(true);
          backgroundImage.gameObject.SetActive(false);
          if (itemUsesSeperateShapedIcon) {
            defaultIconScaler.SetActive(false);

            // Move the stack qty icon if nessisary
            if (item.type.StackSize > 1) {
              itemStackSizeIndicator.anchorMax = new Vector2(
                itemStackSizeIndicator.anchorMax.x + item.type.ShapeSize.x - 1,
                itemStackSizeIndicator.anchorMax.y
              );
              itemStackSizeIndicator.anchorMin = new Vector2(
                itemStackSizeIndicator.anchorMin.x + item.type.ShapeSize.x - 1,
                itemStackSizeIndicator.anchorMin.y
              );
              itemStackSizeIndicator.SetLTRB(0);
            }
          }

        /// toggle shaped icon off
        } else {
          shapedIconScaler.SetActive(false);
          backgroundImage.gameObject.SetActive(true);
          if (itemUsesSeperateShapedIcon) {
            defaultIconScaler.SetActive(true);

            // Move the stack qty icon if nessisary
            if (item.type.StackSize > 1) {
              itemStackSizeIndicator.anchorMax = new Vector2(
                itemStackSizeIndicator.anchorMax.x - item.type.ShapeSize.x - 1,
                itemStackSizeIndicator.anchorMax.y
              );
              itemStackSizeIndicator.anchorMin = new Vector2(
                itemStackSizeIndicator.anchorMin.x - item.type.ShapeSize.x - 1,
                itemStackSizeIndicator.anchorMin.y
              );
              itemStackSizeIndicator.SetLTRB(0);
            }
          }
        }
      }
    }

    /// <summary>
    /// Parent this to a new item
    /// </summary>
    /// <param name="parent"></param>
    public void parentTo(Transform parent) {
      transform.SetParent(parent, false);
    }

    /// <summary>
    /// update the stack count of this item
    /// </summary>
    public void updateStackCount() {
      if (item.type.StackSize > 1) {
        itemStackSizeIndicatorText.text = item.quantity != 100 
          ? item.quantity.ToString() 
          : "C";
      }
    }

    /// <summary>
    /// Set the color of the icon's current background image
    /// </summary>
    /// <param name="color"></param>
    public void setBGColor(Color? color = null) {
      Color colorToSet = color ?? previousBGColor ?? backgroundImage.color;
      previousBGColor = backgroundImage.color;
      backgroundImage.color = colorToSet;
    }
  }
}

/// <summary>
/// rectangle transform extentions
/// </summary>
public static class RectTransformExtensions {
  public static void SetLeft(this RectTransform rt, float left) {
    rt.offsetMin = new Vector2(left, rt.offsetMin.y);
  }

  public static void SetRight(this RectTransform rt, float right) {
    rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
  }

  public static void SetTop(this RectTransform rt, float top) {
    rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
  }

  public static void SetBottom(this RectTransform rt, float bottom) {
    rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
  }

  /// <summary>
  /// Set all of them
  /// </summary>
  /// <param name="rt"></param>
  /// <param name="all"></param>
  public static void SetLTRB(this RectTransform rt, float all) {
    rt.offsetMin = new Vector2(all, all);
    rt.offsetMax = new Vector2(-all, -all);
  }
}