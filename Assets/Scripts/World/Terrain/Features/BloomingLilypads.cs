using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using System.Collections.Generic;

namespace SpiritWorld.World.Terrain.Features {
  public partial struct TileFeature {
    public static partial class Types {
      public static Type BloomingLilypads = new BloomingLilypads();
    }

    /// <summary>
    /// 3 Connifer trees, one big, one small, one dead.
    /// </summary>
    public class BloomingLilypads : LimitedUseType {
      internal BloomingLilypads() : base(
        4,
        "Blooming Lilypads",
        Layer.Resource,
        1
      ) {
        Drops = new Dictionary<Tool.Type, DropChanceCollection[]>[] {
          new Dictionary<Tool.Type, DropChanceCollection[]> {{
            Tool.Type.Any,
            new DropChanceCollection[] {
              // mode 0: empty
              new DropChanceCollection(
                new (int, Inventory)[0]
              ),
              // mode 1: has flowers
              new DropChanceCollection(
                new (int, Inventory)[] {
                  (2, new Inventory{
                    {Item.Types.WaterLily, new Item(Item.Types.WaterLily, 1)},
                  }),
                  (1, new Inventory{
                    {Item.Types.WaterLily, new Item(Item.Types.WaterLily, 2)},
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
