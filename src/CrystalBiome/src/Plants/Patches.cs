using Harmony;

using TUNING;

namespace CrystalBiome.Plants
{
    public class Patches
    {
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
