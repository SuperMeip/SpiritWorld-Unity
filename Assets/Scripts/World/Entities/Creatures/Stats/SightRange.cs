namespace SpiritWorld.World.Entities.Creatures.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type SightRange = new SightRange();
    }

    /// <summary>
    /// Stamina stat. How many actions/round you can do etc
    /// </summary>
    public class SightRange : Type {
      internal SightRange() : base(201, "Sight Range", "SR", VariationGroups.Perception) { }
    }
  }
}
