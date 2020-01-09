using System;
using System.Collections.Generic;
using System.Linq;
using TUNING;
using Harmony;
using STRINGS;
using UnityEngine;

namespace AquaBulb
{
    public class AquaBulbPatches
    {
        [HarmonyPatch(typeof(EntityConfigManager), "LoadGeneratedEntities")]
        public class EntityConfigManager_LoadGeneratedEntities_Patch
        {
            public static void Prefix()
            {
                // aqua bulb sack
                Strings.Add($"STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.{AquaBulbSackConfig.Id.ToUpperInvariant()}.NAME", AquaBulbSackConfig.Name);
                Strings.Add($"STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.{AquaBulbSackConfig.Id.ToUpperInvariant()}.DESC", AquaBulbSackConfig.Description);

                // aqua bulb plant
                Strings.Add($"STRINGS.CREATURES.SPECIES.{AquaBulbConfig.Id.ToUpperInvariant()}.NAME", AquaBulbConfig.Name);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{AquaBulbConfig.Id.ToUpperInvariant()}.DESC", AquaBulbConfig.Description);
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

                list.Add(new CarePackageInfo(AquaBulbSackConfig.Id, 6f, () => true));

                field.SetValue(list.ToArray());
            }
        }

        [HarmonyPatch(typeof(RockCrusherConfig), nameof(RockCrusherConfig.ConfigureBuildingTemplate))]
        public static class RockCrusherConfig_ConfigureBuildingTemplate_Patch
        {
            public static void Postfix()
            {
                ComplexRecipe.RecipeElement[] ingredients1 = new ComplexRecipe.RecipeElement[1]
                {
                    new ComplexRecipe.RecipeElement(AquaBulbSackConfig.Id.ToTag(), 1f)
                };
                ComplexRecipe.RecipeElement[] results1 = new ComplexRecipe.RecipeElement[1]
                {
                    new ComplexRecipe.RecipeElement(GameTags.Water, 1000f)
                };
                string str1 = ComplexRecipeManager.MakeRecipeID(RockCrusherConfig.ID, ingredients1, results1);
                new ComplexRecipe(str1, ingredients1, results1)
                {
                    time = 40f,
                    nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
                    description = string.Format(STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, AquaBulbSackConfig.Name, ElementLoader.FindElementByHash(SimHashes.Water).name)
                }.fabricators = new List<Tag>()
                {
                    RockCrusherConfig.ID.ToTag()
                };
            }
        }
    }
}
