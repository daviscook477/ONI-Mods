using System.Collections.Generic;
using TUNING;
using UnityEngine;
using System;
using System.Linq;

namespace ArtifactCabinet
{
    public class ArtifactCabinetConfig : IBuildingConfig
    {
        private static readonly LogicPorts.Port OUTPUT_PORT = LogicPorts.Port.OutputPort(UncategorizedFilteredStorage.FULL_PORT_ID, new CellOffset(0, 1), 
            (string)STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT, 
            (string)STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_ACTIVE, 
            (string)STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_INACTIVE, 
            false, false);

        public const string Id = "InteriorDecorArtifactCabinet";
        public static string DisplayName = "Artifact Cabinet";
        public const string Description = "Stores artifacts.";
        public const string Effect = "A handy cabinet for storing trinkets from space. Increases decor for each item stored.";

        public static List<Tag> ArtifactsFilterTagList = new List<Tag>()
        {
            "artifact_sandstone".ToTag(),
            "artifact_sink".ToTag(),
            "artifact_rubikscube".ToTag(),
            "artifact_officemug".ToTag(),
            "artifact_obelisk".ToTag(),
            "artifact_okayxray".ToTag(),
            "artifact_blender".ToTag(),
            "artifact_moldavite".ToTag(),
            "artifact_vhs".ToTag(),
            "artifact_saxophone".ToTag(),
            "artifact_modernart".ToTag(),
            "artifact_ameliaswatch".ToTag(),
            "artifact_teapot".ToTag(),
            "artifact_brickphone".ToTag(),
            "artifact_robotarm".ToTag(),
            "artifact_shieldgenerator".ToTag(),
            "artifact_bioluminescentrock".ToTag(),
            "artifact_stethoscope".ToTag(),
            "artifact_eggrock".ToTag(),
            "artifact_hatchfossil".ToTag(),
            "artifact_rocktornado".ToTag(),
            "artifact_pacupercolator".ToTag(),
            "artifact_magmalamp".ToTag(),
            "artifact_dnamodel".ToTag(),
            "artifact_rainboweggrock".ToTag(),
            "artifact_plasmalamp".ToTag(),
            "artifact_solarsystem".ToTag(),
            "artifact_moonmoonmoon".ToTag()
        };

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: Id,
                width: 2,
                height: 3,
                anim: "artifact_cabinet_kanim",
                hitpoints: 100,
                construction_time: 120f,
                construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER3,
                construction_materials: MATERIALS.RAW_MINERALS,
                melting_point: 1600f,
                build_location_rule: BuildLocationRule.OnFloor,
                decor: BUILDINGS.DECOR.BONUS.TIER2,
                noise: NOISE_POLLUTION.NONE
            );

            buildingDef.AudioCategory = "Metal";

            return buildingDef;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, ArtifactCabinetConfig.OUTPUT_PORT);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, ArtifactCabinetConfig.OUTPUT_PORT);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, ArtifactCabinetConfig.OUTPUT_PORT);
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 2000f;
            storage.showInUI = true;
            storage.showDescriptor = true;
            storage.allowItemRemoval = true;
            storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
            storage.storageFilters = ArtifactsFilterTagList;
            go.AddOrGet<UncategorizedFilterable>();
            go.AddOrGet<ArtifactCabinet>();
            Prioritizable.AddRef(go);
            go.AddOrGet<DropAllWorkable>();
            go.AddOrGetDef<StorageController.Def>();
        }
    }
}
