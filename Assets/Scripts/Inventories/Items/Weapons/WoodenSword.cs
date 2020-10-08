using SpiritWorld.Stats;

namespace SpiritWorld.Inventories.Items {
  public partial class Weapon {

    public static partial class Types {

      /// <summary>
      /// A sword made of wood, also sort of a starter axe/tool
      /// </summary>
      public static WoodenSword WoodenSword = new WoodenSword();
    }

    /// <summary>
    /// A Sword made of wood, can also be used as an axe
    /// </summary>
    public class WoodenSword : Type, ITool {

      /// <summary>
      /// Tool data for this to be used as a starter axe as well
      /// </summary>
      #region ITool
        public Tool.Type ToolType 
          => Tool.Type.Axe;
        public int UpgradeLevel 
          => 0;
        public string UpgradeLevelName 
          => "Wooden(Axe)";
      #endregion

      /// <summary>
      /// The stat block for this weapon type
      /// </summary>
      public override WeaponBaseStatCollection WeaponStats
        => new WeaponBaseStatCollection(10, 1, 1, 1, 1, 0, 20, 10);

      internal WoodenSword() : base(2011, "Wooden Sword", 1) {}
    }
  }
}
