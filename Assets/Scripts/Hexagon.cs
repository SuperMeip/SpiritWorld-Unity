using UnityEngine;

public static class Hexagon {

  /// <summary>
  /// The sides of the hexigon in order
  /// Named by cardinal directions
  /// </summary>
  public enum Edges { N, NE, SE, S, SW, NW }

  /// <summary>
  /// The vertexes of the hexigons in order
  /// Named by carinal direction
  /// </summary>
  public enum Vertexes { NW, NE, E, SE, SW, W }

  /// <summary>
  /// Locations on a hexagon around the edges and corners
  /// </summary>
  public enum PerimeterSlots {
    Center,
    N_Edge,
    NE_Vertex,
    NE_Edge,
    E_Vertex,
    SE_Edge,
    SE_Vertex,
    S_Edge,
    SW_Vertex,
    SW_Edge,
    W_Vertex,
    NW_Edge,
    NW_Vertex
  }

  /// <summary>
  /// How many sides does a hexagon have
  /// </summary>
  public const int SideCount = 6;

  /// <summary>
  /// The legth of a hexagon side
  /// </summary>
  public const float SideLength = Universe.HexRadius;

  /// <summary>
  /// Inner radius of a hexagon
  /// </summary>
  static float InnerRadius = Universe.HexRadius * (Mathf.Sqrt(3) / 2);

  /// <summary>
  /// The offset values for axial coordinat hexagon neighbors
  /// </summary>
  public static Coordinate[] AxialOffests = {
    (0, 1),
    (1, 0),
    (1, -1),
    (0, -1),
    (-1 , 0),
    (-1, 1)
  };

  /// <summary>
  /// Degrees each angle vertex is offset with E being 0
  /// </summary>
  public static int[] VertexOffsetDegrees = {
    240,
    300,
    0,
    60,
    120,
    180
  };

  /// <summary>
  /// Degrees each angle vertex is offset with E being 0
  /// </summary>
  public static int[] EdgeCenterOffsetDegrees = {
    270,
    330,
    30,
    90,
    150,
    210
  };

