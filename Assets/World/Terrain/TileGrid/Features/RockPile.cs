using UnityEngine;

namespace SpiritWorld.World.Terrain.TileGrid.Features {
  public partial struct TileFeature {
    public static partial class Types {
      public static Type RockPile = new RockPile();
    }

    /// <summary>
    /// 3 Connifer trees, one big, one small, one dead.
    /// </summary>
    public class RockPile : Type {
      internal RockPile() : base(3, Layer.Decoration) {
        InteractionCount = 2;
        IsInteractive = false;
      }

      /// <summary>
      /// Set the cloud type based on the 3 possibl models
      /// </summary>
      /// <param name="feature"></param>
      internal override void initializeFeature(TileFeature feature) {
        feature.remainingInteractions = Random.Range(0, 3);
      }
    }
  }
}
