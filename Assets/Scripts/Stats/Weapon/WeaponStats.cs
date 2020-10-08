namespace SpiritWorld.Stats {

  /// <summary>
  /// A weapon's stats
  /// </summary>
  public class WeaponStats : StatSheet {

    /// <summary>
    /// Make a block of weapon stats
    /// </summary>
    public WeaponStats(
      int DUR, // durability
      int RNG, // The range of the weapon in tiles
      int SLA, // slashing damage mod
      int PIR, // pericing damage mod
      int BLG, // bludgeoning damage mod
      int MAG, // attunment magnifier (stalves, wands, magic weapons)
      int GRD, // how good this is at blocking
      int LUK  // critical hit chance multiplier
    ) {
      this[Stat.Types.WeaponDurability] = new Stat(Stat.Types.WeaponDurability, DUR);
      this[Stat.Types.WeaponRange] = new Stat(Stat.Types.WeaponRange, RNG);
      this[Stat.Types.SlashingDamageModifier] = new Stat(Stat.Types.SlashingDamageModifier, SLA);
      this[Stat.Types.PiercingDamageModifier] = new Stat(Stat.Types.PiercingDamageModifier, PIR);
      this[Stat.Types.BludgeoningDamageModifier] = new Stat(Stat.Types.BludgeoningDamageModifier, BLG);
      this[Stat.Types.AttunementMagnifier] = new Stat(Stat.Types.AttunementMagnifier, MAG);
      this[Stat.Types.GuardCoverage] = new Stat(Stat.Types.GuardCoverage, GRD);
      this[Stat.Types.Luck] = new Stat(Stat.Types.Luck, LUK);
    }
  }
}
