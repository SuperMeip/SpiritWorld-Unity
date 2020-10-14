using SpiritWorld.Inventories.Items;
using SpiritWorld.World.Entities.Creatures;
using SpiritWorld.Events;
using UnityEngine;

namespace SpiritWorld.Controllers {
  public class PlayerController : MonoBehaviour {

    /// <summary>
    /// The player this controller is for
    /// </summary>
    public Player player;

    /// <summary>
    /// Try to pick up the given item stack
    /// </summary>
    /// <param name="itemStack"></param>
    /// <returns></returns>
    public Item tryToPickUp(Item itemStack) {
      return player.inventory.tryToAdd(itemStack);
    }

    public struct PlayerObtainItemEvent : IEvent {
      public string name
        => "Player picked up item";

      /// <summary>
      /// The item that was picked up
      /// </summary>
      public Item item {
        get;
      }

      /// <summary>
      /// The player who picked it up
      /// </summary>
      public Player player {
        get;
      }

      /// <summary>
      /// Make an event of this kind
      /// </summary>
      /// <param name="player"></param>
      /// <param name="item"></param>
      public PlayerObtainItemEvent(Player player, Item item) {
        this.item = item;
        this.player = player;
      }
    }
  }
}
