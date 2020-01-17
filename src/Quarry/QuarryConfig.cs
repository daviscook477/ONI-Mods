using System.Collections.Generic;

using TUNING;
using UnityEngine;
using STRINGS;

namespace Quarry
{
    public class QuarryConfig : IBuildingConfig
    {
        public const string Id = "OreQuarry";
        public static string DisplayName = UI.FormatAsLink("Quarry", Id.ToUpper());
        public static string Description = $"Extracts {UI.FormatAsLink("Metal Ore", "RAWMETAL")} from the ground with a powerful drill.";
        public static string Effect = $"Produces raw {UI.FormatAsLink("Metal Ore", "RAWMETAL")} at the cost of a significant amount of {UI.FormatAsLink("Power", "POWER")}.";

        private const float WATER_INPUT_RATE = 0.5f;
        private const float DIRTY_WATER_OUTPUT_RATE = 0.5f;
        private const float ORE_OUTPUT_RATE = 2.0f;

        public override BuildingDef CreateBuildingDef()
        {

            var buildingDef = BuildingTemplates.CreateBuildingDef(
                id: Id,
                width: 5,
                height: 4,
                anim: "quarry_kanim",
                hitpoints: TUNING.BUILDINGS.HITPOINTS.TIER3,
                construction_time: TUNING.BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER3,
                construction_mass: TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4,
                construction_materials: MATERIALS.REFINED_METALS,
                melting_point: TUNING.BUILDINGS.MELTING_POINT_KELVIN.TIER0,
                build_location_rule: BuildLocationRule.OnFloor,
                decor: TUNING.BUILDINGS.DECOR.NONE,
                noise: NOISE_POLLUTION.NOISY.TIER3,
                1f
                );
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 1800f;
            buildingDef.ExhaustKilowattsWhenActive = 24f;
            buildingDef.SelfHeatKilowattsWhenActive = 0f;
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.Floodable = true;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.PowerInputOffset = new CellOffset(-2, 0);
            buildingDef.UtilityInputOffset = new CellOffset(2, 1);
            buildingDef.UtilityOutputOffset = new CellOffset(2, 0);
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            buildingDef.AttachmentSlotTag = IronPileConfig.Id.ToTag();
            buildingDef.BuildLocationRule = BuildLocationRule.BuildingAttachPoint;
            buildingDef.ObjectLayer = ObjectLayer.AttachableBuilding;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);

            Storage storage = go.AddOrGet<Storage>();
            storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            storage.showInUI = false;
            storage.capacityKg = 20f;

            go.AddOrGet<LoopingSounds>();
            go.AddOrGet<Quarry>();

            ElementConverter elementConverter1 = go.AddComponent<ElementConverter>();
            elementConverter1.consumedElements = new ElementConverter.ConsumedElement[] {
                new ElementConverter.ConsumedElement("Water", WATER_INPUT_RATE)
            };
            elementConverter1.outputElements = new ElementConverter.OutputElement[]
            {
                new ElementConverter.OutputElement(DIRTY_WATER_OUTPUT_RATE, SimHashes.DirtyWater,
                    minOutputTemperature: 0.0f,
                    useEntityTemperature: true,
                    storeOutput: true,
                    outputElementOffsetx: 0.0f,
                    outputElementOffsety: 0.5f,
                    0.75f, byte.MaxValue, 0),
                new ElementConverter.OutputElement(ORE_OUTPUT_RATE, SimHashes.IronOre,
                minOutputTemperature: 0.0f,
                    useEntityTemperature: true,
                    storeOutput: false,
                    outputElementOffsetx: 0.0f,
                    outputElementOffsety: 0.5f,
                    1f, byte.MaxValue, 0)
            };

            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.consumptionRate = 10f;
            conduitConsumer.capacityKG = 20f;
            conduitConsumer.capacityTag = GameTags.Water;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.elementFilter = new SimHashes[1] { SimHashes.DirtyWater };
        }
        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {

        }
    }
}
