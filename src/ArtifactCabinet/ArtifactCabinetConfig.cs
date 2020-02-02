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
            STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT, 
            STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_ACTIVE, 
            STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_INACTIVE, 
            false, false);

        public const string Id = "InteriorDecorArtifactCabinet";
        public static string DisplayName = "Artifact Cabinet";
        public const string Description = "Stores artifacts.";
        public const string Effect = "A handy cabinet for storing trinkets from space. Increases decor for each item stored.";

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
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 2000f;
            storage.showInUI = true;
            storage.showDescriptor = true;
            storage.allowItemRemoval = true;
            storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
            storage.storageFilters = ArtifactCabinet.ArtifactsFilterTagList;
            go.AddOrGet<UncategorizedFilterable>();
            go.AddOrGet<ArtifactCabinet>();
            Prioritizable.AddRef(go);
            go.AddOrGet<DropAllWorkable>();
            go.AddOrGetDef<StorageController.Def>();
        }
    }
}
