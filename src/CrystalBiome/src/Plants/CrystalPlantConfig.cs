using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace CrystalBiome
{
    public class CrystalPlantConfig : IEntityConfig
    {
        public const string Id = "CrystalPlant";
        public const string SeedId = "CrystalPlantSeed";

        public static string Name = UI.FormatAsLink("Galvanized Outcrop", Id.ToUpper());
        public static string Description = $"An electrified crystal {UI.FormatAsLink("formation", "PLANTS")} that grows by absorbing all of the solid mass in mineral water.";
        public static string DomesticatedDescription = $"This {UI.FormatAsLink("outcrop", "PLANTS")} of crystals produces high energy density fragments that can be mined off when fully grown.";

        public static string SeedName = UI.FormatAsLink("Crystal Starter", Id.ToUpper());
        public static string SeedDescription = $"The beginnings of a {Name}. Just add {UI.FormatAsLink("mineral water", ElementLoader.FindElementByHash(SimHashes.SaltWater).nameUpperCase)}";

        public const float DefaultTemperature = 273.15f;
        public const float TemperatureLethalLow = 223.15f;
        public const float TemperatureWarningLow = 243.15f;
        public const float TemperatureWarningHigh = 288.15f;
        public const float TemperatureLethalHigh = 293.15f;

        public const float IrrigationRate = 0.05f;

        public GameObject CreatePrefab()
        {
            var placedEntity = EntityTemplates.CreatePlacedEntity(
                id: Id,
                name: Name,
                desc: Description,
                mass: 1f,
                anim: Assets.GetAnim("crystal_plant"),
                initialAnim: "idle_empty",
                sceneLayer: Grid.SceneLayer.BuildingFront,
                width: 1,
                height: 2,
                decor: TUNING.DECOR.BONUS.TIER2,
                defaultTemperature: DefaultTemperature);

            EntityTemplates.ExtendEntityToBasicPlant(
                template: placedEntity,
                temperature_lethal_low: TemperatureLethalLow,
                temperature_warning_low: TemperatureWarningLow,
                temperature_warning_high: TemperatureWarningHigh,
                temperature_lethal_high: TemperatureLethalHigh,
                safe_elements: new[] { SimHashes.Oxygen },
                pressure_sensitive: true,
                pressure_lethal_low: 0.0f,
                pressure_warning_low: 0.15f,
                crop_id: SeedId);

            EntityTemplates.ExtendPlantToIrrigated(
                template: placedEntity,
                info: new PlantElementAbsorber.ConsumeInfo()
                {
                    tag = ElementLoader.FindElementByHash(SimHashes.SaltWater).tag,
                    massConsumptionRate = IrrigationRate
                });

            placedEntity.AddOrGet<StandardCropPlant>();

            var seed = EntityTemplates.CreateAndRegisterSeedForPlant(
                plant: placedEntity,
                productionType: SeedProducer.ProductionType.DigOnly,
                id: SeedId,
                name: SeedName,
                desc: SeedDescription,
                anim: Assets.GetAnim("crystal_plant"),
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
                ignoreDefaultSeedTag: true); ;

            EntityTemplates.CreateAndRegisterPreviewForPlant(
                seed: seed,
                id: $"{Id}_preview",
                anim: Assets.GetAnim("crystal_plant"),
                initialAnim: "place",
                width: 1,
                height: 1);

            SoundEventVolumeCache.instance.AddVolume("crystal_plant", $"{Id}_grow", TUNING.NOISE_POLLUTION.CREATURES.TIER3);
            SoundEventVolumeCache.instance.AddVolume("crystal_plant", $"{Id}_harvest", TUNING.NOISE_POLLUTION.CREATURES.TIER3);
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
