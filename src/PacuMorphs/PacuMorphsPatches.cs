using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace PacuMorphs
{
    public class PacuMorphsPatches
    {
        public static void OnLoad()
        {
            TUNING.CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                Traverse.Create(typeof(TUNING.CREATURES.EGG_CHANCE_MODIFIERS))
                .Method("CreateTemperatureModifier", new[] { typeof(string), typeof(Tag), typeof(float), typeof(float), typeof(float), typeof(bool) })
                .GetValue<System.Action>(
                    BetaPacuConfig.ID,
                    BetaPacuConfig.EGG_ID.ToTag(),
                    328.15f,
                    353.15f,
                    CustomPacuTuning.MODIFIER_PER_SECOND, false));

            PacuTuning.EGG_CHANCES_BASE.Add(
                new FertilityMonitor.BreedingChance
                {
                    egg = BetaPacuConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                });
            PacuTuning.EGG_CHANCES_TROPICAL.Add(
                new FertilityMonitor.BreedingChance
                {
                    egg = BetaPacuConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                });
            PacuTuning.EGG_CHANCES_CLEANER.Add(
                new FertilityMonitor.BreedingChance
                {
                    egg = BetaPacuConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                });
        }
    }
}
