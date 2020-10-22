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
        "Small Rocks",
        Layer.Decoration
      ) {
        Drops = ParseDropDataJSON(@"
          [
            [
              {
                ""Tool"": ""Any"",
                ""DropPossibilities"" : [
                  [
                    {
                      ""Weight"": 1,
                      ""Items"": ""5:1""
                    }
                  ]
                ]
              }
            ]
          ]
        ");
        /*new System.Collections.Generic.Dictionary<Tool.Type, DropChanceCollection[]>[] {
        new System.Collections.Generic.Dictionary<Tool.Type, DropChanceCollection[]> {{
          Tool.Type.Any,
          new DropChanceCollection[] {
            new DropChanceCollection(
              new (int, IInventory)[] {
                (1, new BasicInventory() {
                  {Item.Types.Stone, new Item(Item.Types.Stone, 1)}
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
