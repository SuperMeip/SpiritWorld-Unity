namespace SpiritWorld.World.Terrain.Features {
  public partial struct TileFeature {
    public static partial class Types {
      public static Type SmallLilypads = new SmallLilypads();
    }

    /// <summary>
    /// 3 Connifer trees, one big, one small, one dead.
    /// </summary>
    public class SmallLilypads : Type {
      internal SmallLilypads() : base(5, "Small Lilypads", Layer.Decoration) {}
    }
  }
}
