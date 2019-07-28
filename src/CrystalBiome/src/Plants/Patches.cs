using System;
using System.Collections.Generic;

using Harmony;
using UnityEngine;

using TUNING;

namespace CrystalBiome.Plants
{
    public class Patches
    {
        public const float ConversionRate = 0.7f;
        public const float MinDumpAmount = 1.0f;

        [HarmonyPatch(typeof(PlantElementAbsorbers), nameof(PlantElementAbsorbers.Sim200ms))]
        public class PlantElementAbsorbers_Sim200ms_Patch
        {
            private static void Prefix(PlantElementAbsorbers __instance, float dt)
            {
                List<PlantElementAbsorber> data = Traverse.Create(__instance).Field("data").GetValue<List<PlantElementAbsorber>>();
                int count = data.Count;
                Traverse.Create(__instance).Field("updating").SetValue(true);
                for (int i = 0; i < count; ++i)
                {
                    PlantElementAbsorber plantElementAbsorber = data[i];
                    if (plantElementAbsorber.storage != null && plantElementAbsorber.consumedElements == null)
                    {
                        float a = plantElementAbsorber.localInfo.massConsumptionRate * dt;
                        PrimaryElement firstWithMass = plantElementAbsorber.storage.FindFirstWithMass(plantElementAbsorber.localInfo.tag);
                        if (firstWithMass != null)
                        {
                            if (firstWithMass.Element.tag.Equals(ElementLoader.FindElementByHash(Elements.MineralWaterElement.SimHash).tag))
                            {
                                float amount = Mathf.Min(a, firstWithMass.Mass);
                                float convertedAmount = amount * ConversionRate;
                                plantElementAbsorber.storage.AddLiquid(SimHashes.Water,
                                    convertedAmount, firstWithMass.Temperature, byte.MaxValue, 0);
                                float availableMass = plantElementAbsorber.storage.GetMassAvailable(SimHashes.Water);
                                Console.WriteLine(string.Format("checking available mass is {0}", availableMass));
                                if (availableMass >= MinDumpAmount)
                                {
                                    PrimaryElement element = plantElementAbsorber.storage.FindFirstWithMass(ElementLoader.FindElementByHash(SimHashes.Water).tag);
                                    Dumpable component = element.gameObject.GetComponent<Dumpable>();
                                    if (component == null)
                                    {
                                        DebugUtil.LogWarningArgs("Was unable to dump water converted from mineral water!");
                                        continue;
                                    }
                                    component.Dump(plantElementAbsorber.storage.transform.GetPosition());
                                }
                            }
                        }
                    }
                }
            }
        }

        public const float CyclesForGrowth = 0.1f;

        [HarmonyPatch(typeof(EntityConfigManager), "LoadGeneratedEntities")]
        public class EntityConfigManager_LoadGeneratedEntities_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{CrystalPlantFloorConfig.Id.ToUpperInvariant()}.NAME", CrystalPlantFloorConfig.SeedName);
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{CrystalPlantFloorConfig.Id.ToUpperInvariant()}.DESC", CrystalPlantFloorConfig.SeedDescription);
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{CrystalPlantCeilingConfig.Id.ToUpperInvariant()}.NAME", CrystalPlantCeilingConfig.SeedName);
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{CrystalPlantCeilingConfig.Id.ToUpperInvariant()}.DESC", CrystalPlantCeilingConfig.SeedDescription);

                Strings.Add($"STRINGS.CREATURES.SPECIES.{CrystalPlantFloorConfig.Id.ToUpperInvariant()}.NAME", CrystalPlantFloorConfig.Name);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{CrystalPlantFloorConfig.Id.ToUpperInvariant()}.DESC", CrystalPlantFloorConfig.Description);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{CrystalPlantFloorConfig.Id.ToUpperInvariant()}.DOMESTICATEDDESC", CrystalPlantFloorConfig.DomesticatedDescription);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{CrystalPlantCeilingConfig.Id.ToUpperInvariant()}.NAME", CrystalPlantCeilingConfig.Name);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{CrystalPlantCeilingConfig.Id.ToUpperInvariant()}.DESC", CrystalPlantCeilingConfig.Description);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{CrystalPlantCeilingConfig.Id.ToUpperInvariant()}.DOMESTICATEDDESC", CrystalPlantCeilingConfig.DomesticatedDescription);

                CROPS.CROP_TYPES.Add(new Crop.CropVal(Elements.CrystalElement.Id, CyclesForGrowth * 600.0f, 12));
                CROPS.CROP_TYPES.Add(new Crop.CropVal(Elements.CrystalElement.Id, CyclesForGrowth * 600.0f, 12));
            }
        }
    }
}
