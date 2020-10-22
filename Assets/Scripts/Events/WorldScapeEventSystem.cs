namespace SpiritWorld.Events {
  public class WorldScapeEventSystem : EventSystem<WorldScapeEventSystem.Channels> {
    public enum Channels {Basic, TileUpdates, LocalPlayerUpdates };
  }
}
