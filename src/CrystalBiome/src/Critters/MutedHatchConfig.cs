using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

using STRINGS;

namespace CrystalBiome.Critters
{

    public class HatchMutedConfig : IEntityConfig
    {
        public const float KgFoodEatenPerCycle = 140f;
        public static float CaloriesPerKgOfFood = HatchTuning.STANDARD_CALORIES_PER_CYCLE / KgFoodEatenPerCycle;
        public const float MinPoopSizeKg = 50f;
        public static int EggSortOrder = HatchConfig.EGG_SORT_ORDER + 3; // so the base hatches are +0, +1, and +2
        public const SimHashes EmitElement = SimHashes.Carbon;
        public const string BaseTraitId = "HatchMutedBaseTrait";
        public const float FertilityCycles = 60.0f;
        public const float IncubationCycles = 20.0f;
        public const float MaxAge = 100.0f;
        public const float Hitpoints = 25.0f;
        public const string SymbolOverride = "veg_";

        public static List<Diet.Info> MutedDiet(
            Tag poopTag,
            float caloriesPerKg,
            float producedConversionRate,
            string diseaseId = null,
            float diseasePerKgProduced = 0.0f)
        {
            return new List<Diet.Info>()
            {
              new Diet.Info(new HashSet<Tag>()
              {
                SimHashes.Diamond.CreateTag(),
                SimHashes.Fertilizer.CreateTag(),
              }, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false, false)
            };
        }

        public static Trait CreateTrait(string name)
        {
            Trait trait = Db.Get().CreateTrait(BaseTraitId, name, name, null, false, null, true, true);
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, HatchTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float)(-HatchTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, Hitpoints, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, MaxAge, name, false, false, true));
            return trait;
        }

        public static GameObject CreateHatch(
          string id,
          string name,
          string desc,
          string anim_file,
          bool is_baby)
        {
            GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(
                BaseHatchConfig.BaseHatch(
                    id,
                    name,
                    desc,
                    anim_file,
                    BaseTraitId,
                    is_baby,
                    SymbolOverride
                ), 
                HatchTuning.PEN_SIZE_PER_CREATURE,
                MaxAge);
            CreateTrait(name);

            List<Diet.Info> diet_infos = MutedDiet(
                poopTag: EmitElement.CreateTag(),
                caloriesPerKg: CaloriesPerKgOfFood,
                producedConversionRate: TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_3);
            return BaseHatchConfig.SetupDiet(wildCreature, diet_infos, CaloriesPerKgOfFood, MinPoopSizeKg);
        }

        public static List<FertilityMonitor.BreedingChance> EggChances = new List<FertilityMonitor.BreedingChance>()
        {
            new FertilityMonitor.BreedingChance()
            {
                egg = "HatchEgg".ToTag(),
                weight = 0.33f
            },
            new FertilityMonitor.BreedingChance()
            {
                egg = EggId.ToTag(),
                weight = 0.67f
            }
        };

        public const string Id = "HatchMuted";
        public static string Name = UI.FormatAsLink("Muted Hatch", Id);
        public const string Description = "Not very colorful.";
        public const string EggId = "HatchMutedEgg";
        public static string EggName = UI.FormatAsLink("Muted Hatchling Egg", EggId);
        public const string BabyId = "HatchMutedBaby";
        public static string BabyName = UI.FormatAsLink("Muted Hatchling", BabyId);
        public const string BabyDescription = "Not very colorful but very cute.";

        public GameObject CreatePrefab()
        {
            return EntityTemplates.ExtendEntityToFertileCreature(
                CreateHatch(
                    id: Id,
                    name: Name,
                    desc: Description,
                    anim_file: "hatch_new", // this is your new hatch anim - it should be made from the unmodified anim + build but your modified texture/png file.
                    is_baby: false
                ),
                EggId,
                EggName,
                Description,
                "egg_hatch_kanim", // replace this with your egg anim
                HatchTuning.EGG_MASS,
                BabyId,
                FertilityCycles,
                IncubationCycles,
                EggChances,
                HatchVeggieConfig.EGG_SORT_ORDER);
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }

}
