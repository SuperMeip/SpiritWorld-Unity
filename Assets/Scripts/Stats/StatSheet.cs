using System.Collections.Generic;

namespace SpiritWorld.Stats {

  /// <summary>
  /// A basic sheet of stats
  /// </summary>
  public abstract class StatSheet : Dictionary<Stat.Type, Stat> {

    /// <summary>
    /// Block set. Make get return empty stats as default
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public new Stat this[Stat.Type type] {
      get => TryGetValue(type, out Stat stat) 
        ? stat 
        : default;
      protected set => base[type] = value;
    }
  }
}
