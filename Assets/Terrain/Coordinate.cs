using UnityEngine;
/// <summary>
/// A tile position in a level
/// </summary>
public struct Coordinate {

  /// <summary>
  /// 0, 0 coordinate constant
  /// </summary>
  public static Coordinate Zero = (0, 0);

  /// <summary>
  /// east west
  /// </summary>
  public short x;

  /// <summary>
  /// north south
  /// </summary>
  public short y;

  /// <summary>
  /// Get this as a unity vector 2
  /// </summary>
  public Vector2 Vec2 {
    get => new Vector2(x, y);
  }

  /// <summary>
  /// If this coordinate is valid and was properly initialized
  /// </summary>
  public bool isInitialized {
    get;
    private set;
  }

  /// <summary>
  /// Create a 3d coordinate
  /// </summary>
  /// <param name="x"></param>
  /// <param name="y"></param>
  /// <param name="z"></param>
  public Coordinate(short x, short y) {
    this.x = x;
    this.y = y;
    isInitialized = true;
  }

  /// <summary>
  /// Create a coordinate from a touple.
  /// </summary>
  /// <param name="coordinates">(X, Z)</param>
  public static implicit operator Coordinate((int, int) coordinates) {
    return new Coordinate((short)coordinates.Item1, (short)coordinates.Item2);
  }

  /// <summary>
  /// Create a coordinate from a touple.
  /// </summary>
  /// <param name="coordinates">(X, Z)</param>
  public static implicit operator Coordinate((short, short) coordinates) {
    return new Coordinate(coordinates.Item1, coordinates.Item2);
  }

  ///OVERRIDES
  ///===================================
  public static Coordinate operator +(Coordinate a, Coordinate b) {
    return (
      a.x + b.x,
      a.y + b.y
    );
  }

  public static Coordinate operator +(Coordinate a, int b) {
    return (
      a.x + b,
      a.y + b
    );
  }

  public static Coordinate operator *(Coordinate a, Coordinate b) {
    return (
      a.x * b.x,
      a.y * b.y
    );
  }

  public static Coordinate operator *(Coordinate a, int b) {
    return (
      a.x * b,
      a.y * b
    );
  }

  /// <summary>
  /// Is limited to (short.MAX, short.MAX)
  /// </summary>
  /// <returns></returns>
  public override int GetHashCode() {
    return (y << 16) | x;
  }
}