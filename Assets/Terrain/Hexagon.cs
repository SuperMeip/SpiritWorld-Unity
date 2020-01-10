using UnityEngine;

public static class Hexagon {

  /// <summary>
  /// The sides of the hexigon in order
  /// Named by cardinal directions
  /// </summary>
  public enum Sides { N, NE, SE, S, SW, NW}

  /// <summary>
  /// The vertexes of the hexigons in order
  /// Named by carinal direction
  /// </summary>
  public enum Vertexes { NW, NE, E, SE, SW, W}

  /// <summary>
  /// How many sides does a hexagon have
  /// </summary>
  public const int SideCount = 6;

  /// <summary>
  /// The legth of a hexagon side
  /// </summary>
  public const float SideLength = 0.25f;

  /// <summary>
  /// The diameter of the hexagons
  /// </summary>
  public const float Diameter = SideLength * 2;

  /// <summary>
  /// The offset values for axial coordinat hexagon neighbors
  /// </summary>
  public static Coordinate[] AxialOffests = {
    (0, -1),
    (1, -1),
    (1 , 0),
    (0, 1),
    (-1, 1),
    (-1, 0)
  };

  /// <summary>
  /// The 2 vertexes bordering each side of the hexagon.
  /// </summary>
  static Vertexes[][] VertexesPerSide = {
    new Vertexes[] {Vertexes.NW, Vertexes.NE },
    new Vertexes[] {Vertexes.NE, Vertexes.E },
    new Vertexes[] {Vertexes.E, Vertexes.SE },
    new Vertexes[] {Vertexes.SE, Vertexes.SW },
    new Vertexes[] {Vertexes.SW, Vertexes.W },
    new Vertexes[] {Vertexes.W, Vertexes.NW }
  };

  /// <summary>
  /// Get the opposite side of the hexagon
  /// </summary>
  /// <param name="side"></param>
  /// <returns></returns>
  public static Sides Opposite(Sides side) {
    switch (side) {
      case Sides.N:  return Sides.S;
      case Sides.NE: return Sides.SW;
      case Sides.SE: return Sides.NW;
      case Sides.S:  return Sides.N;
      case Sides.SW: return Sides.NE;
      case Sides.NW: return Sides.SE;
      default: throw new System.Exception("Invalid side");
    }
  }

  /// <summary>
  /// Get the neighnoring hexagon's location
  /// </summary>
  /// <param name="initlalLocation">the starting hexagon's location</param>
  /// <param name="direction">the side direction to move in</param>
  /// <param name="magnitude">how many hexagons to move in the given side direction</param>
  /// <returns></returns>
  public static Coordinate Move(Coordinate initlalLocation, Sides direction, int magnitude = 1) {
    return initlalLocation + AxialOffests[(int)direction] * magnitude;
  }

  /// <summary>
  /// Get all the vertexes for the given hexagon side
  /// </summary>
  /// <param name="side"></param>
  /// <returns></returns>
  public static Vertexes[] VertexesFor(Sides side) {
    return VertexesPerSide[(int)side];
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="hexAxialKey"></param>
  /// <returns></returns>
  public static Vector2 AxialKeyToWorldLocation(Coordinate hexAxialKey) {
    return new Vector2(
      World.HexSize * (3 / 2 * hexAxialKey.x),
      World.HexSize * (Mathf.Sqrt(3) / 2 * hexAxialKey.x + Mathf.Sqrt(3) * hexAxialKey.y)
    );
  }

  /// <summary>
  /// Convert a world location to an axial location
  /// </summary>
  /// <param name="hexWorldLocation"></param>
  /// <returns></returns>
  public static Coordinate WorldLocationToAxialKey(Vector3 hexWorldLocation) {
    return HexRound(new Vector2(
      (2/3 * hexWorldLocation.x) / World.HexSize,
      (-1/3 * hexWorldLocation.x + Mathf.Sqrt(3)/3 * hexWorldLocation.y) / World.HexSize
    ));
  }

  /// <summary>
  /// Convert cube hex coordinates to axial hex coordinates
  /// </summary>
  /// <param name="hexCubeKey"></param>
  /// <returns></returns>
  static Coordinate CubeToAxialKey(Vector3 hexCubeKey) {
    return (
      (int)hexCubeKey.x,
      (int)hexCubeKey.z
    );
  }

  /// <summary>
  /// Convert axial hex coordinates to cube hex coordinates
  /// </summary>
  /// <param name="hexAxialKey"></param>
  /// <returns></returns>
  static Vector3 AxialToCubeKey(Vector2 hexAxialKey) {
    return new Vector3(
      hexAxialKey.x,
      -hexAxialKey.x - hexAxialKey.y,
      hexAxialKey.y
    );
  }

  /// <summary>
  /// Round floating axial coordinates
  /// </summary>
  /// <param name="worldLocation2D"></param>
  /// <returns></returns>
  static Coordinate HexRound(Vector2 worldLocation2D) {
    return CubeToAxialKey(CubeRound(AxialToCubeKey(worldLocation2D)));
  }

  /// <summary>
  /// round floating cube coordinates
  /// </summary>
  /// <param name="worldLocation"></param>
  /// <returns></returns>
  static Vector3 CubeRound(Vector3 worldLocation) {
    Vector3 roundedLocation = new Vector3 (
      Mathf.RoundToInt(worldLocation.x),
      Mathf.RoundToInt(worldLocation.y),
      Mathf.RoundToInt(worldLocation.z)
    );

    float xDiff = Mathf.Abs(roundedLocation.x - worldLocation.x);
    float yDiff = Mathf.Abs(roundedLocation.y - worldLocation.y);
    float zDiff = Mathf.Abs(roundedLocation.z - worldLocation.z);

    if (xDiff > yDiff && xDiff > zDiff) {
      roundedLocation.x = -roundedLocation.y - roundedLocation.z;
    } else if (yDiff > zDiff) {
      roundedLocation.y = -roundedLocation.x - roundedLocation.z;
    } else {
      roundedLocation.z = -roundedLocation.x - roundedLocation.y;
    }

    return roundedLocation;
  }

  /// <summary>
  /// Get the location of the given vertex for a hexagon centered on the given location
  /// </summary>
  /// <param name="hexagonCenterLocation"></param>
  /// <param name="vertex"></param>
  /// <returns></returns>
  public static Vector2 VertexLocation(Coordinate hexagonCenterLocation, Vertexes vertex) {
    switch (vertex) {
      case Vertexes.NW: return hexagonCenterLocation.Vec2 + new Vector2(-(SideLength / 2), SideLength);
      case Vertexes.NE: return hexagonCenterLocation.Vec2 + new Vector2(SideLength / 2, SideLength);
      case Vertexes.E:  return hexagonCenterLocation.Vec2 + new Vector2(SideLength, 0);
      case Vertexes.SE: return hexagonCenterLocation.Vec2 + new Vector2(SideLength / 2, -SideLength);
      case Vertexes.SW: return hexagonCenterLocation.Vec2 + new Vector2(-(SideLength / 2), -SideLength);
      case Vertexes.W:  return hexagonCenterLocation.Vec2 + new Vector2(-SideLength, 0);
      default: throw new System.Exception("Invalid vertex");
    }
  }
}