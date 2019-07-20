using System.Collections.Generic;
using Database;
using Harmony;
using TUNING;

namespace MagmaFern
{
    public class MagmaFernPatches
    {

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{HardenedFarmTileConfig.Id.ToUpperInvariant()}.NAME", HardenedFarmTileConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{HardenedFarmTileConfig.Id.ToUpperInvariant()}.DESC", HardenedFarmTileConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{HardenedFarmTileConfig.Id.ToUpperInvariant()}.EFFECT", HardenedFarmTileConfig.Effect);

                ModUtil.AddBuildingToPlanScreen("Food", HardenedFarmTileConfig.Id);
            }
        }

        [HarmonyPatch(typeof(Immigration))]
        [HarmonyPatch("ConfigureCarePackages")]
        public class Immigration_ConfigureCarePackages_Patch
        {
            private static void Postfix(ref CarePackageInfo[] ___carePackages)
            {
                var carePackages = new List<CarePackageInfo>(___carePackages);
                carePackages.Add(new CarePackageInfo(MagmaFernConfig.SeedId, 3f, (() => GameClock.Instance.GetCycle() >= 48)));
                ___carePackages = carePackages.ToArray();
            }

        }

        /*[HarmonyPatch(typeof(SettingsCache))]
        [HarmonyPatch("LoadSubworlds")]
        public class SettingsCache_LoadSubworlds_Patch
        {
            private static void PopulateSubworld(string key)
            {
                if (SettingsCache.subworlds.ContainsKey(key))
                {
                    foreach (WeightedBiome biome in SettingsCache.subworlds[key].biomes)
                    {
                        if (!biome.tags.Contains(MagmaFernConfig.Id))
                        {
                            biome.tags.Add(MagmaFernConfig.Id);
                        }
                    }
                }
            }

            private static void Postfix()
            {
                PopulateSubworld("subworlds\\magma\\VolcanoBiome");
                PopulateSubworld("subworlds\\magma\\Bottom");
            }
        }

        [HarmonyPatch(typeof(SettingsCache))]
        [HarmonyPatch("LoadFiles")]
        public class SettingsCache_LoadFiles_Patch
        {
            private static void Postfix()
            {
                if (!SettingsCache.mobs.MobLookupTable.ContainsKey(MagmaFernConfig.Id))
                {
                    Mob mob = new Mob(Mob.Location.AnyFloor);
                    Traverse.Create(mob).Field("selectMethod").SetValue(SampleDescriber.PointSelectionMethod.Centroid);
                    Traverse.Create(mob).Field("density").SetValue(new MinMax(0.16f, 0.25f));
                    Traverse.Create(mob).Field("width").SetValue(1);
                    Traverse.Create(mob).Field("height").SetValue(1);
                    SettingsCache.mobs.MobLookupTable[MagmaFernConfig.Id] = mob;
                }
            }
        }*/

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            private static void Prefix()
            {
                var fineDining = new List<string>(Techs.TECH_GROUPING["FinerDining"]) { HardenedFarmTileConfig.Id };
                Techs.TECH_GROUPING["FinerDining"] = fineDining.ToArray();
            }

            private static void Postfix(Db __instance)
            {
                if (__instance.SpaceDestinationTypes.VolcanoPlanet.recoverableEntities == null)
                {
                    __instance.SpaceDestinationTypes.VolcanoPlanet.recoverableEntities = new Dictionary<string, int>();
                }
                __instance.SpaceDestinationTypes.VolcanoPlanet.recoverableEntities.Add(MagmaFernConfig.SeedId, 3);
            }
        }

        public const int CyclesForGrowth = 24;

        [HarmonyPatch(typeof(EntityConfigManager))]
        [HarmonyPatch("LoadGeneratedEntities")]
        public class EntityConfigManager_LoadGeneratedEntities_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{MagmaFernConfig.Id.ToUpperInvariant()}.NAME", MagmaFernConfig.SeedName);
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{MagmaFernConfig.Id.ToUpperInvariant()}.DESC", MagmaFernConfig.SeedDescription);

                Strings.Add($"STRINGS.CREATURES.SPECIES.{MagmaFernConfig.Id.ToUpperInvariant()}.NAME", MagmaFernConfig.Name);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{MagmaFernConfig.Id.ToUpperInvariant()}.DESC", MagmaFernConfig.Description);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{MagmaFernConfig.Id.ToUpperInvariant()}.DOMESTICATEDDESC", MagmaFernConfig.DomesticatedDescription);

                Strings.Add($"STRINGS.ITEMS.FOOD.{MagmaFernConfig.Id.ToUpperInvariant()}.NAME", MagmaFernConfig.SeedName);
                Strings.Add($"STRINGS.ITEMS.FOOD.{MagmaFernConfig.Id.ToUpperInvariant()}.DESC", MagmaFernConfig.SeedDescription);

                Strings.Add($"STRINGS.ITEMS.FOOD.{LavaCakeConfig.Id.ToUpperInvariant()}.NAME", LavaCakeConfig.Name);
                Strings.Add($"STRINGS.ITEMS.FOOD.{LavaCakeConfig.Id.ToUpperInvariant()}.DESC", LavaCakeConfig.Description);
                Strings.Add($"STRINGS.ITEMS.FOOD.{LavaCakeConfig.Id.ToUpperInvariant()}.RECIPEDESC", LavaCakeConfig.RecipeDescription);


                CROPS.CROP_TYPES.Add(new Crop.CropVal(MagmaFernConfig.SeedId, CyclesForGrowth * 600.0f, 12));
            }
        }

    }
}
