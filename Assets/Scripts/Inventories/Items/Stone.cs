namespace SpiritWorld.Inventories.Items {
  public partial class Item {

    public partial class Types {
      public static Stone Stone = new Stone();
    }

    /// <summary>
    /// Rocks
    /// </summary>
    public class Stone : Type {
      internal Stone() : base(5, "Stone") { }
    }
  }
}
