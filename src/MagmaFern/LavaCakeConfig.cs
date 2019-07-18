using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using STRINGS;

namespace MagmaFern
{
    public class LavaCakeConfig : IEntityConfig
    {

        public const string Id = "LavaCake";
        public static string Name = UI.FormatAsLink("Lava Cake", Id.ToUpper());
        public static string Description = $"Roasted {MagmaFernConfig.SeedName}. \n\nOnce removed from the {MagmaFernConfig.Name}, the spores must be brought back to high temperatures to release the goey lava-like substance inside.";
        public static string RecipeDescription = $"Warm and goey lava cakes made from some {MagmaFernConfig.SeedName}.";

        public ComplexRecipe Recipe;

        public GameObject CreatePrefab()
        {
            var entity = EntityTemplates.CreateLooseEntity(
                id: Id,
                name: Name,
                desc: Description,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("lavacake"),
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.8f,
                height: 0.4f,
                isPickupable: true);

            var foodInfo = new EdiblesManager.FoodInfo(
                id: Id,
                caloriesPerUnit: 4000000f,
                quality: 6,
                preserveTemperatue: 255.15f,
                rotTemperature: 277.15f,
                spoilTime: TUNING.FOOD.SPOIL_TIME.SLOW,
                can_rot: true);

            var food = EntityTemplates.ExtendEntityToFood(
                template: entity,
                foodInfo: foodInfo);

            ComplexRecipe.RecipeElement[] ingredients =
            {
                new ComplexRecipe.RecipeElement(MagmaFernConfig.SeedId, 2f)
            };

            ComplexRecipe.RecipeElement[] results =
            {
                new ComplexRecipe.RecipeElement(Id, 1f)
            };

            Recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(CookingStationConfig.ID, ingredients, results), ingredients, results)
            {
                time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME,
                description = RecipeDescription,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                fabricators = new List<Tag> { GourmetCookingStationConfig.ID },
                sortOrder = 120
            };

            return food;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

    }
}
