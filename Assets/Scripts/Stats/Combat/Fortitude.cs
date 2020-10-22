namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type Fortitude = new TypeFortitude();
    }

    /// <summary>
    /// Mental Fortitude, mental resistance to magic and effects
    /// </summary>
    class TypeFortitude : Type {
      internal TypeFortitude() : base(14, "Fortitude", "FOR", VariationGroups.Combat) { }
    }
  }
}
