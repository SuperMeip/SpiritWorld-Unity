/// <summary>
/// A board of tiles making up the terrain of a world.
/// </summary>
public class Tileboard {
  
  /// <summary>
  /// The max x and y of this tileboard;
  ///     X = West -> East;
  ///     Y = South -> North
  /// </summary>
  public Coordinate bounds {
    get;
    private set;
  }

  /// <summary>
  /// The tiles in this board
  /// </summary>
  Tile[,] tiles;

  /// <summary>
  /// Create a board of empty tiles
  /// </summary>
  /// <param name="bounds">The size of the board</param>
  public Tileboard(Coordinate bounds) {
    this.bounds = bounds;
    tiles       = new Tile[bounds.x, bounds.y];
  }

  /// <summary>
  /// Get and set functionality for coordinates
  /// </summary>
  /// <param name="location"></param>
  /// <returns></returns>
  public Tile this[Coordinate location] {
    get => tiles[location.x, location.y];
    set { tiles[location.x, location.y] = value; }
  }

  /// <summary>
  /// Get and set functionality x, y
  /// </summary>
  /// <param name="x"></param>
  /// <param name="y"></param>
  /// <returns></returns>
  public Tile this[int x, int y] {
    get => this[(x, y)];
    set {this[(x, y)] = value ;}
  }
}
