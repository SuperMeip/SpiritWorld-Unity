using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpiritWorld.World.Terrain.TileGrid {

  /// <summary>
  /// A mesh generator for the hex shaped terrain
  /// </summary>
  public class HexGridMeshGenerator {

    /// <summary>
    /// Changeable: The amount of hexagon top texture rows in the texture map
    /// </summary>
    const int HexTopTextureRowCount = 2;

    /// <summary>
    /// Changeable: The amount of hexagon top texture columns in the texture map
    /// </summary>
    const int HexTopTextureColumnCount = 2;

    /// <summary>
    /// Changeable: The amount of hexagon column side texture rows in the texture map
    /// </summary>
    const int HexColumnSideTextureRowCount = 1;

    /// <summary>
    /// The diameter (height and width) of the top texture of the hexagon
    /// </summary>
    const int HexTopTextureDiameter = 300;

    /// <summary>
    /// The height of the side texture of the hexagon column
    /// </summary>
    const int HexColumnSideTextureHeight = 400;

    /// <summary>
    /// The width of the side texture of the hexagon column
    /// </summary>
    const int HexColumnSideTextureWidth = 200;

    /// <summary>
    /// Shortcut for radius of the top hex texture
    /// </summary>
    const int HexTopTextureRadius = HexTopTextureDiameter / 2;

    /// <summary>
    /// The overall height of the texture map used to render
    /// </summary>
    const int TextureMapHeight = HexTopTextureDiameter * HexTopTextureRowCount + HexColumnSideTextureHeight;

    /// <summary>
    /// The overall width of the texture map used to render
    /// </summary>
    const int TextureMapWidth = HexTopTextureDiameter * HexTopTextureColumnCount;

    /// <summary>
    /// The 0,0 of the Hex Tops part of the texture map
    /// </summary>
    static Coordinate HexTopTexturesOrigin = (0, HexColumnSideTextureHeight * HexColumnSideTextureRowCount);

    /// <summary>
    /// All of the sides of a hexagon
    /// </summary>
    static Hexagon.Sides[] AllHexagonSides
      = (Hexagon.Sides[])Enum.GetValues(typeof(Hexagon.Sides));

    /// <summary>
    /// Generate the mesh for a grid of hex tiles
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="chunkLocation">The chunk location in world space / it's id</param>
    /// <returns></returns>
    public static Mesh generate(HexGrid tiles, Coordinate chunkLocation = default) {
      Vector3 chunkWorldOffset = Vector3.zero;
      List<Vector3> verticies = new List<Vector3>();
      List<int> triangles = new List<int>();
      List<Vector2> uvs = new List<Vector2>();
      int triangleCount = 0;

      /// Calculate the units to move all the verts over -X and -Z if this tile is in a chunk not at 0,0.
      if (!chunkLocation.Equals(Coordinate.Zero)) {
        chunkWorldOffset = new Vector3(
          -chunkLocation.x * RectangularBoard.ChunkWorldOffset.x,
          0
          -chunkLocation.z * RectangularBoard.ChunkWorldOffset.z
        );
      }

      // Generate each tile
      tiles.forEach((axialKey, tile) => {
        /// Get vars
        // the center of the hex tile's top in space
        Vector3 hexCenter = Hexagon.AxialKeyToWorldLocation(axialKey);
        // the center of the hex top's texture on the texture map in pixels
        Coordinate textureCenter = HexTopTexturesOrigin // top texture origins
          + (tile.type.TopTextureMapLocation * HexTopTextureDiameter) // this type's texture's origin
          + (HexTopTextureRadius); // move to the middle of texture
        Vector2[] columnSideUVs = GetHexColumnSideUVCoordinates(tile.type);

        // generate the 6 sides of each tile and the 6 top triangles
        foreach (Hexagon.Sides side in AllHexagonSides) {
          Vector3[] sideVerts = null;
          Tile neighboringTile = tiles.get(Hexagon.Move(axialKey, side));
          hexCenter.y = tile.height * Universe.StepHeight;

          /// Generate the upright side.
          // we only need to generate a mesh for this side if the neighbor is shorter or empty
          if (neighboringTile.type == Tile.Types.Empty || neighboringTile.height < tile.height) {
            sideVerts = GenerateVertsForHexBlockSide(
              hexCenter,
              side,
              neighboringTile.height * Universe.StepHeight,
              tile.height * Universe.StepHeight,
              chunkWorldOffset
            );
            verticies.AddRange(sideVerts);
            triangles.AddRange(new int[]{
              triangleCount + 5,
              triangleCount + 4,
              triangleCount + 3,
              triangleCount + 2,
              triangleCount + 1,
              triangleCount
            });
            uvs.AddRange(columnSideUVs);
            triangleCount += 6;
          }

          /// Generate the top triangle
          Vector3[] topTriangleVerts = new Vector3[3];
          if (sideVerts != null) {
            // we can use the side verts if we already have them
            topTriangleVerts[0] = sideVerts[1];
            topTriangleVerts[1] = sideVerts[2];
          } else {
            // if we don't have them yet, we just need to loop through and calculate them quick
            int index = 0;
            foreach (Hexagon.Vertexes vertex in Hexagon.VerticiesFor(side)) {
              Vector3 worldVertLocation = Hexagon.VertexLocation(hexCenter, vertex);
              topTriangleVerts[index++] = worldVertLocation += chunkWorldOffset;
            }
          }

          // then we just need the center point at the correct height
          topTriangleVerts[2] = hexCenter + chunkWorldOffset;

          // UVs for the top
          Vector2[] topTriangleUVs = GetHexTopUVCoordinates(textureCenter, side);

          // index the top triangle of 3 verts
          verticies.AddRange(topTriangleVerts);
          triangles.AddRange(new int[] { triangleCount, triangleCount + 1, triangleCount + 2 });
          uvs.AddRange(topTriangleUVs);
          triangleCount += 3;
        }
      });

      Mesh mesh = new Mesh {
        vertices = verticies.ToArray(),
        triangles = triangles.ToArray()
      };
      mesh.SetUVs(0, uvs);

      return mesh;
    }

    /// <summary>
    /// Generate the 2 tris worth of verticies for the given side of the given hexagon
    /// </summary>
    /// <param name="hexBlockCenter">The center location of the hexagon this side is for</param>
    /// <param name="side">The side we want the verts for</param>
    /// <param name="bottomHeight">The bottom height (where the side begins to be exposed from the ground up)</param>
    /// <param name="topHeight">The height of the top of the side (where it meets the hex face)</param>
    /// <param name="chunkWorldOffset">The position of the 0,0 of the chunk, used to offset the world location of the tiles for mesh construction in other chunks</param>
    /// <returns></returns>
    static Vector3[] GenerateVertsForHexBlockSide(
      Vector3 hexBlockCenter,
      Hexagon.Sides side,
      float bottomHeight,
      float topHeight,
      Vector3 chunkWorldOffset = default
    ) {
      Vector3[] verticies = new Vector3[4];
      int index = 0;
      foreach (Hexagon.Vertexes vertex in Hexagon.VerticiesFor(side)) {
        Vector3 vertexLocation2D = Hexagon.VertexLocation(hexBlockCenter, vertex);
        vertexLocation2D += chunkWorldOffset;
        Vector3 bottomVertexLocation = new Vector3(vertexLocation2D.x, bottomHeight, vertexLocation2D.z);
        Vector3 topVertexLocation = new Vector3(vertexLocation2D.x, topHeight, vertexLocation2D.z);
        verticies[index++] = bottomVertexLocation;
        verticies[index++] = topVertexLocation;
      }

      // Return the calculated points as 2 triangles
      return new Vector3[] {
        verticies[0],
        verticies[1],
        verticies[3],
        verticies[0],
        verticies[3],
        verticies[2]
      };
    }

    /// <summary>
    /// Get the uvs for the given triangle for the given texture.
    /// </summary>
    /// <param name="tileType"></param>
    static Vector2[] GetHexTopUVCoordinates(Coordinate textureCenter, Hexagon.Sides triangle) {
      int vertexIndex = 0;
      Vector2[] uvs = new Vector2[3];

      // for the two poitns on the sides, calculate their location on the tilemap
      foreach (Hexagon.Vertexes vertex in Hexagon.VerticiesFor(triangle)) {
        Vector3 uv3D = Hexagon.VertexLocation(textureCenter.Vec3, vertex, HexTopTextureRadius);
        uvs[vertexIndex++] = new Vector2(uv3D.x / TextureMapWidth, uv3D.z / TextureMapHeight);
      }
      // add in the center point last
      uvs[2] = new Vector2((float)textureCenter.x / TextureMapWidth, (float)textureCenter.z / TextureMapHeight);

      return uvs;
    }

    /// <summary>
    /// Get all of the texture UVs for the given locations
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    static Vector2[] GetHexColumnSideUVCoordinates(Tile.Type tileType) {
      int index = 0;
      Vector2[] uvs = new Vector2[6];
      Coordinate textureOrigin = (tileType.SideTextureMapIndex * HexColumnSideTextureWidth, 0);

      /// calculate all 6 pixel locations for the points
      Vector2[] pixelLocations = new Vector2[] {
        textureOrigin + new Vector2(HexColumnSideTextureWidth, 0),
        textureOrigin + new Vector2(HexColumnSideTextureWidth, HexColumnSideTextureHeight),
        textureOrigin + new Vector2(0, HexColumnSideTextureHeight),
        textureOrigin + new Vector2(HexColumnSideTextureWidth, 0),
        textureOrigin + new Vector2(0, HexColumnSideTextureHeight),
        textureOrigin + new Vector2(0, 0),
      };

      /// pecentagize them
      foreach (Vector2 pixelLocation in pixelLocations) {
        uvs[index++] = new Vector2(pixelLocation.x / TextureMapWidth, pixelLocation.y / TextureMapHeight);
      }

      return uvs;
    }
  }
}
