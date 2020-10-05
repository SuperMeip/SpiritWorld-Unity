namespace SpiritWorld.World.Terrain.TileGrid {

  /// <summary>
  /// Extensions for the Water tile type
  /// </summary>
  public partial struct Tile {

    /// <summary>
    /// Tile type singleton constants
    /// </summary>
    public static partial class Types {
      public static Type Water = new Water();
    }

    /// <summary>
    /// Empty tile type
    /// </summary>
    public class Water : Type {
      internal Water() : base(3, "Fresh Water") {
        TopTextureMapLocation = (1, 0);
      }
    }
  }
}