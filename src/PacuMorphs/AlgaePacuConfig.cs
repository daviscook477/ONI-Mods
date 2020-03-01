using UnityEngine;
using STRINGS;

namespace PacuMorphs
{
    public class AlgaePacuConfig : IEntityConfig
    {
        public static readonly EffectorValues DECOR = TUNING.BUILDINGS.DECOR.BONUS.TIER4;
        public const string ID = "PacuAlgae";
        public const string BASE_TRAIT_ID = "PacuAlgaeBaseTrait";
        public const string EGG_ID = "PacuAlgaeEgg";
        public const int EGG_SORT_ORDER = 504;

        public static string NAME = UI.FormatAsLink("Overgrown Pacu", ID.ToUpper());
        public static string DESCRIPTION = "Every organism in the known universe finds the Pacu extremely delicious";
        public static string EGG_NAME = UI.FormatAsLink("Overgrown Fry Egg", ID.ToUpper());

        public static GameObject CreatePacu(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby)
        {
            GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BasePacuConfig.CreatePrefab(id, BASE_TRAIT_ID, name, desc, anim_file, is_baby, null, 303.15f, 353.15f), PacuTuning.PEN_SIZE_PER_CREATURE, 25f);
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
