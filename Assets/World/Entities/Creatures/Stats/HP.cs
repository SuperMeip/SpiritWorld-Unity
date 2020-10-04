namespace SpiritWorld.World.Entities.Creatures.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type HP = new HP();
    }

    /// <summary>
    /// Health stat
    /// </summary>
    public class HP : Type {
      internal HP() : base(1, "Health", "HP") { }
    }
  }
}
