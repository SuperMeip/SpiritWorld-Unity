using SpiritWorld.Events;
using SpiritWorld.Inventories;
using SpiritWorld.Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpiritWorld.Game.Controllers {
  public class PlayerPackGridController : MonoBehaviour, IObserver {

    /// <summary>
    /// The tile texture for the grid.
    /// </summary>
    public Texture2D GridTileTexture;

    /// <summary>
    /// The background image aspect ratio filter
    /// </summary>
    public AspectRatioFitter backgroundAspectRatio;

    /// <summary>
    /// dimensions for the texture
    /// </summary>
    public Coordinate GridTileDimensions 
      => (GridTileTexture.width, GridTileTexture.height);

    /// <summary>
    /// Items based on pivot point
    /// </summary>
    Dictionary<Coordinate, ItemIconController> itemsByPivot
      = new Dictionary<Coordinate, ItemIconController>();

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
    ShapedPack packInventory
      => Universe.LocalPlayer.packInventory;

    /// <summary>
    /// consts and connections
    /// </summary>
    void Awake() {
      gridImage = GetComponent<Image>();
      aspectRatioFitter = GetComponent<AspectRatioFitter>();
    }

    #region Update Loop

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
      packInventory.forEach((pivot, stack, stackId) => {
        ItemIconController iconController = ItemIconController.Make(stack, transform, stackId, true);
        iconController.setShaped(true);
        placeIcon(iconController, pivot);
      });
    }

    #endregion

    /// <summary>
    /// Place the given shaped item icon at the given grid location
    /// </summary>
    /// <param name="iconController"></param>
    /// <param name="gridLocation"></param>
    void placeIcon(ItemIconController iconController, Coordinate gridLocation) {
      // create and store the new icon
      // TODO: one day i'll replacve all the instantiates with pools
      RectTransform iconTransform = iconController.GetComponent<RectTransform>();
      itemsByPivot[gridLocation] = iconController;

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

    /// <summary>
    /// Receive notifications
    /// </summary>
    /// <param name="event"></param>
    public void notifyOf(IEvent @event) {
      switch (@event) {
        case PlayerManager.PackInventoryItemsUpdatedEvent pcPIIUE:
          if (pcPIIUE.updatedInventoryType == World.Entities.Creatures.Player.InventoryTypes.GridPack) {
            foreach (Coordinate updatedItemPivot in pcPIIUE.modifiedPivots) {
              if (itemsByPivot.TryGetValue(updatedItemPivot, out ItemIconController iconController)) {
                iconController.updateStackCount();
              } else {
                iconController = ItemIconController.Make(
                  packInventory.getItemStackAt(updatedItemPivot, out int stackId),
                  transform,
                  stackId,
                  true
                );
                iconController.setShaped(true);
                placeIcon(iconController, updatedItemPivot);
              }
            }
          }
          break;
        default:
          break;
      }
    }
  }
}
