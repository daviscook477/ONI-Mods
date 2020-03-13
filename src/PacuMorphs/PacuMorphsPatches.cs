using System;
using Harmony;

namespace PacuMorphs
{
    public class PacuMorphsPatches
    {
        [HarmonyPatch(typeof(EntityConfigManager))]
        [HarmonyPatch(nameof(EntityConfigManager.LoadGeneratedEntities))]
        public class EntityConfigManager_LoadGeneratedEntities_Patch
        {
            public static void Prefix()
            {
                Strings.Add($"STRINGS.CODEX.PACUALGAE.TITLE", "Kelp Pacu");
                Strings.Add($"STRINGS.CODEX.PACUALGAE.SUBTITLE", "Critter Morph");

                Strings.Add($"STRINGS.CODEX.PACUBETA.TITLE", "Beta Fish");
                Strings.Add($"STRINGS.CODEX.PACUBETA.SUBTITLE", "Critter Morph");

                Strings.Add($"STRINGS.CODEX.PACUPLATE.TITLE", "Plated Pacu");
                Strings.Add($"STRINGS.CODEX.PACUPLATE.SUBTITLE", "Critter Morph");
            }
        }

        public static readonly Type[] CREATE_TEMPERATURE_MODIFIER_METHOD_TYPES = new[] { typeof(string), typeof(Tag), typeof(float), typeof(float), typeof(float), typeof(bool) };

        public static void OnLoad()
        {
            TUNING.CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                Traverse.Create(typeof(TUNING.CREATURES.EGG_CHANCE_MODIFIERS))
                .Method("CreateTemperatureModifier", CREATE_TEMPERATURE_MODIFIER_METHOD_TYPES)
                .GetValue<System.Action>(
                    BetaPacuConfig.ID,
                    BetaPacuConfig.EGG_ID.ToTag(),
                    BetaPacuConfig.MIN_TEMP,
                    BetaPacuConfig.MAX_TEMP,
                    CustomPacuTuning.MODIFIER_PER_SECOND, false));

            PacuTuning.EGG_CHANCES_BASE.Add(
                new FertilityMonitor.BreedingChance
                {
                    egg = BetaPacuConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                });

            TUNING.CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                Traverse.Create(typeof(TUNING.CREATURES.EGG_CHANCE_MODIFIERS))
                .Method("CreateTemperatureModifier", CREATE_TEMPERATURE_MODIFIER_METHOD_TYPES)
                .GetValue<System.Action>(
                    AlgaePacuConfig.ID,
                    AlgaePacuConfig.EGG_ID.ToTag(),
                    AlgaePacuConfig.MIN_TEMP,
                    AlgaePacuConfig.MAX_TEMP,
                    CustomPacuTuning.MODIFIER_PER_SECOND, false));

            PacuTuning.EGG_CHANCES_TROPICAL.Add(
                new FertilityMonitor.BreedingChance
                {
                    egg = AlgaePacuConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                });

            TUNING.CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                Traverse.Create(typeof(TUNING.CREATURES.EGG_CHANCE_MODIFIERS))
                .Method("CreateTemperatureModifier", CREATE_TEMPERATURE_MODIFIER_METHOD_TYPES)
                .GetValue<System.Action>(
                    PlatePacuConfig.ID,
                    PlatePacuConfig.EGG_ID.ToTag(),
                    PlatePacuConfig.MIN_TEMP,
                    PlatePacuConfig.MAX_TEMP,
                    CustomPacuTuning.MODIFIER_PER_SECOND, false));

            PacuTuning.EGG_CHANCES_CLEANER.Add(
                new FertilityMonitor.BreedingChance
                {
                    egg = PlatePacuConfig.EGG_ID.ToTag(),
                    weight = 0.02f
                });
        }
    }
}
