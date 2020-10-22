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
        Drops = ParseDropDataJSON(@"
          [
            [
              {
                ""Tool"": ""Pickaxe"",
                ""DropPossibilities"" : [
                  [
                    {
                      ""Weight"": 5,
                      ""Items"": ""15:1""
                    },
                    {
                      ""Weight"": 1,
                      ""Items"": ""15:2""
                    }
                  ]
                ]
              }
            ]
          ]
        ");
          /*new SortedDictionary<Tool.Requirement, DropChanceCollection[]> {
          /// mode 0
          new SortedDictionary<Tool.Requirement, DropChanceCollection[]>() {{
            new Tool.Requirement(Tool.Type.Pickaxe),
            new DropChanceCollection[] {
              new DropChanceCollection(
                new (int, IInventory)[] {
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
        };*/
      }
    }
  }
}
