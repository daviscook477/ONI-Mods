using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using Harmony;

namespace CactusFruit
{
    public class CactusFruitPatch
    {
        public const float CyclesForGrowth = 0.1f;

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


                // cactus fruit plant
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{CactusFruitConfig.SeedId.ToUpperInvariant()}.NAME", CactusFruitConfig.SeedName);
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{CactusFruitConfig.SeedId.ToUpperInvariant()}.DESC", CactusFruitConfig.SeedDescription);

                Strings.Add($"STRINGS.CREATURES.SPECIES.{CactusFruitConfig.Id.ToUpperInvariant()}.NAME", CactusFruitConfig.Name);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{CactusFruitConfig.Id.ToUpperInvariant()}.DESC", CactusFruitConfig.Description);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{CactusFruitConfig.Id.ToUpperInvariant()}.DOMESTICATEDDESC", CactusFruitConfig.DomesticatedDescription);

                CROPS.CROP_TYPES.Add(new Crop.CropVal(CactusFleshConfig.Id, CyclesForGrowth * 600.0f, 5));
            }
        }
    }
}
