namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type Stamina = new Stamina();
    }

    /// <summary>
    /// Stamina stat. How many actions/round you can do etc
    /// </summary>
    public class Stamina : Type {
      internal Stamina() : base(2, "Stamina", "SP", VariationGroups.Depleteable) { }
    }
  }
}
