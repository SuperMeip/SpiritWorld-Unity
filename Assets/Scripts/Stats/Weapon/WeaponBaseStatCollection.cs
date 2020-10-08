using System.Collections.Generic;

namespace SpiritWorld.Stats {

  /// <summary>
  /// A base stat collection and management class for different creatures
  /// </summary>
  public class WeaponBaseStatCollection : BaseStatCollection<WeaponStats> {

    /// <summary>
    /// Make a block of weapon stats
    /// </summary>
    public WeaponBaseStatCollection(
      (int value, int variance) DUR,
      (int value, int variance) RNG,
      (int value, int variance) SLA,
      (int value, int variance) PIR,
      (int value, int variance) BLG,
      (int value, int variance) MAG,
      (int value, int variance) GRD,
      (int value, int variance) LUK
    ) {
      /// depletable stats
      add(Stat.Types.WeaponDurability, DUR);
      add(Stat.Types.WeaponRange, RNG);
      add(Stat.Types.SlashingDamageModifier, SLA);
      add(Stat.Types.PiercingDamageModifier, PIR);
      add(Stat.Types.BludgeoningDamageModifier, BLG);
      add(Stat.Types.AttunementMagnifier, MAG);
      add(Stat.Types.GuardCoverage, GRD);
      add(Stat.Types.Luck, LUK);
    }

    /// <summary>
    /// Add with no variance
    /// </summary>
    public WeaponBaseStatCollection(
      int DUR,
      int RNG,
      int SLA,
      int PIR,
      int BLG,
      int MAG,
      int GRD,
      int LUK
    ) {
      /// depletable stats
      add(Stat.Types.WeaponDurability, DUR);
      add(Stat.Types.WeaponRange, RNG);
      add(Stat.Types.SlashingDamageModifier, SLA);
      add(Stat.Types.PiercingDamageModifier, PIR);
      add(Stat.Types.BludgeoningDamageModifier, BLG);
      add(Stat.Types.AttunementMagnifier, MAG);
      add(Stat.Types.GuardCoverage, GRD);
      add(Stat.Types.Luck, LUK);
    }

    /// <summary>
    /// Return formatted weapon stats
    /// </summary>
    /// <returns></returns>
    protected override WeaponStats formatStatValuesForOutput(Dictionary<Stat.Type, int> statValues) {
      return new WeaponStats(
        statValues[Stat.Types.WeaponDurability],
        statValues[Stat.Types.WeaponRange],
        statValues[Stat.Types.SlashingDamageModifier],
        statValues[Stat.Types.PiercingDamageModifier],
        statValues[Stat.Types.BludgeoningDamageModifier],
        statValues[Stat.Types.AttunementMagnifier],
        statValues[Stat.Types.GuardCoverage],
        statValues[Stat.Types.Luck]
      );
    }
  }
}
