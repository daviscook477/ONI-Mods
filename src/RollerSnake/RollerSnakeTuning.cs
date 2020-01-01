using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;

namespace RollerSnake
{
    public class RollerSnakeTuning
    {
        public static float STANDARD_CALORIES_PER_CYCLE = 700000f;
        public static float STANDARD_STARVE_CYCLES = 10f;
        public static float STANDARD_STOMACH_SIZE = STANDARD_CALORIES_PER_CYCLE * STANDARD_STARVE_CYCLES;
        public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER3;
        public static float EGG_MASS = 1f;
    }
}
