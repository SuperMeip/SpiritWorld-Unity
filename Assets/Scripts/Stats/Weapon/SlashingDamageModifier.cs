namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type SlashingDamageModifier = new SlashingDamageModifier();
    }

    /// <summary>
    /// Energy stat. How many spells/mental powers you can use
    /// </summary>
    public class SlashingDamageModifier : Type {
      internal SlashingDamageModifier() : base(62, "Slashing Damage", "SLA", VariationGroups.Weapon) { }
    }
  }
}
