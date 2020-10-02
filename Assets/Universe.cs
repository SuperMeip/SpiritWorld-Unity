using SpiritWorld.World.Terrain.TileGrid;

public class Universe {
  public static SpiritWorld.World.WorldScape CurrentScape;
  public static int CurrentBoardId = 0;
  public const float HexRadius = 1;
  public const float StepHeight = 1.0f / 6.0f;

  /// <summary>
  /// Get the active tile board of the world
  /// </summary>
  /// <returns></returns>
  public static TileBoard GetActiveBoard() {
    return CurrentScape.getBoard(CurrentBoardId);
  }
}
