using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STRINGS;
using UnityEngine;

namespace CactusFruit
{
    public class CactusFruitConfig : IEntityConfig
    {
        public const string Id = "CactusFruit";
        public const string SeedId = "CactusFruitSeed";

        public static string Name = UI.FormatAsLink("Prickly Pear", Id.ToUpper());
        public static string SeedName = UI.FormatAsLink("Prickly Pear Seed", SeedId.ToUpper());

        public static string Description = $"Spiky to the touch, the {Name} produces both an edible {UI.FormatAsLink("Flesh", CactusFleshConfig.Id)} and {UI.FormatAsLink("Flower", CactusFlowerConfig.Id)}.";
        public static string SeedDescription = $"The beginnings of a {Name}. Just add {UI.FormatAsLink("Water", "WATER")}.";
        public static string DomesticatedDescription = $"This cactus {UI.FormatAsLink("Plant", "PLANTS")} survives well in the harsh, dry desert. To thrive and flower, it needs copious amounts of {UI.FormatAsLink("Water", "WATER")}.";

        public const float DefaultTemperature = 320f;
        public const float TemperatureLethalLow = 258.15f;
        public const float TemperatureWarningLow = 308.15f;
        public const float TemperatureWarningHigh = 358.15f;
        public const float TemperatureLethalHigh = 448.15f;

        public const float IrrigationRate = 0.04f;

        public GameObject CreatePrefab()
        {
            var placedEntity = EntityTemplates.CreatePlacedEntity(
                id: Id,
                name: Name,
                desc: Description,
                mass: 1f,
                anim: Assets.GetAnim("cactusfruit_kanim"),
                initialAnim: "idle_empty",
                sceneLayer: Grid.SceneLayer.BuildingFront,
                width: 1,
                height: 1,
                decor: TUNING.DECOR.BONUS.TIER1,
                defaultTemperature: DefaultTemperature
                );

            EntityTemplates.ExtendEntityToBasicPlant(
                template: placedEntity,
                temperature_lethal_low: TemperatureLethalLow,
                temperature_warning_low: TemperatureWarningLow,
                temperature_warning_high: TemperatureWarningHigh,
                temperature_lethal_high: TemperatureLethalHigh,
                safe_elements: new[] { SimHashes.Oxygen, SimHashes.CarbonDioxide },
                pressure_sensitive: true,
                pressure_lethal_low: 0.0f,
                pressure_warning_low: 0.15f,
                crop_id: CactusFleshConfig.Id);

            EntityTemplates.ExtendPlantToIrrigated(
                template: placedEntity,
                info: new PlantElementAbsorber.ConsumeInfo()
                {
                    tag = GameTags.Water,
                    massConsumptionRate = IrrigationRate
                });

            placedEntity.AddOrGet<StandardCropPlant>();

            var seed = EntityTemplates.CreateAndRegisterSeedForPlant(
                plant: placedEntity,
                productionType: SeedProducer.ProductionType.Harvest,
                id: SeedId,
                name: SeedName,
                desc: SeedDescription,
                anim: Assets.GetAnim("cactusseed_kanim"),
                initialAnim: "object",
                numberOfSeeds: 1,
                additionalTags: new List<Tag>() { GameTags.CropSeed },
                planterDirection: SingleEntityReceptacle.ReceptacleDirection.Top,
                replantGroundTag: new Tag(),
                sortOrder: 2,
                domesticatedDescription: DomesticatedDescription,
                collisionShape: EntityTemplates.CollisionShape.CIRCLE,
                width: 0.2f,
                height: 0.2f);

            EntityTemplates.CreateAndRegisterPreviewForPlant(
                seed: seed,
                id: $"{Id}_preview",
                anim: Assets.GetAnim("cactusfruit_kanim"),
                initialAnim: "place",
                width: 1,
                height: 1);

            return placedEntity;
        }

        public void OnPrefabInit(GameObject inst) { }

        public void OnSpawn(GameObject inst) { }
    }
}
