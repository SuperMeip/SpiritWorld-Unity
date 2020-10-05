namespace SpiritWorld.World.Entities.Creatures.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type VisionAccuracy = new VisionAccuracy();
    }

    /// <summary>
    /// Stamina stat. How many actions/round you can do etc
    /// </summary>
    public class VisionAccuracy : Type {
      internal VisionAccuracy() : base(202, "Vision Accuracy", "VA") { }
    }
  }
}
