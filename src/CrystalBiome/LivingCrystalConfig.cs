using System;
using System.Collections.Generic;
using UnityEngine;
using Klei.AI;

namespace CrystalBiome
{
    public class LivingCrystalConfig : IEntityConfig
    {

        public const string Id = "LivingCrystal";
        public const string Name = "Cryolite Composite";
        public const string PluralName = "Living Crystals";
        public const string Description = "A floating, moving, thinking pile of rocks. Draws thermal energy from pools of ethanol to power itself.";
        public const string BaseTraitId = "LivingCrystalBaseTrait";

        public const float Hitpoints = 75.0f;
        public const float Lifespan = 75.0f;
        public const string NavGridId = "WalkerNavGrid1x1";
        public const float Mass = 100.0f;
        public const float DefaultTemperature = 283.15f;
        public const float LethalLowTemperature = 100.0f;
        public const float WarningLowTemperature = 123.15f;
        public const float WarningHighTemperature = 328.15f;
        public const float LethalHighTemperature = 348.15f;

        public static int PenSizePerCreature = TUNING.CREATURES.SPACE_REQUIREMENTS.TIER3;
        public const float CaloriesPerCycle = 120000.0f;
        public const float StarveCycles = 5.0f;
        public const float StomachSize = CaloriesPerCycle * StarveCycles;

        public const float KgEatenPerCycle = 490.0f;
        public const float EatTimesPerCycle = 3.0f;
        public const float CaloriesPerKg = CaloriesPerCycle / KgEatenPerCycle;
        public const float ProducedConversionRate = 1.0f;
        public const float OutputTemperatureDelta = -15.0f;
        public const float MinPoopSizeInKg = 0.5f;

        private static bool hasGeneratedBaseTrait = false;
        private static object _lock = new object();

        public static void GenerateBaseTrait()
        {
            lock(_lock)
            {
                if (hasGeneratedBaseTrait)
                {
                    return;
                }
                Trait trait = Db.Get().CreateTrait(BaseTraitId, Id, Id, null, false, null, true, true);
                trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, StomachSize, Id, false, false, true));
                trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float)(-CaloriesPerCycle / 600.0), Id, false, false, true));
                trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, Hitpoints, Id, false, false, true));
                trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, Lifespan, Id, false, false, true));
                hasGeneratedBaseTrait = true;
            }
        }

        private static int AdjustSpawnLocation(int cell)
        {
            int nextCell;
            for (; !Grid.Solid[cell]; cell = nextCell)
            {
                nextCell = Grid.CellBelow(cell);
                if (!Grid.IsValidCell(cell))
                    break;
            }
            return cell;
        }

        private static GameObject GeneratePlacedEntity()
        {
            GameObject placedEntity = EntityTemplates.CreatePlacedEntity(
                id: Id, 
                name: Name, 
                desc: Description, 
                mass: Mass, 
                anim: Assets.GetAnim("living_crystal"), 
                initialAnim: "idle_loop", 
                sceneLayer: Grid.SceneLayer.Creatures,
                width: 1,
                height: 2, 
                decor: TUNING.DECOR.BONUS.TIER3,
                noise: new EffectorValues(),
                element: SimHashes.Creature,
                additionalTags: null,
                defaultTemperature: DefaultTemperature);
            EntityTemplates.ExtendEntityToBasicCreature(
                template: placedEntity,
                faction: FactionManager.FactionID.Prey,
                initialTraitID: BaseTraitId,
                NavGridName: NavGridId,
                navType: NavType.Floor,
                max_probing_radius: 32,
                moveSpeed: 2f,
                onDeathDropID: "Meat",
                onDeathDropCount: 3,
                drownVulnerable: false,
                entombVulnerable: true,
                warningLowTemperature: WarningLowTemperature,
                warningHighTemperature: WarningHighTemperature,
                lethalLowTemperature: LethalLowTemperature,
                lethalHighTemperature: LethalHighTemperature);
            placedEntity.AddOrGet<Trappable>();
            placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();
            WorldSpawnableMonitor.Def def = placedEntity.AddOrGetDef<WorldSpawnableMonitor.Def>();
            def.adjustSpawnLocationCb = AdjustSpawnLocation;
            placedEntity.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Scuffed;
            placedEntity.AddOrGetDef<RanchableMonitor.Def>();
            EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, true, false);
            KPrefabID component = placedEntity.GetComponent<KPrefabID>();
            component.AddTag(GameTags.Creatures.Walker, false);
            ChoreTable.Builder chore_table = new ChoreTable.Builder()
                .Add(new DeathStates.Def(), true)
                .Add(new AnimInterruptStates.Def(), true)
                .Add(new TrappedStates.Def(), true)
                .Add(new BaggedStates.Def(), true)
                .Add(new FallStates.Def(), true)
                .Add(new StunnedStates.Def(), true)
                .Add(new DebugGoToStates.Def(), true)
                .Add(new FleeStates.Def(), true)
                .Add(new CreatureSleepStates.Def(), true).PushInterruptGroup()
                .Add(new FixedCaptureStates.Def(), true)
                .Add(new RanchedStates.Def(), true)
                .Add(new LayEggStates.Def(), true)
                .Add(new InhaleStates.Def(), true)
                .Add(new SameSpotPoopStates.Def(), true)
                .Add(new CallAdultStates.Def(), true).PopInterruptGroup()
                .Add(new IdleStates.Def(), true);
            EntityTemplates.AddCreatureBrain(placedEntity, chore_table, Id.ToTag(), null);
            return placedEntity;
        }

        public static GameObject CreateLivingCrystal()
        {
            GameObject placedEntity = GeneratePlacedEntity();
            EntityTemplates.ExtendEntityToWildCreature(placedEntity, PenSizePerCreature, Lifespan);
            Diet diet = new Diet(new Diet.Info[1]
            {
              new Diet.Info(new HashSet<Tag>()
              {
                ElementLoader.FindElementByHash(SimHashes.Ethanol).tag
              }, ElementLoader.FindElementByHash(SimHashes.SolidEthanol).tag, CaloriesPerKg, ProducedConversionRate, null, 0.0f, false)
            });
            CreatureCalorieMonitor.Def calorieMonitor = placedEntity.AddOrGetDef<CreatureCalorieMonitor.Def>();
            calorieMonitor.diet = diet;
            calorieMonitor.minPoopSizeInCalories = MinPoopSizeInKg * CaloriesPerKg;
            GasAndLiquidConsumerMonitor.Def consumerMonitor = placedEntity.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>();
            consumerMonitor.diet = diet;
            consumerMonitor.consumptionRate = KgEatenPerCycle / EatTimesPerCycle;
            placedEntity.AddOrGet<LivingCrystal>().Initialize();
            return placedEntity;
        }

        public GameObject CreatePrefab()
        {
            GenerateBaseTrait();
            return CreateLivingCrystal();
        }

        public void OnPrefabInit(GameObject prefab) { }

        public void OnSpawn(GameObject inst) { }
    }
}
