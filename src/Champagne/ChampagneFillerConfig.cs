using TUNING;
using UnityEngine;
using STRINGS;

namespace Champagne
{
    public class ChampagneFillerConfig : IBuildingConfig
    {
        public const string Id = "ChampagneFiller";
        public const string DisplayName = "Champagne Filler";
        public static string Description = $"Creates champagne from {UI.FormatAsLink("Ethanol", "ETHANOL")} and {UI.FormatAsLink("Grapeberries", GrapeberryConfig.Id.ToUpper())}. Drinking a glass of champagne improves Duplicant {UI.FormatAsLink("Morale", "MORALE")}.";
        public static string Effect = $"Sparkling wine helps the Duplicants to feel alright.";

        public override BuildingDef CreateBuildingDef()
        {
            string id = Id;
            int width = 2;
            int height = 2;
            string anim = "champagnefiller_kanim";
            int hitpoints = 30;
            float construction_time = 10f;
            float[] tieR3 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            float melting_point = 1600f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues none = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tieR3, refinedMetals, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.BONUS.TIER1, none, 0.2f);
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.Floodable = true;
            buildingDef.AudioCategory = "Metal";
            buildingDef.Overheatable = true;
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.UtilityInputOffset = new CellOffset(1, 1);
            buildingDef.RequiresPowerInput = true;
            buildingDef.PowerInputOffset = new CellOffset(1, 0);
            buildingDef.EnergyConsumptionWhenActive = 480f;
            buildingDef.SelfHeatKilowattsWhenActive = 1f;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RecBuilding, false);
            Storage storage = go.AddOrGet<Storage>();
            storage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Ethanol).tag;
            conduitConsumer.capacityKG = 20f;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(storage);
            manualDeliveryKg.requestedItemTag = GrapeberryConfig.Id.ToTag();
            manualDeliveryKg.capacity = 4f;
            manualDeliveryKg.refillMass = 1f;
            manualDeliveryKg.minimumMass = 0.5f;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
            go.AddOrGet<ChampagneFillerWorkable>().basePriority = RELAXATION.PRIORITY.TIER5;
            ChampagneFiller champagneFiller = go.AddOrGet<ChampagneFiller>();
            champagneFiller.specificEffect = "SodaFountain";
            champagneFiller.trackingEffect = "RecentlyRecDrink";
            champagneFiller.ingredientTag = GrapeberryConfig.Id.ToTag();
            champagneFiller.ingredientMassPerUse = 1f;
            champagneFiller.ethanolMassPerUse = 5f;
            RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
            roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
            roomTracker.requirement = RoomTracker.Requirement.Recommended;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }

}