using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SpiritWorld.Game.Controllers {
  public class PlayerPackGridController : MonoBehaviour {

    /// <summary>
    /// The tile texture for the grid.
    /// </summary>
    public Texture2D GridTileTexture;

    /// <summary>
    /// The background image for the item grid
    /// </summary>
    public AspectRatioFitter backgroundAspectRatio;

    /// <summary>
    /// dimensions for the texture
    /// </summary>
    public Coordinate GridTileDimensions 
      => (GridTileTexture.width, GridTileTexture.height);

    /// <summary>
    /// The background image for the item grid
    /// </summary>
    Image gridImage;

    /// <summary>
    /// The texture we use for the item grid
    /// </summary>
    Texture2D itemGridTexture;

    /// <summary>
    /// The aspect ratio filter
    /// </summary>
    AspectRatioFitter aspectRatioFitter;

    /// <summary>
    /// The inventory this manages for the local player
    /// </summary>
    ShapedPack packInventory;
    //=> Universe.LocalPlayer.packInventory;

    /// <summary>
    /// consts and connections
    /// </summary>
    void Awake() {
      packInventory = new ShapedPack((5, 7), new (Item, Coordinate)[] {
        (new Item(Item.Types.Iron, 2), (0,0)),
        (new Item(Item.Types.Iron, 1), (2,2)),
        (new Item(Item.Types.Wood, 2), (2,5))
      });
      gridImage = GetComponent<Image>();
      aspectRatioFitter = GetComponent<AspectRatioFitter>();
    }

    /// <summary>
    /// Set up the grid based on player inventory
    /// </summary>
    void Start() {
      updateGridSize();
      populateGridFromPlayerInventory();
    }

    void updateGridSize() {
      // make the image texture
      itemGridTexture = new Texture2D(
        GridTileTexture.width * packInventory.dimensions.x,
        GridTileTexture.height * packInventory.dimensions.y
      );
      aspectRatioFitter.aspectRatio = (float)packInventory.dimensions.x / (float)packInventory.dimensions.y;
      backgroundAspectRatio.aspectRatio = aspectRatioFitter.aspectRatio;
      itemGridTexture.filterMode = FilterMode.Point;

      // add each slot to the texture's pixel collection.
      // loop though each image spot in the grid
      Coordinate.Zero.until(packInventory.dimensions, textureGridLocation => {
        // loop though each pixel in that image.
        Coordinate.Zero.until(GridTileDimensions, localPixelOffset => {
          Coordinate test = localPixelOffset;
          (int x, int y) = (textureGridLocation * GridTileDimensions) + localPixelOffset;
          Color pixel = GridTileTexture.GetPixel(localPixelOffset.x, localPixelOffset.y);
          itemGridTexture.SetPixel(x, y, pixel);
        });
      });

      // set all the pixels we got and make a sprite out of it
      itemGridTexture.Apply();
      gridImage.sprite = Sprite.Create(
        itemGridTexture,
        new Rect(
          0,
          0,
          itemGridTexture.width,
          itemGridTexture.height
        ),
        new Vector2(0.5f, 0.5f)
      );
    }

    /// <summary>
    /// create and place each inventory item
    /// </summary>
    void populateGridFromPlayerInventory() {
      packInventory.forEach((pivot, stack) => {
        ItemIconController iconController = ItemIconController.Make(stack, transform, true);
        iconController.setShaped(true);
        placeIcon(iconController, pivot);
      });
    }

    /// <summary>
    /// Place the given shaped item icon at the given grid location
    /// </summary>
    /// <param name="iconController"></param>
    /// <param name="gridLocation"></param>
    void placeIcon(ItemIconController iconController, Coordinate gridLocation) {
      RectTransform iconTransform = iconController.GetComponent<RectTransform>();
      float gridUnitWidth = (float)1 / (float)packInventory.dimensions.x;
      float gridUnitHeight = (float)1 / (float)packInventory.dimensions.y;
      iconTransform.anchorMin = new Vector2(
        gridLocation.x * gridUnitWidth,
        gridLocation.y * gridUnitHeight
      );
      iconTransform.anchorMax 
        = iconTransform.anchorMin + new Vector2(gridUnitWidth, gridUnitHeight);
      iconTransform.SetLTRB(0);
    }
  }
}
