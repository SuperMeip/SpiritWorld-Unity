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
    public class ConniferTrio : LimitedUseType {
      internal ConniferTrio() : base(
        1,
        "Firr Trees",
        Layer.Resource,
        2,
        new DropChanceCollection[] {
          // mode 0: stump
          new DropChanceCollection(
            new (int, Inventory)[] {
              (2, new Inventory{
                {Item.Types.PineCone, new Item(Item.Types.PineCone, 1)},
                {Item.Types.Log, new Item(Item.Types.Log, 2)}
              }),
              (3, new Inventory{
                {Item.Types.Log, new Item(Item.Types.Log, 2)}
              })
            }),
          // mode 1: Big tree
          new DropChanceCollection(
            new (int, Inventory)[] {
              (3, new Inventory{
                {Item.Types.Spapple, new Item(Item.Types.Spapple, 1)},
                {Item.Types.PineCone, new Item(Item.Types.PineCone, 1)},
                {Item.Types.Log, new Item(Item.Types.Log, 2)}
              }),
              (1, new Inventory{
                {Item.Types.PineCone, new Item(Item.Types.PineCone, 1)},
                {Item.Types.Log, new Item(Item.Types.Log, 3)}
              }),
              (5, new Inventory{
                {Item.Types.Log, new Item(Item.Types.Log, 2)}
              })
            }),
          // mode 2: Small and dead tree
          new DropChanceCollection(
            new (int, Inventory)[] {
              (1, new Inventory{
                {Item.Types.Spapple, new Item(Item.Types.Spapple, 1)},
                {Item.Types.Log, new Item(Item.Types.Log, 2)}
              }),
              (2, new Inventory{
                {Item.Types.PineCone, new Item(Item.Types.PineCone, 1)},
                {Item.Types.Log, new Item(Item.Types.Log, 2)}
              }),
              (3, new Inventory{
                {Item.Types.Log, new Item(Item.Types.Log, 2)}
              })
            }),
          }
      ) { }
    }
  }
}
