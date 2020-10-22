namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type Defence = new Defence();
    }

    /// <summary>
    /// Defence, physical resistance to damange
    /// </summary>
    class Defence : Type {
      public Defence() : base(12, "Defence", "DEF", VariationGroups.Combat) { }
    }
  }
}
