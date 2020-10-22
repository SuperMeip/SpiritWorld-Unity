namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type ExtraSensoryPerception = new ExtraSensoryPerception();
    }

    /// <summary>
    /// Stamina stat. How many actions/round you can do etc
    /// </summary>
    public class ExtraSensoryPerception : Type {
      internal ExtraSensoryPerception() : base(206, "Exra Sensory", "EX", VariationGroups.Perception) { }
    }
  }
}
