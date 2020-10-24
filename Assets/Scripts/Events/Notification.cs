using SpiritWorld.Game.Controllers;
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
    public Object icon => itemIcon;

    /// <summary>
    /// The message text of the notification
    /// </summary>
    public string message {
      get;
    }

    /// <summary>
    /// The icon for the item;
    /// </summary>
    ItemIconController itemIcon;

    /// <summary>
    /// Make a new notification
    /// </summary>
    /// <param name="message"></param>
    /// <param name="icon"></param>
    public Notification(string message, ItemIconController icon) {
      this.message = message;
      itemIcon = icon;
      displayTime = DefaultDisplayTime;
    }
  }
}