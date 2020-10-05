namespace SpiritWorld.Events {
  public interface IRecorded {

    /// <summary>
    /// Record an event
    /// </summary>
    /// <param name="event"></param>
    void recordEvent(string @event);

    /// <summary>
    /// Get the last recorded events
    /// </summary>
    /// <param name="lastX">specify how many of the most recent events to grab</param>
    (string timestamp, string @event)[] getRecordedEvents(int? lastX = null);
  }
}
