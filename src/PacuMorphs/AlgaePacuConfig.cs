using UnityEngine;
using STRINGS;
using System.Collections.Generic;   

namespace PacuMorphs
{
    public class AlgaePacuConfig : IEntityConfig
    {
        private static float KG_ORE_EATEN_PER_CYCLE = 140f;
        private static float CALORIES_PER_KG_OF_ORE = PacuTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

        public static readonly EffectorValues DECOR = TUNING.BUILDINGS.DECOR.BONUS.TIER4;
        public const string ID = "PacuAlgae";
        public const string BASE_TRAIT_ID = "PacuAlgaeBaseTrait";
        public const string EGG_ID = "PacuAlgaeEgg";
        public const int EGG_SORT_ORDER = 504;

        public static string NAME = UI.FormatAsLink("Kelp Pacu", ID.ToUpper());
        public static string DESCRIPTION = "Every organism in the known universe finds the Pacu extremely delicious";
        public static string EGG_NAME = UI.FormatAsLink("Kelp Fry Egg", ID.ToUpper());

        public const float MIN_TEMP = 333.15f;
        public const float MAX_TEMP = 353.15f;

        public static GameObject CreatePacu(
            string id,
            string name,
            string desc,
            string anim_file,
            bool is_baby)
        {
            GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BasePacuConfig.CreatePrefab(id, BASE_TRAIT_ID, name, desc, anim_file, is_baby, null, MIN_TEMP, MAX_TEMP), PacuTuning.PEN_SIZE_PER_CREATURE, 25f);
            Diet diet = new Diet(new Diet.Info[1]
            {
              new Diet.Info(new HashSet<Tag>()
              {
                SimHashes.Phosphorite.CreateTag(),
                SimHashes.Fertilizer.CreateTag(),
              }, SimHashes.Algae.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0.0f, false, false)
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
                    "algaepacu_kanim",
                    false),
                    PacuTuning.PEN_SIZE_PER_CREATURE,
                    25f),
                EGG_ID,
                EGG_NAME,
                DESCRIPTION,
                "egg_algaepacu_kanim",
                PacuTuning.EGG_MASS,
                BabyAlgaePacuConfig.ID,
                15f,
                5f,
                CustomPacuTuning.EGG_CHANCES_ALGAE,
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
