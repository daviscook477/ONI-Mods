using UnityEngine;
using STRINGS;
using System.Collections.Generic;

namespace PacuMorphs
{
    public class PlatePacuConfig : IEntityConfig
    {
        private static float KG_ORE_EATEN_PER_CYCLE = 140f;
        private static float CALORIES_PER_KG_OF_ORE = PacuTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

        public static readonly EffectorValues DECOR = TUNING.BUILDINGS.DECOR.BONUS.TIER4;
        public const string ID = "PacuPlate";
        public const string BASE_TRAIT_ID = "PacuPlateBaseTrait";
        public const string EGG_ID = "PacuPlateEgg";
        public const int EGG_SORT_ORDER = 505;

        public static string NAME = UI.FormatAsLink("Plated Pacu", ID.ToUpper());
        public static string DESCRIPTION = "Every organism in the known universe finds the Pacu extremely delicious";
        public static string EGG_NAME = UI.FormatAsLink("Plated Fry Egg", ID.ToUpper());

        public const float MIN_TEMP = 243.15f;
        public const float MAX_TEMP = 253.15f;

        public static GameObject CreatePacu(
            string id,
            string name,
            string desc,
            string anim_file,
            bool is_baby)
        {
            GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BasePacuConfig.CreatePrefab(id, BASE_TRAIT_ID, name, desc, anim_file, is_baby, null, MIN_TEMP, MAX_TEMP), PacuTuning.PEN_SIZE_PER_CREATURE, 25f);
            Diet diet = new Diet(new Diet.Info[6]
            {
              new Diet.Info(new HashSet<Tag>()
              {
                SimHashes.Copper.CreateTag(),
              }, SimHashes.Gold.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_1, null, 0.0f, false, false),
              new Diet.Info(new HashSet<Tag>()
              {
                SimHashes.Aluminum.CreateTag(),
              }, SimHashes.Iron.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_1, null, 0.0f, false, false),
              new Diet.Info(new HashSet<Tag>()
              {
                SimHashes.Cuprite.CreateTag(),
              }, SimHashes.GoldAmalgam.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_1, null, 0.0f, false, false),
              new Diet.Info(new HashSet<Tag>()
              {
                SimHashes.AluminumOre.CreateTag(),
              }, SimHashes.IronOre.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_1, null, 0.0f, false, false),
              // this one is refined -> ore b/c otherwise it would be unbalanced by giving the player too easy tungsten
              new Diet.Info(new HashSet<Tag>()
              {
                SimHashes.Lead.CreateTag(),
              }, SimHashes.Wolframite.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_1, null, 0.0f, false, false),
              new Diet.Info(new HashSet<Tag>()
              {
                SimHashes.Regolith.CreateTag(),
              }, SimHashes.Salt.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_1, null, 0.0f, false, false)
            });
            wildCreature.GetDef<CreatureCalorieMonitor.Def>().diet = diet;
            wildCreature.GetDef<SolidConsumerMonitor.Def>().diet = diet;
            return wildCreature;
        }

        public GameObject CreatePrefab()
        {
            return EntityTemplates.ExtendEntityToFertileCreature(
                EntityTemplates.ExtendEntityToWildCreature(
                    CreatePacu(ID,
                    NAME,
                    DESCRIPTION,
                    "metalpacu_kanim",
                    false),
                    PacuTuning.PEN_SIZE_PER_CREATURE,
                    25f),
                EGG_ID,
                EGG_NAME,
                DESCRIPTION,
                "egg_metalpacu_kanim",
                PacuTuning.EGG_MASS,
                BabyPlatePacuConfig.ID,
                15f,
                5f,
                CustomPacuTuning.EGG_CHANCES_PLATE,
                EGG_SORT_ORDER,
                false,
                true,
                false,
                0.75f);
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
