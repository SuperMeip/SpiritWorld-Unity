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
    /// A rock pile that shrinks when used
    /// </summary>
    public class RockPile : Type {
      internal RockPile() : base(
        3,
        "Rocks",
        Layer.Resource,
        true,
        2
      ) {
        Drops = new Dictionary<Tool.Type, DropChanceCollection[]>[] {
          /// mode 0
          new Dictionary<Tool.Type, DropChanceCollection[]> {{
            Tool.Type.Any,
            new DropChanceCollection[] {
              new DropChanceCollection(
                new (int, BasicInventory)[] {
                  (1, new BasicInventory() {
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
                new (int, BasicInventory)[] {
                  (1, new BasicInventory() {
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
                new (int, BasicInventory)[] {
                  (1, new BasicInventory() {
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
