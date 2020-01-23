using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacuMorphs
{
    public static class CustomPacuTuning
    {
        public static float MODIFIER_PER_SECOND = 8.333333E-05f;

        public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BETA = new List<FertilityMonitor.BreedingChance>()
        {
            new FertilityMonitor.BreedingChance()
            {
                egg = "PacuEgg".ToTag(),
                weight = 0.32f
            },
            new FertilityMonitor.BreedingChance()
            {
                egg = BetaPacuConfig.EGG_ID.ToTag(),
                weight = 0.65f
            },
            new FertilityMonitor.BreedingChance()
            {
                egg = "PacuTropicalEgg".ToTag(),
                weight = 0.02f
            },
            new FertilityMonitor.BreedingChance()
            {
                egg = "PacuCleanerEgg".ToTag(),
                weight = 0.02f
            }
        };
    }
}
