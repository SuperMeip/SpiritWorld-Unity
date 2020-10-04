using SpiritWorld.Managers;
using SpiritWorld.World;
using SpiritWorld.World.Terrain.TileGrid;

public class Universe {
  public static WorldScape CurrentScape;
  public static BoardManager ActiveBoardManager;
  public static int CurrentBoardId = 0;
  public const float HexRadius = 1;
  public const float StepHeight = 1.0f / 6.0f;
}
