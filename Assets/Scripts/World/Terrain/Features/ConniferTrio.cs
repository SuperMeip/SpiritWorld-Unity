using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using System;

namespace SpiritWorld.World.Terrain.Features {
  public partial struct TileFeature {
    public static partial class Types {
      public static Type ConniferTrio = new ConniferTrio();
    }

    /// <summary>
    /// 3 Connifer trees, one big, one small, one dead.
    /// </summary>
    public class ConniferTrio : Type {
      internal ConniferTrio() : base(
        1,
        "Firr Trees",
        Layer.Resource,
        true,
        2
      ) {
        Drops = new System.Collections.Generic.Dictionary<Tool.Type, DropChanceCollection[]>[] {
          // mode 0: stump
          new System.Collections.Generic.Dictionary<Tool.Type, DropChanceCollection[]> {{
            Tool.Type.Shovel,
            new DropChanceCollection[] {
              new DropChanceCollection(
                new (int, BasicInventory)[] {
                  (2, new BasicInventory{
                    {Item.Types.PineCone, new Item(Item.Types.PineCone, 1)},
                    {Item.Types.Wood, new Item(Item.Types.Wood, 2)}
                  }),
                  (3, new BasicInventory{
                    {Item.Types.Wood, new Item(Item.Types.Wood, 2)}
                  })
                }
              ),
            }
          }},
          // mode 1: Big Tree
          new System.Collections.Generic.Dictionary<Tool.Type, DropChanceCollection[]> {{
            Tool.Type.Axe,
            new DropChanceCollection[] {
              new DropChanceCollection(
                new (int, BasicInventory)[] {
                  (3, new BasicInventory{
                    {Item.Types.Spapple, new Item(Item.Types.Spapple, 1)},
                    {Item.Types.PineCone, new Item(Item.Types.PineCone, 1)},
                    {Item.Types.Wood, new Item(Item.Types.Wood, 2)}
                  }),
                  (1, new BasicInventory{
                    {Item.Types.PineCone, new Item(Item.Types.PineCone, 1)},
                    {Item.Types.Wood, new Item(Item.Types.Wood, 3)}
                  }),
                  (5, new BasicInventory{
                    {Item.Types.Wood, new Item(Item.Types.Wood, 2)}
                  })
                }
              ),
            }
          }},
          // mode 1: Small and Dead Tree
          new System.Collections.Generic.Dictionary<Tool.Type, DropChanceCollection[]> {{
            Tool.Type.Axe,
            new DropChanceCollection[] {
              new DropChanceCollection(
                new (int, BasicInventory)[] {
                  (1, new BasicInventory{
                    {Item.Types.Spapple, new Item(Item.Types.Spapple, 1)},
                    {Item.Types.Wood, new Item(Item.Types.Wood, 2)}
                  }),
                  (2, new BasicInventory{
                    {Item.Types.PineCone, new Item(Item.Types.PineCone, 1)},
                    {Item.Types.Wood, new Item(Item.Types.Wood, 2)}
                  }),
                  (3, new BasicInventory{
                    {Item.Types.Wood, new Item(Item.Types.Wood, 2)}
                  })
                }
              ),
            }
          }},
        };
      }
    }
  }
}
