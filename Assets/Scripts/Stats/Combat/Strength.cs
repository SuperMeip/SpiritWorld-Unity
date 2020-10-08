namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type Strength = new Strength();
    }

    /// <summary>
    /// Physical strenth and attack damage
    /// </summary>
    public class Strength : Type {
      internal Strength() : base(11, "Strength", "STR", VariationGroups.Combat) { }
    }
  }
}
