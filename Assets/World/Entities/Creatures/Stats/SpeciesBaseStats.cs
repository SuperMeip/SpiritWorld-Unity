using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritWorld.World.Entities.Creatures.Stats {
  public class SpeciesBaseStats : BaseStatCollection {

    /// <summary>
    /// Make a block of basic stats
    /// </summary>
    public SpeciesBaseStats(
      (int value, int varriance) HP,
      (int value, int varriance) EP,
      (int value, int varriance) SP,
      (int value, int varriance) STR,
      (int value, int varriance) DEF,
      (int value, int varriance) ATN,
      (int value, int varriance) FOR
    ) {
      /// depletable stats
      this[Stat.Types.HP] = new VariantStat(Stat.Types.HP, HP.value, HP.varriance);
      this[Stat.Types.Energy] = new VariantStat(Stat.Types.Energy, EP.value, EP.varriance);
      this[Stat.Types.Stamina] = new VariantStat(Stat.Types.Stamina, SP.value, SP.varriance);

      /// mostly set in stone stats
      this[Stat.Types.Strength] = new VariantStat(Stat.Types.Strength, STR.value, STR.varriance);
      this[Stat.Types.Defence] = new VariantStat(Stat.Types.Defence, DEF.value, DEF.varriance);
      this[Stat.Types.Attunement] = new VariantStat(Stat.Types.Attunement, ATN.value, ATN.varriance);
      this[Stat.Types.Fortitude] = new VariantStat(Stat.Types.Fortitude, FOR.value, FOR.varriance);
    }
  }
}
