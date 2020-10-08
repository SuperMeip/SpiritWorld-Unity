namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type AttunementMagnifier = new AttunementMagnifier();
    }

    /// <summary>
    /// Energy stat. How many spells/mental powers you can use
    /// </summary>
    public class AttunementMagnifier : Type {
      internal AttunementMagnifier() : base(65, "Attunement Magnifier", "MAG", VariationGroups.Weapon) { }
    }
  }
}
