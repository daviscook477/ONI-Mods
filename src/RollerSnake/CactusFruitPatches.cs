using System.Linq;
using TUNING;
using Harmony;
using STRINGS;
using UnityEngine;

namespace CactusFruit
{
    public class CactusFruitPatches
    {
        public const float CyclesForGrowth = 4f;

        [HarmonyPatch(typeof(EntityConfigManager), "LoadGeneratedEntities")]
        public class EntityConfigManager_LoadGeneratedEntities_Patch
        {
            private static void Prefix()
            {
                // cactus fruit flesh
                Strings.Add($"STRINGS.ITEMS.FOOD.{CactusFleshConfig.Id.ToUpperInvariant()}.NAME", CactusFleshConfig.Name);
                Strings.Add($"STRINGS.ITEMS.FOOD.{CactusFleshConfig.Id.ToUpperInvariant()}.DESC", CactusFleshConfig.Description);

                // cactus fruit flesh grilled
                Strings.Add($"STRINGS.ITEMS.FOOD.{CactusFleshGrilledConfig.Id.ToUpperInvariant()}.NAME", CactusFleshGrilledConfig.Name);
                Strings.Add($"STRINGS.ITEMS.FOOD.{CactusFleshGrilledConfig.Id.ToUpperInvariant()}.DESC", CactusFleshGrilledConfig.Description);
                Strings.Add($"STRINGS.ITEMS.FOOD.{CactusFleshGrilledConfig.Id.ToUpperInvariant()}.RECIPEDESC", CactusFleshGrilledConfig.RecipeDescription);

                // cactus fruit flower
                Strings.Add($"STRINGS.ITEMS.FOOD.{CactusFlowerConfig.Id.ToUpperInvariant()}.NAME", CactusFlowerConfig.Name);
                Strings.Add($"STRINGS.ITEMS.FOOD.{CactusFlowerConfig.Id.ToUpperInvariant()}.DESC", CactusFlowerConfig.Description);

                // cactus fruit flower salad
                Strings.Add($"STRINGS.ITEMS.FOOD.{CactusFlowerSaladConfig.Id.ToUpperInvariant()}.NAME", CactusFlowerSaladConfig.Name);
                Strings.Add($"STRINGS.ITEMS.FOOD.{CactusFlowerSaladConfig.Id.ToUpperInvariant()}.DESC", CactusFlowerSaladConfig.Description);
                Strings.Add($"STRINGS.ITEMS.FOOD.{CactusFlowerSaladConfig.Id.ToUpperInvariant()}.RECIPEDESC", CactusFlowerSaladConfig.RecipeDescription);

                // cactus fruit plant seed
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{CactusFruitConfig.SeedId.ToUpperInvariant()}.NAME", CactusFruitConfig.SeedName);
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{CactusFruitConfig.SeedId.ToUpperInvariant()}.DESC", CactusFruitConfig.SeedDescription);

                // cactus fruit plant
                Strings.Add($"STRINGS.CREATURES.SPECIES.{CactusFruitConfig.Id.ToUpperInvariant()}.NAME", CactusFruitConfig.Name);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{CactusFruitConfig.Id.ToUpperInvariant()}.DESC", CactusFruitConfig.Description);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{CactusFruitConfig.Id.ToUpperInvariant()}.DOMESTICATEDDESC", CactusFruitConfig.DomesticatedDescription);
                CROPS.CROP_TYPES.Add(new Crop.CropVal(CactusFleshConfig.Id, CyclesForGrowth * 600.0f, 1));
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

                list.Add(new CarePackageInfo(CactusFruitConfig.SeedId, 1f, () => true));

                field.SetValue(list.ToArray());
            }
        }

        [HarmonyPatch(typeof(Crop), "SpawnFruit")]
        public class Crop_SpawnFruit_Patch
        {
            private static void Postfix(Crop __instance)
            {
                if (__instance == null)
                    return;
                Crop.CropVal cropVal = __instance.cropVal;
                if (string.IsNullOrEmpty(cropVal.cropId))
                    return;
                if (cropVal.cropId != CactusFleshConfig.Id)
                    return;
                GameObject gameObject = Scenario.SpawnPrefab(Grid.PosToCell(__instance.gameObject), 0, 0, CactusFlowerConfig.Id, Grid.SceneLayer.Ore);
                if (gameObject != null)
                {
                    float y = 0.75f;
                    gameObject.transform.SetPosition(gameObject.transform.GetPosition() + new Vector3(0.0f, y, 0.0f));
                    gameObject.SetActive(true);
                    PrimaryElement component1 = gameObject.GetComponent<PrimaryElement>();
                    component1.Units = 1.0f; // cactus produces 1 unit of flower to go with 5 units of flesh
                    component1.Temperature = __instance.gameObject.GetComponent<PrimaryElement>().Temperature;
                    Edible component2 = gameObject.GetComponent<Edible>();
                    if ((bool)((UnityEngine.Object)component2))
                        ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component2.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.HARVESTED, "{0}", component2.GetProperName()), UI.ENDOFDAYREPORT.NOTES.HARVESTED_CONTEXT);
                }
                else
                    DebugUtil.LogErrorArgs(__instance.gameObject, "tried to spawn an invalid crop prefab:", CactusFlowerConfig.Id);
                __instance.Trigger(-1072826864, null);
            }
        }
    }
}
