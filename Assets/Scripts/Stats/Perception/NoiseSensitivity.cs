namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type NoiseSensitivity = new NoiseSensitivity();
    }

    /// <summary>
    /// Stamina stat. How many actions/round you can do etc
    /// </summary>
    public class NoiseSensitivity : Type {
      internal NoiseSensitivity() : base(204, "Noise Sensitivity", "NS", VariationGroups.Perception) { }
    }
  }
}
