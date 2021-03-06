﻿using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using SpiritWorld.World.Terrain.TileGrid;
using System;

namespace SpiritWorld.World.Terrain.Features {

  /// <summary>
  /// Something on a tile
  /// </summary>
  public partial struct TileFeature : IEquatable<TileFeature> {

    /// <summary>
    /// There usually can only be one feature of each layer type on each tile at most.
    /// </summary>
    public enum Layer {
      Decoration, // ground level decorations. Can overlap with resources, but will be targeted by tools when no resources are on the tile.
      Resource, // targeted by tools, usually are mined or harvested and provide items.
      Sky // clouds etc
    }

    /// <summary>
    /// The type of rotation to use when positioning this feature in world
    /// </summary>
    public enum RotationType {
      Static, // set by model
      Random//, // random.range, 1->360, around the Y
      //Set // based on the facingDirection value of the TileFeature itself 
    }

    /// <summary>
    /// The type data for this feature
    /// </summary>
    public Type type {
      get;
    }

    /// <summary>
    /// The mode this feature is in
    /// Used to change the model/other details about it.
    /// </summary>
    public int mode {
      get;
      internal set;
    }

    /// <summary>
    /// How many more times this resource can be interacted with
    /// </summary>
    public int remainingInteractions {
      get;
      internal set;
    }

    /// <summary>
    /// If this feature's interactions have been used up
    /// </summary>
    public bool isUsedUp {
      get => type.IsInteractive && remainingInteractions == 0;
    }

    /// <summary>
    /// Make a new tile feature of the given type
    /// </summary>
    /// <param name="featureType"></param>
    public TileFeature(Type featureType, int? mode = null) {
      type = featureType;

      /// set the current mode:
      // use the provided, if one isnt provided
      // for features that can be used up, we start at the highest mode.
      // otherwise we start at mode 0, the base mode
      this.mode = mode == null
        ? type.IsInteractive
          ? type.NumberOfModes - 1
          : 0
        : (int)mode;

      // if this is interactive and use-upable, record our remaining use count.
      remainingInteractions = type.IsInteractive 
        ? type.NumberOfUses
        : Type.UnlimitedInteractions;
    }

    /// <summary>
    /// Use this tile feature
    /// </summary>
    /// <returns>The updated feature, or null if the feature should be destroyed.</returns>
    public TileFeature? interact(ITool toolUsed, float totalTimeUsedForSoFar, out IInventory drops) {
      // record drops if we get any back from the feature
      drops = null;
      // if we still have normal interactions left and this tool is valid for this feature, check if we're using one up.
      if (remainingInteractions != 0 && type.CanBeMinedBy(toolUsed, mode)) {
        if (type.IsInteractive && type.TryToUseOnce(totalTimeUsedForSoFar)) {
          remainingInteractions--;
          drops = type.GetRandomDrops(toolUsed, mode);
          // if this is a transitional type, it's out of uses, and we've held use for long enough, transition to the next tile type
          if (remainingInteractions == 0 && type is TransitionalResourceType transitionalResourceType) {
            return new TileFeature(transitionalResourceType.NextFeatureType);
          }
          
          // else update the mode
          updateModeBasedOnRemainingInteractions();
        }

      //shovel works at 0 (except in the sky lol), it can mine base level stuff off a tile.
      } else if (type.Layer != Layer.Sky && toolUsed.ToolType == Tool.Type.Shovel && type.TryToUseOnce(totalTimeUsedForSoFar)) {
        return null;
      }

      return this;
    }

    /// <summary>
    /// Tile equality check
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(TileFeature other) {
      return other.type == type 
        && other.mode == mode 
        && other.remainingInteractions == remainingInteractions;
    }

    /// <summary>
    /// Used for initalizing models to less than their normal mode or interaction count
    /// </summary>
    /// <param name="remainingInteractions"></param>
    public void setRemainingInteractions(int remainingInteractions) {
      this.remainingInteractions = remainingInteractions;
      updateModeBasedOnRemainingInteractions();
    }

    /// <summary>
    /// get the mode to switch to based on the % of uses remaining for this feature
    /// This assumes mode 0 is empty.
    /// </summary>
    /// <param name="count"></param>
    void updateModeBasedOnRemainingInteractions() {
      float percentRemaining = (float)remainingInteractions / type.NumberOfUses;
      mode = (int)((type.NumberOfModes - 1) * percentRemaining);
    }
  }
}

