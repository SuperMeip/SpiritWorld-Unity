using SpiritWorld.Inventories.Items;
using UnityEngine;

/// <summary>
/// Data map for items and their models
/// </summary>
public class ItemModelDataMaper : MonoBehaviour {

  /// <summary>
  /// Input for item models by ID
  /// Leave 0 empty.
  /// item_id => item_object,
  /// </summary>
  [SerializeField]
  public GameObject[] ItemModelsById;

  /// <summary>
  /// const for item models by id
  /// </summary>
  public static GameObject[] ItemModels;


  // Start is called before the first frame update
  void Awake() {
    ItemModels = ItemModelsById;
  }

  /// <summary>
  /// Get a model for an item
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  public static GameObject GetModelForItem(Item item) {
    return ItemModels[item.type.Id];
  }
}
