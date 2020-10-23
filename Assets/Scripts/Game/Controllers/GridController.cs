using SpiritWorld.World.Terrain.TileGrid;
using SpiritWorld.World.Terrain.Features;
using System.Collections.Generic;
using UnityEngine;
using SpiritWorld.Inventories.Items;

namespace SpiritWorld.Game.Controllers {

  /// <summary>
  /// Controller for a grid chunk
  /// </summary>
  [RequireComponent(typeof(MeshCollider))]
  [RequireComponent(typeof(MeshRenderer))]
  [RequireComponent(typeof(MeshFilter))]
  public class GridController : MonoBehaviour {

    /// <summary>
    /// The item drop prefab
    /// </summary>
    public GameObject ItemDrop;

    /// <summary>
    /// The collider for this chunk
    /// </summary>
    MeshCollider meshCollider;

    /// <summary>
    /// The mesh renderer for this chunk
    /// </summary>
    MeshFilter meshFilter;

    /// <summary>
    /// The grid active for this controller
    /// </summary>
    HexGrid activeGrid;

    /// <summary>
    /// If this controller is already assigned to a chunk
    /// </summary>
    public bool isInUse
      => activeGrid != null;

    /// <summary>
    /// Grid features that are visible on the map
    /// Indexed by the tile's axial coord and then by the feature layer
    /// </summary>
    readonly Dictionary<Coordinate, Dictionary<TileFeature.Layer, GameObject>> visibleFeatures
      = new Dictionary<Coordinate, Dictionary<TileFeature.Layer, GameObject>>();

    /// <summary>
    /// A list of features with tick updates that need to be run
    /// </summary>
    readonly List<TileFeature> activeFeatures 
      = new List<TileFeature>();

    /// <summary>
    /// Item drops in this chunk
    /// </summary>
    readonly List<ItemDropController> drops 
      = new List<ItemDropController>();

    // Start is called before the first frame update
    void Awake() {
      meshCollider = GetComponent<MeshCollider>();
      meshFilter = GetComponent<MeshFilter>();
      meshFilter.mesh = new Mesh();
      meshFilter.mesh.Clear();
    }

    /// <summary>
    /// Change the grid this controller is controlling/displaying
    /// </summary>
    /// <param name="newGrid"></param>
    public void updateGridTo(HexGrid newGrid, Coordinate gridChunkLocation) {
      if (newGrid != null) {
        activeGrid = newGrid;
        /// move the whole chunk to the right location
        Vector3 chunkWorldLocation = RectangularBoard.ChunkWorldOffset * gridChunkLocation;
        transform.position = chunkWorldLocation;

        /// update the mesh
        // get the new mesh values based on the new grid
        // TODO: generate only values here and replace them after clearing the mesh below
        Mesh mesh = HexGridMeshGenerator.generate(newGrid);
        /// instantiate all of the tile features in the grid and store them
        activeGrid.forEach((tileAxialKey, tile, features) => {
          foreach (KeyValuePair<TileFeature.Layer, TileFeature> feature in features) {
            // instantiate the feature on top of the tile
            GameObject featureModel = instantiateFeature(tile, feature.Value, true);
            // store the newly instantiated feature's model
            if (visibleFeatures.ContainsKey(tileAxialKey)) {
              visibleFeatures[tileAxialKey][feature.Value.type.Layer] = featureModel;
            } else {
              visibleFeatures[tileAxialKey] = new Dictionary<TileFeature.Layer, GameObject> {
                [feature.Value.type.Layer] = featureModel
              };
            }
          }
        });

        // update the mesh with the new values
        // TODO: clear mesh here and then just import the values from HexGridMeshGenerator.generate
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshFilter.mesh.RecalculateNormals();
        gameObject.SetActive(true);
      }
    }

    /// <summary>
    /// Update the model for a given feature
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="feature"></param>
    public void updateFeatureModel(Tile tile, TileFeature feature) {
      if (visibleFeatures.TryGetValue(tile.axialKey, out Dictionary<TileFeature.Layer, GameObject> features)) {
        if (features.TryGetValue(feature.type.Layer, out GameObject oldFeatureModel)) {
          GameObject newFeatureModel = instantiateFeature(tile, feature);
          newFeatureModel.transform.rotation = oldFeatureModel.transform.rotation;
          // TODO: change this to features[layer] = X;
          visibleFeatures[tile.axialKey][feature.type.Layer] = newFeatureModel;
          Destroy(oldFeatureModel);
        }
      }
    }

    /// <summary>
    /// Remove a feature from a tile by the layer
    /// </summary>
    public void removeFeature(Tile tile, TileFeature.Layer featureLayerToRemove) {
      if (visibleFeatures.TryGetValue(tile.axialKey, out Dictionary<TileFeature.Layer, GameObject> features)) {
        if (features.TryGetValue(featureLayerToRemove, out GameObject oldFeatureModel)) {
          features.Remove(featureLayerToRemove);
          Destroy(oldFeatureModel);
        }
      }
    }

    /// <summary>
    /// remove the grid this is managing and hide this controller
    /// </summary>
    public void clear() {
      /// hide it all
      gameObject.SetActive(false);
      /// remove active grid
      activeGrid = null;
      /// delete all the features
      foreach (KeyValuePair<Coordinate, Dictionary<TileFeature.Layer, GameObject>> tileWithFeatures in visibleFeatures) {
        foreach (KeyValuePair<TileFeature.Layer, GameObject> featureModel in tileWithFeatures.Value) {
          Destroy(featureModel.Value);
        }
      }
      visibleFeatures.Clear();

      foreach (ItemDropController item in drops) {
        Destroy(item.gameObject);
      }
      drops.Clear();

      /// de-mesh the grid
      meshFilter.mesh.Clear();
    }

    /// <summary>
    /// Drop items onto the grid
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="drops"></param>
    public void addItemDrops(Tile tile, Item[] drops) {
      foreach (Item drop in drops) {
        GameObject dropObject = Instantiate(
          ItemDrop,
          new Vector3(
            tile.worldLocation.x + Random.Range(-0.5f, 0.5f),
            tile.worldLocation.y * Universe.StepHeight + Random.Range(0.1f, 0.5f),
            tile.worldLocation.z + Random.Range(-0.5f, 0.5f)
          ),
          Quaternion.Euler(0, UnityEngine.Random.Range(1, 360), 0),
          transform
        );
        ItemDropController itemDropController = dropObject.GetComponent<ItemDropController>();
        this.drops.Add(itemDropController);
        itemDropController.setItem(drop);
      }
    }

    /// <summary>
    /// Instantiate a feature's model on the given tile
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="feature"></param>
    /// <returns></returns>
    GameObject instantiateFeature(Tile tile, TileFeature feature, bool randomizeDirection = false) {
      return Instantiate(
        TileFeatureDataMapper.GetModelFor(feature),
        new Vector3(
          tile.worldLocation.x,
          tile.worldLocation.y * Universe.StepHeight,
          tile.worldLocation.z
        ),
        feature.type.PlacementRotationType == TileFeature.RotationType.Random
          ? Quaternion.Euler(0, UnityEngine.Random.Range(1, 360), 0)
          : Quaternion.Euler(0, 0, 0),
        transform
      );
    }
  }
}

