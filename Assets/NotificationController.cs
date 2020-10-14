using SpiritWorld.Events;
using UnityEngine;
using UnityEngine.UI;

namespace SpiritWorld.Controllers {

  public class NotificationController : MonoBehaviour {

    /// <summary>
    /// How long it should take to slide in seconds
    /// </summary>
    const float SlideTime = 1;

    /// <summary>
    /// The initial y position of the top notification
    /// </summary>
    const float NotificationTopY = -32.5f;

    /// <summary>
    /// How much to offset (subtract from) the y position of the notification according to it's desired position
    /// </summary>
    const float NotificationHeight = 67.5f;

    /// <summary>
    /// The y we want new notifications to slide in from
    /// </summary>
    const float BottomYPosition = -200;

    /// <summary>
    /// The position index to use for the very bottom one
    /// </summary>
    const int BottomPositionIndex = -1;

    /// <summary>
    /// The object controlling the icon
    /// </summary>
    public Image icon;

    /// <summary>
    /// The object controlling the icon
    /// </summary>
    public GameObject modelIconScaler;

    /// <summary>
    /// The image background of the icon
    /// </summary>
    public Image iconBackground;

    /// <summary>
    /// The message text object
    /// </summary>
    public Text messageText;

    /// <summary>
    /// The background image of the message text
    /// </summary>
    public Image messageTextBackground;

    /// <summary>
    /// The parent controller for callbacks
    /// </summary>
    LocalNotificationController notificationsController;

    /// <summary>
    /// The display timer for the notification
    /// </summary>
    float displayTimer;

    /// <summary>
    /// If this is executing the fade effect
    /// </summary>
    bool isFading = false;

    /// <summary>
    /// the timer for the fade effect
    /// </summary>
    float fadeTimer;

    /// <summary>
    /// If this is executing the slide effect
    /// </summary>
    bool isSliding = false;

    /// <summary>
    /// The timer for the slide effect
    /// </summary>
    float slideTimer;

    /// <summary>
    /// The model icon we're using.
    /// </summary>
    GameObject currentModelIcon;

    /// <summary>
    /// The position this notification is currently in
    /// </summary>
    int currentPosition;

    /// <summary>
    /// The previous position for the slide effect
    /// </summary>
    int previousPosition;

    /// <summary>
    /// If this notification controller is being used
    /// </summary>
    public bool isInUse {
      get;
      private set;
    }

    /// <summary>
    /// Connections
    /// </summary>
    void Awake() {
      notificationsController 
        = GameObject.FindWithTag("Local Notification Controller").GetComponent<LocalNotificationController>();
    }

    /// <summary>
    /// Slowly count down then fade out
    /// </summary>
    void Update() {
      if (isInUse) {
        if (displayTimer <= 0) {
          displayTimer = 0;
          clearNotification();
        } else {
          displayTimer -= Time.deltaTime;
        }
      }
    }

    /// <summary>
    /// Clear this notification from the list
    /// TODO: before this is called, make a function to set a fade bool first that fades the notification out in update, then after the fade timer runs out we can destroy the object and call clear
    /// </summary>
    void clearNotification() {
      if (currentModelIcon != null) {
        Destroy(currentModelIcon);
      }
      notificationsController.notificationCleared(currentPosition);

      isInUse = false;
    }

    /// <summary>
    /// Display the notification in the given slot
    /// </summary>
    /// <param name="notification">The notification</param>
    /// <param name="position">Which position to show it in, top is 0</param>
    public void displayNotification(Notification notification, int position) {
      // slide notification into it's proper spot
      transform.position = new Vector3(
        transform.position.x,
        NotificationTopY + (position * NotificationHeight),
        transform.position.z
      );
      isInUse = true;
      gameObject.SetActive(true);
      beginSliding(BottomPositionIndex, position);
      displayTimer = notification.displayTime;
      Object notificationIcon = notification.icon;
      if (notificationIcon is GameObject modelIcon) {
        currentModelIcon = Instantiate(modelIcon, modelIconScaler.transform);
      }
      if (notificationIcon is Sprite spriteIcon) {
        icon.sprite = spriteIcon;
      }
      messageText.text = notification.message;
    }

    /// <summary>
    /// slide this notification up when one vanishes
    /// </summary>
    public void slideUp() {
      beginSliding(currentPosition, currentPosition--);
    }

    /// <summary>
    /// begin sliding from a position to another one
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    void beginSliding(int from, int to) {
      previousPosition = from;
      currentPosition = to;
      isSliding = true;
      slideTimer = SlideTime;
    }
  }
}