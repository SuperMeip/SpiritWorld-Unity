using SpiritWorld.World.Entities;
using SpiritWorld.World.Entities.Creatures.AI;
using UnityEngine;

namespace SpiritWorld.Controllers {

  /// <summary>
  /// Controls any combat capable entities, including players
  /// </summary>
  public class CreatureController : MonoBehaviour {

    /// <summary>
    /// The data for the creature entity we're controlling
    /// </summary>
    public Creature creature {
      get;
      private set;
    }

    /// <summary>
    /// The current state the creature AI is in
    /// </summary>
    public AI.States state {
      get;
      private set;
    } = AI.States.Initial;

    void Update() {
      switch (state) {
        case AI.States.Initial:
          //creature.type.AI.initialize();
          break;
        default:
          break;
      }
    }

    void OnMouseEnter() {
      // highlight if it's not yourself
    }

    void OnMouseExit() {
      // un-highlight
    }

    void OnMouseDown() {
      // give creature info and also:
      // enable hold down for combat
    }

    void lookAround() {
      //Physics.OverlapSphere()
    }
  }
}
