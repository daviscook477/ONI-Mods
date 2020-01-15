using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Champagne
{
    public class GrapeVineConfig : IEntityConfig
    {
        public const string Id = "GrapeVine";
        public const string SeedId = "GrapeVineSeed";

        public static string Name = UI.FormatAsLink("Grapeberry Vine", Id.ToUpper());
        public static string SeedName = UI.FormatAsLink("Grapeberry Seed", Id.ToUpper());

        public static string Description = $"With a deep purple color, this large fruiting {Name} is crawls towards the sky.";
        public static string SeedDescription = $"The spiky beginnings of a {Name}.";
        public static string DomesticatedDescription = $"This vine {UI.FormatAsLink("Plant", "PLANTS")} grows well when winding around a small pole.";

        public const float DefaultTemperature = 295f;
        public const float TemperatureLethalLow = 263.15f;
        public const float TemperatureWarningLow = 283.15f;
        public const float TemperatureWarningHigh = 313.15f;
        public const float TemperatureLethalHigh = 333.15f;

        public const float IrrigationRate = 0.04f;

        public GameObject CreatePrefab()
        {
            var placedEntity = EntityTemplates.CreatePlacedEntity(
                id: Id,
                name: Name,
                desc: Description,
                mass: 1f,
                anim: Assets.GetAnim("grapeberryvine_kanim"),
                initialAnim: "idle_empty",
                sceneLayer: Grid.SceneLayer.BuildingFront,
                width: 1,
                height: 2,
                decor: TUNING.DECOR.BONUS.TIER1,
                defaultTemperature: DefaultTemperature
                );

            EntityTemplates.ExtendEntityToBasicPlant(
                template: placedEntity,
                temperature_lethal_low: TemperatureLethalLow,
                temperature_warning_low: TemperatureWarningLow,
                temperature_warning_high: TemperatureWarningHigh,
                temperature_lethal_high: TemperatureLethalHigh,
                safe_elements: new[] { SimHashes.Methane },
                pressure_sensitive: true,
                pressure_lethal_low: 0.0f,
                pressure_warning_low: 0.15f,
                crop_id: GrapeberryConfig.Id);

            EntityTemplates.ExtendPlantToIrrigated(
                template: placedEntity,
                info: new PlantElementAbsorber.ConsumeInfo()
                {
                    tag = GameTags.DirtyWater,
                    massConsumptionRate = IrrigationRate
                });

            placedEntity.AddOrGet<StandardCropPlant>();

            var seed = EntityTemplates.CreateAndRegisterSeedForPlant(
                plant: placedEntity,
                productionType: SeedProducer.ProductionType.Harvest,
                id: SeedId,
                name: SeedName,
                desc: SeedDescription,
                anim: Assets.GetAnim("grapeseed_kanim"),
                initialAnim: "object",
                numberOfSeeds: 1,
                additionalTags: new List<Tag>() { GameTags.CropSeed },
                planterDirection: SingleEntityReceptacle.ReceptacleDirection.Top,
                replantGroundTag: new Tag(),
                sortOrder: 3,
                domesticatedDescription: DomesticatedDescription,
                collisionShape: EntityTemplates.CollisionShape.CIRCLE,
                width: 0.2f,
                height: 0.2f);

            EntityTemplates.CreateAndRegisterPreviewForPlant(
                seed: seed,
                id: $"{Id}_preview",
                anim: Assets.GetAnim("grapeberryvine_kanim"),
                initialAnim: "place",
                width: 1,
                height: 1);

            return placedEntity;
        }

        public void OnPrefabInit(GameObject inst) { }

        public void OnSpawn(GameObject inst) { }

    }
}
