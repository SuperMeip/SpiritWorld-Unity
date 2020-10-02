using UnityEngine;

namespace SpiritWorld.World.Terrain.Features {
  public partial struct TileFeature {
    public static partial class Types {
      public static Type WhiteClouds = new WhiteClouds();
    }

    /// <summary>
    /// 3 Connifer trees, one big, one small, one dead.
    /// </summary>
    public class WhiteClouds : Type {
      internal WhiteClouds() : base(2, Layer.Sky) {
        NumberOfModes = 3;
      }
    }
  }
}
