using SpiritWorld.Stats;

namespace SpiritWorld.Inventories.Items {

  /// <summary>
  /// Interface for a generic weapon
  /// </summary>
  internal interface IWeapon {

    /// <summary>
    /// The weapon's stats
    /// </summary>
    WeaponBaseStatCollection WeaponStats {
      get;
    }
  }
}