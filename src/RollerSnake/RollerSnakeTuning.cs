using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;

namespace RollerSnake
{
    public class RollerSnakeTuning
    {
        public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>()
        {
            new FertilityMonitor.BreedingChance()
            {
                egg = RollerSnakeConfig.EggId.ToTag(),
                weight = 0.98f
            },
            new FertilityMonitor.BreedingChance()
            {
                egg = SteelRollerSnakeConfig.EggId.ToTag(),
                weight = 0.02f
            },
        };
        public static float STANDARD_CALORIES_PER_CYCLE = 700000f;
        public static float STANDARD_STARVE_CYCLES = 10f;
        public static float STANDARD_STOMACH_SIZE = STANDARD_CALORIES_PER_CYCLE * STANDARD_STARVE_CYCLES;
        public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER3;
        public static float EGG_MASS = 1f;
    }
}
