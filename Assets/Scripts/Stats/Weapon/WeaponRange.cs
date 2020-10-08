namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type WeaponRange = new WeaponRange();
    }

    /// <summary>
    /// Energy stat. How many spells/mental powers you can use
    /// </summary>
    public class WeaponRange : Type {
      internal WeaponRange() : base(61, "Range", "RNG", VariationGroups.Weapon) { }
    }
  }
}
