using SpiritWorld.Inventories;
using SpiritWorld.World.Entities.Creatures;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    /// original max anchor
    /// </summary>
    Vector2 originalMaxAnchor;

    /// <summary>
    /// original min anchor
    /// </summary>
    Vector2 originalMinAnchor;

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
    public override void OnDrag(PointerEventData eventData) {
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
            // TODO: give the item grab pannel it's own canvas, so you can put the icons to it's own scale while being dragged.
          } else if (Input.mousePosition.x < screenCenter && !parentController.isShaped) {
            parentController.setShaped(true);
          }
        }

        // if we're in item bar teritory.
        if (!Universe.LocalPlayerManager.PackGridController.packMenuIsOpen || !parentController.isShaped) {
          Universe.LocalPlayerManager.ItemHotBarController.checkAndExpandBarSlotAroundMousePosition(eventData.position);
        }
      }
    }

    /// <summary>
    /// set being dragged
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerDown(PointerEventData eventData) {
      if (!AnItemIsBeingDragged) {
        enableDrag();
      }
    }

    /// <summary>
    /// set being let go
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerUp(PointerEventData eventData) {
      if (isBeingDragged) {
        disableDrag();

        // check if we're dropping it into the shaped inventory.
        if (parentController.isShaped) {
          if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Universe.LocalPlayerManager.PackGridController.rectTransform,
            eventData.position,
            Universe.LocalPlayerManager.PackGridController.UICamera,
            out Vector2 localCursor
          )) {
            Vector2 gridClickLocation = localCursor + Universe.LocalPlayerManager.PackGridController.rectTransform.rect.size / 2;
            Vector2 gridSize
              = Universe.LocalPlayerManager.PackGridController.getGridSquareSize()
                * Universe.LocalPlayerManager.PackGridController.rectTransform.rect.size;
            Coordinate gridItemLocation = new Coordinate(
              (short)(gridClickLocation.x / gridSize.x),
              (short)(gridClickLocation.y / gridSize.y)
            );
            Debug.Log(gridItemLocation);
            // return;
          }
          // check if we're dropping it into the item bar
        } else if (ItemHotBarController.TryToGetItemBarHoverPosition(eventData.position, out short barSlotIndex, out float barSlotPlacementOffset)) {
          Coordinate gridItemLocation = new Coordinate(barSlotIndex, 0);
          Debug.Log(gridItemLocation);
          Debug.Log(barSlotPlacementOffset > 0.78f || barSlotPlacementOffset < 0.20f ? "In Between" : "On the Mark");
          // return;
        }

        resetToOriginalPosition(); 
      }
    }

    /// <summary>
    /// Turn dragging on
    /// </summary>
    void enableDrag() {
      // record original values
      originalParent = transform.parent;
      originalLocation = transform.position;
      originalScale = parentController.currentSize;
      originalContainerInventory = containingInventory;
      isBeingDragged = AnItemIsBeingDragged = true;
      originalOpacity = parentController.currentOpacity;
      wasShapedOriginally = containingInventory == Player.InventoryTypes.GridPack
        ? true
        : false;

      /// update for dragging
      parentController.setOpacity(1);
      // if the pack menu is open it can manage all of the dragging
      if (Universe.LocalPlayerManager.PackGridController.packMenuIsOpen && containingInventory != Player.InventoryTypes.GridPack) {
        parentController.resize();
        // save and replace the anchor values
        originalMaxAnchor = parentController.rectTransform.anchorMax;
        originalMinAnchor = parentController.rectTransform.anchorMin;
        Vector2 gridSize = Universe.LocalPlayerManager.PackGridController.getGridSquareSize();
        parentController.rectTransform.anchorMin = Vector2.zero;
        parentController.rectTransform.anchorMax = gridSize + gridSize;

        // re-parent to the open grid
        containingInventory = Player.InventoryTypes.GridPack;
        parentController.rectTransform.SetParent(Universe.LocalPlayerManager.PackGridController.gridTransform);
      }
    }

    /// <summary>
    /// disable the dragging
    /// </summary>
    void disableDrag() {
      isBeingDragged = AnItemIsBeingDragged = false;
    }

    /// <summary>
    /// Reset the location to when we started dragging
    /// </summary>
    void resetToOriginalPosition() {
      containingInventory = originalContainerInventory;
      // if we have to switch the grid back, make sure to re-size and scale
      if (originalContainerInventory != Player.InventoryTypes.GridPack && originalParent != parentController.rectTransform.parent) {
        parentController.rectTransform.SetParent(originalParent);
        parentController.rectTransform.anchorMax = originalMaxAnchor;
        parentController.rectTransform.anchorMin = originalMinAnchor;
        parentController.resize(originalScale.x);
      }

      // reset other values
      transform.position = originalLocation;
      parentController.setOpacity(originalOpacity);
      if (wasShapedOriginally != parentController.isShaped) {
        parentController.setShaped(wasShapedOriginally);
      }

      // if we expanded a slot and we're resetting, reset the slot
      Universe.LocalPlayerManager.ItemHotBarController.resetExpandedBarSlots();
    }
  }
}
