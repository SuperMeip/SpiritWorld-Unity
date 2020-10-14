using SpiritWorld.Events;
using SpiritWorld.Inventories.Items;
using SpiritWorld.World.Entities.Creatures;
using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace SpiritWorld.Controllers {
  public class LocalNotificationController : MonoBehaviour, IObserver {

    /// <summary>
    /// The times symbol used by notifications
    /// </summary>
    public const char TimesSymbol = '×';

    /// <summary>
    /// The default sprite for notifications
    /// </summary>
    public Sprite DefaultSprite;

    /// <summary>
    /// The notification object pool
    /// </summary>
    public NotificationController[] notificationPool;

    /// <summary>
    /// How many notifications we're showing atm
    /// </summary>
    int currentlyDisplayedNotificationCount = 0;

    /// <summary>
    /// How many notifications should we show at once?
    /// </summary>
    int maxVisibleNotifications 
      => notificationPool.Length;

    /// <summary>
    /// The pending notifications for the local player
    /// </summary>
    readonly ConcurrentQueue<Notification> pendingNotifications
      = new ConcurrentQueue<Notification>();

    /// <summary>
    /// Fill the pool
    /// </summary>
    void Awake() {
      notificationPool = GetComponentsInChildren<NotificationController>(true);
    }

    void Update() {
      if (currentlyDisplayedNotificationCount < maxVisibleNotifications 
        && getFreeController(out NotificationController freeController) 
        && pendingNotifications.TryDequeue(out Notification notification)
      ) {
        displayNotification(freeController, notification, currentlyDisplayedNotificationCount++);
      }
    }

    /// <summary>
    /// Receive notifications
    /// </summary>
    /// <param name="event"></param>
    public void notifyOf(IEvent @event) {
      switch (@event) {
        case PlayerController.PlayerObtainItemEvent pcPOI:
          pendingNotifications.Enqueue(getPlayerPickupItemNotification(pcPOI.item, pcPOI.player));
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Call when a notification has vanished from the given position.
    /// </summary>
    /// <param name="currentPosition"></param>
    internal void notificationCleared(int currentPosition) {
      // go though each active alert below it and set them to slide to a new position
    }

    /// <summary>
    /// Pass a notification off to the given controller
    /// </summary>
    void displayNotification(NotificationController freeController, Notification notification, int positionIndex) {
      freeController.displayNotification(notification, positionIndex);
    }

    /// <summary>
    /// Make a notification for when a player picks up an item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    Notification getPlayerPickupItemNotification(Item item, Player player) {
      return new Notification(TimesSymbol + " " + item.quantity.ToString(), ItemDataMaper.GetModelFor(item));
    }

    /// <summary>
    /// Get the first free notification controller
    /// </summary>
    /// <param name="freeController"></param>
    /// <returns></returns>
    bool getFreeController(out NotificationController freeController) {
      foreach(NotificationController notificationController in notificationPool) {
        if (!notificationController.isInUse) {
          freeController = notificationController;
          return true;
        }
      }
      
      freeController = null;
      return false;
    }
  }
}
