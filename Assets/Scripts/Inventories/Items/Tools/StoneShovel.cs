namespace SpiritWorld.Inventories.Items {
  public partial class UseableItem {

    public static partial class Types {
      public static StoneShovel WoodenShovel = new StoneShovel();
    }

    /// <summary>
    /// A shovel made of wood
    /// </summary>
    public class StoneShovel : Type, ITool {

      /// <summary>
      /// The type of tool
      /// </summary>
      public Tool.Type ToolType
        => Tool.Type.Shovel;

      /// <summary>
      /// The level of this tool
      /// </summary>
      public int UpgradeLevel 
        => 0;

      /// <summary>
      /// Shovel is made of wood
      /// </summary>
      public string UpgradeLevelName 
        => "Wooden";

      /// <summary>
      /// the wooden shovel, can break some small rocks and clear stumps
      /// </summary>
      public StoneShovel() : base(1011, "Wooden Shovel", 25, 1) {}
    }

  }
}
