using TUNING;
using UnityEngine;

namespace TeleStorage
{
    public class TeleStorageLiquidConfig : IBuildingConfig
    {
        public const string Id = "TeleStorageLiquid";
        public const string DisplayName = "Advanced Liquid Storage";
        public const string Description = "Stores liquid inside an alternate dimension.";
        public const string Effect = "Compresses liquids into an alternate dimension for more effective storage.";

        public override BuildingDef CreateBuildingDef()
        {
            float[] construction_mass = new float[]
            {
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER4[0],
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0],
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0]
            };
            string[] construction_materials = new string[]
            {
                "Steel",
                "Plastic",
                "Diamond"
            };

            var buildingDef = BuildingTemplates.CreateBuildingDef(
                id: Id,
                width: 2,
                height: 3,
                anim: "liquidtele",
                hitpoints: BUILDINGS.HITPOINTS.TIER2,
                construction_time: BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER4,
                construction_mass: construction_mass,
                construction_materials: construction_materials,
                melting_point: BUILDINGS.MELTING_POINT_KELVIN.TIER0,
                build_location_rule: BuildLocationRule.OnFloor,
                decor: BUILDINGS.DECOR.PENALTY.TIER1,
                noise: NOISE_POLLUTION.NOISY.TIER0,
                0.2f
                );
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.Floodable = false;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.UtilityInputOffset = new CellOffset(1, 2);
            buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<Filterable>().filterElementState = Filterable.ElementState.Liquid;
            go.AddOrGet<TeleStorageFlowControl>();
            go.AddOrGet<TeleStorage>().Type = ConduitType.Liquid;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_1);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_1);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_1);
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGet<Operational>();

            Object.DestroyImmediate(go.GetComponent<RequireInputs>());
            Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
            Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());

            BuildingTemplates.DoPostConfigure(go);
        }
    }
}
