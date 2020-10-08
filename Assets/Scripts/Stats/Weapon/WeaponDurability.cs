namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      /// <summary>
      /// On average, how many battles will this weapon last before needing repair?
      /// </summary>
      public static Type WeaponDurability = new WeaponDurability();
    }

    /// <summary>
    /// On average, how many battles will this weapon last?
    /// </summary>
    public class WeaponDurability : Type {
      internal WeaponDurability() : base(60, "Durability", "DUR", VariationGroups.Weapon) { }
    }
  }
}
