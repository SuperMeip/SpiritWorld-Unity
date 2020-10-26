using SpiritWorld.Game.Controllers;
using SpiritWorld.Events;
using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using SpiritWorld.World.Terrain.Features;
using SpiritWorld.World.Terrain.TileGrid;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpiritWorld.World.Entities.Creatures;

namespace SpiritWorld.Managers {
  public class TileSelectionManager : MonoBehaviour {

    /// <summary>
    /// The camera used to show the UI
    /// </summary>
    public Camera UICamera;

    /// <summary>
    /// The minimum time a key must be held down in order to get a hold action instead of a click action
    /// </summary>
    const float MinimumHoldDownTime = 0.3f;

    /// <summary>
    /// The object used to hilight the selected tile
    /// </summary>
    public GameObject selectedTileHilight;

    /// <summary>
    /// The name/type text for the investigated tile
    /// </summary>
    public Text investigatedTileNameText;

    /// <summary>
    /// The position (x.y.z) text for the investigated tile
    /// </summary>
    public Text investigatedTilePositionText;

    /// <summary>
    /// The tile features text for the investigated tile
    /// </summary>
    public Text investigatedTileFeaturesText;

    /// <summary>
    /// The gameobject to hide and unhide used for the "working on tile ui"
    /// </summary>
    public GameObject workOnTileIndicator;

    /// <summary>
    /// The progress circle image for the work on tile indicator
    /// </summary>
    public Image workOnTileProgressCircle;

    /// <summary>
    /// The tool type icon for the work on tile indicator
    /// </summary>
    public Image workOnTileToolTypeIndicator;

    /// <summary>
    /// The currently selected tile
    /// </summary>
    public Tile selectedTile {
      get;
      private set;
    }

    /// <summary>
    /// The features for the selected tile
    /// </summary>
    public FeaturesByLayer selectedTileFeatures
      => Universe.ActiveBoardManager.activeBoard.getFeaturesFor(selectedTile);

    /// <summary>
    /// A timer for performing an action
    /// </summary>
    float actionTimer = 0.0f;

    // Update is called once per frame
    void Update() {
      selectHoveredTile();
      tryToActOnSelectedTile();
    }

