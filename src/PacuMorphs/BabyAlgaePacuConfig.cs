using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using STRINGS;

namespace PacuMorphs
{
    public class BabyAlgaePacuConfig : IEntityConfig
    {
        public const string ID = "PacuAlgaeBaby";

        public static string NAME = UI.FormatAsLink("Kelp Fry", AlgaePacuConfig.ID.ToUpper());
        public static string DESCRIPTION = $"A wriggly little Kelp Fry.\n\nIn time it will mature into an adult {UI.FormatAsLink("Kelp Pacu", AlgaePacuConfig.ID.ToUpper())}.";

        public GameObject CreatePrefab()
        {
            GameObject pacu = AlgaePacuConfig.CreatePacu(ID,
                NAME,
                DESCRIPTION,
                "algaebaby_pacu_kanim",
                true);
            EntityTemplates.ExtendEntityToBeingABaby(pacu, AlgaePacuConfig.ID, null);
            return pacu;
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
