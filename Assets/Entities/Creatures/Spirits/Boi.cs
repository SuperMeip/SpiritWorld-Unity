public partial class Spirit {

  /// <summary>
  /// Add this monster to the proper entity types
  /// </summary>
  public partial class Encyclopedia {
    public static Species Boi = new Boi();
  }

  /// <summary>
  /// A small wild boi of the forest
  /// </summary>
  public class Boi : Species {
    internal Boi() : base(1, "Boi") { }
  }
}