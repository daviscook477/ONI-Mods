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
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{CrystalPlantConfig.Id.ToUpperInvariant()}.NAME", CrystalPlantConfig.SeedName);
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{CrystalPlantConfig.Id.ToUpperInvariant()}.DESC", CrystalPlantConfig.SeedDescription);

                Strings.Add($"STRINGS.CREATURES.SPECIES.{CrystalPlantConfig.Id.ToUpperInvariant()}.NAME", CrystalPlantConfig.Name);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{CrystalPlantConfig.Id.ToUpperInvariant()}.DESC", CrystalPlantConfig.Description);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{CrystalPlantConfig.Id.ToUpperInvariant()}.DOMESTICATEDDESC", CrystalPlantConfig.DomesticatedDescription);

                CROPS.CROP_TYPES.Add(new Crop.CropVal(CrystalPlantConfig.SeedId, CyclesForGrowth * 600.0f, 12));
            }
        }
    }
}
