namespace SpiritWorld.World.Terrain.Features {
  public partial struct TileFeature {

    /// <summary>
    /// subtype for limited use features that turn into another feature
    /// </summary>
    public abstract class TransitionalResourceType : Type {

      /// <summary>
      /// The type of feature this one turns into once used up
      /// </summary>
      public Type NextFeatureType {
        get;
      }

      /// <summary>
      /// Make a new interactive feature type
      /// </summary>
      protected TransitionalResourceType(
        byte id,
        string name,
        Layer layer,
        Type nextFeatureType,
        int numberOfUses = 1,
        float useTime = 2.0f
      ) : base(id, name, layer, true, numberOfUses, useTime) {
        NextFeatureType = nextFeatureType;
        // overrite the mode number offset used for the limited resouce type.
        NumberOfModes = numberOfUses;
      }
    }
  }
}
