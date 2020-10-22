using UnityEngine;

namespace SpiritWorld.Events {
  public struct Notification {

    /// <summary>
    /// How long to display each notification in seconds
    /// </summary>
    public const float DefaultDisplayTime = 5f;

    /// <summary>
    /// How long we want to show the notification for
    /// </summary>
    public float displayTime;

    /// <summary>
    /// Get the icon to use.
    /// Either a gameobject model or sprite
    /// </summary>
    public Object icon => imageIcon 
      ?? (Object)modelIcon;

    /// <summary>
    /// The message text of the notification
    /// </summary>
    public string message {
      get;
    }

    /// <summary>
    /// The object being used as an icon.
    /// Can be a model or image.
    /// </summary>
    Sprite imageIcon;

    /// <summary>
    /// The object being used as an icon.
    /// Can be a model or image.
    /// </summary>
    GameObject modelIcon;

    /// <summary>
    /// Make a new notification
    /// </summary>
    /// <param name="message"></param>
    /// <param name="icon"></param>
    public Notification(string message, GameObject icon) {
      this.message = message;
      modelIcon = icon;
      imageIcon = null;
      displayTime = DefaultDisplayTime;
    }

    /// <summary>
    /// Make a new notification
    /// </summary>
    /// <param name="message"></param>
    /// <param name="icon"></param>
    public Notification(string message, Sprite icon) {
      this.message = message;
      imageIcon = icon;
      modelIcon = null;
      displayTime = DefaultDisplayTime;
    }
  }
}