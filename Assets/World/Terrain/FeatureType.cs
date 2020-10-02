using System.Collections.Generic;

namespace SpiritWorld.World.Terrain.Features {
 
  public partial struct TileFeature {

    /// <summary>
    /// Constant for unlimited interacounts count
    /// </summary>
    public const int UnlimitedInteractions = -1;

    /// <summary>
    /// Singleton class for tile feature type data
    /// </summary>
    public abstract class Type {

      /// <summary>
      /// The id of this feature type
      /// </summary>
      public byte Id {
        get;
      }

      /// <summary>
      /// The layer that this feature type sits on
      /// </summary>
      public Layer Layer {
        get;
      }

      /// <summary>
      /// If this feature can be interacted with/harvested/mined/picked up
      /// </summary>
      public bool IsInteractive {
        get;
        protected set;
      } = false;

      /// <summary>
      /// If this resource can be mined/used up with interactions
      /// </summary>
      public bool HasLimitedUses {
        get;
        protected set;
      } = false;

      /// <summary>
      /// How many modes/models this reouces has.
      /// </summary>
      public int NumberOfModes {
        get;
        protected set;
      } = 1;

      /// <summary>
      /// How many times this resource can be interacted with before it's used up if HasLimitedUses is true
      /// </summary>
      public int NumberOfUses {
        get;
        protected set;
      } = UnlimitedInteractions;

      /// <summary>
      /// For making new types
      /// </summary>
      /// <param name="id"></param>
      protected Type(byte id, Layer layer) {
        Id = id;
        Layer = layer;

        // on creation, add the singleton to the all types list.
        Types.Add(this);
      }

      /// <summary>
      /// Apply special creation rules to some features
      /// </summary>
      /// <param name="feature"></param>
      internal virtual void initializeFeature(TileFeature feature) { }
    }

    /// <summary>
    /// Tile type singleton constants
    /// </summary>
    public static partial class Types {

      /// <summary>
      /// All registered block types as an ordered array
      /// </summary>
      public static Type[] All {
        get {
          Type[] types = new Type[all.Count];
          all.Values.CopyTo(types, 0);
          return types;
        }
      }

      /// <summary>
      /// The dictionary of type values
      /// </summary>
      static SortedDictionary<byte, Type> all = new SortedDictionary<byte, Type>();

      /// <summary>
      /// Get a block by it's type id
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public static Type Get(byte id) {
        return all[id];
      }

      /// <summary>
      /// Add a type to the list of all types
      /// </summary>
      /// <param name="type"></param>
      internal static void Add(Type type) {
        if (all.ContainsKey(type.Id)) {
          throw new System.Exception("Attempted to register a new Feature type with an existing type's Id");
        } else {
          all.Add(type.Id, type);
        }
      }
    }
  }
}