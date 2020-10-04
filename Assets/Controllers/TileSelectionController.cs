using SpiritWorld.World.Terrain.TileGrid;
using UnityEngine;

namespace SpiritWorld.Controllers {
  public class TileSelectionController : MonoBehaviour {

    /// <summary>
    /// The object used to hilight the selected tile
    /// </summary>
    public GameObject SelectedTileHilight;

    /// <summary>
    /// The currently selected tile
    /// </summary>
    public Tile selectedTile {
      get;
      private set;
    }

    /// <summary>
    /// A timer for performing an action
    /// </summary>
    float actionTimer = 0.0f;

    /// <summary>
    /// Local player controller
    /// </summary>
    PlayerController playerController;

    /// <summary>
    /// Get the local player controller
    /// </summary>
    private void Awake() {
      playerController = GameObject.FindWithTag("Local Player").GetComponent<PlayerController>();
    }
  }
}
