namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type Attunement = new TypeAttunement();
    }

    /// <summary>
    /// Magical power and prowess with spells
    /// </summary>
    class TypeAttunement : Type {
      internal TypeAttunement() : base(13, "Attunement", "ATN", VariationGroups.Combat) { }
    }
  }
}
