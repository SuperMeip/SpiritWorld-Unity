namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      /// <summary>
      /// Critical hit chance
      /// </summary>
      public static Type Luck = new Luck();
    }

    /// <summary>
    /// Critical hit chance for weapons
    /// </summary>
    public class Luck : Type {
      internal Luck() : base(77, "Luck", "LUK", VariationGroups.Weapon) { }
    }
  }
}
