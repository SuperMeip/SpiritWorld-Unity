using SpiritWorld.World.Terrain.TileGrid;

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
    /// The type of rotation to use when positioning this feature in world
    /// </summary>
    public enum RotationType {
      Static, // set by model
      Random//, // random.range, 1->360, around the Y
      //Set // based on the facingDirection value of the TileFeature itself 
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
      get => (type is LimitedUseType) && remainingInteractions == 0;
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
        ? type is LimitedUseType
          ? type.NumberOfModes - 1
          : 0
        : (int)mode;

      // if this is interactive and use-upable, record our remaining use count.
      remainingInteractions = type is LimitedUseType 
        ? (type as LimitedUseType).NumberOfUses
        : Type.UnlimitedInteractions;
    }

    /// <summary>
    /// Use this tile feature
    /// </summary>
    /// <param name="totalTimeUsedForSoFar"></param>
    /// <param name="deltaTimeUsedFor"></param>
    public void interact(float totalTimeUsedForSoFar = 0) {
      if (remainingInteractions != 0) {
        if (type is LimitedUseType limitedUseType && limitedUseType.tryToUseOnce(totalTimeUsedForSoFar)) {
          remainingInteractions--;
          updateModeBasedOnRemainingInteractions();
        }
      }
    }

    /// <summary>
    /// get the mode to switch to based on the % of uses remaining for this feature
    /// This assumes mode 0 is empty.
    /// </summary>
    /// <param name="count"></param>
    void updateModeBasedOnRemainingInteractions() {
      float percentRemaining = (float)remainingInteractions / (type as LimitedUseType).NumberOfUses;
      mode = (int)((type.NumberOfModes - 1) * percentRemaining);
    }
  }
}

