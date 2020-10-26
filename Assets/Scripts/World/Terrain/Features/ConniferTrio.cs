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

        Drops = ParseDropDataJSON(@"
          [
            [
              {
                ""Tool"": ""Shovel"",
                ""DropPossibilities"" : [
                  [
                    {
                      ""Weight"": 2,
                      ""Items"": ""3:1,2:2""
                    },
                    {
                      ""Weight"": 3,
                      ""Items"": ""2:2""
                    }
                  ]
                ]
              }
            ],
            [
              {
                ""Tool"": ""Axe"",
                ""DropPossibilities"" : [
                  [
                    {
                      ""Weight"": 3,
                      ""Items"": ""1:1,3:1,2:2""
                    },
                    {
                      ""Weight"": 1,
                      ""Items"": ""2:3,3:1""
                    },
                    {
                      ""Weight"": 5,
                      ""Items"": ""2:2""
                    }
                  ]
                ]
              }
            ],
            [
              {
                ""Tool"": ""Axe"",
                ""DropPossibilities"" : [
                  [
                    {
                      ""Weight"": 1,
                      ""Items"": ""1:1,2:2""
                    },
                    {
                      ""Weight"": 2,
                      ""Items"": ""2:2,3:1""
                    },
                    {
                      ""Weight"": 3,
                      ""Items"": ""2:2""
                    }
                  ]
                ]
              },
              {
                ""Tool"": ""Any"",
                ""DropPossibilities"" : [
                  [
                    {
                      ""Weight"": 1,
                      ""Items"": ""2:1""
                    },
                    {
                      ""Weight"": 1,
                      ""Items"": ""3:1""
                    },
                    {
                      ""Weight"": 1,
                      ""Items"": ""1:1""
                    }
                  ]
                ]
              }
            ]
          ]
        ");
        /*
        Drops = new System.Collections.Generic.Dictionary<Tool.Type, DropChanceCollection[]>[] {
          // mode 0: stump
          new System.Collections.Generic.Dictionary<Tool.Type, DropChanceCollection[]> {{
            Tool.Type.Shovel,
            new DropChanceCollection[] {
              new DropChanceCollection(
                new (int, IInventory)[] {
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
                new (int, IInventory)[] {
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
                new (int, IInventory)[] {
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
        };*/
      }
    }
  }
}
