using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritWorld.World.Terrain.TileGrid {
  public static class HexGridShaper {

    /// <summary>
    /// Do an action on every axial location in a rectangular area
    /// </summary>
    /// <param name="action"></param>
    public static void Rectangle((int width, int depth) size, Action<Coordinate> action) {
      for (int x = 0; x < size.width; x++) {
        int xOffset = x >> 1;
        for (int y = -xOffset; y < size.depth - xOffset; y++) {
          action((x, y));
        }
      }
    }

    /// <summary>
    /// Do an action on every axial location in a diamond shaped area
    /// </summary>
    /// <param name="action">A function to run on each axial coordinate</param>
    public static void Diamond((int width, int depth) size, Action<Coordinate> action) {
      for (int y = 0; y < size.depth; y++) {
        int yOffset = y >> 1;
        for (int x = -yOffset; x < size.width - yOffset; x++) {
          action((x, y));
        }
      }
    }
  }
}
