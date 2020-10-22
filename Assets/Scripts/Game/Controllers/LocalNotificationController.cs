using SpiritWorld.Events;
using SpiritWorld.Inventories.Items;
using SpiritWorld.World.Entities.Creatures;
using System.Collections.Concurrent;
using System.Threading.Tasks;
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
    NotificationController[] notificationPool;

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

    /// <summary>
    /// check if we can display notifications, and if we can, do so
    /// </summary>
    void Update() {
      if (currentlyDisplayedNotificationCount < maxVisibleNotifications 
        && pendingNotifications.TryDequeue(out Notification notification)
        && getFreeController(out NotificationController freeController)
      ) {
        displayNotification(freeController, notification, currentlyDisplayedNotificationCount++);
      }
    }

    /// <summary>
    /// Receive notifications
    /// </summary>
    /// <param name="event"></param>
    public void notifyOf(IEvent @event) {
      // notifications are fired off in tasks
      switch (@event) {
        case PlayerController.PlayerObtainItemEvent pcPOI:
          // if it's the local player we show a notification
          if (pcPOI.player == Universe.LocalPlayer) {
            new Task(() => {
              pendingNotifications.Enqueue(getPlayerPickupItemNotification(pcPOI.item, pcPOI.player));
            }).Start();
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Call when a notification has vanished from the given position.
    /// </summary>
    /// <param name="clearedPosition"></param>
    public void notificationCleared(int clearedPosition) {
      currentlyDisplayedNotificationCount--;
      foreach(NotificationController notificationController in notificationPool) {
        if (notificationController.isActive && notificationController.currentPosition >= clearedPosition) {
          notificationController.slideUp();
        }
      }
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
      return new Notification(TimesSymbol + " " + item.quantity.ToString() + " " + item.type.Name, ItemDataMapper.GetModelFor(item));
    }

    /// <summary>
    /// Get the first free notification controller
    /// </summary>
    /// <param name="freeController"></param>
    /// <returns></returns>
    bool getFreeController(out NotificationController freeController) {
      foreach(NotificationController notificationController in notificationPool) {
        lock (notificationController) {
          if (notificationController.tryToGetLock()) {
            freeController = notificationController;
            return true;
          }
        }
      }
      
      freeController = null;
      return false;
    }
  }
}
