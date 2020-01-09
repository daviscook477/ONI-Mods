using STRINGS;
using UnityEngine;

namespace CactusFruit
{
    public class CactusFleshConfig : IEntityConfig
    {
        public const string Id = "CactusFlesh";

        public static string Name = UI.FormatAsLink("Cactus Flesh", Id.ToUpper());

        public static string Description = $"The flesh of a {CactusFruitConfig.Name}. Juicy, edible, and uncomfortable.";

        public GameObject CreatePrefab()
        {
            var looseEntity = EntityTemplates.CreateLooseEntity(
                id: Id,
                name: Name,
                desc: Description,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("cactusflesh_kanim"),
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.8f,
                height: 0.4f,
                isPickupable: true);

            var foodInfo = new EdiblesManager.FoodInfo(
                id: Id,
                caloriesPerUnit: 1200000f,
                quality: TUNING.FOOD.FOOD_QUALITY_AWFUL,
                preserveTemperatue: 255.15f,
                rotTemperature: 277.15f,
                spoilTime: 2400f,
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
