namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type HearingRadius = new HearingRadius();
    }

    /// <summary>
    /// Stamina stat. How many actions/round you can do etc
    /// </summary>
    public class HearingRadius : Type {
      internal HearingRadius() : base(205, "Hearing Radius", "HR", VariationGroups.Perception) { }
    }
  }
}
