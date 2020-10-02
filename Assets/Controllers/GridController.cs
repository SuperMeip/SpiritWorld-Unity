
using SpiritWorld.World.Terrain.Generation.Noise;
using SpiritWorld.World.Terrain.TileGrid;
using SpiritWorld.World.Terrain.Features;
using SpiritWorld.World.Terrain.TileGrid.Generation;
using System.Collections.Generic;
using UnityEngine;
using SpiritWorld.World;

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

  // Start is called before the first frame update
  void Start() {
    meshCollider = GetComponent<MeshCollider>();
    meshFilter = GetComponent<MeshFilter>();
    meshFilter.mesh = new Mesh();
    meshFilter.mesh.Clear();

    HexGrid testGrid = testWorldScape();
    Mesh mesh = HexGridMeshGenerator.generate(testGrid);
    testGrid.forEach((tileAxialKey, tile, features) => {
      foreach (KeyValuePair<TileFeature.Layer, TileFeature> feature in features) {
        Instantiate(
          TileFeatureModelsDataMaper.GetModelForFeature(feature.Value),
          new Vector3 (
            tile.worldLocation.x,
            tile.worldLocation.y * Universe.StepHeight,
            tile.worldLocation.z
          ),
          Quaternion.Euler(0, Random.Range(1, 360), 0),
          transform
        );
      }
    });

    meshFilter.mesh = mesh;
    meshCollider.sharedMesh = mesh;
    meshFilter.mesh.RecalculateNormals();

  }

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
      testGrid.set(new Tile(Tile.Types.Grass, axialKey, Random.Range(1, 6)));
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
}

