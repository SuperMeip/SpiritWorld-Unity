namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type Intellegence = new Intellegence();
    }

    /// <summary>
    /// Mental Fortitude, mental resistance to magic and effects
    /// </summary>
    class Intellegence : Type {
      internal Intellegence() : base(15, "Intellegence", "INT", VariationGroups.Combat) { }
    }
  }
}
