public partial class Spirit {

  /// <summary>
  /// Add this monster to the proper entity types
  /// </summary>
  public partial class Encyclopeidia {
    public static Species Orbo = new Orbo();
  }

  /// <summary>
  /// A basic orb shaped monster
  /// </summary>
  public class Orbo : Species {
    internal Orbo() : base(2, "Orbo") { }
  }
}