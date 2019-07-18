using TUNING;
using System.Collections.Generic;
using UnityEngine;

namespace MagmaFern
{
    public class HardenedFarmTileConfig : IBuildingConfig
    {
        public const string Id = "HardenedFarmTile";

        public static string DisplayName = STRINGS.UI.FormatAsLink("Hardened Farm Tile", Id);
        public static string Description = "A reinforced farm tile made from raw minerals; capable of withstanding harsh conditions.";
        public static string Effect = STRINGS.BUILDINGS.PREFABS.FARMTILE.EFFECT;

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(
                id: Id,
                width: 1,
                height: 1,
                anim: "hardenedfarmtile",
                hitpoints: BUILDINGS.HITPOINTS.TIER3,
                construction_time: BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER2,
                construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER2,
                construction_materials: MATERIALS.RAW_MINERALS,
                melting_point: BUILDINGS.MELTING_POINT_KELVIN.TIER3,
                build_location_rule: BuildLocationRule.Tile,
                decor: DECOR.NONE,
                noise: NOISE_POLLUTION.NONE,
                0.2f
                );
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.Overheatable = false;
            buildingDef.IsFoundation = true;
            buildingDef.TileLayer = ObjectLayer.FoundationTile;
            buildingDef.ReplacementLayer = ObjectLayer.ReplacementTile;
            buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingBack;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
            buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
            buildingDef.PermittedRotations = PermittedRotations.FlipV;
            buildingDef.isSolidTile = false;
            buildingDef.DragBuild = true;
            buildingDef.ReplacementTags = new List<Tag>();
            buildingDef.ReplacementTags.Add(GameTags.FloorTiles);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
            simCellOccupier.doReplaceElement = true;
            simCellOccupier.notifyOnMelt = true;
            go.AddOrGet<TileTemperature>();
            BuildingTemplates.CreateDefaultStorage(go, false).SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
            plantablePlot.occupyingObjectRelativePosition = new Vector3(0.0f, 1f, 0.0f);
            plantablePlot.AddDepositTag(GameTags.CropSeed);
            plantablePlot.AddDepositTag(GameTags.WaterSeed);
            plantablePlot.SetFertilizationFlags(true, false);
            go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.Farm;
            go.AddOrGet<AnimTileable>();
            Prioritizable.AddRef(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RemoveLoopingSounds(go);
            go.GetComponent<KPrefabID>().AddTag(GameTags.FarmTiles, false);
            FarmTileConfig.SetUpFarmPlotTags(go);
        }

    }
}
