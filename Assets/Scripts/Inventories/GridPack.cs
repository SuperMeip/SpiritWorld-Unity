using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritWorld.Inventories {

  /// <summary>
  /// A player inventory represented by a square grid
  /// </summary>
  public class GridPack : StackedInventory {

    /// <summary>
    /// The dimensions of this pack
    /// </summary>
    (int x, int y) dimensions {
      get;
    }

    /// <summary>
    /// The grid used to hold item info.
    /// Each X,Y represents a tile in the grid, and each int is and 
    /// index in the inventory stacks list.
    /// </summary>
    int[,] stackSlotGrid;

    public GridPack((int x, int y) dimensions) : base(dimensions.x * dimensions.y) {

    }
  }
}
