using System;

namespace SpiritWorld.Inventories.Items {
  public partial class Item {

    public partial class Types {
      public static Wood Wood = new Wood();
    }

    /// <summary>
    /// A wooden log
    /// </summary>
    public class Wood : Type {
      internal Wood() : base(2, "Wood") {
        // slightly long
        Shape = new ShapeBlocks[,] {
          {ShapeBlocks.Pivot},
          {ShapeBlocks.Solid}
        };
      }
    }
  }
}
