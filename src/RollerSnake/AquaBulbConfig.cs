using STRINGS;
using UnityEngine;

namespace AquaBulb
{
    public class AquaBulbConfig : IEntityConfig
    {
        public const string Id = "AquaBulb";

        public static string Name = UI.FormatAsLink("Aqua Bulb", Id.ToUpper());

        public static string Description = $"Aqua Bulbs are incapable of propagating but can be harvested for a single, {UI.FormatAsLink("Water", "WATER")}-filled sack. The {UI.FormatAsLink("Rock Crusher", RockCrusherConfig.ID.ToUpper())} can be used to extract the water within it.";

        public const float DefaultTemperature = 320f;

        public GameObject CreatePrefab()
        {
            GameObject placedEntity = EntityTemplates.CreatePlacedEntity(
                id: Id,
                name: Name,
                desc: Description,
                mass: 1000f,
                anim: Assets.GetAnim("aquabulb_kanim"),
                initialAnim: "idle",
                sceneLayer: Grid.SceneLayer.BuildingFront,
                width: 1,
                height: 2,
                decor: TUNING.DECOR.BONUS.TIER3,
                defaultTemperature: DefaultTemperature);

            placedEntity.AddOrGet<SimTemperatureTransfer>();
            placedEntity.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1]
            {
                ObjectLayer.Building
            };
            placedEntity.AddOrGet<EntombVulnerable>();
            placedEntity.AddOrGet<DrowningMonitor>();
            placedEntity.AddOrGet<Prioritizable>();
            placedEntity.AddOrGet<Uprootable>();
            placedEntity.AddOrGet<UprootedMonitor>();
            placedEntity.AddOrGet<Harvestable>();
            placedEntity.AddOrGet<HarvestDesignatable>();
            placedEntity.AddOrGet<SeedProducer>().Configure(AquaBulbSackConfig.Id, SeedProducer.ProductionType.DigOnly, 1);
            placedEntity.AddOrGet<BasicForagePlantPlanted>();
            placedEntity.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
            return placedEntity;
        }

        public void OnPrefabInit(GameObject inst) { }

        public void OnSpawn(GameObject inst) { }
    }
}
