using SpiritWorld.World.Terrain.TileGrid;
using System;
using System.Collections.Generic;

namespace SpiritWorld.World {

  /// <summary>
  /// A Level/Land/World in the game, made up of entities, tile boards, and structures.
  /// </summary>
  public class WorldScape {

    /// <summary>
    /// The index of the parent board of all other boards in this scape
    /// </summary>
    public const int MainBoardIndex = 0;

    /// <summary>
    /// The tile boards that make up this worldscape
    /// </summary>
    List<TileBoard> boards 
      = new List<TileBoard>();

    /// <summary>
    /// The main/base board of the level, from which other boards stem
    /// </summary>
    public TileBoard mainBoard {
      get => boards[MainBoardIndex];
    }

    /// <summary>
    /// Get a board from this scape by id
    /// </summary>
    /// <param name="boardId"></param>
    /// <returns></returns>
    public TileBoard getBoard(int boardId) {
      return boards[boardId];
    }

    /// <summary>
    /// Add a board to this worldscape
    /// </summary>
    /// <param name="board"></param>
    /// <param name="index"></param>
    public void addBoard(TileBoard board, int? index = null) {
      if (index == null) {
        boards.Add(board);
      } else {
        boards.Insert((int)index, board);
      }
    }
  }
}
