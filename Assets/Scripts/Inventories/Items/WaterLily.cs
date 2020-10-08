namespace SpiritWorld.Inventories.Items {
  public partial class Item {

    public partial class Types {
      public static WaterLily WaterLily = new WaterLily();
    }

    /// <summary>
    /// A wooden log
    /// </summary>
    public class WaterLily : Type {
      internal WaterLily() : base(4, "Water Lily") { }
    }
  }
}
