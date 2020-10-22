using SpiritWorld.Inventories.Items;

namespace SpiritWorld.Inventories {

  /// <summary>
  /// A player inventory represented by a square grid
  /// </summary>
  public abstract class GridBasedInventory : StackedInventory, IGridInventory {

    /// <summary>
    /// Value for empty grid slots
    /// </summary>
    public const int EmptyGridSlot = -1;

    /// <summary>
    /// The dimensions of this pack
    /// </summary>
    protected (int x, int y) dimensions {
      get;
    }

    /// <summary>
    /// The grid used to hold item info.
    /// Each X,Y represents a tile in the grid, and each int is and 
    /// index in the inventory stacks list.
    /// </summary>
    protected readonly int[][] stackSlotGrid;

    /// <summary>
    /// Make a new grid pack of the given capacity
    /// </summary>
    /// <param name="dimensions"></param>
    public GridBasedInventory((int x, int y) dimensions) : base(dimensions.x * dimensions.y) {
      this.dimensions = dimensions;
      stackSlotGrid = new int[dimensions.x][];
      for (int x = 0; x < dimensions.x; x++) {
        stackSlotGrid[x] = new int[dimensions.y];
        stackSlotGrid[x].Populate(EmptyGridSlot);
      }
    }

    /// <summary>
    /// Make a new empty grid with one dimension (a bar or column)
    /// </summary>
    /// <param name="dimensions"></param>
    public GridBasedInventory(int x) : base(x) {
      dimensions = (x, 1);
      stackSlotGrid = new int[dimensions.x][];
      for (int i = 0; i < x; i++) {
        stackSlotGrid[i] = new int[] { EmptyGridSlot };
      }
    }

    public abstract Item tryToAdd(Item item, Coordinate location, out Item successfullyAddedItem);

    public abstract Item[] removeAt(Coordinate itemLocation);

    /// <summary>
    /// Try to swap an item at a given location with another
    /// </summary>
    /// <param name="location"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual bool tryToSwapOut(Coordinate location, Item newItem, out Item oldItem) {
      // gather old item info and clear it out
      oldItem = getItemStackAt(location);
      int oldStackId = stackSlotGrid[location.x][location.z];
      if (oldStackId != EmptyGridSlot) {
        clearStack(oldStackId);
      }

      // add the new item stack
      int newItemStackId = addNewStack(newItem);

      // set the new slot, yay!
      stackSlotGrid[location.x][location.z] = newItemStackId;
      return true;
    }

    /// <summary>
    /// If the grid location is within this item grid
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    protected bool isWithinGrid(Coordinate gridLocation) {
      return gridLocation.isWithin((0, 0), dimensions);
    }
    /// <summary>
    /// Get the item stack stored at the given grid spot
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    protected Item getItemStackAt(Coordinate location) {
      int stackId = stackSlotGrid[location.x][location.z];
      return stackId != EmptyGridSlot 
        ? stacks[stackId] 
        : null;
    }
  }
}
