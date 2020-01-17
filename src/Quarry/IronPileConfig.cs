using System.Collections.Generic;
using UnityEngine;
using TUNING;
using STRINGS;

namespace Quarry
{
    public class IronPileConfig : IEntityConfig
    {
        public const string Id = "IronPile";
        public static string Name = UI.FormatAsLink("Iron Pile", Id.ToUpper());
        public static string Description = $"An incredibly deep pile of {UI.FormatAsLink("Iron Ore", "IRONORE")}";

        public const float DefaultTemperature = 293f;

        public GameObject CreatePrefab()
        {
            GameObject placedEntity = EntityTemplates.CreatePlacedEntity(
                id: Id,
                name: Name,
                desc: Description,
                mass: 2000f,
                anim: Assets.GetAnim("ironpile_kanim"),
                initialAnim: "idle",
                Grid.SceneLayer.BuildingBack,
                width: 3,
                height: 2,
                decor: DECOR.BONUS.TIER1,
                defaultTemperature: DefaultTemperature);
            placedEntity.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[1]
            {
                ObjectLayer.Building
            };
            PrimaryElement component = placedEntity.GetComponent<PrimaryElement>();
            component.SetElement(SimHashes.SedimentaryRock);
            component.Temperature = 372.15f;
            placedEntity.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 0), Id.ToTag(), null)
            };
            SoundEventVolumeCache.instance.AddVolume("geyser_side_methane_kanim", "GeyserMethane_shake_LP", NOISE_POLLUTION.NOISY.TIER5);
            SoundEventVolumeCache.instance.AddVolume("geyser_side_methane_kanim", "GeyserMethane_erupt_LP", NOISE_POLLUTION.NOISY.TIER6);
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
