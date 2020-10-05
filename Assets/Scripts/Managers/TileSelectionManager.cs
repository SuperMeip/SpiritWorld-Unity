using SpiritWorld.Controllers;
using SpiritWorld.Events;
using SpiritWorld.World.Terrain.Features;
using SpiritWorld.World.Terrain.TileGrid;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpiritWorld.Managers {
  public class TileSelectionManager : MonoBehaviour {

    /// <summary>
    /// The minimum time a key must be held down in order to get a hold action instead of a click action
    /// </summary>
    const float MinimumHoldDownTime = 0.5f;

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
    /// The currently selected tile
    /// </summary>
    public Tile selectedTile {
      get;
      private set;
    }

    /// <summary>
    /// A timer for performing an action
    /// </summary>
    float actionTimer = 0.0f;

    /// <summary>
    /// Local player controller
    /// </summary>
    LocalPlayerController playerController;

    /// <summary>
    /// Get the local player controller
    /// </summary>
    private void Awake() {
      playerController = GameObject.FindWithTag("Local Player").GetComponent<LocalPlayerController>();
    }

    // Update is called once per frame
    void Update() {
      selectHoveredTile();
      tryToActOnSelectedTile();
    }

    /// <summary>
    /// Select and hilight the tile the mouse is hovering over
    /// </summary>
    void selectHoveredTile() {
      // we only want to change the selected tile when we're not acting on a tile atm
      if (!Input.GetButton("Act")) {
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
      // if we're holding the button
      if (Input.GetButton("Act")) {
        actionTimer += Time.deltaTime;
        // if we've been holding it for the minimum hold time at least, act via hold action
        if (actionTimer >= MinimumHoldDownTime) {
          holdDownActionOnSelectedTile();
        }
      }
      // if we've let go of the button
      if (Input.GetButtonUp("Act")) {
        // if we haven't gone over minimum hold time, do the single click action on the tile
        if (actionTimer < MinimumHoldDownTime) {
          clickActionOnSelectedTile();
        }
        // reset the action timer
        actionTimer = 0;
      }
    }

    /// <summary>
    /// Use an equiped tool on the tile's feature
    /// </summary>
    /// <param name="actionTimer"></param>
    void holdDownActionOnSelectedTile() {
      // check if the tile has a resource. If it does, we'll try to mine it
      FeaturesByLayer features = Universe.ActiveBoardManager.activeBoard.getFeaturesFor(selectedTile);
      if (features != null && features.TryGetValue(TileFeature.Layer.Resource, out TileFeature resource)) {
        TileFeature beforeResourceValues = resource;
        resource.interact(actionTimer);
        Universe.ActiveBoardManager.activeBoard.update(selectedTile, resource);
        if (beforeResourceValues.mode != resource.mode) {
          actionTimer = 0;
          Universe.EventSystem.notifyChannelOf(
            new TileFeatureModeChanged(selectedTile, resource),
            WorldScapeEventSystem.Channels.TileUpdates
          );
        }
      }
    }

    /// <summary>
    /// display info about the tile
    /// </summary>
    void clickActionOnSelectedTile() {
      investigate(selectedTile);
    }

    /// <summary>
    /// Investigate the given tile for it's info, and display it
    /// </summary>
    /// <param name="tile"></param>
    void investigate(Tile tile) {
      investigatedTileNameText.text = tile.type.Name;
      investigatedTilePositionText.text = $"({tile.axialKey.x}, {tile.height}, {tile.axialKey.z})";

      /// if we have features, name them.
      string featureText;
      FeaturesByLayer features = Universe.ActiveBoardManager.activeBoard.getFeaturesFor(selectedTile);
      if (features != null) {
        featureText = "Features:\n";
        foreach(KeyValuePair<TileFeature.Layer, TileFeature> feature in features) {
          featureText += $"{feature.Value.type.Name} {(feature.Value.type is TileFeature.LimitedUseType featureType ? $"({feature.Value.remainingInteractions}/{featureType.NumberOfUses})" : "")}\n";
        }
      } else {
        featureText = "Empty";
      }
      investigatedTileFeaturesText.text = featureText;
    }
    
    /// <summary>
    /// An event to send off to let managers know a tile has been changed
    /// </summary>
    public struct TileFeatureModeChanged : IEvent {

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
      public TileFeatureModeChanged(Tile tile, TileFeature feature) {
        updatedTile = tile;
        updatedFeature = feature;
      }
    }
  }
}
