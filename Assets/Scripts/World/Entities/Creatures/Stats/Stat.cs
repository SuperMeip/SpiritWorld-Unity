using System;

namespace SpiritWorld.World.Entities.Creatures.Stats {

  /// <summary>
  /// A stat value struct
  /// </summary>
  public partial struct Stat {

    /// <summary>
    /// Groupings for different stats according to how variation points are dolled out
    /// </summary>
    public enum VariationGroups {
      Depleteable,
      Combat,
      Perception
    }

    /// <summary>
    /// The type of stat
    /// </summary>
    public Type type;

    /// <summary>
    /// The current stat value
    /// </summary>
    public int current {
      get;
      private set;
    }

    /// <summary>
    /// The max value of this stat
    /// </summary>
    public int max {
      get;
    }

    /// <summary>
    /// the min value of this stat
    /// </summary>
    public int min {
      get;
    }

    /// <summary>
    /// Make a stat
    /// </summary>
    /// <param name="type"></param>
    /// <param name="max"></param>
    /// <param name="current"></param>
    /// <param name="min"></param>
    public Stat(Type type, int max, int current = 0, int min = 0) {
      this.type = type;
      this.max = max;
      this.min = min;
      this.current = Math.Min(current, max);
    }

    /// <summary>
    /// Update the current stat.
    /// </summary>
    /// <param name="oldStat"></param>
    /// <param name="newMax"></param>
    public Stat(Stat oldStat, int newMax) {
      type = oldStat.type;
      max = newMax;
      min = oldStat.min;
      current = newMax;
    }
  }
}
