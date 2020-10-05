namespace SpiritWorld.World.Terrain.TileGrid {
  public partial struct Tile {

    /// <summary>
    /// Tile type singleton constants
    /// </summary>
    public static partial class Types {
      public static Type Grass = new Grass();
    }

    /// <summary>
    /// Empty tile type
    /// </summary>
    public class Grass : Type {
      internal Grass() : base(1, "Grassy") {
        TopTextureMapLocation = (0, 1);
        SideTextureMapIndex = 1;
      }
    }
  }
}