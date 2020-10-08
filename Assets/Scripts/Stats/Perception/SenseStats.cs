namespace SpiritWorld.Stats {
  public class SenseStats : StatSheet {

    /// <summary>
    /// Sensory stats, for perception and detection
    /// </summary>
    public SenseStats(
      int SR = 7,
      int VA = 5,
      int SM = 5,
      int HR = 12,
      int NS = 5,
      int EX = 2
    ) {
      this[Stat.Types.SightRange] = new Stat(Stat.Types.SightRange, SR);
      this[Stat.Types.VisionAccuracy] = new Stat(Stat.Types.VisionAccuracy, VA);
      this[Stat.Types.Smell] = new Stat(Stat.Types.Smell, SM);
      this[Stat.Types.HearingRadius] = new Stat(Stat.Types.HearingRadius, HR);
      this[Stat.Types.NoiseSensitivity] = new Stat(Stat.Types.NoiseSensitivity, NS);
      this[Stat.Types.ExtraSensoryPerception] = new Stat(Stat.Types.ExtraSensoryPerception, EX);
    }
  }
}
