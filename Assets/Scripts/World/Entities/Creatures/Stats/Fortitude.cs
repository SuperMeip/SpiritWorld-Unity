namespace SpiritWorld.World.Entities.Creatures.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type Fortitude = new Fortitude();
    }

    /// <summary>
    /// Mental Fortitude, mental resistance to magic and effects
    /// </summary>
    public class Fortitude : Type {
      internal Fortitude() : base(14, "Fortitude", "FOR") { }
    }
  }
}
