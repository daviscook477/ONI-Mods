using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using STRINGS;

namespace PacuMorphs
{
    public class BabyPlatePacuConfig : IEntityConfig
    {
        public const string ID = "PacuPlateBaby";

        public static string NAME = UI.FormatAsLink("Plated Fry", PlatePacuConfig.ID.ToUpper());
        public static string DESCRIPTION = $"A wriggly little Plated Fry.\n\nIn time it will mature into an adult {UI.FormatAsLink("Plated Pacu", PlatePacuConfig.ID.ToUpper())}.";

        public GameObject CreatePrefab()
        {
            GameObject pacu = PlatePacuConfig.CreatePacu(ID,
                NAME,
                DESCRIPTION,
                "metalbaby_pacu_kanim",
                true);
            EntityTemplates.ExtendEntityToBeingABaby(pacu, PlatePacuConfig.ID, null);
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
