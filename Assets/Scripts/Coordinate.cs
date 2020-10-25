using System;
using System.Collections.Generic;
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
  /// up down in 2D
  /// </summary>
  public short y => z;

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
  /// get this as a cube coord
  /// </summary>
  public Vector3 Cube {
    get => new Vector3(x, -x - z, z);
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
  /// preform the acton on all coordinates between this one and the end coordinate
  /// </summary>
  /// <param name="end">The final point to run on, exclusive</param>
  /// <param name="action">the function to run on each point</param>
  /// <param name="step">the value by which the coordinate values are incrimented</param>
  public void until(Coordinate end, Action<Coordinate> action, short step = 1) {
    Coordinate current = (x, z);
    for (current.x = x; current.x < end.x; current.x += step) {
      for (current.z = z; current.z < end.z; current.z += step) {
        action(current);
      }
    }
  }

  /// <summary>
  /// preform the acton on all coordinates between this one and the end coordinate
  /// </summary>
  /// <param name="end">The final point to run on, exclusive</param>
  /// <param name="action">the function to run on each point</param>
  /// <param name="step">the value by which the coordinate values are incrimented</param>
  public void until(Coordinate end, Func<Coordinate, bool> action, short step = 1) {
    Coordinate current = (x, z);
    for (current.x = x; current.x < end.x; current.x += step) {
      for (current.z = z; current.z < end.z; current.z += step) {
        if (!action(current)) return;
      }
    }
  }

  /// <summary>
  /// Checks if this coordinate is within a set, dictated by boundaries. (inclusive, and exclusive)
  /// </summary>
  public bool isWithin((Coordinate minInclusive, Coordinate maxExclusive) bounds) {
    return isWithin(bounds.minInclusive, bounds.maxExclusive);
  }

  /// <summary>
  /// Checks if this coordinate is within a set, dictated by boundaries. (inclusive, and exclusive)
  /// </summary>
  /// <param name="start">The starting location to check if the point is within, inclusive</param>
  /// <param name="bounds">The outer boundary to check for point inclusion. Exclusive</param>
  /// <returns></returns>
  public bool isWithin(Coordinate start, Coordinate bounds) {
    return isWithin(bounds) && isAtLeastOrBeyond(start);
  }

  /// <summary>
  /// Checks if this coordinate is within a set, dictated by boundaries. (inclusive, and exclusive)
  /// </summary>
  /// <param name="bounds">The inner [0](inclusive) and outer [1](exclusive) boundary to check for point inclusion</param>
  /// <returns></returns>
  public bool isWithin(Coordinate[] bounds) {
    if (bounds != null && bounds.Length == 2) {
      return isWithin(bounds[1]) && isAtLeastOrBeyond(bounds[0]);
    } else throw new ArgumentOutOfRangeException("Coordinate.isWithin must take an array of size 2.");
  }

  /// <summary>
  /// Checks if this coordinate is greater than (or equal to) a lower bounds coordinate (Inclusive)
  /// </summary>
  public bool isAtLeastOrBeyond(Coordinate bounds) {
    return x >= bounds.x
      && z >= bounds.z;
  }

  /// <summary>
  /// Checks if this coordinate is within a bounds coodinate (exclusive)
  /// </summary>
  public bool isWithin(Coordinate bounds) {
    return x < bounds.x
      && z < bounds.z;
  }

  /// implicit opperators
  ///===================================
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
  /// Create a vector 3 from a coordinate
  /// </summary>
  /// <param name="coordinates">(X, Z)</param>
  public static implicit operator Vector3(Coordinate coordinate) {
    return new Vector3(coordinate.x, 0, coordinate.z);
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
  
  public static Coordinate operator -(Coordinate a, Coordinate b) {
    return (
      a.x - b.x,
      a.z - b.z
    );
  }
  
  public static Coordinate operator -(Coordinate a, int b) {
    return (
      a.x - b,
      a.z - b
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


  public static Vector3 operator *(Vector3 a, Coordinate b) {
     return new Vector3(
      a.x * b.x,
      a.y,
      a.z * b.z
    );
  }

  public static Coordinate operator *(Coordinate a, int b) {
    return (
      a.x * b,
      a.z * b
    );
  }

  public static Vector3 operator %(Coordinate a, Vector3 b) {
    return new Vector3(
      a.x % b.x,
      b.y,
      a.z % b.z
    );
  }

  public static Vector3 operator +(Coordinate a, Vector3 b) {
    return new Vector3(
      a.x + b.x,
      b.y,
      a.z + b.z
    );
  }

  public static Vector3 operator +(Vector3 a, Coordinate b) {
    return new Vector3(
      a.x + b.x,
      a.y,
      a.z + b.z
    );
  }

  public static bool operator >(Coordinate a, Coordinate b) {
    return a.x > b.x || a.y > b.y;
  }

  public static bool operator <(Coordinate a, Coordinate b) {
    return a.x < b.x && a.y < b.y;
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

  /// <summary>
  /// deconstruct
  /// </summary>
  /// <param name="x"></param>
  /// <param name="z"></param>
  internal void Deconstruct(out int x, out int z) {
    x = this.x;
    z = this.z;
  }
}

#region Directions

/// <summary>
/// Direction constants
/// </summary>
public static class Directions {

  /// <summary>
  /// A valid direction
  /// </summary>
  public class Direction : IEquatable<Direction> {

    /// <summary>
    /// The id of the direction
    /// </summary>
    public int Value {
      get;
      private set;
    }

    /// <summary>
    /// The name of this direction
    /// </summary>
    public string Name {
      get;
      private set;
    }

    /// <summary>
    /// The x y z offset of this direction from the origin
    /// </summary>
    public Coordinate Offset {
      get => Offsets[Value];
    }

    /// <summary>
    /// Get the oposite of this direction
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Direction Reverse {
      get {
        if (Equals(North)) {
          return South;
        }
        if (Equals(South)) {
          return North;
        }
        if (Equals(East)) {
          return West;
        }

        return East;
      }
    }

    internal Direction(int value, string name) {
      Value = value;
      Name = name;
    }

    /// <summary>
    /// To string
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
      return Name;
    }

    public override int GetHashCode() {
      return Value;
    }

    /// <summary>
    /// Equatable
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Direction other) {
      return other.Value == Value;
    }

    /// <summary>
    /// Override equals
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj) {
      return (obj != null)
        && !GetType().Equals(obj.GetType())
        && ((Direction)obj).Value == Value;
    }
  }

  /// <summary>
  /// Z+
  /// </summary>
  public static Direction North = new Direction(0, "North");

  /// <summary>
  /// X+
  /// </summary>
  public static Direction East = new Direction(1, "East");

  /// <summary>
  /// Z-
  /// </summary>
  public static Direction South = new Direction(2, "South");

  /// <summary>
  /// X-
  /// </summary>
  public static Direction West = new Direction(3, "West");

  /// <summary>
  /// All the directions in order
  /// </summary>
  public static Direction[] All = new Direction[4] {
      North,
      East,
      South,
      West
    };

  /// <summary>
  /// The cardinal directions. Non Y related
  /// </summary>
  public static Direction[] Cardinal = new Direction[4] {
      North,
      East,
      South,
      West
    };

  /// <summary>
  /// The coordinate directional offsets
  /// </summary>
  public static Coordinate[] Offsets = new Coordinate[4] {
      (0,1),
      (1,0),
      (0,-1),
      (-1, 0)
    };
}

#endregion

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

#region Array Utilities
public static class ArrayUtilities {

  public static void Populate<T>(this T[,] arr, T value) {
    for (int i = 0; i < arr.Length; i++) {
      for (int j = 0; j < arr.Length; j++) {
        arr[i, j] = value;
      }
    }
  }

  public static void Deconstruct<TKey, TValue>(
      this KeyValuePair<TKey, TValue> kvp,
      out TKey key,
      out TValue value) {
    key = kvp.Key;
    value = kvp.Value;
  }

  public static void Populate<T>(this T[] arr, T value) {
    for (int i = 0; i < arr.Length; i++) {
        arr[i] = value;
    }
  }
}
  #endregion

#region Sort Utilities

  public static class SortExtentions {
  private static System.Random rng = new System.Random();

  public static IList<T> Shuffle<T>(this IList<T> list) {
    int n = list.Count;
    while (n > 1) {
      n--;
      int k = rng.Next(n + 1);
      T value = list[k];
      list[k] = list[n];
      list[n] = value;
    }

    return list;
  }
}

#endregion

#region Bit Utilities

public static class BitIntExtentions {
  public static int TurnBitOn(this int value, int bitToTurnOn) {
    return (value | bitToTurnOn);
  }

  public static int TurnBitOff(this int value, int bitToTurnOff) {
    return (value & ~bitToTurnOff);
  }

  public static int FlipBit(this int value, int bitToFlip) {
    return (value ^ bitToFlip);
  }
}

#endregion