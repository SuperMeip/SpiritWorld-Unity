namespace SpiritWorld.World.Entities.Creatures.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type Energy = new Energy();
    }

    /// <summary>
    /// Energy stat. How many spells/mental powers you can use
    /// </summary>
    public class Energy : Type {
      internal Energy() : base(3, "Energy", "EP", VariationGroups.Depleteable) { }
    }
  }
}
