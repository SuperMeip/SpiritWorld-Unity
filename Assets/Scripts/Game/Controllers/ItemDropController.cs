using SpiritWorld.Events;
using SpiritWorld.Inventories.Items;
using SpiritWorld.World.Entities.Creatures;
using UnityEngine;
using UnityEditor;

namespace SpiritWorld.Controllers {

  /// <summary>
  /// Controller for item drops
  /// </summary>
  public class ItemDropController : MonoBehaviour {

    /// <summary>
    /// How fast the hilight rotates around the item
    /// </summary>
    public int rotationSpeed
      = 20;

    /// <summary>
    /// The prefab used to hilight item drops
    /// </summary>
    public GameObject ItemHilight;

#if UNITY_EDITOR

    /// <summary>
    /// Testable item id useable for adding an empty item drop and generating it with the load button below.
    /// </summary>
    public ItemTypeField testItemId;

    /// <summary>
    /// How many items we want in our test stack
    /// </summary>
    public byte testQuantity;

#endif

    /// <summary>
    /// The id of the item this drop is for
    /// </summary>
    public Item item {
      get;
      private set;
    }

    /// <summary>
    /// The item used to hilight this item
    /// </summary>
    GameObject itemHilight;

    /// <summary>
    /// The item model for the item
    /// </summary>
    GameObject itemModel;

    /// <summary>
    /// Item hilight effects
    /// </summary>
    void Update() {
      if (item != null) {
        // we can destroy this if it's somehow empty
        if (item.quantity <= 0) {
          Destroy(gameObject);
        }

        /// spin the highlight
        if (itemHilight != null) {
          itemHilight.transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
        }
      }
    }

    /// <summary>
    /// Set the current item to display
    /// </summary>
    /// <param name="item"></param>
    public void setItem(Item item) {
      this.item = item;
      if (item != null && item.type != null) {
        itemModel = Instantiate(ItemDataMapper.GetModelFor(item), transform);
        // add a rigidbody for gravity and enable the colliders which are disabled by default.
        itemModel.AddComponent(typeof(Rigidbody));
        Collider[] itemColliders = itemModel.GetComponentsInChildren<Collider>(true);
        foreach (Collider itemCollider in itemColliders) {
          itemCollider.enabled = true;
        }
        itemHilight = Instantiate(ItemHilight, itemModel.transform);
      }
    }

    /// <summary>
    /// Add the item to the player's inventory when they walk into it
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision) {
      if (item != null && item.quantity > 0 && collision.gameObject.CompareTag("Player")) {
        tryToGetPickedUpBy(collision.gameObject.GetComponent<PlayerController>());
      }
    }

    /// <summary>
    /// If the local player clicks, have them try to pick up the item
    /// </summary>
    private void OnMouseDown() {
      tryToGetPickedUpBy(Universe.LocalPlayerController);
    }

    /// <summary>
    /// Try to get picked up by the given player
    /// </summary>
    /// <param name="player"></param>
    void tryToGetPickedUpBy(PlayerController player) {
      if (player != null) {
        // set the item to the leftover stack when the player tries to pick it up
        item = player.tryToPickUp(item);
        // if the stack is now empty, we can destroy it
        if (item != null && item.quantity <= 0) {
          Destroy(gameObject);
        }
      }
    }
  }

#if UNITY_EDITOR

  /// <summary>
  /// Make a button to load for testing
  /// </summary>
  [CustomEditor(typeof(ItemDropController))]
  public class ItemDropEditor : Editor {
    public override void OnInspectorGUI() {
      DrawDefaultInspector();

      ItemDropController itemDropController = (ItemDropController)target;
      if (GUILayout.Button("Build Object")) {
        itemDropController.setItem(new Item(Item.Types.Get((short)itemDropController.testItemId.itemId), itemDropController.testQuantity));
      }
    }
  }

#endif
}