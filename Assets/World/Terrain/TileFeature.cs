namespace SpiritWorld.World.Terrain.Features {

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
    /// The mode this feature is in
    /// Used to change the model/other details about it.
    /// </summary>
    public int mode {
      get;
      internal set;
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
      get => type.IsInteractive && type.HasLimitedUses && remainingInteractions == 0;
    }

    /// <summary>
    /// Make a new tile feature of the given type
    /// </summary>
    /// <param name="featureType"></param>
    public TileFeature(Type featureType, int? mode = null) {
      type = featureType;

      /// set the current mode:
      // use the provided, if one isnt provided
      // for features that can be used up, we start at the highest mode.
      // otherwise we start at mode 0, the base mode
      this.mode = mode == null
        ? type.HasLimitedUses
          ? type.NumberOfModes - 1
          : 0
        : (int)mode;

      // if this is interactive and use-upable, record our remaining use count.
      remainingInteractions = type.IsInteractive && type.HasLimitedUses 
        ? type.NumberOfUses
        : UnlimitedInteractions;
    }

    /// <summary>
    /// Use up X uses of this resource
    /// </summary>
    /// <param name="count"></param>
    public void useUp(int count = 1) {
      if (type.HasLimitedUses && remainingInteractions > 0) {
        // decrement the remaining uses.
        remainingInteractions -= count;

        // get the mode to switch to based on the % of uses remaining for this feature
        float percentRemaining = remainingInteractions / type.NumberOfUses;
        mode = (int)(type.NumberOfModes * percentRemaining);
      }
    }
  }
}

