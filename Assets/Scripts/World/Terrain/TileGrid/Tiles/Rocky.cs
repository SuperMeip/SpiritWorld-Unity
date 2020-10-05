namespace SpiritWorld.World.Terrain.TileGrid {

  /// <summary>
  /// Extensions for the Grass tile type
  /// </summary>
  public partial struct Tile {

    /// <summary>
    /// Tile type singleton constants
    /// </summary>
    public static partial class Types {
      public static Type Rocky = new Rocky();
    }

    /// <summary>
    /// Empty tile type
    /// </summary>
    public class Rocky : Type {
      internal Rocky() : base(2, "Rocky") {
        TopTextureMapLocation = (1, 1);
        SideTextureMapIndex = 2;
      }
    }
  }
}