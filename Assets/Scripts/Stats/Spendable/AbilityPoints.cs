namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type AbilityPoints = new AbilityPoints();
    }

    /// <summary>
    /// Defence, physical resistance to damange
    /// </summary>
    public class AbilityPoints : Type {
      internal AbilityPoints() : base(40, "Ability Points", "ABIL", VariationGroups.Spendable) { }
    }
  }
}
