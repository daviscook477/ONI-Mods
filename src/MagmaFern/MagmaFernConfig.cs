using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace MagmaFern
{

    public class MagmaFernConfig : IEntityConfig
    {
        public const string Id = "MagmaFern";
        public const string SeedId = "MagmaFernSeed";

        public static string Name = UI.FormatAsLink("Magma Fern", Id.ToUpper());
        public static string Description = $"A hardy {UI.FormatAsLink("fern", "PLANTS")} that thrives on the geothermal heat produced by magma.";
        public static string DomesticatedDescription = $"This {UI.FormatAsLink("fern", "PLANTS")} produces edible spores that may be processed into high-calorie {UI.FormatAsLink("Food", "FOOD")}.";

        public static string SeedName = UI.FormatAsLink("Magma Fern Spore", Id.ToUpper());
        public static string SeedDescription = $"An incredibly energy dense spore of a {Name}";

        public const float DefaultTemperature = 2023.15f;
        public const float TemperatureLethalLow = 1723.15f;
        public const float TemperatureWarningLow = 1773.15f;
        public const float TemperatureWarningHigh = 2223.15f;
        public const float TemperatureLethalHigh = 2273.15f;

        public const float FertilizationRate = 0.008333334f;
        public const float IrrigationRate = 0.05f;

        public const float PreserveTemperature = 1473.15f;
        public const float RotTemperature = 2473.15f;

        public GameObject CreatePrefab()
        {
            var placedEntity = EntityTemplates.CreatePlacedEntity(
                id: Id,
                name: Name,
                desc: Description,
                mass: 1f,
                anim: Assets.GetAnim("magmafern"),
                initialAnim: "idle_empty",
                sceneLayer: Grid.SceneLayer.BuildingFront,
                width: 1,
                height: 1,
                decor: TUNING.DECOR.BONUS.TIER2,
                defaultTemperature: DefaultTemperature);

            EntityTemplates.ExtendEntityToBasicPlant(
                template: placedEntity,
                temperature_lethal_low: TemperatureLethalLow,
                temperature_warning_low: TemperatureWarningLow,
                temperature_warning_high: TemperatureWarningHigh,
                temperature_lethal_high: TemperatureLethalHigh,
                safe_elements: new[] { SimHashes.Magma },
                pressure_sensitive: false,
                crop_id: SeedId);

            EntityTemplates.ExtendPlantToFertilizable(
                template: placedEntity,
                fertilizers: new[]
                {
                    new PlantElementAbsorber.ConsumeInfo()
                    {
                        tag = ElementLoader.FindElementByHash(SimHashes.Obsidian).tag,
                        massConsumptionRate = FertilizationRate
                    }
                })
                ;
            EntityTemplates.ExtendPlantToIrrigated(
                template: placedEntity,
                info: new PlantElementAbsorber.ConsumeInfo()
                {
                    tag = ElementLoader.FindElementByHash(SimHashes.Magma).tag,
                    massConsumptionRate = IrrigationRate
                });

            placedEntity.AddOrGet<StandardCropPlant>();

            var seed = EntityTemplates.CreateAndRegisterSeedForPlant(
                plant: placedEntity,
                productionType: SeedProducer.ProductionType.DigOnly,
                id: SeedId,
                name: SeedName,
                desc: SeedDescription,
                anim: Assets.GetAnim("seed_magmafern"),
                initialAnim: "object",
                numberOfSeeds: 1,
                additionalTags: new List<Tag>() { GameTags.CropSeed },
                planterDirection: SingleEntityReceptacle.ReceptacleDirection.Top,
                replantGroundTag: new Tag(),
                sortOrder: 2,
                domesticatedDescription: DomesticatedDescription,
                collisionShape: EntityTemplates.CollisionShape.CIRCLE,
                width: 0.2f,
                height: 0.2f,
                ignoreDefaultSeedTag: true);

            var foodInfo = new EdiblesManager.FoodInfo(
                SeedId,
                caloriesPerUnit: 0.0f,
                quality: 0,
                preserveTemperatue: PreserveTemperature,
                rotTemperature: RotTemperature,
                spoilTime: TUNING.FOOD.SPOIL_TIME.SLOW,
                can_rot: true);
            EntityTemplates.ExtendEntityToFood(
                template: seed,
                foodInfo: foodInfo);

            EntityTemplates.CreateAndRegisterPreviewForPlant(
                seed: seed,
                id: "MagmaFern_preview",
                anim: Assets.GetAnim("magmafern"),
                initialAnim: "place",
                width: 1,
                height: 1);

            SoundEventVolumeCache.instance.AddVolume("magmafern", "MagmaFern_grow", TUNING.NOISE_POLLUTION.CREATURES.TIER3);
            SoundEventVolumeCache.instance.AddVolume("magmafern", "MagmaFern_harvest", TUNING.NOISE_POLLUTION.CREATURES.TIER3);
            return placedEntity;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
