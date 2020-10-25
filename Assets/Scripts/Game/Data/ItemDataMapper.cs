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
  /// The shaped item sprite backgrouns, indexed by bitflag.
  /// grid is 9x9
  /// 6 7 8
  /// 3 4 5
  /// 0 1 2
  /// </summary>
  public static Dictionary<int, Sprite> ShapedItemSpriteBackgrounds
    = new Dictionary<int, Sprite>();

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
  /// The shaped icon data map
  /// </summary>
  [SerializeField]
  public ShapedItemSpriteBackgroundDataMap[] ShapedItemSpriteBackgroundDataMaps
    = new ShapedItemSpriteBackgroundDataMap[0];

  /// <summary>
  /// set up consts
  /// </summary>
  void Awake() {
    ItemIconPrefab = itemIconPrefab;
    ItemUIShader = itemUIShader;
    // set up this map by item id
    foreach (ItemDataMap itemDataMap in ItemDataMaps) {
      if (itemDataMap.ItemType.itemId != 0) {
        Items.Add(itemDataMap.ItemType.itemId, itemDataMap);
      }
    }

    // store this map as bitmasks
    foreach(ShapedItemSpriteBackgroundDataMap shapedSpriteOutlineMap in ShapedItemSpriteBackgroundDataMaps) {
      int solidBlockFlags = 0;
      int blockIndex = 0;
      int mapWidth = shapedSpriteOutlineMap.ShapeMap.Map[0].Map.Length;
      int mapHeight = shapedSpriteOutlineMap.ShapeMap.Map.Length;
      for (int x = 0; x < 3; x++) {
        // if we're not in bounds it's not solid
        if (x >= mapWidth) {
          break;
        } else {
          for (int y = 0; y < 3; y++) {
            // if we're not in bounds it's not solid
            if (y >= mapHeight) {
              break;
            } else if (shapedSpriteOutlineMap.ShapeMap.Map[y].Map[x]) {
              solidBlockFlags = solidBlockFlags.TurnBitOn(++blockIndex);
            }
          }
        }
      }

      // set the value at the bitmask
      ShapedItemSpriteBackgrounds[solidBlockFlags] = shapedSpriteOutlineMap.ShapedBackgroundSprite;
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

  /// <summary>
  /// Get the outline for the shaped sprite
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  public static Sprite GetShapedOutlineFor(Item item) {
    return ShapedItemSpriteBackgrounds[GetSolidBlockBitflags(item.type)];
  }

  /// <summary>
  /// Convert the block type to a bitflag.
  /// TODO move this to item.type?
  /// </summary>
  /// <param name="itemType"></param>
  /// <returns></returns>
  static int GetSolidBlockBitflags(Item.Type itemType) {
    int solidBlockFlags = 0;
    int blockIndex = 0;
    (int shapeWidth, int shapeHeight) = itemType.ShapeSize;
    for (int x = 0; x < 3; x++) {
      // if we're not in bounds it's not solid
      if (x >= shapeWidth) {
        break;
      } else {
        for (int y = 0; y < 3; y++) {
          // if we're not in bounds it's not solid
          if (y >= shapeHeight) {
            break;
          } else if (itemType.Shape[x, y] != Item.Type.ShapeBlocks.Empty) {
            solidBlockFlags = solidBlockFlags.TurnBitOn(++blockIndex);
          }
        }
      }
    }

    return solidBlockFlags;
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
    DrawPropertiesExcluding(serializedObject, new string[] { "ItemDataMaps", "m_Script", "ShapedItemSpriteBackgroundDataMaps" });
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
    EditorList.Show(
      serializedObject.FindProperty("ShapedItemSpriteBackgroundDataMaps"),
      EditorListOption.ButtonsBelow | EditorListOption.ListLabel | EditorListOption.Buttons | EditorListOption.ElementLabels
    );
    serializedObject.ApplyModifiedProperties();
  }
}

[System.Serializable]
public class ShapedItemSpriteBackgroundDataMap {

  /// <summary>
  /// The shape map (8 = 2,2)
  /// 6 7 8
  /// 3 4 5
  /// 0 1 2
  /// </summary>
  [SerializeField]
  public BoolMap ShapeMap 
    = new BoolMap(3, 3);

  /// <summary>
  /// The shaped bg sprite.
  /// </summary>
  [SerializeField]
  public Sprite ShapedBackgroundSprite;
}

[System.Serializable]
public class BoolMap {

  /// <summary>
  /// Map
  /// </summary>
  [SerializeField]
  public BoolRow[] Map;

  /// <summary>
  /// Make a new empty map
  /// </summary>
  /// <param name="width"></param>
  /// <param name="height"></param>
  public BoolMap(int width, int height) {
    Map = new BoolRow[height];
    for(int x = 0; x < width; x++) {
      Map[x] = new BoolRow(width);
    }
  }
}

[System.Serializable]
public class BoolRow {

  /// <summary>
  /// Map
  /// </summary>
  [SerializeField]
  public bool[] Map;

  /// <summary>
  /// Make a new empty map
  /// </summary>
  /// <param name="width"></param>
  /// <param name="height"></param>
  public BoolRow(int width) {
    Map = new bool[width];
  }
}

/// <summary>
/// custom inspector gui for item type
/// </summary>
[CustomPropertyDrawer(typeof(BoolMap))]
public class BoolMapPropertyDrawer : PropertyDrawer {

  /// <summary>
  /// Checbox size
  /// </summary>
  static Rect CheckboxSize 
    = new Rect(0, 0, 50, 50);

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    SerializedProperty boolRows = property.FindPropertyRelative("Map");
    int width = boolRows?.arraySize ?? 0;
    if (width > 0) {
      int height = boolRows.GetArrayElementAtIndex(0).FindPropertyRelative("Map").arraySize;
      //----------------------------------
      EditorGUI.BeginProperty(position, label, property);
      GUILayout.ExpandWidth(false);
      // go though each checkbox and get and set
      for (int y = height -= 1; y >= 0; y--) {
        SerializedProperty boolRow = boolRows.GetArrayElementAtIndex(y).FindPropertyRelative("Map");
        EditorGUILayout.BeginHorizontal();
        for (int x = 0; x < width; x++) {
          bool currentBoolValue = boolRow.GetArrayElementAtIndex(x).boolValue;
          boolRow.GetArrayElementAtIndex(x).boolValue
            = EditorGUILayout.Toggle($"({x},{y})", currentBoolValue, GUILayout.Width(25));
        }
        EditorGUILayout.EndHorizontal();
      }

      GUILayout.ExpandWidth(true);
      //----------------------------------
      EditorGUI.EndProperty();
    }

  }
}