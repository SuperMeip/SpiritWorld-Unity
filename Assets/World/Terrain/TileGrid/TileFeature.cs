namespace SpiritWorld.World.Terrain.TileGrid.Features {

  /// <summary>
  /// Something on a tile
  /// </summary>
  public partial struct TileFeature {
    /// <summary>
    /// There usually can only be one feature of each layer type on each tile at most.
    /// </summary>
    public enum Layer {
      Decoration,
      Resource,
      Item,
      Sky
    }

    /// <summary>
    /// The type data for this feature
    /// </summary>
    public Type type {
      get;
    }

    /// <summary>
    /// How many more times this resource can be interacted with
    /// </summary>
    public int remainingInteractions {
      get;
      internal set;
    }

    /// <summary>
    /// If this feature's interactions have been used up
    /// </summary>
    public bool isUsedUp {
      get => type.IsInteractive && remainingInteractions == 0;
    }

    /// <summary>
    /// Make a new tile feature of the given type
    /// </summary>
    /// <param name="featureType"></param>
    public TileFeature(Type featureType) {
      type = featureType;
      remainingInteractions = type.IsInteractive ? type.InteractionCount : 0;
      type.initializeFeature(this);
    }
  }
}
