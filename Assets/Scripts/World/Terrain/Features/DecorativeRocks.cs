using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;

namespace SpiritWorld.World.Terrain.Features {
  public partial struct TileFeature {
    public static partial class Types {
      public static Type DecorativeRocks = new DecorativeRocks();
    }

    /// <summary>
    /// Just some small decorative rocks
    /// </summary>
    public class DecorativeRocks : Type {
      internal DecorativeRocks() : base(
        6,
        "Rocks",
        Layer.Decoration,
        false,
        new DropChanceCollection[] {
          new DropChanceCollection(
            new (int, Inventory)[] {
              (1, new Inventory() {
                {Item.Types.Stone, new Item(Item.Types.Stone, 1)}
              })
            }
          )
        }
      ) {}
    }
  }
}
