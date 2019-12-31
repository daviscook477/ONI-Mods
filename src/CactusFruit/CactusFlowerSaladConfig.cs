using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STRINGS;
using UnityEngine;

namespace CactusFruit
{
    public class CactusFlowerSaladConfig : IEntityConfig
    {
        public const string Id = "CactusFlowerSalad";

        public static string Name = UI.FormatAsLink("Flowers and Greens", Id.ToUpper());

        public static string Description = $"Delectable {CactusFlowerConfig.Name} mixed with crunchy {UI.FormatAsLink("Lettuce", "LETTUCE")}. The blend of cool and sweet makes one's taste buds delight.";

        public static string RecipeDescription = $"Mixed and seared {CactusFlowerSaladConfig.Name}.";

        public ComplexRecipe Recipe;

        public GameObject CreatePrefab()
        {
            var looseEntity = EntityTemplates.CreateLooseEntity(
                id: Id,
                name: Name,
                desc: Description,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("cactusflowersalad_kanim"),
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.7f,
                height: 0.5f,
                isPickupable: true);

            var foodInfo = new EdiblesManager.FoodInfo(
                id: Id,
                caloriesPerUnit: 4200000f,
                quality: TUNING.FOOD.FOOD_QUALITY_AMAZING,
                preserveTemperatue: 255.15f,
                rotTemperature: 277.15f,
                spoilTime: 2400f,
                can_rot: true);

            var foodEntity = EntityTemplates.ExtendEntityToFood(
                template: looseEntity,
                foodInfo: foodInfo);

            var input = new[] { new ComplexRecipe.RecipeElement(CactusFlowerConfig.Id, 2f), new ComplexRecipe.RecipeElement(LettuceConfig.ID, 4f) };
            var output = new[] { new ComplexRecipe.RecipeElement(Id, 1f) };

            var recipeId = ComplexRecipeManager.MakeRecipeID(
                fabricator: GourmetCookingStationConfig.ID,
                inputs: input,
                outputs: output);

            Recipe = new ComplexRecipe(recipeId, input, output)
            {
                time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME,
                description = RecipeDescription,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                fabricators = new List<Tag> { GourmetCookingStationConfig.ID },
                sortOrder = 2,
                requiredTech = null
            };

            return foodEntity;
        }

        public void OnPrefabInit(GameObject inst) { }

        public void OnSpawn(GameObject inst) { }
    }
}
