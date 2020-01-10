using UnityEngine;
/// <summary>
/// A tile of terrain on a gameboard
/// </summary>
public partial struct Tile {

  /// <summary>
  /// The height of this tile
  /// </summary>
  public byte height;

  /// <summary>
  /// The type of tile
  /// </summary>
  public Type type;

  /// <summary>
  /// The in world location of the center of this hex
  /// </summary>
  public Vector3 worldLocation {
    get;
  }

  /// <summary>
  /// The key used to store hexagons in the grid
  /// </summary>
  public Coordinate axialKey {
    get => Hexagon.WorldLocationToAxialKey(worldLocation);
  }

  /// <summary>
  /// Make a new tile of the desired type
  /// </summary>
  /// <param name="type"></param>
  /// <param name="height"></param>
  public Tile(Vector2 worldLocation2D = default, Type type = null, byte height = 0) {
    worldLocation = new Vector3(worldLocation2D.x, height, worldLocation2D.y);
    this.type   = type ?? Types.Empty;
    this.height = height;
  }
}