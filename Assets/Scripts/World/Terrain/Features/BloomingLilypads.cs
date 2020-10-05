namespace SpiritWorld.World.Terrain.Features {
  public partial struct TileFeature {
    public static partial class Types {
      public static Type BloomingLilypads = new BloomingLilypads();
    }

    /// <summary>
    /// 3 Connifer trees, one big, one small, one dead.
    /// </summary>
    public class BloomingLilypads : LimitedUseType {
      internal BloomingLilypads() : base(4, "Blooming Lilypads", Layer.Resource, 1) {}
    }
  }
}
