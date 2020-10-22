using System.Collections;

namespace SpiritWorld.Inventories.Items {

  /// <summary>
  /// Just tool constants idk
  /// </summary>
  public static class Tool {

    /// <summary>
    /// Types of tools that are useable
    /// </summary>
    public enum Type {
      None = -1,
      Any,
      Axe,
      Shovel,
      Pickaxe,
      Multi = 100
    }

    /// <summary>
    /// A small struct used to store tool requirements for harvesting or mining
    /// </summary>
    public struct Requirement {
      /// <summary>
      /// The type of tool needed
      /// </summary>
      public Type ToolType {
        get;
      }

      /// <summary>
      /// The minimum level of the tool needed
      /// </summary>
      public int MinimumLevel {
        get;
      }

      /// <summary>
      /// Make a new requirement
      /// </summary>
      /// <param name="type"></param>
      /// <param name="minlevel"></param>
      public Requirement(Type type, int minlevel = 0) {
        ToolType = type;
        MinimumLevel = minlevel;
      }
    }
  }
}
