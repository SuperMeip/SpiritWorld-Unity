namespace SpiritWorld.World.Entities {

  /// <summary>
  /// An object in the game world on the terrain
  /// </summary>
  public abstract partial class Entity : IEntity {

    /// <summary>
    /// Used to hand out unique entity ids
    /// </summary>
    static int CurrentMaxEntityId = 0;

    /// <summary>
    /// The unique id of this entity in the current loaded game
    /// </summary>
    public int id {
      get;
    }

    /// <summary>
    /// The type of entity this is
    /// </summary>
    public Type type {
      get;
      protected set;
    }

    /// <summary>
    /// Make an entity and set it's unique id
    /// </summary>
    protected Entity() {
      id = CurrentMaxEntityId++;
    }
  }
}