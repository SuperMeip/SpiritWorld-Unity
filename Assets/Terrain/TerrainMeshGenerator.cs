using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A mesh generator for the terrain
/// </summary>
class TerrainMeshGenerator {

  public static Mesh generate(HexGrid tiles) {
    // Hexes are generated in 'HexBlocks' six sided blocks.
    // Since many blocks will share sides, we determine the 
    // side tris and vers once (checking for collisions)
    // and them squash them all.
    var allHexagonSides = Enum.GetValues(typeof(Hexagon.Sides));
    List<Vector3> verticies = new List<Vector3>();
    List<int> triangles = new List<int>();
    int triangleCount = 0;

    // Generate each tile
    tiles.forEach((location, tile) => {
      byte hexBlockHeight = tile.height;

      // generate the 6 sides of each tile and the 6 top triangles
      foreach (Hexagon.Sides side in allHexagonSides) {
        Vector3[] sideVerts = null;
        Tile neighboringTile = tiles.get(Hexagon.Move(location, side));

        // Generate the upright side.
        // we only need to generate a mesh for this side if the neighbor is shorter or empty
        if (neighboringTile.type == Tile.Types.Empty || neighboringTile.height < tile.height) {
          sideVerts = GenerateVertsForHexBlockSide(location, side, neighboringTile.height, tile.height);
          verticies.AddRange(sideVerts);
          triangles.AddRange(new int[]{
            triangleCount,
            triangleCount + 2,
            triangleCount + 1,
            triangleCount,
            triangleCount + 3,
            triangleCount + 2
          });
          triangleCount += 6;
        }

        // Generate the top triangle
        Vector3[] topTriangleVerts = new Vector3[3];
        if (sideVerts != null) {
          // we can use the side verts if we already have them
          topTriangleVerts[0] = sideVerts[0];
          topTriangleVerts[1] = sideVerts[1];
        } else {
          // if we don't have them yet, we just need to loop through and calculate them quick
          int index = 0;
          foreach (Hexagon.Vertexes vertex in Hexagon.VertexesFor(side)) {
            topTriangleVerts[index++] = Hexagon.VertexLocation(location, vertex);
          }
        }
        // then we just need the center point at the correct height
        topTriangleVerts[2] = new Vector3(location.Vec2.x, tile.height, location.Vec2.y);

        verticies.AddRange(topTriangleVerts);
        triangles.AddRange(new int[] { triangleCount, triangleCount + 1, triangleCount + 2 });
        triangleCount += 3;
      }
    });

    Mesh mesh = new Mesh {
      vertices = verticies.ToArray(),
      triangles = triangles.ToArray()
    };

    return mesh;
  }

  /// <summary>
  /// Generate the 2 tris worth of verticies for the given side of the given hexagon
  /// </summary>
  /// <param name="hexBlockCenter">The center location of the hexagon this side is for</param>
  /// <param name="side">The side we want the verts for</param>
  /// <param name="bottomHeight">The bottom height (where the side begins to be exposed from the ground up)</param>
  /// <param name="topHeight">The height of the top of the side (where it meets the hex face)</param>
  /// <returns></returns>
  static Vector3[] GenerateVertsForHexBlockSide(Coordinate hexBlockCenter, Hexagon.Sides side, int bottomHeight, int topHeight) {
    Vector3[] verticies = new Vector3[4];
    int index = 0;
    foreach (Hexagon.Vertexes vertex in Hexagon.VertexesFor(side)) {
      Vector2 vertexLocation2D = Hexagon.VertexLocation(hexBlockCenter, vertex);
      Vector3 bottomVertexLocation =  new Vector3 (vertexLocation2D.x, bottomHeight, vertexLocation2D.y);
      Vector3 topVertexLocation = new Vector3(vertexLocation2D.x, topHeight, vertexLocation2D.y);
      verticies[index++] = bottomVertexLocation;
      verticies[index++] = topVertexLocation;
    }

    return verticies;
  }
}
