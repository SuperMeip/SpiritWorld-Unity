namespace SpiritWorld.World.Entities.Creatures.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type Skill = new Skill();
    }

    /// <summary>
    /// Mental Fortitude, mental resistance to magic and effects
    /// </summary>
    public class Skill : Type {
      internal Skill() : base(16, "Skill", "SKL") { }
    }
  }
}
