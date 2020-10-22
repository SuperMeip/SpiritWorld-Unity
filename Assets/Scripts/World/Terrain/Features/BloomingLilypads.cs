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
    public class BloomingLilypads : Type {
      internal BloomingLilypads() : base(
        4,
        "Blooming Lilypads",
        Layer.Resource,
        true,
        1
      ) {
        Drops = ParseDropDataJSON(@"
          [
            [
              {
                ""Tool"": ""Any"",
                ""DropPossibilities"" : []
              }
            ],
            [
              {
                ""Tool"": ""Any"",
                ""DropPossibilities"" : [
                  [
                    {
                      ""Weight"": 3,
                      ""Items"": ""4:1""
                    },
                    {
                      ""Weight"": 1,
                      ""Items"": ""4:2""
                    }
                  ]
                ]
              }
            ]
          ]
        ");
          /*new Dictionary<Tool.Type, DropChanceCollection[]>[] {
          // mode 0: empty
          new Dictionary<Tool.Type, DropChanceCollection[]> {{
            Tool.Type.Any,
            new DropChanceCollection[] {
              new DropChanceCollection(
                new (int, IInventory)[0]
              )
            }
          }},
          // mode 1: has flowers
          new Dictionary<Tool.Type, DropChanceCollection[]> {{
            Tool.Type.Any,
            new DropChanceCollection[] {
              new DropChanceCollection(
                new (int, IInventory)[] {
                  (2, new BasicInventory{
                    {Item.Types.WaterLily, new Item(Item.Types.WaterLily, 1)},
                  }),
                  (1, new BasicInventory{
                    {Item.Types.WaterLily, new Item(Item.Types.WaterLily, 2)},
                  })
                }
              )
            }
          }}
        };*/
      }
    }
  }
}
