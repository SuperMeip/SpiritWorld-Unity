namespace SpiritWorld.Inventories.Items {
  public partial class Item {

    public partial class Types {
      public static PineCone PineCone = new PineCone();
    }

    /// <summary>
    /// A wooden log
    /// </summary>
    public class PineCone : Type {
      internal PineCone() : base(3, "Pine Cone") { }
    }
  }
}
