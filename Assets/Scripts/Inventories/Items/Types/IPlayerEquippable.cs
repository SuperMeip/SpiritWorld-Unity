using SpiritWorld.World.Entities.Creatures;

namespace SpiritWorld.Inventories.Items {

  /// <summary>
  /// Used for item types that are equipable to players
  /// </summary>
  public interface IPlayerEquippable {

    /// <summary>
    /// This type must declare an equip slot
    /// </summary>
    Player.EquiptmentSlots EquipSlot {
      get;
    }
  }
}
