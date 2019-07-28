using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace CrystalBiome.Plants
{

    public class BaseCrystalPlant
    {
        public const float DefaultTemperature = 273.15f;
        public const float TemperatureLethalLow = 223.15f;
        public const float TemperatureWarningLow = 243.15f;
        public const float TemperatureWarningHigh = 288.15f;
        public const float TemperatureLethalHigh = 293.15f;

        public const float IrrigationRate = 0.05f;

        public static GameObject CreateCrystalPlantPrefab(string Id, string SeedId, string Name, string SeedName, 
            string Description, string SeedDescription, string DomesticatedDescription, string anim, SingleEntityReceptacle.ReceptacleDirection direction)
        {
            var placedEntity = EntityTemplates.CreatePlacedEntity(
                    id: Id,
                    name: Name,
                    desc: Description,
                    mass: 1f,
                    anim: Assets.GetAnim(anim),
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
                crop_id: Elements.CrystalElement.Id);

            EntityTemplates.ExtendPlantToIrrigated(
                template: placedEntity,
                info: new PlantElementAbsorber.ConsumeInfo()
                {
                    tag = ElementLoader.FindElementByHash(Elements.MineralWaterElement.SimHash).tag,
                    massConsumptionRate = IrrigationRate
                });

            if (direction == SingleEntityReceptacle.ReceptacleDirection.Bottom)
            {
                EntityTemplates.MakeHangingOffsets(placedEntity, 1, 2);
                placedEntity.GetComponent<UprootedMonitor>().monitorCell = new CellOffset(0, 1);
            }

            placedEntity.AddOrGet<StandardCropPlant>();

            var seed = EntityTemplates.CreateAndRegisterSeedForPlant(
                plant: placedEntity,
                productionType: SeedProducer.ProductionType.DigOnly,
                id: SeedId,
                name: SeedName,
                desc: SeedDescription,
                anim: Assets.GetAnim(anim),
                initialAnim: "object",
                numberOfSeeds: 1,
                additionalTags: new List<Tag>() { GameTags.CropSeed },
                planterDirection: direction,
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
                anim: Assets.GetAnim(anim),
                initialAnim: "place",
                width: 1,
                height: 1);

            SoundEventVolumeCache.instance.AddVolume(anim, $"{Id}_grow", TUNING.NOISE_POLLUTION.CREATURES.TIER3);
            SoundEventVolumeCache.instance.AddVolume(anim, $"{Id}_harvest", TUNING.NOISE_POLLUTION.CREATURES.TIER3);
            return placedEntity;
        }
    }
}
