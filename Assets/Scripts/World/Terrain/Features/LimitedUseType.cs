using SpiritWorld.Inventories;

namespace SpiritWorld.World.Terrain.Features {
  public partial struct TileFeature {

    /// <summary>
    /// subtype for interactinve tilefeatures
    /// </summary>
    public abstract class LimitedUseType : Type {

      /// <summary>
      /// The time it takes to use up one unit of this feature
      /// </summary>
      public float TimeToUse {
        get;
      } = 2.0f;

      /// <summary>
      /// How many times this resource can be interacted with before it's used up if HasLimitedUses is true
      /// </summary>
      public int NumberOfUses {
        get;
      } = UnlimitedInteractions;

      /// <summary>
      /// Make a new interactive feature type
      /// </summary>
      /// <param name="id"></param>
      /// <param name="layer"></param>
      /// <param name="hasLimitedUses"></param>
      /// <param name="useTime"></param>
      /// <param name="numberOfUses"></param>
      protected LimitedUseType(
        byte id,
        string name,
        Layer layer,
        int numberOfUses = UnlimitedInteractions,
        DropChanceCollection[] drops = null,
        float useTime = 2.0f
      ) : base(id, name, layer, true, drops) {
        NumberOfModes = numberOfUses + 1;
        TimeToUse = useTime;
        NumberOfUses = numberOfUses;
      }

      /// <summary>
      /// Try to 'use'/interact with this tile
      /// </summary>
      /// <returns></returns>
      public bool TryToUseOnce(float totalTimeInteractedWithForSoFar) {
        return totalTimeInteractedWithForSoFar >= TimeToUse;
      }
    }
  }
}