  /// <summary>
  /// The 2 vertexes bordering each side of the hexagon.
  /// </summary>
  static Vertexes[][] VertexesPerEdge = {
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
  /// <param name="edge"></param>
  /// <returns></returns>
  public static Edges Opposite(Edges edge) {
    switch (edge) {
      case Edges.N:  return Edges.S;
      case Edges.NE: return Edges.SW;
      case Edges.SE: return Edges.NW;
      case Edges.S:  return Edges.N;
      case Edges.SW: return Edges.NE;
      case Edges.NW: return Edges.SE;
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
  public static Coordinate Move(Coordinate initlalLocation, Edges direction, int magnitude = 1) {
    return initlalLocation + AxialOffests[(int)direction] * magnitude;
  }

  /// <summary>
  /// Get the vertex from a given placement slot
  /// </summary>
  public static Vertexes GetVertexFromPerimeterSlot(PerimeterSlots placementLocation) {
    switch (placementLocation) {
      case PerimeterSlots.E_Vertex:
        return Vertexes.E;
      case PerimeterSlots.NE_Vertex:
        return Vertexes.NE;
      case PerimeterSlots.NW_Vertex:
        return Vertexes.NW;
      case PerimeterSlots.SE_Vertex:
        return Vertexes.SE;
      case PerimeterSlots.SW_Vertex:
        return Vertexes.SW;
      case PerimeterSlots.W_Vertex:
        return Vertexes.W;
      default:
        throw new System.ArgumentOutOfRangeException($"The hexagon placement location {placementLocation} does not have an associated vertex");
    }
  }

  /// <summary>
  /// Get the vertex from a given placement slot
  /// </summary>
  public static Edges GetEdgeFromPerimeterSlot(PerimeterSlots placementLocation) {
    switch (placementLocation) {
      case PerimeterSlots.N_Edge:
        return Edges.N;
      case PerimeterSlots.NE_Edge:
        return Edges.NE;
      case PerimeterSlots.NW_Edge:
        return Edges.NW;
      case PerimeterSlots.SE_Edge:
        return Edges.SE;
      case PerimeterSlots.SW_Edge:
        return Edges.SW;
      case PerimeterSlots.S_Edge:
        return Edges.S;
      default:
        throw new System.ArgumentOutOfRangeException($"The hexagon placement location {placementLocation} does not have an associated Edge/Side");
    }
  }

  /// <summary>
  /// Get all the vertexes for the given hexagon side
  /// </summary>
  /// <param name="side"></param>
  /// <returns></returns>
  public static Vertexes[] VerticiesFor(Edges side) {
    return VertexesPerEdge[(int)side];
  }

  /// <summary>
  /// Get a world location of an axial key
  /// </summary>
  /// <param name="hexAxialKey"></param>
  /// <returns></returns>
  public static Vector3 AxialKeyToWorldLocation(Coordinate hexAxialKey) {
    float x = Universe.HexRadius * (3.0f / 2.0f * hexAxialKey.x);
    float matrixX = (Mathf.Sqrt(3) / 2.0f) * hexAxialKey.x;
    float matrixZ = Mathf.Sqrt(3) * hexAxialKey.z;
    float z = Universe.HexRadius * (matrixX + matrixZ);

    return new Vector3(x, 0, z);
  }

  /// <summary>
  /// Convert a world location to an axial location
  /// </summary>
  /// <param name="hexWorldLocation"></param>
  /// <returns></returns>
  public static Coordinate WorldLocationToAxialKey(Vector3 hexWorldLocation) {
    return HexRound(WorldLocationToFractionalAxialKey(hexWorldLocation));
  }

  /// <summary>
  /// Convert a world location to an axial location
  /// </summary>
  /// <param name="hexWorldLocation"></param>
  /// <returns></returns>
  public static Vector2 WorldLocationToFractionalAxialKey(Vector3 hexWorldLocation) {
    return new Vector2(
      (2.0f / 3.0f * hexWorldLocation.x) / Universe.HexRadius,
      (-1.0f / 3.0f * hexWorldLocation.x + Mathf.Sqrt(3) / 3.0f * hexWorldLocation.z) / Universe.HexRadius
    );
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
  public static Vector3 VertexLocation(Vector3 hexagonCenterLocation, Vertexes vertex) {
    return VertexLocation(hexagonCenterLocation, vertex, SideLength, InnerRadius);
  }

  /// <summary>
  /// Get the location of the given vertex for a hexagon centered on the given location
  /// </summary>
  /// <param name="hexagonCenterLocation"></param>
  /// <param name="vertex"></param>
  /// <returns></returns>
  public static Vector3 VertexLocation(Vector3 hexagonCenterLocation, Vertexes vertex, float sideLength, float? innerRadius = null) {
    innerRadius = innerRadius ?? sideLength * (Mathf.Sqrt(3) / 2);

    switch (vertex) {
      case Vertexes.NW: return hexagonCenterLocation + new Vector3(-(sideLength / 2), 0, (float)innerRadius);
      case Vertexes.NE: return hexagonCenterLocation + new Vector3(sideLength / 2, 0, (float)innerRadius);
      case Vertexes.E: return hexagonCenterLocation + new Vector3(sideLength, 0, 0);
      case Vertexes.SE: return hexagonCenterLocation + new Vector3(sideLength / 2, 0, -(float)innerRadius);
      case Vertexes.SW: return hexagonCenterLocation + new Vector3(-(sideLength / 2), 0, -(float)innerRadius);
      case Vertexes.W: return hexagonCenterLocation + new Vector3(-sideLength, 0, 0);
      default: throw new System.Exception("Invalid vertex");
    }
  }

  /// <summary>
  /// Get the center point of the given edge of a hexagon at the given location
  /// </summary>
  /// <returns></returns>
  public static Vector3 EdgeCenterLocation(Vector3 hexagonCenterLocation, Edges edge, float sideLength, float? innerRadius = null) {
    float r = innerRadius ?? sideLength * (Mathf.Sqrt(3) / 2);

    switch (edge) {
      case Edges.N: return hexagonCenterLocation + new Vector3(0, 0, r);
      case Edges.NE: return hexagonCenterLocation + new Vector3(r, 0, r / 2);
      case Edges.SE: return hexagonCenterLocation + new Vector3(r, 0, -r / 2);
      case Edges.S: return hexagonCenterLocation + new Vector3(0, 0, -r);
      case Edges.SW: return hexagonCenterLocation + new Vector3(-r, 0, -r / 2);
      case Edges.NW: return hexagonCenterLocation + new Vector3(-r, 0, r / 2);
      default: throw new System.Exception("Invalid Edge");
    }
  }
}