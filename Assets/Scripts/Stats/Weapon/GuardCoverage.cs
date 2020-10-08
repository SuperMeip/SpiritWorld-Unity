namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type GuardCoverage = new GuardCoverage();
    }

    /// <summary>
    /// Energy stat. How many spells/mental powers you can use
    /// </summary>
    public class GuardCoverage : Type {
      internal GuardCoverage() : base(66, "Guard Coverage", "GRD", VariationGroups.Weapon) { }
    }
  }
}
