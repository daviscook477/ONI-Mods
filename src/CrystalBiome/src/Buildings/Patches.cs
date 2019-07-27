using System.Collections.Generic;

using Harmony;
using UnityEngine;

using STRINGS;

namespace CrystalBiome.Buildings
{
    public class Patches
    {
        public static void OnLoad()
        {
            BUILDINGS.PREFABS.DESALINATOR.DESC += " Aluminum salt can be refined into aluminum metal.";
            BUILDINGS.PREFABS.DESALINATOR.EFFECT += $" Additionally removes {UI.FormatAsLink("Aluminum Salt", Elements.AluminumSaltElement.Id)} from {UI.FormatAsLink("Mineral Water", Elements.MineralWaterElement.Id)}.";
        }

        [HarmonyPatch(typeof(DesalinatorConfig), nameof(DesalinatorConfig.ConfigureBuildingTemplate))]
        public static class DesalinatorConfig_ConfigureBuildingTemplate
        {
            private static void Postfix(GameObject go)
            {
                ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
                var filter = new List<SimHashes>(conduitDispenser.elementFilter) { Elements.MineralWaterElement.SimHash } ;
                conduitDispenser.elementFilter = filter.ToArray();

                ElementConverter elementConverter = go.AddComponent<ElementConverter>();
                elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
                {
                    new ElementConverter.ConsumedElement(ElementLoader.FindElementByHash(Elements.MineralWaterElement.SimHash).tag, 5f)
                };
                elementConverter.outputElements = new ElementConverter.OutputElement[2]
                {
                    new ElementConverter.OutputElement(3.5f, SimHashes.Water, 0.0f, false, true, 0.0f, 0.5f, 0.75f, byte.MaxValue, 0),
                    new ElementConverter.OutputElement(1.5f, Elements.AluminumSaltElement.SimHash, 0.0f, false, true, 0.0f, 0.5f, 0.25f, byte.MaxValue, 0)
                };
            }
        }


        [HarmonyPatch(typeof(MetalRefineryConfig), nameof(MetalRefineryConfig.ConfigureBuildingTemplate))]
        public static class MetalRefineryConfig_ConfigureBuildingTemplate_Patch
        {
            private static void Postfix()
            {
                ComplexRecipe.RecipeElement[] ingredients1 = new ComplexRecipe.RecipeElement[1]
                {
                      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(Elements.AluminumSaltElement.SimHash).tag, 200f)
                };
                ComplexRecipe.RecipeElement[] results1 = new ComplexRecipe.RecipeElement[1]
                {
                    new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Aluminum).tag, 100f)
                };
                string str1 = ComplexRecipeManager.MakeRecipeID("MetalRefinery", ingredients1, results1);
                new ComplexRecipe(str1, ingredients1, results1)
                {
                    time = 40f,
                    nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
                    description = string.Format(BUILDINGS.PREFABS.METALREFINERY.RECIPE_DESCRIPTION, ElementLoader.FindElementByHash(SimHashes.Aluminum).name, ElementLoader.FindElementByHash(Elements.AluminumSaltElement.SimHash).name)
                }.fabricators = new List<Tag>()
                {
                  TagManager.Create("MetalRefinery")
                };
            }
        }

    }
}
