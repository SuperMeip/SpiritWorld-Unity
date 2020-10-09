using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;

namespace SpiritWorld.World.Terrain.Features {
  public partial struct TileFeature {
    public static partial class Types {
      /// <summary>
      /// Small rocks scattered about
      /// </summary>
      public static Type DecorativeRocks = new DecorativeRocks();
    }

    /// <summary>
    /// Just some small decorative rocks
    /// </summary>
    public class DecorativeRocks : Type {
      internal DecorativeRocks() : base(
        6,
        "Rocks",
        Layer.Decoration
      ) {
        Drops = new System.Collections.Generic.Dictionary<Tool.Type, DropChanceCollection[]>[] {
          new System.Collections.Generic.Dictionary<Tool.Type, DropChanceCollection[]> {{
            Tool.Type.Any,
            new DropChanceCollection[] {
              new DropChanceCollection(
                new (int, Inventory)[] {
                  (1, new Inventory() {
                    {Item.Types.Stone, new Item(Item.Types.Stone, 1)}
                  })
                }
              )
            }
          }}
        };
      }
    }
  }
}
