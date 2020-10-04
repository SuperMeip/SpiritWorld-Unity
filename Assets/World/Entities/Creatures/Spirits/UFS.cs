using SpiritWorld.World.Entities.Creatures.Stats;

namespace SpiritWorld.Entities.Spirits {
  public partial class Spirit {

    public static partial class Encyclopeidia {
      public static Species UFS = new UFS();
    } 

    /// <summary>
    /// A cute lil UFO boi
    /// </summary>
    public class UFS : Species {
      internal UFS() : base(101, "UFS", new SpeciesBaseStats(
        (20, 5),
        (7, 2),
        (5, 2),
        (1, 1),
        (5, 3),
        (5, 3),
        (4, 2)
      )) { }
    }
  }
}
