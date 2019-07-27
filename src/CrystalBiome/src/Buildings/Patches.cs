using System.Collections.Generic;

using Harmony;

using STRINGS;

namespace CrystalBiome.Buildings
{
    public class Patches
    {

        [HarmonyPatch(typeof(MetalRefineryConfig), "ConfigureBuildingTemplate")]
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
