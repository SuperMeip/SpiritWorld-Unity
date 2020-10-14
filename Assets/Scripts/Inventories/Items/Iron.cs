namespace SpiritWorld.Inventories.Items {
  public partial class Item {

    public partial class Types {
      public static Iron Iron = new Iron();
    }

    /// <summary>
    /// A spirit apple, food yay!
    /// </summary>
    public class Iron : Type {
      internal Iron() : base(15, "Iron") { }
    }
  }
}
