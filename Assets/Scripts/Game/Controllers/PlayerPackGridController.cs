using SpiritWorld.Events;
using SpiritWorld.Inventories;
using SpiritWorld.Managers;
using SpiritWorld.World.Entities.Creatures;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpiritWorld.Game.Controllers {
  public class PlayerPackGridController : MonoBehaviour, IObserver {

    #region Constants

    /// <summary>
    /// The item bar camera
    /// </summary>
    public Camera UICamera;

    /// <summary>
    /// The canvas for this
    /// </summary>
    public Canvas Canvas;

    /// <summary>
    /// The tile texture for the grid.
    /// </summary>
    public Texture2D GridTileTexture;

    /// <summary>
    /// The background image aspect ratio filter
    /// </summary>
    public AspectRatioFitter BackgroundAspectRatio;

    /// <summary>
    /// The menu controller that controls the local player grid menu
    /// TODO: eventually make this a manager probably and attach all the grid parts to it.
    /// </summary>
    public ItemPackMenuController MenuController;

    /// <summary>
    /// dimensions for the texture
    /// </summary>
    public Coordinate GridTileDimensions 
      => (GridTileTexture.width, GridTileTexture.height);

    #endregion

    /// <summary>
    /// The target object for items to be dropped into for this inventory
    /// </summary>
    public GameObject dropTarget
      => gameObject;

    /// <summary>
    /// This's transform
    /// </summary>
    public RectTransform rectTransform
      => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());
    RectTransform _rectTransform;

    /// <summary>
    /// If the menu is currently open
    /// </summary>
    public bool packMenuIsOpen
      => MenuController.menuIsVisible;

    /// <summary>
    /// The transform of the grid
    /// </summary>
    public Transform gridTransform
      => gridImage.gameObject.transform;

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


    #region Initalization

    /// <summary>
    /// If this has been initalized yet.
    /// </summary>
    bool isInitalized = false;

    /// <summary>
    /// Set up the grid based on player inventory
    /// </summary>
    void Start() {
      if (!isInitalized) {
        initalize();
      }
    }

    /// <summary>
    /// Initalize the grid
    /// </summary>
    public void initalize() {
      gridImage = GetComponent<Image>();
      aspectRatioFitter = GetComponent<AspectRatioFitter>();
      updateGridSize();
      populateGridFromPlayerInventory();
      isInitalized = true;
    }

    #endregion

    #region Update Loop

    void updateGridSize() {
      // make the image texture
      itemGridTexture = new Texture2D(
        GridTileTexture.width * packInventory.dimensions.x,
        GridTileTexture.height * packInventory.dimensions.y
      );
      aspectRatioFitter.aspectRatio = (float)packInventory.dimensions.x / (float)packInventory.dimensions.y;
      BackgroundAspectRatio.aspectRatio = aspectRatioFitter.aspectRatio;
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
        ItemIconController iconController = ItemIconController.Make(
          stack,
          transform,
          true,
          true,
          stackId,
          pivot,
          Player.InventoryTypes.GridPack
        );
        iconController.setShaped(true);
        placeIcon(iconController, pivot);
      });
    }

    #endregion

    /// <summary>
    /// Get the current diameter of a grid square
    /// </summary>
    /// <returns></returns>
    public Vector2 getGridSquareSize() {
      return new Vector2(
        (float)1 / (float)packInventory.dimensions.x,
        (float)1 / (float)packInventory.dimensions.y
      );
    }

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

      Vector2 gridSize = getGridSquareSize();
      iconTransform.anchorMin = new Vector2(gridLocation.x * gridSize.x, gridLocation.y * gridSize.y);
      iconTransform.anchorMax = iconTransform.anchorMin + gridSize;
      iconTransform.SetLTRB(0);
    }

    /// <summary>
    /// Remove the icon at the pivot location from the grid
    /// </summary>
    /// <param name=""></param>
    void removeIcon(Coordinate pivotLocation) {
      ItemIconController iconController = itemsByPivot[pivotLocation];
      itemsByPivot.Remove(pivotLocation);

      Destroy(iconController.itemStackSizeIndicator.gameObject);
      Destroy(iconController.gameObject);
    }

    #region IObserver

    /// <summary>
    /// Receive notifications
    /// </summary>
    /// <param name="event"></param>
    public void notifyOf(IEvent @event) {
      switch (@event) {
        case PlayerManager.LocalPlayerInventoryItemsUpdatedEvent pcPIIUE:
          if (isInitalized && pcPIIUE.updatedInventoryType == World.Entities.Creatures.Player.InventoryTypes.GridPack) {
            foreach (Coordinate updatedItemPivot in pcPIIUE.modifiedPivots) {
              if (itemsByPivot.TryGetValue(updatedItemPivot, out ItemIconController iconController)) {
                iconController.updateStackCount();
              } else {
                iconController = ItemIconController.Make(
                  packInventory.getItemStackAt(updatedItemPivot, out int stackId),
                  transform,
                  true,
                  true,
                  stackId,
                  updatedItemPivot,
                  Player.InventoryTypes.GridPack
                );
                iconController.setShaped(true);
                placeIcon(iconController, updatedItemPivot);
                iconController.parentQuantityIndicatorTo(transform);
              }
            }
          }
          break;
        case PlayerManager.LocalPlayerInventoryItemsRemovedEvent pcPIIRE:
          if (pcPIIRE.updatedInventoryType == World.Entities.Creatures.Player.InventoryTypes.GridPack) {
            foreach (Coordinate updatedItemPivot in pcPIIRE.modifiedPivots) {
              // remove the item icons from the given slots
              removeIcon(updatedItemPivot);
            }
          }
          break;
        default:
          break;
      }
    }

    #endregion

  }
}
