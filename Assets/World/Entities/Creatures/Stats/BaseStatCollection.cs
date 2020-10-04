using System;
using System.Collections.Generic;

namespace SpiritWorld.World.Entities.Creatures.Stats {

  /// <summary>
  /// USed to hold a set of base stats
  /// </summary>
  public abstract class BaseStatCollection : Dictionary<Stat.Type, BaseStatCollection.VariantStat> {

    /// <summary>
    /// A stat with variant values struct
    /// </summary>
    public struct VariantStat {

      /// <summary>
      /// The type of stat
      /// </summary>
      public Stat.Type type;

      /// <summary>
      /// The current stat value
      /// </summary>
      public int variance {
        get;
        private set;
      }

      /// <summary>
      /// The max value of this stat
      /// </summary>
      public int defaultMax {
        get;
      }

      /// <summary>
      /// Make a stat
      /// </summary>
      /// <param name="type"></param>
      /// <param name="max"></param>
      /// <param name="current"></param>
      /// <param name="min"></param>
      public VariantStat(Stat.Type type, int defaultMax, int variance = 0) {
        this.type = type;
        this.defaultMax = defaultMax;
        this.variance = variance;
      }
    }
  }
}
