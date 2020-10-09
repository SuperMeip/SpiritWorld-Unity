using System.Linq;
using UnityEngine;

namespace SpiritWorld.World.Terrain.Constructs {

  /// <summary>
  /// A construct or structure on a tile, an item created and placed by a player.
  /// </summary>
  public partial struct TileStruct {

    /// <summary>
    /// The type of tile this is
    /// </summary>
    public Type type {
      get;
    } 

    /// <summary>
    /// If this is placed in the very center, around hte middle ring, or around the border of the hex
    /// </summary>
    public PlacementSlotTypes placementSlotType {
      get;
    }

    /// <summary>
    /// The slot this is placed in
    /// </summary>
    public Hexagon.PerimeterSlots placementSlot {
      get;
    }

    /// <summary>
    /// The direction of the 12 this is facing towards
    /// </summary>
    public Hexagon.PerimeterSlots facingDirection {
      get;
    }

    /// <summary>
    /// Make a new tile struct of the given type
    /// </summary>
    /// <param name="type"></param>
    public TileStruct(Type type) {
      this.type = type;

      // drop it in a default position that's valid for the type
      if (type.placementSlotTypes.Contains(PlacementSlotTypes.Center)) {
        placementSlot = Hexagon.PerimeterSlots.Center;
        placementSlotType = PlacementSlotTypes.Center;
      } else if (type.placementSlotTypes.Contains(PlacementSlotTypes.Inner)) {
        placementSlot = Hexagon.PerimeterSlots.E_Vertex;
        placementSlotType = PlacementSlotTypes.Inner;
      } else {
        placementSlot = Hexagon.PerimeterSlots.E_Vertex;
        placementSlotType = PlacementSlotTypes.Border;
      }

      facingDirection = Hexagon.PerimeterSlots.E_Vertex;
    }

    /// <summary>
    /// Get the world placement offset of the 
    /// </summary>
    public Vector3 getPlacementOffset() {
      if (placementSlotType == PlacementSlotTypes.Center) {
        return Vector3.zero;
      }

      // if this is spaced acording to a vertex as opposed to an edge among the 12 options
      bool isAlongVertex = (int)placementSlot % 2 == 0;

      // determine if this is being placed along the inside ring, or border ring of the hexagon.
      float distanceFromCenterOfHex = placementSlotType == PlacementSlotTypes.Border
        ? Universe.HexRadius
        : Universe.HexRadius / 2;

      return isAlongVertex
        ? Hexagon.VertexLocation(Vector3.zero, Hexagon.GetVertexFromPerimeterSlot(placementSlot), distanceFromCenterOfHex)
        : Hexagon.EdgeCenterLocation(Vector3.zero, Hexagon.GetEdgeFromPerimeterSlot(placementSlot), distanceFromCenterOfHex);
    }

    /// <summary>
    /// Get the rotation to use for this structure
    /// </summary>
    /// <returns></returns>
    public Quaternion getRotation() {
      // if this is spaced acording to a vertex as opposed to an edge among the 12 options
      bool isFacingVertex = (int)facingDirection % 2 == 0;
      
      return Quaternion.Euler(
        0,
        isFacingVertex
          ? Hexagon.VertexOffsetDegrees[(int)Hexagon.GetEdgeFromPerimeterSlot(facingDirection)]
          : Hexagon.EdgeCenterOffsetDegrees[(int)Hexagon.GetEdgeFromPerimeterSlot(facingDirection)],
        0
      );
    }
  }
}
