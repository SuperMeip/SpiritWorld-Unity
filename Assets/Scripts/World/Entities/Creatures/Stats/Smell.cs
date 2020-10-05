namespace SpiritWorld.World.Entities.Creatures.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type Smell = new Smell();
    }

    /// <summary>
    /// Stamina stat. How many actions/round you can do etc
    /// </summary>
    public class Smell : Type {
      internal Smell() : base(203, "Smell Power", "SM", VariationGroups.Perception) { }
    }
  }
}
