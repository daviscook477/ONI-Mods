using System.Linq;
using System.Collections.Generic;
using TUNING;
using Harmony;
using STRINGS;
using UnityEngine;

namespace Champagne
{
    public class GrapeVinePatches
    {
        public const float CyclesForGrowth = 8f;

        [HarmonyPatch(typeof(EntityConfigManager), "LoadGeneratedEntities")]
        public class EntityConfigManager_LoadGeneratedEntities_Patch
        {
            public static void Prefix()
            {
                // cactus fruit flesh
                Strings.Add($"STRINGS.ITEMS.FOOD.{GrapeberryConfig.Id.ToUpperInvariant()}.NAME", GrapeberryConfig.Name);
                Strings.Add($"STRINGS.ITEMS.FOOD.{GrapeberryConfig.Id.ToUpperInvariant()}.DESC", GrapeberryConfig.Description);

                // cactus fruit plant seed
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{GrapeVineConfig.SeedId.ToUpperInvariant()}.NAME", GrapeVineConfig.SeedName);
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{GrapeVineConfig.SeedId.ToUpperInvariant()}.DESC", GrapeVineConfig.SeedDescription);

                // cactus fruit plant
                Strings.Add($"STRINGS.CREATURES.SPECIES.{GrapeVineConfig.Id.ToUpperInvariant()}.NAME", GrapeVineConfig.Name);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{GrapeVineConfig.Id.ToUpperInvariant()}.DESC", GrapeVineConfig.Description);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{GrapeVineConfig.Id.ToUpperInvariant()}.DOMESTICATEDDESC", GrapeVineConfig.DomesticatedDescription);
                CROPS.CROP_TYPES.Add(new Crop.CropVal(GrapeberryConfig.Id, CyclesForGrowth * 600.0f, 1));

                // codex info
                Strings.Add($"STRINGS.CODEX.GRAPEVINE.TITLE", "Grapeberry Vine");
                Strings.Add($"STRINGS.CODEX.GRAPEVINE.SUBTITLE", "Edible Plant");
                Strings.Add($"STRINGS.CODEX.GRAPEVINE.BODY.CONTAINER1", "Capable of growing in a gaseous environment, the Grapeberry Vine draws copious amounts of nutrients from the ground in order to produce such large and juicy berries.\n\nThe berries produces by the Grapberry Vine are coveted by Duplicants for their excellent taste. Unfortunately the berries do not ripen until the vine has grown quite tall.");
            }
        }

        [HarmonyPatch(typeof(Immigration))]
        [HarmonyPatch("ConfigureCarePackages")]
        public static class Immigration_ConfigureCarePackages_Patch
        {
            public static void Postfix(ref Immigration __instance)
            {
                var field = Traverse.Create(__instance).Field("carePackages");
                var list = field.GetValue<CarePackageInfo[]>().ToList();

                list.Add(new CarePackageInfo(GrapeVineConfig.SeedId, 1f, () => true));

                field.SetValue(list.ToArray());
            }
        }
    }
}