    /// <summary>
    /// Select and hilight the tile the mouse is hovering over
    /// </summary>
    void selectHoveredTile() {
      // we only want to change the selected tile when we're not acting on a tile atm.
      //Also select tiles if we're dragging an item
      // TODO add a way to check if we're holding an item over an inventory vs a tile.
      if (!Input.GetButton("Act") || (ItemInventoryDragController.AnItemIsBeingDragged/* && !ItemIsHoveredOverInventoryBackgroundOfSomeKind*/)) {
        /// if the mouse is pointing at the tileboard, use a ray get data about the tile
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 25, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, 25) && hit.collider.gameObject.CompareTag("TileBoard")) {
          // get the place we hit, plus a little forward in case we hit the side of a column
          Vector3 gridHitPosition = hit.point + (hit.normal * 0.1f);
          // zero out the y, we dont' account for height in the coords.
          gridHitPosition.y = 0;
          // get which tile we hit and the chunk it's in
          selectedTile = Universe.ActiveBoardManager.activeBoard.get(gridHitPosition);

          /// Move the selected tile hilight to the correct location
          if (selectedTileHilight != null) {
            selectedTileHilight.transform.position = new Vector3(
              selectedTile.worldLocation.x,
              selectedTile.height * Universe.StepHeight,
              selectedTile.worldLocation.z
            );
          }
        }
      }
    }

    /// <summary>
    /// Act on the selected tile when appropriate
    /// </summary>
    void tryToActOnSelectedTile() {
      if (!ItemInventoryDragController.AnItemIsBeingDragged) {
        /// if we just pressed the button, investigate the newly selected tile
        if (Input.GetButtonDown("Act")) {
          investigate(selectedTile);
        }
        // if we're holding the button
        if (Input.GetButton("Act")) {
          actionTimer += Time.deltaTime;
          // if we've been holding it for the minimum hold time at least, act via hold action
          if (actionTimer >= MinimumHoldDownTime) {
            holdDownActionOnSelectedTile();
          }
        }
      }
      // if we've let go of the button
      if (Input.GetButtonUp("Act")) {
        // if we were holding an item and release it on a tile, check if that does something special:
        if (ItemInventoryDragController.AnItemIsBeingDragged) {
          // TODO: try to use the item on the tile
        } else {
          // if we haven't gone over minimum hold time, do the single click action on the tile
          if (actionTimer < MinimumHoldDownTime) {
            clickActionOnSelectedTile();
          }
          // reset the action timer and progress wheel
          actionTimer = 0;
          workOnTileIndicator.SetActive(false);
        }
      }
    }

    /// <summary>
    /// Use an equiped tool on the tile's feature
    /// </summary>
    /// <param name="actionTimer"></param>
    void holdDownActionOnSelectedTile() {
      IInventory drops = null;
      // check if the tile has a resource. If it does, we'll try to mine it
      FeaturesByLayer features = selectedTileFeatures;
      Item selectedItem = Universe.LocalPlayerManager.ItemHotBarController.selectedItem;
      ITool selectedTool = selectedItem is ITool tool ? tool : Player.EmptyHand;
      // try to mine a resource feature.
      if (features != null) {
        if (features.TryGetValue(TileFeature.Layer.Resource, out TileFeature resource)
          && resource.type.CanBeMinedBy(selectedTool, resource.mode)
        ) {
          TileFeature beforeResourceValues = resource;
          TileFeature? workedResource = resource.interact(selectedTool, actionTimer, out drops);
          // if the tile resource was null, destroy it.
          if (workedResource == null) {
            updateTileProgressBar(0, resource.type.TimeToUse);
            workOnTileIndicator.SetActive(false);

            // remove the tile feature and sent the update to the board to update the feature
            Universe.ActiveBoardManager.activeBoard.remove(selectedTile, beforeResourceValues.type.Layer);
            actionTimer = 0;
            Universe.EventSystem.notifyChannelOf(
              new TileFeatureDestroyed(selectedTile, beforeResourceValues.type.Layer),
              WorldScapeEventSystem.Channels.TileUpdates
            );
          // if it wasn't we need to update it.
          } else {
            TileFeature updatedResource = (TileFeature)workedResource;
            updateTileProgressBar(updatedResource.remainingInteractions == 0 || !updatedResource.type.CanBeMinedBy(selectedTool, updatedResource.mode) 
              ? 0 
              : actionTimer, resource.type.TimeToUse
            );

            // if the updated resource doesn't match the old one, we need to update it
            if (!beforeResourceValues.Equals(updatedResource)) {
              Universe.ActiveBoardManager.activeBoard.update(selectedTile, updatedResource);
              actionTimer = 0;
              Universe.EventSystem.notifyChannelOf(
                new TileFeatureUpdated(selectedTile, updatedResource),
                WorldScapeEventSystem.Channels.TileUpdates
              );

              // set the info to the updated tile.
              investigate(selectedTile);
            }
          }
        // shovels can break decorations
        } else if (selectedTool.ToolType == Tool.Type.Shovel && features.TryGetValue(TileFeature.Layer.Decoration, out TileFeature decoration)) {
          TileFeature beforeShoveledValues = decoration;
          TileFeature? shoveledDecoration = decoration.interact(selectedTool, actionTimer, out drops);
          if (shoveledDecoration == null) {
            updateTileProgressBar(0, resource.type.TimeToUse);

            // remove the tile feature and sent the update to the board to update the feature
            Universe.ActiveBoardManager.activeBoard.remove(selectedTile, beforeShoveledValues.type.Layer);
            actionTimer = 0;
            Universe.EventSystem.notifyChannelOf(
              new TileFeatureDestroyed(selectedTile, beforeShoveledValues.type.Layer),
              WorldScapeEventSystem.Channels.TileUpdates
            );
          }
        }
      }

      // If the tile feature or decoration had drops, give them to the player or drop them
      if (drops != null) {
        Item[] leftoverDrops = Universe.LocalPlayerManager.tryToEmpty(drops);
        Universe.EventSystem.notifyChannelOf(
          new TileFeatureDropsLeftover(selectedTile, leftoverDrops),
          WorldScapeEventSystem.Channels.TileUpdates
        );
      }
    }

    /// <summary>
    /// display info about the tile
    /// </summary>
    void clickActionOnSelectedTile() {
      // empty for now
    }

    /// <summary>
    /// Investigate the given tile for it's info, and display it
    /// </summary>
    /// <param name="tile"></param>
    void investigate(Tile tile) {
      if (tile.type != null) {
        investigatedTileNameText.text = tile.type.Name;
        investigatedTilePositionText.text = $"({tile.axialKey.x}, {tile.height}, {tile.axialKey.z})";

        /// if we have features, name them.
        string featureText;
        FeaturesByLayer features = selectedTileFeatures;
        if (features != null) {
          featureText = "Features:\n";
          foreach (KeyValuePair<TileFeature.Layer, TileFeature> feature in features) {
            featureText += $"{feature.Value.type.Name} {(feature.Value.type.IsInteractive ? $"({feature.Value.remainingInteractions}/{feature.Value.type.NumberOfUses})" : "")}\n";
          }
        } else {
          featureText = "Empty";
        }
        investigatedTileFeaturesText.text = featureText;
      }
    }

    /// <summary>
    /// Update the tile progress bar with time remaining
    /// </summary>
    /// <param name="holdTimeSoFar"></param>
    /// <param name="useTimeofFeature"></param>
    void updateTileProgressBar(float holdTimeSoFar, float useTimeofFeature) {
      // hide it unless we're holding the bar in
      if (holdTimeSoFar > MinimumHoldDownTime) {
        if (workOnTileIndicator.activeSelf == false) {
          workOnTileIndicator.SetActive(true);
        }
        float wheelPercentage = holdTimeSoFar / useTimeofFeature;
        workOnTileProgressCircle.fillAmount = wheelPercentage;
        workOnTileIndicator.transform.position = UICamera.ScreenToWorldPoint(Input.mousePosition);
      } else if (workOnTileIndicator.activeSelf == true) {
        workOnTileIndicator.SetActive(false);
      }
    }
    
    /// <summary>
    /// An event to send off to let managers know a tile feature for a certain layer has been changed
    /// </summary>
    public struct TileFeatureUpdated : IEvent {

      /// <summary>
      /// Name
      /// </summary>
      public string name 
        => "Tile Feature Mode Changed";

      /// <summary>
      /// The tile that was updated
      /// </summary>
      public Tile updatedTile {
        get;
      }

      /// <summary>
      /// The feature that was updated
      /// </summary>
      public TileFeature updatedFeature {
        get;
      }

      /// <summary>
      /// Make a new event
      /// </summary>
      /// <param name="tile"></param>
      /// <param name="feature"></param>
      public TileFeatureUpdated(Tile tile, TileFeature feature) {
        updatedTile = tile;
        updatedFeature = feature;
      }
    }
    
    /// <summary>
    /// An event to send off to let managers know a tile feature for a certain layer has been destroyed
    /// </summary>
    public struct TileFeatureDestroyed : IEvent {

      /// <summary>
      /// Name
      /// </summary>
      public string name 
        => "Tile Feature Destroyed";

      /// <summary>
      /// The feature that was updated
      /// </summary>
      public Tile updatedTile {
        get;
      }

      /// <summary>
      /// The tile that was updated
      /// </summary>
      public TileFeature.Layer removedLayer {
        get;
      }

      /// <summary>
      /// Make a new event
      /// </summary>
      /// <param name="tile"></param>
      /// <param name="feature"></param>
      public TileFeatureDestroyed(Tile tile, TileFeature.Layer removedLayer) {
        updatedTile = tile;
        this.removedLayer = removedLayer;
      }
    }
    
    /// <summary>
    /// An event to send off to let managers know a tile feature was mined and some drops were leftover
    /// </summary>
    public struct TileFeatureDropsLeftover : IEvent {

      /// <summary>
      /// Name
      /// </summary>
      public string name 
        => "Tile Feature Drops Leftover";

      /// <summary>
      /// The feature that was updated
      /// </summary>
      public Tile updatedTile {
        get;
      }

      /// <summary>
      /// The leftover drops
      /// </summary>
      public Item[] drops {
        get;
      }

      /// <summary>
      /// Make a new event
      /// </summary>
      /// <param name="tile"></param>
      /// <param name="feature"></param>
      public TileFeatureDropsLeftover(Tile tile, Item[] drops) {
        updatedTile = tile;
        this.drops = drops;
      }
    }
  }
}
