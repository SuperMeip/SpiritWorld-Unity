namespace SpiritWorld.Inventories.Items {
  public partial class Item {

    public partial class Types {
      public static Spapple Spapple = new Spapple();
    }

    /// <summary>
    /// A spirit apple, food yay!
    /// </summary>
    public class Spapple : Type {
      internal Spapple() : base(1, "Spapple") { }
    }
  }
}
