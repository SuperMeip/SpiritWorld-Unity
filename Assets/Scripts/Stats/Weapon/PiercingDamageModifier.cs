namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type PiercingDamageModifier = new PiercingDamageModifier();
    }

    /// <summary>
    /// Energy stat. How many spells/mental powers you can use
    /// </summary>
    public class PiercingDamageModifier : Type {
      internal PiercingDamageModifier() : base(63, "Piercing Damage", "PIR", VariationGroups.Weapon) { }
    }
  }
}
