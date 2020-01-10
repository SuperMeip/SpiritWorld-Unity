/// <summary>
/// Extensions for the empty tile type
/// </summary>
public partial struct Tile {

  /// <summary>
  /// Tile type singleton constants
  /// </summary>
  public static partial class Types {
    public static Type Empty = new Empty();
  }

  /// <summary>
  /// Empty tile type
  /// </summary>
  public class Empty : Type {
    internal Empty() : base(0) { }
  }
}