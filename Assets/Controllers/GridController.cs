
using SpiritWorld.World.Terrain.Generation.Noise;
using SpiritWorld.World.Terrain.TileGrid;
using SpiritWorld.World.Terrain.Features;
using SpiritWorld.World.Terrain.TileGrid.Generation;
using System.Collections.Generic;
using UnityEngine;
using SpiritWorld.World;

namespace SpiritWorld.Controllers {

  /// <summary>
  /// Controller for a grid chunk
  /// </summary>
  [RequireComponent(typeof(MeshCollider))]
  [RequireComponent(typeof(MeshRenderer))]
  [RequireComponent(typeof(MeshFilter))]
  public class GridController : MonoBehaviour {

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
    readonly Dictionary<Coordinate, Dictionary<TileFeature.Layer, GameObject>> visibleGridFeatures
      = new Dictionary<Coordinate, Dictionary<TileFeature.Layer, GameObject>>();

    /// <summary>
    /// A list of features with tick updates that need to be run
    /// </summary>
    readonly List<TileFeature> activeFeatures 
      = new List<TileFeature>();

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
            GameObject featureModel = Instantiate(
              TileFeatureModelsDataMaper.GetModelForFeature(feature.Value),
              new Vector3(
                tile.worldLocation.x + chunkWorldLocation.x,
                tile.worldLocation.y * Universe.StepHeight,
                tile.worldLocation.z + chunkWorldLocation.z
              ),
              feature.Value.type.PlacementRotationType == TileFeature.RotationType.Random
                ? Quaternion.Euler(0, UnityEngine.Random.Range(1, 360), 0)
                : Quaternion.Euler(0, 0, 0),
              transform
            );

            // store the newly instantiated feature's model
            if (visibleGridFeatures.ContainsKey(tileAxialKey)) {
              visibleGridFeatures[tileAxialKey][feature.Value.type.Layer] = featureModel;
            } else {
              visibleGridFeatures[tileAxialKey] = new Dictionary<TileFeature.Layer, GameObject> {
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
    /// remove the grid this is managing and hide this controller
    /// </summary>
    public void clear() {
      /// hide it all
      gameObject.SetActive(false);
      /// remove active grid
      activeGrid = null;
      /// delete all the features
      foreach(KeyValuePair<Coordinate, Dictionary<TileFeature.Layer, GameObject>> tileWithFeatures in visibleGridFeatures) {
        foreach (KeyValuePair<TileFeature.Layer, GameObject> featureModel in tileWithFeatures.Value) {
          Destroy(featureModel.Value);
        }
      }
      /// de-mesh the grid
      meshFilter.mesh.Clear();
    }

    #region Test grid generation

    HexGrid testWorldScape() {
      WorldScape testScape = new WorldScape();
      Biome testForest = new Biome(Biome.Types.RockyForest, 1234);
      testScape.mainBoard.createNewGrid((0, 0), testForest);
      testScape.mainBoard.createNewGrid((1, 0), testForest);

      return testScape.mainBoard[(0, 0)];
    }

    HexGrid testBiome() {
      HexGrid testGrid = new HexGrid();
      Biome testBiome = new Biome(Biome.Types.RockyForest, 1234);
      HexGridShaper.Rectangle((36, 36), axialKey => {
        testGrid.set(testBiome.generateAt(axialKey));
      });

      return testGrid;
    }

    HexGrid getTestGrid() {
      HexGrid testGrid = new HexGrid();
      testGrid.set(new Tile(Tile.Types.Grass, (0, 0), 1));
      testGrid.set(new Tile(Tile.Types.Grass, (0, 1), 2));
      testGrid.set(new Tile(Tile.Types.Grass, (1, 0), 3));
      testGrid.set(new Tile(Tile.Types.Grass, (1, -1), 5));
      testGrid.set(new Tile(Tile.Types.Grass, (0, -1), 5));
      testGrid.set(new Tile(Tile.Types.Grass, (-1, 0), 6));
      testGrid.set(new Tile(Tile.Types.Grass, (-1, 1), 7));

      return testGrid;
    }

    HexGrid getTestGeneratedGrid() {
      HexGrid testGrid = new HexGrid();
      HexGridShaper.Rectangle((20, 20), axialKey => {
        testGrid.set(new Tile(Tile.Types.Grass, axialKey, UnityEngine.Random.Range(1, 6)));
      });

      return testGrid;
    }

    HexGrid getTestNoiseGrid() {
      HexGrid testGrid = new HexGrid();
      FastNoise noise = new FastNoise(1234);
      FastNoise terrainNoise = new FastNoise(1111);
      HexGridShaper.Rectangle((30, 30), axialKey => {
        float noiseValue = noise.GetPerlinFractal(axialKey.x * 20, axialKey.z * 20);
        float terrainValue = terrainNoise.GetPerlinFractal(axialKey.x * 10, axialKey.z * 10);
        int scaledNoise = Mathf.Max((int)noiseValue.scale(20, 1), 7);
        testGrid.set(new Tile(
          scaledNoise == 7
            ? Tile.Types.Water
            : terrainValue > 0 && scaledNoise > 10
              ? Tile.Types.Rocky
              : Tile.Types.Grass,
          axialKey,
          scaledNoise
        ));
      });

      return testGrid;
    }

    #endregion
  }
}

