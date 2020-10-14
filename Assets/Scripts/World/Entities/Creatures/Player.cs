using SpiritWorld.Inventories;
using SpiritWorld.Stats;
using System.Collections.Generic;

namespace SpiritWorld.World.Entities.Creatures {

  /// <summary>
  /// A player entity
  /// </summary>
  public class Player : Creature {

    /// <summary>
    /// Inventory types the player has to store items
    /// </summary>
    public enum InventorySlots {
      Pouch,
      Pockets,
      Backpack
    }

    /// <summary>
    /// Slots on the player where different IEquipable items can be worn
    /// </summary>
    public enum EquiptmentSlots {
      Head,
      Chest,
      Legs,
      Neck,
      Ring, // there are 2 ring slots
    }

    /// <summary>
    /// The ways a weapon can be held
    /// </summary>
    public enum WeaponSlots {
      Hand, // left or right
      BothHands,
      Claws // players can't use this one
    }

    /// <summary>
    /// Hands a player can hold weapons in, wear rings on etc
    /// </summary>
    public enum Hands {
      Left,
      Right
    }

    /// <summary>
    /// player's name
    /// </summary>
    public string name {
      get;
    }

    /// <summary>
    /// How many stat points the player has to spend on combat stats
    /// </summary>
    public readonly Stat availableStatPoints
      = new Stat(Stat.Types.AvailableStatPoints, 21, 6);

    /// <summary>
    /// Points spendable on abilities
    /// </summary>
    public readonly Stat abilityPoints
      = new Stat(Stat.Types.AbilityPoints, 10, 10);

    /// <summary>
    /// The players inventories
    /// </summary>
    public GridPack inventory {
      get;
      protected set;
    }

    /// <summary>
    /// An inventory to hold the hot bar items.
    /// </summary>
    public readonly BasicInventory hotBarItems
      = new BasicInventory();

    /// <summary>
    /// Initialize a new player
    /// </summary>
    /// <param name="name"></param>
    public Player(string name) : base() {
      this.name = name;
    }
  }
}