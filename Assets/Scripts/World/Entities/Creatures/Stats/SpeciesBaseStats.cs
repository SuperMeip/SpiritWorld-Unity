using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorld.World.Entities.Creatures.Stats {

  /// <summary>
  /// A base stat collection and management class for different creatures
  /// </summary>
  public class SpeciesBaseStats : BaseStatCollection {

    /// <summary>
    /// Make a block of basic stats
    /// </summary>
    public SpeciesBaseStats(
      (int value, int variance) HP,
      (int value, int variance) EP,
      (int value, int variance) SP,
      (int value, int variance) STR,
      (int value, int variance) DEF,
      (int value, int variance) ATN,
      (int value, int variance) FOR,
      (int value, int variance) INT,
      (int value, int variance) SKL,
      (int value, int variance) SR,
      (int value, int variance) VA,
      (int value, int variance) SM,
      (int value, int variance) HR,
      (int value, int variance) NS,
      (int value, int variance) EX
    ) {
      /// depletable stats
      add(Stat.Types.HP, HP);
      add(Stat.Types.Energy, EP);
      add(Stat.Types.Stamina, SP);

      /// combat stats
      add(Stat.Types.Strength, STR);
      add(Stat.Types.Defence, DEF);
      add(Stat.Types.Attunement, ATN);
      add(Stat.Types.Fortitude, FOR);
      add(Stat.Types.Intellegence, INT);
      add(Stat.Types.Skill, SKL);

      /// perception stats
      add(Stat.Types.SightRange, SR);
      add(Stat.Types.VisionAccuracy, VA);
      add(Stat.Types.Smell, SM);
      add(Stat.Types.HearingRadius, HR);
      add(Stat.Types.NoiseSensitivity, NS);
      add(Stat.Types.ExtraSensoryPerception, EX);
    }

    /// <summary>
    /// Generate a set of random but balanced stat blocks from this stat base.
    /// </summary>
    public (CombatStats combatStats, SenseStats senseStats) getStatBlocks() {
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
      return (
        new CombatStats(
          statValues[Stat.Types.HP],
          statValues[Stat.Types.Energy],
          statValues[Stat.Types.Stamina],
          statValues[Stat.Types.Strength],
          statValues[Stat.Types.Defence],
          statValues[Stat.Types.Attunement],
          statValues[Stat.Types.Fortitude],
          statValues[Stat.Types.Intellegence],
          statValues[Stat.Types.Skill]
        ),
        new SenseStats(
          statValues[Stat.Types.SightRange],
          statValues[Stat.Types.VisionAccuracy],
          statValues[Stat.Types.Smell],
          statValues[Stat.Types.HearingRadius],
          statValues[Stat.Types.NoiseSensitivity],
          statValues[Stat.Types.ExtraSensoryPerception]
        )
      );
    }

    /// <summary>
    /// Add a stat to this collection
    /// </summary>
    /// <param name="type"></param>
    /// <param name="statValues"></param>
    void add(Stat.Type type, (int value, int variance) statValues) {
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

    #region Old Code

    /// <summary>
    /// Generate a set of random but balanced stat blocks from this stat base.
    /// </summary>
    (CombatStats combatStats, SenseStats senseStats) getStatBlocksAlt() {
      Dictionary<Stat.Type, Stat> stats = new Dictionary<Stat.Type, Stat>();
      foreach (KeyValuePair<Stat.VariationGroups, (Dictionary<Stat.Type, VariantStat> stats, int variancePointPool)> statVariancePool in this) {
        int pointsSpentLastLoop = 0;
        int pointsSpentThisLoop = 0;
        // we add and remove stat variations 4 times, remove, add, remove, add, each time the same amount in different values.
        foreach (int positiveNegativeMultiplier in new int[] { -1, 1, -1, 1 }) {
          // the first time though, pointsSpentLastLoop should remain 0 and we should only need to run this once,
          // hence the do while. This is because, we collect the offset we're going to be mirroring back and forth on
          // the first run.
          do {
            pointsSpentThisLoop = 0;
            // go though each stat in this variance group and spend a random # of variance points.
            foreach (var variantStat in statVariancePool.Value.stats.Values.ToList().Shuffle()) {
              if (stats.TryGetValue(variantStat.type, out Stat currentStat)) {
                int originalStatValue = currentStat.max;
                int variationValue = UnityEngine.Random.Range(0, variantStat.variance) * positiveNegativeMultiplier;
                int newStatValue;
                if (positiveNegativeMultiplier == 1) {
                  newStatValue = Math.Min(originalStatValue + variationValue, variantStat.variance + variantStat.defaultMax);
                } else {
                  newStatValue = Math.Max(originalStatValue + variationValue, variantStat.defaultMax - variantStat.variance);
                }
                // if the stat diff is too high
                int statDiff = (newStatValue - originalStatValue) * positiveNegativeMultiplier;
                if (pointsSpentLastLoop != 0 && (pointsSpentThisLoop + statDiff > pointsSpentLastLoop)) {
                  // get the offset we want to trim off of our value to avoid over spending points
                  int pointsToTrim = (pointsSpentThisLoop + statDiff - pointsSpentLastLoop) * positiveNegativeMultiplier;
                  newStatValue += pointsToTrim;
                }
                // set the updated stat
                stats[currentStat.type] = new Stat(currentStat, newStatValue);
                // update our points so far
                pointsSpentThisLoop += (newStatValue - originalStatValue) * positiveNegativeMultiplier;
              } else {
                // first loop, when there's no stats we just randomize and set them, and save the points spent this loop as our original mark
                // should always be a negative loop
                int pointsToSpend = UnityEngine.Random.Range(0, variantStat.variance);
                stats[variantStat.type] = new Stat(variantStat.type, variantStat.defaultMax - pointsToSpend);
                pointsSpentThisLoop += pointsToSpend;
              }
            }
          } while (pointsSpentThisLoop < pointsSpentLastLoop);
          // mostly matters after the first loop cuts out for the first time
          pointsSpentLastLoop = pointsSpentThisLoop;
        }
      }

      return (
        new CombatStats(
          stats[Stat.Types.HP].max,
          stats[Stat.Types.Energy].max,
          stats[Stat.Types.Stamina].max,
          stats[Stat.Types.Strength].max,
          stats[Stat.Types.Defence].max,
          stats[Stat.Types.Attunement].max,
          stats[Stat.Types.Fortitude].max,
          stats[Stat.Types.Intellegence].max,
          stats[Stat.Types.Skill].max
        ),
        new SenseStats(
          stats[Stat.Types.SightRange].max,
          stats[Stat.Types.VisionAccuracy].max,
          stats[Stat.Types.Smell].max,
          stats[Stat.Types.HearingRadius].max,
          stats[Stat.Types.NoiseSensitivity].max,
          stats[Stat.Types.ExtraSensoryPerception].max
        )
      );
    }

    #endregion
  }
}
