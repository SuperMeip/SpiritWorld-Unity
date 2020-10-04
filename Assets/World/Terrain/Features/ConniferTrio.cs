using System;

namespace SpiritWorld.World.Terrain.Features {
  public partial struct TileFeature {
    public static partial class Types {
      public static Type ConniferTrio = new ConniferTrio();
    }

    /// <summary>
    /// 3 Connifer trees, one big, one small, one dead.
    /// </summary>
    public class ConniferTrio : LimitedUseType {
      internal ConniferTrio() : base(1, Layer.Resource, 2) {}
    }
  }
}
