using STRINGS;
using UnityEngine;

namespace CactusFruit
{
    public class CactusFlowerConfig : IEntityConfig
    {
        public const string Id = "CactusFlower";

        public static string Name = UI.FormatAsLink("Cactus Flower", Id.ToUpper());

        public static string Description = $"The red flower of a {CactusFruitConfig.Name}. Pretty, edible, and sugary.";

        public GameObject CreatePrefab()
        {
            var looseEntity = EntityTemplates.CreateLooseEntity(
                id: Id,
                name: Name,
                desc: Description,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("cactusflower_kanim"),
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.6f,
                height: 0.4f,
                isPickupable: true);

            var foodInfo = new EdiblesManager.FoodInfo(
                id: Id,
                caloriesPerUnit: 1200000f,
                quality: TUNING.FOOD.FOOD_QUALITY_GOOD,
                preserveTemperatue: 255.15f,
                rotTemperature: 277.15f,
                spoilTime: 3600f,
                can_rot: true);

            var foodEntity = EntityTemplates.ExtendEntityToFood(
                template: looseEntity,
                foodInfo: foodInfo);

            return foodEntity;
        }

        public void OnPrefabInit(GameObject inst) { }

        public void OnSpawn(GameObject inst) { }
    }

}
