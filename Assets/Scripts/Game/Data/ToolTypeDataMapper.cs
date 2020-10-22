using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ToolTypeDataMapper : MonoBehaviour {

  /// <summary>
  /// A map of models used for tile features organized like so
  /// </summary>
  [SerializeField]
  public ToolTypeDataMap[] ToolTypeDataMaps;

  /// <summary>
  /// const for item models by id
  /// </summary>
  public static Dictionary<SpiritWorld.Inventories.Items.Tool.Type, ToolTypeDataMap> ToolData
    = new Dictionary<SpiritWorld.Inventories.Items.Tool.Type, ToolTypeDataMap>();


  // Start is called before the first frame update
  void Awake() {
    // set up the map
    foreach (ToolTypeDataMap toolDataMap in ToolTypeDataMaps) {
      ToolData.Add(toolDataMap.ToolType, toolDataMap);
    }
  }

  /// <summary>
  /// Get a model for an item
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  public static Sprite GetIconFor(SpiritWorld.Inventories.Items.Tool.Type type) {
    return ToolData[type]?.Icon;
  }
}

/// <summary>
/// A map representing the models for one type of terrain feature sorted by how many usages it has left
/// </summary>
[System.Serializable]
public class ToolTypeDataMap {

  /// <summary>
  /// the id of the tile feature this is for
  /// </summary>
  [SerializeField]
  public SpiritWorld.Inventories.Items.Tool.Type ToolType;

  /// <summary>
  /// The sprite to use for this type of tool
  /// </summary>
  [SerializeField]
  public Sprite Icon;
}

/// <summary>
/// Custom array drawer
/// </summary>
[CustomEditor(typeof(ToolTypeDataMapper))]
public class ToolTypeDataMapPropertyDrawer : Editor {
  public override void OnInspectorGUI() {
    serializedObject.Update();
    EditorList.Show(
      serializedObject.FindProperty("ToolTypeDataMaps"),
      EditorListOption.ButtonsBelow | EditorListOption.ListLabel | EditorListOption.Buttons | EditorListOption.ElementLabels,
      (element, _) => {
        int? enumId = element.FindPropertyRelative("ToolType")?.intValue;
        return Enum.GetName(typeof(SpiritWorld.Inventories.Items.Tool.Type), enumId);
      }
    );
    serializedObject.ApplyModifiedProperties();
  }
}