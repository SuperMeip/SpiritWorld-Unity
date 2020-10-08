namespace SpiritWorld.Stats {
  public partial struct Stat {
    public static partial class Types {
      public static Type AvailableStatPoints = new AvailableStatPoints();
    }

    /// <summary>
    /// Defence, physical resistance to damange
    /// </summary>
    public class AvailableStatPoints : Type {
      internal AvailableStatPoints() : base(40, "Stat Points Available To Spend", "STAT", VariationGroups.Spendable) { }
    }
  }
}
