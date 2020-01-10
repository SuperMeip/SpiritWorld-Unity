using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

  // Start is called before the first frame update
  void Start() {
    HexGrid testGrid = new HexGrid();
    testGrid.set(new Vector2(1, 1), Tile.Types.Grass, 3);
    Mesh mesh = TerrainMeshGenerator.generate(testGrid);
  }

  // Update is called once per frame
  void Update() {

  }
}
