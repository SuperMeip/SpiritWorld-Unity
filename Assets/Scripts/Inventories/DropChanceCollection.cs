using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpiritWorld.Inventories {

  /// <summary>
  /// A collection used to generate random drops and store chances and said drops
  /// </summary>
  public class DropChanceCollection {

    /// <summary>
    /// potential prize pools
    /// </summary>
    readonly List<(int prizeWeight, IInventory rewards)> drops;

    /// <summary>
    /// Create a new set of drops
    /// </summary>
    /// <param name="drops"></param>
    public DropChanceCollection((int prizeWeight, IInventory rewards)[] drops) {
      this.drops = new List<(int prizeWeight, IInventory rewards)>(drops);
    }

    /// <summary>
    /// Make empty one
    /// </summary>
    public DropChanceCollection() {
      drops = new List<(int prizeWeight, IInventory rewards)>();
    }

    /// <summary>
    /// Get a random drop
    /// </summary>
    /// <returns></returns>
    public IInventory getRandomDrop() {
      int currentPrizeIndex = 0;
      List<int> prizeIndexPool = new List<int>();
      // go though each potential drop and fill in the prize pool
      foreach(var potentialDrop in drops) {
        // fill the pool with an entry per prize weight
        for(int index = 0; index < potentialDrop.prizeWeight; index ++) {
          prizeIndexPool.Add(currentPrizeIndex);
        }
        currentPrizeIndex++;
      }

      // shuffle and return a random one
      prizeIndexPool.Shuffle();
      return drops[prizeIndexPool[UnityEngine.Random.Range(0, prizeIndexPool.Count)]].rewards?.copy();
    }

    /// <summary>
    /// Add a possibility
    /// </summary>
    public void add(int weight, IInventory dropCollection) {
      drops.Add((weight, dropCollection));
    }
  }
}
