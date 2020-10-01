using SpiritWorld.World.Terrain.TileGrid;
using System.Collections.Generic;

namespace SpiritWorld.World {

  /// <summary>
  /// A Level/Land/World in the game, made up of entities, tile boards, and structures.
  /// </summary>
  public class Worldscape {

    /// <summary>
    /// The tile boards that make up this worldscape
    /// </summary>
    List<TileBoard> tileboards 
      = new List<TileBoard>();

    /// <summary>
    /// The main/base board of the level, from which other boards stem
    /// </summary>
    public TileBoard mainboard {
      get => tileboards[0];
    }

    /// <summary>
    /// Make a new worldscape with a tileboard of the given size
    /// </summary>
    /// <param name="mainboardSize"></param>
    public Worldscape((int width, int depth) mainboardSize) {
      tileboards[0] = new TileBoard();
    }
  }
}
