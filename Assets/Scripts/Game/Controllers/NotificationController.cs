using SpiritWorld.Events;
using SpiritWorld.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace SpiritWorld.Game.Controllers {

  [RequireComponent(typeof(CanvasGroup))]
  public class NotificationController : MonoBehaviour {

    /// <summary>
    /// How long it should take to slide in seconds
    /// </summary>
    const float SlideTime = 1;

    /// <summary>
    /// Time in seconds it takes a new notification to fade in from transparent
    /// </summary>
    const float FadeInTime = 0.7f;

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
    public Transform iconParent;

    /// <summary>
    /// The message text object
    /// </summary>
    public Text messageText;

    /// <summary>
    /// The background image of the message text
    /// </summary>
    public Image messageTextBackground;

    /// <summary>
    /// If this notification controller is being used
    /// </summary>
    public bool isActive {
      get;
      private set;
    } = false;

    /// <summary>
    /// The position this notification is currently in
    /// </summary>
    public int currentPosition {
      get;
      private set;
    }

    /// <summary>
    /// The parent controller for callbacks
    /// </summary>
    LocalNotificationsManager manager;

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
    /// If this notification controller is being used
    /// </summary>
    bool isLockedForUse
      = false;

    /// <summary>
    /// The model icon we're using.
    /// TODO make an interface for notification icons
    /// </summary>
    ItemIconController icon;

    /// <summary>
    /// The canvas renderer, for fade
    /// </summary>
    CanvasGroup notificationCanvas;

    /// <summary>
    /// The canvas renderer, for fade
    /// </summary>
    RectTransform rectTransform;

    /// <summary>
    /// The previous position for the slide effect
    /// </summary>
    int previousPosition;

    /// <summary>
    /// Connections
    /// </summary>
    void Awake() {
      isLockedForUse = false;
      rectTransform = GetComponent<RectTransform>();
      notificationCanvas = notificationCanvas ?? GetComponent<CanvasGroup>();
      manager = Universe.LocalPlayerManager.NotificationsManager;
    }

    /// <summary>
    /// fade in from below then pop out and slide up
    /// </summary>
    void Update() {
      if (isActive) {
        /// dislay
        if (displayTimer <= 0) {
          displayTimer = 0;
          clearNotification();
        } else {
          displayTimer -= Time.deltaTime;
        }

        /// fade
        if (isFading) {
          if (fadeTimer <= 0) {
            fadeTimer = 0;
            isFading = false;
            notificationCanvas.alpha = 1;
            icon.setOpacity(1);
          } else {
            fadeTimer -= Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, 1 - fadeTimer / FadeInTime);
            notificationCanvas.alpha = alpha;
            icon.setOpacity(alpha);
          }
        }

        // if we have an icon and it's not on yet, turn it on
        if (icon != null && icon.gameObject.activeSelf == false) {
          icon.gameObject.SetActive(true);
        }

        /// slide
        if (isSliding) {
          if (slideTimer <= 0) {
            slideTimer = 0;
            isSliding = false;
            rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, NotificationTopY - (NotificationHeight * currentPosition), 0);
          } else {
            // if we're in the starter bottom position (-1), the previous y position is set to the const
            float previousPositionY = previousPosition == BottomPositionIndex
              ? BottomYPosition
              : (NotificationTopY - (NotificationHeight * previousPosition));
            slideTimer -= Time.deltaTime;
            float interpolatedY = Mathf.Lerp(
              previousPositionY,
              (NotificationTopY - (NotificationHeight * currentPosition)),
              1 - slideTimer / FadeInTime
            );
            rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, interpolatedY, 0);
          }
        }
      }
    }

    /// <summary>
    /// Try to lock this notification for use
    /// </summary>
    /// <returns></returns>
    public bool tryToGetLock() {
      if (!isLockedForUse) {
        isLockedForUse = true;
        return true;
      } else return false;
    }

    /// <summary>
    /// Clear this notification from the list
    /// </summary>
    void clearNotification() {
      gameObject.SetActive(false);
      if (icon != null) {
        Destroy(icon.gameObject);
        icon = null;
      }
      manager.notificationCleared(currentPosition);

      isActive = false;
      isLockedForUse = false;
    }

    /// <summary>
    /// Display the notification in the given slot
    /// </summary>
    /// <param name="notification">The notification</param>
    /// <param name="position">Which position to show it in, top is 0</param>
    public void displayNotification(Notification notification, int position) {
      // set timer
      notificationCanvas = notificationCanvas ?? GetComponent<CanvasGroup>();
      displayTimer = notification.displayTime;
      // set the icon and text
      messageText.text = notification.message;
      icon = notification.icon as ItemIconController;
      icon.parentTo(iconParent);
      icon.resize(65);
      icon.setBGColor(new Color(0, 131, 200));
      // set active and begin animating.
      beginFadeIn();
      beginSliding(BottomPositionIndex, position);
      gameObject.SetActive(true);
      isActive = true;
    }

    /// <summary>
    /// slide this notification up when one vanishes
    /// </summary>
    public void slideUp() {
      beginSliding(isSliding ? previousPosition : currentPosition, --currentPosition);
    }

    /// <summary>
    /// Begin the fade in process
    /// </summary>
    void beginFadeIn() {
      icon.setOpacity(0);
      notificationCanvas.alpha = 0;
      isFading = true;
      fadeTimer = FadeInTime;
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