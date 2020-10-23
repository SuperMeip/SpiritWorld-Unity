using SpiritWorld.Game.Controllers;
using SpiritWorld.Inventories.Items;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Data map for items and their models
/// </summary>
public class ItemDataMapper : MonoBehaviour {

  /// <summary>
  /// the material items use when in the UI
  /// </summary>
  public static Shader ItemUIShader;

  /// <summary>
  /// The prefab object for item icons
  /// </summary>
  public static GameObject ItemIconPrefab;

  /// <summary>
  /// const for item models by id
  /// </summary>
  public static Dictionary<int, ItemDataMap> Items
    = new Dictionary<int, ItemDataMap>();

  /// <summary>
  /// The prefab for item icons
  /// </summary>
  public GameObject itemIconPrefab;

  /// <summary>
  /// the material items use when in the UI
  /// </summary>
  public Shader itemUIShader;

  /// <summary>
  /// Input for item models by ID
  /// Leave 0 empty.
  /// item_id => item_object,
  /// </summary>
  [SerializeField]
  public ItemDataMap[] ItemDataMaps
    = new ItemDataMap[0];

  /// <summary>
  /// set up consts
  /// </summary>
  void Awake() {
    ItemIconPrefab = itemIconPrefab;
    ItemUIShader = itemUIShader;
    // set up the map
    foreach (ItemDataMap itemDataMap in ItemDataMaps) {
      if (itemDataMap.ItemType.itemId != 0) {
        Items.Add(itemDataMap.ItemType.itemId, itemDataMap);
      }
    }
  }

  /// <summary>
  /// Get a model for an item
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  public static GameObject GetModelFor(Item item) {
    return Items[item.type.Id].OverworldModel;
  }

  /// <summary>
  /// Gets the sprite or gameobject we use as an icon for this item
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  public static Sprite GetIconFor(Item item, bool getShapedIcon = false) {
    /// if we want the shaped, see if we have a specific one for shaped
    Sprite itemSprite = null;
    if (getShapedIcon) {
      itemSprite = Items[item.type.Id].ShapedInventoryIcon;
    }

    // if we want just a normal icon or don't have a shaped icon try the square one
    if (itemSprite == null) {
      itemSprite = Items[item.type.Id].SmallIcon;
    }

    return itemSprite;
  }
}

/// <summary>
/// A map representing the models for one type of terrain feature sorted by how many usages it has left
/// </summary>
[System.Serializable]
public class ItemDataMap {

  /// <summary>
  /// The id of the item this entry is for
  /// </summary>
  [SerializeField]
  public ItemTypeField ItemType = default;

  /// <summary>
  /// The model used for this item in the overworld
  /// </summary>
  [SerializeField]
  [Tooltip("The model used for this item in the overworld")]
  public GameObject OverworldModel;

  /// <summary>
  /// Basic small square icon for the item
  /// </summary>
  [SerializeField]
  [Tooltip("The model used for this item in the overworld")]
  public Sprite SmallIcon;

  /// <summary>
  /// Shaped inventory icon for the item
  /// </summary>
  [SerializeField]
  [Tooltip("The model used for this item in the overworld")]
  public Sprite ShapedInventoryIcon;
}

/// <summary>
/// Custom inspector type for item type
/// </summary>
[System.Serializable]
public class ItemTypeField {
  [SerializeField]
  public int itemId = 0;
}

/// <summary>
/// custom inspector gui for item type
/// </summary>
[CustomPropertyDrawer(typeof(ItemTypeField))]
public class ItemTypePropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);

    /// get the selected item
    int selectedItemId = property.FindPropertyRelative("itemId").intValue;

    // get the array values to display for item names and ids
    Item.Type[] allItems = Item.Types.All;
    List<string> names = allItems.Select(type => type.Name).ToList();
    names.Add("--");
    List<int> ids = allItems.Select(type => (int)type.Id).ToList();
    ids.Add(0);

    // display the dropdown
    property.FindPropertyRelative("itemId").intValue 
      = EditorGUI.IntPopup(position, "Item Type:", selectedItemId, names.ToArray(), ids.ToArray(), EditorStyles.popup);

    EditorGUI.EndProperty();
  }
}

  /// <summary>
  /// Custom array drawer
  /// </summary>
  [CustomEditor(typeof(ItemDataMapper))]
public class ItemsDataMapPropertyDrawer : Editor {
  public override void OnInspectorGUI() {
    serializedObject.Update(); 
    DrawPropertiesExcluding(serializedObject, new string[] { "ItemDataMaps", "m_Script" });
    EditorList.Show(
      serializedObject.FindProperty("ItemDataMaps"),
      EditorListOption.ButtonsBelow | EditorListOption.ListLabel | EditorListOption.Buttons | EditorListOption.ElementLabels,
      (element, _) => {
        int? itemId = element.FindPropertyRelative("ItemType").FindPropertyRelative("itemId")?.intValue;
        return itemId != 0 && itemId != null
          ? Item.Types.Get((short)itemId).Name
          : "No Item Set";
      }
    );
    serializedObject.ApplyModifiedProperties();
  }
}