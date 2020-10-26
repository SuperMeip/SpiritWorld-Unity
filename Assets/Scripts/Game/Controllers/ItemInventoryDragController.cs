using SpiritWorld.World.Entities.Creatures;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SpiritWorld.Game.Controllers {

  /// <summary>
  /// controls an item being able to be dragged and dropped into inventories
  /// </summary>
  class ItemInventoryDragController : EventTrigger {

    /// <summary>
    /// If an item is being dragged by one of these controllers
    /// </summary>
    public static bool AnItemIsBeingDragged = false;

    /// <summary>
    /// The stack id this controller represents
    /// </summary>
    public int stackId {
      get;
      private set;
    }

    /// <summary>
    /// The stack id this controller represents
    /// </summary>
    public Player.InventoryTypes containingInventory {
      get;
      private set;
    }

    /// <summary>
    /// If this is the item being dragged at the moment
    /// </summary>
    public bool isBeingDragged = false;

    /// <summary>
    /// The inital location before being dragged
    /// </summary>
    Vector3 originalLocation;

    /// <summary>
    /// The original scale this icon was at.
    /// </summary>
    Vector2 originalScale;

    /// <summary>
    /// The opacity before we started dragging the icon
    /// </summary>
    float originalOpacity;

    /// <summary>
    /// If this icon was originally shaped
    /// </summary>
    bool wasShapedOriginally;

    /// <summary>
    /// The parent item icon controller
    /// </summary>
    ItemIconController parentController;

    /// <summary>
    /// The parent before we started dragging
    /// </summary>
    Transform originalParent;

    /// <summary>
    /// The original inventory this was parented to
    /// </summary>
    Player.InventoryTypes originalContainerInventory;

    /// <summary>
    /// initialize this for the given item
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="stackId"></param>
    public void initialize(ItemIconController parent, int stackId, Player.InventoryTypes containingInventory) {
      parentController = parent;
      this.stackId = stackId;
      this.containingInventory = containingInventory;
    }

    /// <summary>
    /// Drag the item around
    /// </summary>
    public void Update() {
      if (isBeingDragged) {
        transform.position = (containingInventory == Player.InventoryTypes.HotBar
          ? Universe.LocalPlayerManager.ItemHotBarController.UICamera
          : Universe.LocalPlayerManager.PackGridController.UICamera
        ).ScreenToWorldPoint(Input.mousePosition);

        /// when the pack is open, change the icon type around
        if (Universe.LocalPlayerManager.PackGridController.packMenuIsOpen) {
          float screenCenter = (Screen.width * 0.5f);
          // on the right side of the screen
          if (Input.mousePosition.x > screenCenter && parentController.isShaped) {
            parentController.setShaped(false);
            containingInventory = Player.InventoryTypes.HotBar;
            // TODO: give the item grab pannel it's own canvas, so you can put the icons to it's own scale while being dragged.
            parentController.rectTransform.SetParent(Universe.LocalPlayerManager.ItemHotBarController.transform);
          } else if (Input.mousePosition.x < screenCenter && !parentController.isShaped) {
            parentController.setShaped(true);
            containingInventory = Player.InventoryTypes.GridPack;
            parentController.rectTransform.SetParent(Universe.LocalPlayerManager.PackGridController.gridTransform);
          }
        }
      }
    }

    /// <summary>
    /// set being dragged
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerDown(PointerEventData eventData) {
      if (!AnItemIsBeingDragged) {
        originalParent = transform.parent;
        originalLocation = transform.position;
        originalScale = parentController.currentSize;
        originalContainerInventory = containingInventory;
        isBeingDragged = AnItemIsBeingDragged = true;
        originalOpacity = parentController.currentOpacity;
        parentController.setOpacity(1);
        //parentController.resize();
        wasShapedOriginally = containingInventory == Player.InventoryTypes.GridPack 
          ? true 
          : false;
      }
    }

    /// <summary>
    /// set being let go
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerUp(PointerEventData eventData) {
      if (isBeingDragged) {
        isBeingDragged = AnItemIsBeingDragged = false;
        resetToOriginalPosition();
      }
    }

    /// <summary>
    /// Reset the location to when we started dragging
    /// </summary>
    void resetToOriginalPosition() {
      transform.position = originalLocation;
      parentController.rectTransform.SetParent(originalParent);
      parentController.setOpacity(originalOpacity);
      if (originalContainerInventory == Player.InventoryTypes.HotBar) {
        parentController.resize(originalScale.x);
      }
      containingInventory = originalContainerInventory;
      if (wasShapedOriginally != parentController.isShaped) {
        parentController.setShaped(wasShapedOriginally);
      }
    }
  }
}
