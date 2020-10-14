using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using System.Collections.Generic;

namespace SpiritWorld.World.Terrain.Features {
  public partial struct TileFeature {

    public static partial class Types {

      /// <summary>
      /// A mineable pile of rocks with iron
      /// </summary>
      public static Type IronVeinedRocks = new IronVeinedRocks();
    }

    /// <summary>
    /// 3 Connifer trees, one big, one small, one dead.
    /// </summary>
    public class IronVeinedRocks : TransitionalResourceType {
      internal IronVeinedRocks() : base(
        7,
        "Iron Veined Rocks",
        Layer.Resource,
        Types.RockPile,
        1
      ) {
        Drops = new Dictionary<Tool.Type, DropChanceCollection[]>[] {
          /// mode 0
          new Dictionary<Tool.Type, DropChanceCollection[]> {{
            Tool.Type.Any,
            new DropChanceCollection[] {
              new DropChanceCollection(
                new (int, BasicInventory)[] {
                  (5, new BasicInventory() {
                    {Item.Types.Iron, new Item(Item.Types.Iron, 1)}
                  }),
                  (1, new BasicInventory() {
                    {Item.Types.Iron, new Item(Item.Types.Iron, 2)}
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
