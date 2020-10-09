using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using System.Collections.Generic;

namespace SpiritWorld.World.Terrain.Features {
  public partial struct TileFeature {
    public static partial class Types {
      /// <summary>
      /// A mineable pile of rocks
      /// </summary>
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
        Drops = new Dictionary<Tool.Type, DropChanceCollection[]>[] {
          /// mode 0
          new Dictionary<Tool.Type, DropChanceCollection[]> {{
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
          }},
          /// mode 1
          new Dictionary<Tool.Type, DropChanceCollection[]> {{
            Tool.Type.Pickaxe,
            new DropChanceCollection[] {
              new DropChanceCollection(
                new (int, Inventory)[] {
                  (1, new Inventory() {
                    {Item.Types.Stone, new Item(Item.Types.Stone, 2)}
                  })
                }
              )
            }
          }},
          /// mode 2
          new Dictionary<Tool.Type, DropChanceCollection[]> {{
            Tool.Type.Pickaxe,
            new DropChanceCollection[] {
              new DropChanceCollection(
                new (int, Inventory)[] {
                  (1, new Inventory() {
                    {Item.Types.Stone, new Item(Item.Types.Stone, 2)}
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
