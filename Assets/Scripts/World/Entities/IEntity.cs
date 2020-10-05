
namespace SpiritWorld.World.Entities {

  /// <summary>
  /// An object in the game world on the terrain
  /// </summary>
  interface IEntity {

    /// <summary>
    /// The unique id of this entity;
    /// </summary>
    int id {
      get;
    }

    /// <summary>
    /// The type of entity this is
    /// </summary>
    Entity.Type type {
      get;
    }
  }
}