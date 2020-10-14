using SpiritWorld.World.Terrain.Features;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A data map that moves all the models for the tile features into a static collection on awake
/// </summary>
public class TileFeatureDataMaper : MonoBehaviour {

  /// <summary>
  /// A map of models used for tile features organized like so
  /// </summary>
  [SerializeField]
  public TileFeatureDataMap[] TileFeatureDataMaps;

  /// <summary>
  /// const for item models by id
  /// </summary>
  public static Dictionary<int, TileFeatureDataMap> TileFeatures
    = new Dictionary<int, TileFeatureDataMap>();


  // Start is called before the first frame update
  void Awake() {
    // set up the map
    foreach (TileFeatureDataMap tileFeatureDataMap in TileFeatureDataMaps) {
      if (tileFeatureDataMap.TileFeatureType.tileFeatureId != 0) {
        TileFeatures.Add(tileFeatureDataMap.TileFeatureType.tileFeatureId, tileFeatureDataMap);
      }
    }
  }

  /// <summary>
  /// Get a model for an item
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  public static GameObject GetModelFor(TileFeature feature) {
    return TileFeatures[feature.type.Id].OverworldModelsByMode[Mathf.Max(feature.mode, 0)];
  }
}

/// <summary>
/// A map representing the models for one type of terrain feature sorted by how many usages it has left
/// </summary>
[System.Serializable]
public class TileFeatureDataMap {

  /// <summary>
  /// the id of the tile feature this is for
  /// </summary>
  [SerializeField]
  public TileFeatureType TileFeatureType
    = new TileFeatureType();

  /// <summary>
  /// The models to use sorted by how many usages remain
  /// 0 is used for unlimited use/non-interactable features as well
  /// </summary>
  [SerializeField]
  public GameObject[] OverworldModelsByMode;
}

/// <summary>
/// Custom inspector type for item type
/// </summary>
[System.Serializable]
public class TileFeatureType {
  [SerializeField]
  public int tileFeatureId = 0;
}

/// <summary>
/// custom inspector gui for item type
/// </summary>
[CustomPropertyDrawer(typeof(TileFeatureType))]
public class TileFeatureTypePropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);

    /// get the selected item
    int selectedtileFeatureId = property.FindPropertyRelative("tileFeatureId").intValue;

    // get the array values to display for item names and ids
    TileFeature.Type[] allFeatures = TileFeature.Types.All;
    List<string> names = allFeatures.Select(type => type.Name).ToList();
    names.Add("--");
    List<int> ids = allFeatures.Select(type => (int)type.Id).ToList();
    ids.Add(0);

    // display the dropdown
    property.FindPropertyRelative("tileFeatureId").intValue
      = EditorGUI.IntPopup(position, "Feature Type:", selectedtileFeatureId, names.ToArray(), ids.ToArray(), EditorStyles.popup);

    EditorGUI.EndProperty();
  }
}

/// <summary>
/// Custom array drawer
/// </summary>
[CustomEditor(typeof(TileFeatureDataMaper))]
public class TileFeatureDataMapPropertyDrawer : Editor {
  public override void OnInspectorGUI() {
    serializedObject.Update();
    EditorList.Show(
      serializedObject.FindProperty("TileFeatureDataMaps"),
      EditorListOption.ButtonsBelow | EditorListOption.ListLabel | EditorListOption.Buttons | EditorListOption.ElementLabels,
      (element, _) => {
        int? featureId = element.FindPropertyRelative("TileFeatureType").FindPropertyRelative("tileFeatureId")?.intValue;
        return featureId != 0 && featureId != null
          ? TileFeature.Types.Get((byte)featureId).Name
          : "No Feature Set";
      }
    );
    serializedObject.ApplyModifiedProperties();
  }
}
