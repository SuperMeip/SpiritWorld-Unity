namespace SpiritWorld.World.Entities.Creatures.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type Defence = new Defence();
    }

    /// <summary>
    /// Defence, physical resistance to damange
    /// </summary>
    public class Defence : Type {
      internal Defence() : base(12, "Defence", "DEF", VariationGroups.Combat) { }
    }
  }
}
