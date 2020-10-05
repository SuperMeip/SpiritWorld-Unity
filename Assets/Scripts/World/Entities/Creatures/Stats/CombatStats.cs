namespace SpiritWorld.World.Entities.Creatures.Stats {
  /// <summary>
  /// A sheet of basic stats
  /// </summary>
  public class CombatStats : StatSheet {

    /// <summary>
    /// Make a block of basic stats
    /// </summary>
    public CombatStats(
      int HP = 10,
      int EP = 5,
      int SP = 5,
      int STR = 3,
      int DEF = 3,
      int ATN = 3,
      int FOR = 3,
      int INT = 3,
      int SKL = 3
    ) {
      /// depletable stats
      this[Stat.Types.HP]         = new Stat(Stat.Types.HP, HP);
      this[Stat.Types.Energy]     = new Stat(Stat.Types.Energy, EP);
      this[Stat.Types.Stamina]    = new Stat(Stat.Types.Stamina, SP);

      /// mostly set in stone stats
      this[Stat.Types.Strength]     = new Stat(Stat.Types.Strength, STR);
      this[Stat.Types.Defence]      = new Stat(Stat.Types.Defence, DEF);
      this[Stat.Types.Attunement]   = new Stat(Stat.Types.Attunement, ATN);
      this[Stat.Types.Fortitude]    = new Stat(Stat.Types.Fortitude, FOR);
      this[Stat.Types.Intellegence] = new Stat(Stat.Types.Intellegence, INT);
      this[Stat.Types.Skill]        = new Stat(Stat.Types.Skill, SKL);
    }
  }
}
