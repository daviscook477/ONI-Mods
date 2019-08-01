using System.Collections.Generic;

using TUNING;
using UnityEngine;

namespace CrystalBiome.Buildings
{
    public class MineralizerConfig : IBuildingConfig
    {
        public const string Id = "Mineralizer";
        public const string DisplayName = "Mineralizer";
        public static string Description = $"Washing minerals with water dissolves them and creates {STRINGS.UI.FormatAsLink("Mineral Water", Elements.MineralWaterElement.Id)}.";
        public static string Effect = $"Adds minerals to {STRINGS.UI.FormatAsLink("Water", "WATER")}, producing {STRINGS.UI.FormatAsLink("Mineral Water", Elements.MineralWaterElement.Id)}.";

        private const float MINERAL_INPUT_RATE = 1.5f;
        private const float WATER_WITH_MINERAL_INPUT_RATE = 3.5f;
        private const float OUTPUT_RATE = 5.0f;

        public override BuildingDef CreateBuildingDef()
        {

            var buildingDef = BuildingTemplates.CreateBuildingDef(
                id: Id,
                width: 4,
                height: 2,
                anim: "mineralizer",
                hitpoints: BUILDINGS.HITPOINTS.TIER2,
                construction_time: BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER2,
                construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER3,
                construction_materials: MATERIALS.ALL_METALS,
                melting_point: BUILDINGS.MELTING_POINT_KELVIN.TIER0,
                build_location_rule: BuildLocationRule.OnFloor,
                decor: BUILDINGS.DECOR.NONE,
                noise: NOISE_POLLUTION.NOISY.TIER2,
                0.2f
                );
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 360f;
            buildingDef.ExhaustKilowattsWhenActive = 8f;
            buildingDef.SelfHeatKilowattsWhenActive = 0f;
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.Floodable = false;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
            buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
            Storage storage = go.AddOrGet<Storage>();
            storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            storage.showInUI = true;
            storage.capacityKg = 600 * MINERAL_INPUT_RATE;
            go.AddOrGet<LoopingSounds>();
            go.AddOrGet<Mineralizer>();
            Prioritizable.AddRef(go);
            ElementConverter elementConverter1 = go.AddComponent<ElementConverter>();
            elementConverter1.consumedElements = new ElementConverter.ConsumedElement[2]
            {
                new ElementConverter.ConsumedElement("Crystal", MINERAL_INPUT_RATE),
                new ElementConverter.ConsumedElement("Water", WATER_WITH_MINERAL_INPUT_RATE)
            };
            elementConverter1.outputElements = new ElementConverter.OutputElement[1]
            {
              new ElementConverter.OutputElement(OUTPUT_RATE, Elements.MineralWaterElement.SimHash, 0.0f, false, true, 0.0f, 0.5f, 0.75f, byte.MaxValue, 0),
            };

            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(storage);
            manualDeliveryKg.requestedItemTag = new Tag("Crystal");
            manualDeliveryKg.capacity = storage.capacityKg;
            manualDeliveryKg.refillMass = 100 * MINERAL_INPUT_RATE;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;

            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.consumptionRate = 10f;
            conduitConsumer.capacityKG = 4 * WATER_WITH_MINERAL_INPUT_RATE;
            conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.elementFilter = new SimHashes[1] { Elements.MineralWaterElement.SimHash };
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
        }
    }
}
