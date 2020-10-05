namespace SpiritWorld.World.Entities.Creatures.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type Attunement = new Attunement();
    }

    /// <summary>
    /// Magical power and prowess with spells
    /// </summary>
    public class Attunement : Type {
      internal Attunement() : base(13, "Attunement", "ATN", VariationGroups.Combat) { }
    }
  }
}
