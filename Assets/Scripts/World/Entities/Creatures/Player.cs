using SpiritWorld.Events;
using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using SpiritWorld.Stats;

namespace SpiritWorld.World.Entities.Creatures {

  /// <summary>
  /// A player entity
  /// </summary>
  public class Player : Creature {

    /// <summary>
    /// A player's empty hand as a tool
    /// </summary>
    public static ITool EmptyHand = new Hand();

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
    /// The players back pack inventory
    /// </summary>
    public ShapedPack packInventory {
      get;
      protected set;
    } = new ShapedPack((4, 7));

    /// <summary>
    /// The players inventories
    /// </summary>
    public IInventory[] inventories {
      get;
    }

    /// <summary>
    /// An inventory to hold the hot bar items.
    /// </summary>
    public readonly ItemBar hotBarInventory
      = new ItemBar(10, 1);

    /// <summary>
    /// Initialize a new player
    /// </summary>
    /// <param name="name"></param>
    public Player(string name) : base() {
      this.name = name;
      inventories = new IInventory[] {
        hotBarInventory,
        packInventory
      };
    }
  }

  /// <summary>
  /// Type for bare hand tool
  /// </summary>
  public struct Hand : ITool {
    public Tool.Type ToolType 
      => Tool.Type.None;

    public int UpgradeLevel 
      => 0;

    public string UpgradeLevelName
      => "Bare Hands";

    public short NumberOfUses 
      => UseableItem.Type.UnlimitedUses;
  }
}