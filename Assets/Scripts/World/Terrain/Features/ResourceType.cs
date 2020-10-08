using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;

namespace SpiritWorld.World.Terrain.Features {
  public partial struct TileFeature {

    /// <summary>
    /// subtype for interactinve tilefeatures
    /// </summary>
    public abstract class ResourceType : LimitedUseType {

      /// <summary>
      /// Make a new interactive feature type
      /// </summary>
      protected ResourceType(
        byte id,
        string name,
        Layer layer,
        int numberOfUses = UnlimitedInteractions,
        DropChanceCollection[] drops = null,
        float useTime = 2.0f
      ) : base(id, name, layer, numberOfUses, drops, useTime) {}
    }

    public Inventory getDrop(int mode, ITool toolUsed) {
      if (DropsByToolUsed != null) {
      }

      return null;
    } 
  }
}
