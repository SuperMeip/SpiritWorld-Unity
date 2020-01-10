/// <summary>
/// An object in the game world on the terrain
/// </summary>
public partial class Entity : IEntity {

  /// <summary>
  /// The type of entity this is
  /// </summary>
  public Type type {
    get;
    protected set;
  }
}