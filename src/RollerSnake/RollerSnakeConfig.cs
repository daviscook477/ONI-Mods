using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Klei.AI;

namespace RollerSnake
{
    public class RollerSnakeConfig : IEntityConfig
    {
        public const string Id = "RollerSnake";
        public const string Name = "Roller Snake";
        public const string PluralName = "Rolling Snakes";
        public const string Description = "A peculiar critter that moves by winding into a loop and rolling.";
        public const string BaseTraitId = "RollerSnakeBaseTrait";

        public const float Hitpoints = 25f;
        public const float Lifespan = 50f;

        public static int PenSizePerCreature = TUNING.CREATURES.SPACE_REQUIREMENTS.TIER3;
        public const float CaloriesPerCycle = 120000.0f;
        public const float StarveCycles = 5.0f;
        public const float StomachSize = CaloriesPerCycle * StarveCycles;

        public const float KgEatenPerCycle = 140.0f;
        public const float MinPoopSizeInKg = 25.0f;
        public static float CaloriesPerKg = RollerSnakeTuning.STANDARD_CALORIES_PER_CYCLE / KgEatenPerCycle;
        public static float ProducedConversionRate = TUNING.CREATURES.CONVERSION_EFFICIENCY.BAD_1;

        public static float ScaleGrowthTimeCycles = 6.0f;
        public static float GoldAmalganPerCycle = 10.0f;
        public static Tag EmitElement = SimHashes.GoldAmalgam.CreateTag();

        public static GameObject CreateRollerSnake(string id, string name, string desc, string anim_file, bool is_baby)
        {
            GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseRollerSnakeConfig.BaseRollerSnake(id, name, desc, anim_file, BaseTraitId, is_baby, null), RollerSnakeTuning.PEN_SIZE_PER_CREATURE, Lifespan);
            
            Trait trait = Db.Get().CreateTrait(BaseTraitId, name, name, null, false, null, true, true);
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, RollerSnakeTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float)(-RollerSnakeTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, Hitpoints, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, Lifespan, name, false, false, true));
            
            List<Diet.Info> diet_infos = BaseRollerSnakeConfig.BasicRockDiet(
                SimHashes.Carbon.CreateTag(), 
                CaloriesPerKg, 
                ProducedConversionRate, null, 0.0f);
            BaseRollerSnakeConfig.SetupDiet(wildCreature, diet_infos, CaloriesPerKg, MinPoopSizeInKg);

            ScaleGrowthMonitor.Def scale_monitor = wildCreature.AddOrGetDef<ScaleGrowthMonitor.Def>();
            scale_monitor.defaultGrowthRate = (float)(1.0 / ScaleGrowthTimeCycles / 600.0);
            scale_monitor.dropMass = GoldAmalganPerCycle * ScaleGrowthTimeCycles;
            scale_monitor.itemDroppedOnShear = EmitElement;
            scale_monitor.levelCount = 2;
            scale_monitor.targetAtmosphere = SimHashes.Oxygen;

            return wildCreature;
        }
        public GameObject CreatePrefab()
        {
            return (CreateRollerSnake(Id, Name, Description, "rollersnake_kanim", false));
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
