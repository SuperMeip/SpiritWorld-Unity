using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritWorld.World.Terrain.TileGrid {

  /// <summary>
  /// A grid overlayed on a hex grid for combat. Locks players into positions and makes it turn based.
  /// </summary>
  public class CombatGrid {

    /// <summary>
    /// The hex grid the combat grid is projected onto
    /// </summary>
    HexGrid parentGrid;

    /// <summary>
    /// The location of the 0,0 of this grid on it's parent grid
    /// </summary>
    Coordinate anchorPoint;

    /// <summary>
    /// The radius of the combat area in tiles
    /// </summary>
    int radius;
  }
}
