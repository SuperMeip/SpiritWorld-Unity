namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type BludgeoningDamageModifier = new BludgeoningDamageModifier();
    }

    /// <summary>
    /// Energy stat. How many spells/mental powers you can use
    /// </summary>
    public class BludgeoningDamageModifier : Type {
      internal BludgeoningDamageModifier() : base(64, "Bludgeoning Damage", "PIR", VariationGroups.Weapon) { }
    }
  }
}
