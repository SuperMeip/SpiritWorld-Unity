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
  public short z;

  /// <summary>
  /// Get this as a unity vector 2
  /// </summary>
  public Vector3 Vec3 {
    get => new Vector3(x, 0, z);
  }

  /// <summary>
  /// Get this as a unity vector 2
  /// </summary>
  public Vector2 Vec2 {
    get => new Vector2(x, z);
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
  /// <param name="z"></param>
  public Coordinate(short x, short z) {
    this.x = x;
    this.z = z;
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
  public static implicit operator Coordinate(Vector3 coordinate) {
    return new Coordinate((short)coordinate.x, (short)coordinate.z);
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
      a.z + b.z
    );
  }

  public static Coordinate operator +(Coordinate a, int b) {
    return (
      a.x + b,
      a.z + b
    );
  }

  public static Vector2 operator +(Coordinate a, Vector2 b) {
    return new Vector2(
      a.x + b.x,
      a.z + b.y
    );
  }

  public static Coordinate operator *(Coordinate a, Coordinate b) {
    return (
      a.x * b.x,
      a.z * b.z
    );
  }

  public static Coordinate operator *(Coordinate a, int b) {
    return (
      a.x * b,
      a.z * b
    );
  }

  /// <summary>
  /// Is limited to (short.MAX, short.MAX)
  /// </summary>
  /// <returns></returns>
  public override int GetHashCode() {
    return (z << 16) | x;
  }

  /// <summary>
  /// Coord to string
  /// </summary>
  /// <returns></returns>
  public override string ToString() {
    return "{" + x + ", " + z + "}";
  }
}

#region Float Utilities

public static class RangeUtilities {

  /// <summary>
  /// Scale a float value to a new set of maxes and mins.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="newMax"></param>
  /// <param name="newMin"></param>
  /// <param name="oldMax"></param>
  /// <param name="oldMin"></param>
  /// <returns></returns>
  public static float scale(this float value, float newMax, float newMin, float oldMax = 1.0f, float oldMin = -1.0f) {
    float scaled = newMin + (value - oldMin) / (oldMax - oldMin) * (newMax - newMin);
    return scaled;
  }

  /// <summary>
  /// fast clamp a float to between 0 and 1
  /// </summary>
  /// <param name="value"></param>
  /// <param name="minValue"></param>
  /// <param name="maxValue"></param>
  /// <returns></returns>
  public static float clampToFloat(float value, int minValue, int maxValue) {
    return (
      (value - minValue)
      / (maxValue - minValue)
    );
  }

  /// <summary>
  /// fast clamp float to short
  /// </summary>
  /// <param name="value"></param>
  /// <param name="minFloat"></param>
  /// <param name="maxFloat"></param>
  /// <returns></returns>
  public static short clampToShort(float value, float minFloat = 0.0f, float maxFloat = 1.0f) {
    return (short)((short.MaxValue - short.MinValue)
      * ((value - minFloat) / (maxFloat - minFloat))
      + short.MinValue);
  }

  /// <summary>
  /// Clamp a value between two numbers
  /// </summary>
  /// <param name="value"></param>
  /// <param name="startingMin"></param>
  /// <param name="startingMax"></param>
  /// <param name="targetMin"></param>
  /// <param name="targetMax"></param>
  /// <returns></returns>
  public static double clamp(double value, double startingMin, double startingMax, double targetMin, double targetMax) {
    return (targetMax - targetMin)
      * ((value - startingMin) / (startingMax - startingMin))
      + targetMin;
  }

  /// <summary>
  /// Clamp the values between these numbers in a non scaling way.
  /// </summary>
  /// <param name="number"></param>
  /// <param name="min"></param>
  /// <param name="max"></param>
  /// <returns></returns>
  public static float box(this float number, float min, float max) {
    if (number < min)
      return min;
    else if (number > max)
      return max;
    else
      return number;
  }

  /// <summary>
  /// Box a float between 0 and 1
  /// </summary>
  /// <param name="number"></param>
  /// <returns></returns>
  public static float box01(this float number) {
    return box(number, 0, 1);
  }
}

#endregion