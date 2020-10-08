using System;
using System.Collections.Generic;

namespace SpiritWorld.Stats {

  /// <summary>
  /// USed to hold a set of base stats
  /// </summary>
  public abstract class BaseStatCollection<OutputStatSheetType> :
    Dictionary<Stat.VariationGroups, (Dictionary<Stat.Type, BaseStatCollection<OutputStatSheetType>.VariantStat> variancePointPool, int baseStatTotal)> {

    /// <summary>
    /// Helper for getting variant stat const by type.
    /// </summary>
    /// <returns></returns>
    public VariantStat this[Stat.Type type]
      => TryGetValue(type.VariationGroup, out (Dictionary<Stat.Type, VariantStat> variancePointPool, int baseStatTotal) variancePool)
        ? variancePool.variancePointPool.TryGetValue(type, out VariantStat stat)
          ? stat
          : default
        : default;

    /// <summary>
    /// Generate a set of random but balanced stat blocks from this stat base.
    /// </summary>
    public OutputStatSheetType getRandomStatBlocks() {
      // collect all of our stat values as we increment them
      Dictionary<Stat.Type, int> statValues = new Dictionary<Stat.Type, int>();

      // go through each variance pool and set up each stat, and get it's excess values.
      foreach (KeyValuePair<Stat.VariationGroups, (Dictionary<Stat.Type, VariantStat> stats, int variancePointPool)> statVariancePool in this) {
        List<Stat.Type> statPointSlotPool = new List<Stat.Type>();
        foreach (VariantStat variantStat in statVariancePool.Value.stats.Values) {
          // set each stat to minimum to start, and add to our overall stat point pool all of the potential points
          statValues.Add(variantStat.type, variantStat.defaultMax - variantStat.variance);
          for (int index = 0; index < variantStat.variance * 2; index++) {
            statPointSlotPool.Add(variantStat.type);
          }
        }

        /// Shuffle the available stat point slots and fill them in until we've spent all of our remaining stat points
        statPointSlotPool.Shuffle();
        for (int remainingStatPoints = statVariancePool.Value.variancePointPool; remainingStatPoints > 0; remainingStatPoints--) {
          // get a random stat from the pool of available stat point slots to increase
          int randomIndex = UnityEngine.Random.Range(0, statPointSlotPool.Count);
          // update the stat by 1 point
          statValues[statPointSlotPool[randomIndex]] = statValues[statPointSlotPool[randomIndex]] + 1;
          // remove that item from the stat point slot pool as the slot has been used up
          statPointSlotPool.RemoveAt(randomIndex);
        }
      }

      /// return the packaged stat blocks
      return formatStatValuesForOutput(statValues);
    }

    /// <summary>
    /// Get the base stats in a block of the output type
    /// </summary>
    /// <returns></returns>
    public OutputStatSheetType getBaseStatBlock() {
      Dictionary<Stat.Type, int> statValues = new Dictionary<Stat.Type, int>();
      foreach (KeyValuePair<Stat.VariationGroups, (Dictionary<Stat.Type, VariantStat> stats, int variancePointPool)> statVariancePool in this) {
        foreach (VariantStat variantStat in statVariancePool.Value.stats.Values) {
          statValues.Add(variantStat.type, variantStat.defaultMax);
        }
      }

      return formatStatValuesForOutput(statValues);
    }

    /// <summary>
    /// Format the calculated max values in the stat type array for return.
    /// </summary>
    /// <param name="statValues"></param>
    /// <returns></returns>
    protected abstract OutputStatSheetType formatStatValuesForOutput(Dictionary<Stat.Type, int> statValues);

    /// <summary>
    /// Add a stat to this collection
    /// </summary>
    /// <param name="type"></param>
    /// <param name="statValues"></param>
    protected void add(Stat.Type type, (int value, int variance) statValues) {
      if (TryGetValue(type.VariationGroup, out (Dictionary<Stat.Type, VariantStat> stats, int variancePointPool) statVariationGroup)) {
        statVariationGroup.stats.Add(
          type,
          new VariantStat(type, statValues.value, statValues.variance)
        );
        this[type.VariationGroup] = (
          statVariationGroup.stats,
          statVariationGroup.variancePointPool + statValues.variance
        );
      } else {
        this[type.VariationGroup] = (
          new Dictionary<Stat.Type, VariantStat>() {{
            type,
            new VariantStat(type, statValues.value, statValues.variance)
          }},
          statValues.variance
        );
      }
    }

    /// <summary>
    /// Add utility
    /// </summary>
    protected void add(Stat.Type type, int value) {
      add(type, (value, 0));
    }

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
