using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STRINGS;
using UnityEngine;

namespace CactusFruit
{
    public class CactusFleshGrilledConfig : IEntityConfig
    {
        public const string Id = "CactusFleshGrilled";

        public static string Name = UI.FormatAsLink("Grilled Cactus Flesh", Id.ToUpper());

        public static string Description = $"The grilled flesh of a {CactusFruitConfig.Name}. Grilling renders the prickly bits mostly harmless.";

        public static string RecipeDescription = $"Tasty grilled {CactusFleshConfig.Name}.";

        public ComplexRecipe Recipe;

        public GameObject CreatePrefab()
        {
            var looseEntity = EntityTemplates.CreateLooseEntity(
                id: Id,
                name: Name,
                desc: Description,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("cactusfleshgrilled_kanim"),
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.8f,
                height: 0.4f,
                isPickupable: true);

            var foodInfo = new EdiblesManager.FoodInfo(
                id: Id,
                caloriesPerUnit: 2800000f,
                quality: TUNING.FOOD.FOOD_QUALITY_MEDIOCRE,
                preserveTemperatue: 255.15f,
                rotTemperature: 277.15f,
                spoilTime: 2400f,
                can_rot: true);

            var foodEntity = EntityTemplates.ExtendEntityToFood(
                template: looseEntity,
                foodInfo: foodInfo);

            var input = new[] { new ComplexRecipe.RecipeElement(CactusFleshConfig.Id, 1f) };
            var output = new[] { new ComplexRecipe.RecipeElement(Id, 1f) };

            var recipeId = ComplexRecipeManager.MakeRecipeID(
                fabricator: CookingStationConfig.ID,
                inputs: input,
                outputs: output);

            Recipe = new ComplexRecipe(recipeId, input, output)
            {
                time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME,
                description = RecipeDescription,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                fabricators = new List<Tag> { CookingStationConfig.ID },
                sortOrder = 2,
                requiredTech = null
            };

            return foodEntity;
        }

        public void OnPrefabInit(GameObject inst) { }

        public void OnSpawn(GameObject inst) { }

    }
}
