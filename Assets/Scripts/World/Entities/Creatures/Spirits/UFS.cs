using SpiritWorld.World.Entities.Creatures.Stats;

namespace SpiritWorld.World.Entities.Spirits {
  public partial class Spirit {

    public static partial class Encyclopeidia {
      public static Species UFS = new UFS();
    } 

    /// <summary>
    /// A cute lil UFO boi
    /// </summary>
    public class UFS : Species {
      internal UFS() : base(
        101,
        "UFS",
        new SpeciesBaseStats(
          (20, 5),
          (7, 2),
          (5, 2), // SP
          (1, 1),
          (5, 3),
          (5, 3),
          (4, 2),
          (10, 3), 
          (3, 1), // SKL
          (10, 3),
          (4, 1),
          (1, 1),
          (6, 2),
          (4, 5),
          (7, 5) // EX
        )
      ) { }
    }
  }
}
