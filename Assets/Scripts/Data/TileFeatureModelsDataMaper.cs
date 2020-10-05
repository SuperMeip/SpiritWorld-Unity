using SpiritWorld.World.Terrain.Features;
using UnityEngine;

/// <summary>
/// A data map that moves all the models for the tile features into a static collection on awake
/// </summary>
public class TileFeatureModelsDataMaper : MonoBehaviour {

  /// <summary>
  /// A map of models used for tile features organized like so:
  /// feature_id => [
  ///   0 => empty model, or regular model for non interactive/usable features
  ///   1 => 1 usage left model
  ///   2 => 2 usages remaining model ... etc
  /// ],
  /// feature2_id => ...
  /// </summary>
  [SerializeField]
  public TileFeatureModelsDataMap[] TileFeatureModelsByIdAndUsagesRemaining;

  /// <summary>
  /// The stored feature map in a static variable for access
  /// </summary>
  public static TileFeatureModelsDataMap[] TileFeatureModelMapsById;

  // Start is called before the first frame update
  void Awake() {
    TileFeatureModelMapsById = TileFeatureModelsByIdAndUsagesRemaining;
  }

  /// <summary>
  /// Get the model to use from the given feature
  /// </summary>
  /// <param name="feature"></param>
  /// <returns></returns>
  public static GameObject GetModelForFeature(TileFeature feature) {
    return TileFeatureModelMapsById[feature.type.Id].ModelsByMode[Mathf.Max(feature.mode, 0)];
  }
}

/// <summary>
/// A map representing the models for one type of terrain feature sorted by how many usages it has left
/// </summary>
[System.Serializable]
public class TileFeatureModelsDataMap {

  /// <summary>
  /// The models to use sorted by how many usages remain
  /// 0 is used for unlimited use/non-interactable features as well
  /// </summary>
  [SerializeField]
  public GameObject[] ModelsByMode;
}
