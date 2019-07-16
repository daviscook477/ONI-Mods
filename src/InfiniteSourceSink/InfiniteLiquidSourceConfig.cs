using TUNING;
using UnityEngine;

namespace InfiniteSourceSink
{
	public class InfiniteLiquidSourceConfig : IBuildingConfig
	{
		public const string Id = "LiquidSource";
		public const string DisplayName = "Infinite Liquid Source";
		public const string Description = "Materializes liquid from the void.";
		public const string Effect = "Where is all the liquid coming from?";
		public const string TemperatureSliderTitle = "Liquid ouput temperature";
		public const string TemperatureSliderTooltip = "Liquid output temperature";

		public override BuildingDef CreateBuildingDef()
		{
			var buildingDef = BuildingTemplates.CreateBuildingDef(
				id: Id,
				width: 2,
				height: 3,
				anim: "liquidreservoir_kanim",
				hitpoints: BUILDINGS.HITPOINTS.TIER2,
				construction_time: BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER4,
				construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER4,
				construction_materials: MATERIALS.ALL_METALS,
				melting_point: BUILDINGS.MELTING_POINT_KELVIN.TIER0,
				build_location_rule: BuildLocationRule.OnFloor,
				decor: BUILDINGS.DECOR.PENALTY.TIER1,
				noise: NOISE_POLLUTION.NOISY.TIER0,
				0.2f
				);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.Floodable = false;
			buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<Filterable>().filterElementState = Filterable.ElementState.Liquid;
			go.AddOrGet<InfiniteSourceFlowControl>();
			go.AddOrGet<InfiniteSource>().Type = ConduitType.Liquid;
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
