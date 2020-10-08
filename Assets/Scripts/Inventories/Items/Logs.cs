namespace SpiritWorld.Inventories.Items {
  public partial class Item {

    public partial class Types {
      public static Log Log = new Log();
    }

    /// <summary>
    /// A wooden log
    /// </summary>
    public class Log : Type {
      internal Log() : base(2, "Log") { }
    }
  }
}
