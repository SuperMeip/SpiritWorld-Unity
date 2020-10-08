namespace SpiritWorld.Inventories.Items {

  /// <summary>
  /// An interface for item types 
  /// </summary>
  public interface IUpgradeable {

    /// <summary>
    /// What level of the upgrade for the type of item this is
    /// </summary>
    int UpgradeLevel {
      get;
    }

    /// <summary>
    /// What level of the upgrade for the type of item this is
    /// </summary>
    string UpgradeLevelName {
      get;
    }
  }
}
