using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPackMenuController : MonoBehaviour {

  #region Constants

  /// <summary>
  /// The menu we're toggling
  /// </summary>
  public GameObject PackMenu;

  /// <summary>
  /// The button used to toggle
  /// </summary>
  public Button BackpackButton;

  #endregion

  /// <summary>
  /// if the menu is hidden
  /// </summary>
  public bool menuIsVisible {
    get;
    private set;
  } = false;

  /// <summary>
  /// Toggle the menu on and off
  /// </summary>
  public void toggleMenu() {
    menuIsVisible = !menuIsVisible;
    if (menuIsVisible) {
      BackpackButton.gameObject.SetActive(false);
      PackMenu.SetActive(true);
    } else {
      PackMenu.SetActive(false);
      BackpackButton.gameObject.SetActive(true);
    }
  }
}
