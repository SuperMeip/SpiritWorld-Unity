using UnityEngine;

namespace SpiritWorld.World.Terrain.Features {
  public partial struct TileFeature {
    public static partial class Types {
      public static Type RockPile = new RockPile();
    }

    /// <summary>
    /// 3 Connifer trees, one big, one small, one dead.
    /// </summary>
    public class RockPile : Type {
      internal RockPile() : base(3, "Rocks", Layer.Decoration) {
        NumberOfModes = 3;
      }
    }
  }
}
