using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using System.Collections.Generic;

namespace SpiritWorld.World.Terrain.Features {
  public partial struct TileFeature {
    public static partial class Types {
      public static Type RockPile = new RockPile();
    }

    /// <summary>
    /// 3 Connifer trees, one big, one small, one dead.
    /// </summary>
    public class RockPile : LimitedUseType {
      internal RockPile() : base(
        3,
        "Rocks",
        Layer.Resource,
        2
      ) {
        NumberOfModes = 3;
        Drops = new Dictionary<Tool.Type, DropChanceCollection[]>[] {
          new Dictionary<Tool.Type, DropChanceCollection[]> {{
            Tool.Type.Shovel,
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
