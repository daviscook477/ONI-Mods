using System.Collections.Generic;
using TUNING;
using System;
using UnityEngine;

namespace CrystalBiome
{

    public class GemTileConfig : IBuildingConfig
    {
        public static readonly int BlockTileConnectorID = Hash.SDBMLower("tiles_gem_tops");
        public const string Id = "GemTile";
        public const string DisplayName = "Gemstone Tile";
        public const string Description = "Very pretty tiles.";
        public const string Effect = "Used to build pretty walkways.";

        public override BuildingDef CreateBuildingDef()
        {
            string id = "GemTile";
            int width = 1;
            int height = 1;
            string anim = "floor_gem";
            int hitpoints = 100;
            float construction_time = 30f;
            float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] transparents = MATERIALS.TRANSPARENTS;
            float melting_point = 800f;
            BuildLocationRule build_location_rule = BuildLocationRule.Tile;
            EffectorValues none = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tieR2, transparents, melting_point, build_location_rule, BUILDINGS.DECOR.BONUS.TIER0, none, 0.2f);
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.Overheatable = false;
            buildingDef.UseStructureTemperature = false;
            buildingDef.IsFoundation = true;
            buildingDef.TileLayer = ObjectLayer.FoundationTile;
            buildingDef.ReplacementLayer = ObjectLayer.ReplacementTile;
            buildingDef.AudioCategory = "Glass";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.SceneLayer = Grid.SceneLayer.GlassTile;
            buildingDef.isKAnimTile = true;
            buildingDef.isSolidTile = true;
            buildingDef.BlockTileIsTransparent = true;
            /*if (!CrystalBiomePatches.loadedAtlas)
            {
                CrystalBiomePatches.loadedAtlas = true;
                foreach(TextureAtlas atlas2 in CrystalBiomePatches.modAtlasList)
                {
                    Console.WriteLine("Loading atlas with name = " + atlas2 + " and value is " + atlas2.ToString());
                    atlas2.texture = CrystalBiomePatches.specialTex;
                }
                Assets.TextureAtlases.AddRange(CrystalBiomePatches.modAtlasList);
            }*/
            TextureAtlas atlas = Assets.GetTextureAtlas("tiles_gem");
            /*Console.WriteLine("found gem atlas, was " + atlas.ToString());*/
            buildingDef.BlockTileAtlas = atlas;
            buildingDef.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_gem_place");
            buildingDef.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
            buildingDef.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_glass_tops_decor_info");
            buildingDef.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_glass_tops_decor_place_info");
            buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
            buildingDef.ReplacementTags = new List<Tag>();
            buildingDef.ReplacementTags.Add(GameTags.FloorTiles);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
            simCellOccupier.setTransparent = true;
            simCellOccupier.notifyOnMelt = true;
            go.AddOrGet<TileTemperature>();
            go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = GlassTileConfig.BlockTileConnectorID;
            go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
            go.GetComponent<KPrefabID>().AddTag(GameTags.Window, false);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RemoveLoopingSounds(go);
            go.GetComponent<KPrefabID>().AddTag(GameTags.FloorTiles, false);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            go.AddOrGet<KAnimGridTileVisualizer>();
        }
    }
}
