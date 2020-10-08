namespace SpiritWorld.Inventories.Items {

  /// <summary>
  /// Interface for tools.
  /// </summary>
  public interface ITool : IUpgradeable, IUseableItem {

    /// <summary>
    /// What kind of tool this is
    /// </summary>
    Tool.Type ToolType {
      get;
    }
  }
}
