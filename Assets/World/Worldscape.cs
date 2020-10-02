using SpiritWorld.World.Terrain.TileGrid;
using System;
using System.Collections.Generic;

namespace SpiritWorld.World {

  /// <summary>
  /// A Level/Land/World in the game, made up of entities, tile boards, and structures.
  /// </summary>
  public class WorldScape {

    /// <summary>
    /// The tile boards that make up this worldscape
    /// </summary>
    List<TileBoard> boards 
      = new List<TileBoard>();

    /// <summary>
    /// The main/base board of the level, from which other boards stem
    /// </summary>
    public TileBoard mainBoard {
      get => boards[0];
    }

    /// <summary>
    /// Make a new worldscape with a tileboard of the given size
    /// </summary>
    public WorldScape() {
      /// add the main board
      boards.Add(new RectangularBoard());
    }

    /// <summary>
    /// Get a board from this scape by id
    /// </summary>
    /// <param name="boardId"></param>
    /// <returns></returns>
    public TileBoard getBoard(int boardId) {
      return boards[boardId];
    }
  }
}
