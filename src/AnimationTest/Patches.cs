using System;
using System.Collections.Generic;

using Harmony;
using UnityEngine;

using STRINGS;

namespace CrystalBiome.Buildings
{
    public class Patches
    {   

        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{RockPolisherConfig.Id.ToUpperInvariant()}.NAME", RockPolisherConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{RockPolisherConfig.Id.ToUpperInvariant()}.DESC", RockPolisherConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{RockPolisherConfig.Id.ToUpperInvariant()}.EFFECT", RockPolisherConfig.Effect);

                ModUtil.AddBuildingToPlanScreen("Refining", RockPolisherConfig.Id);
            }
        }

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            private static void Prefix()
            {
                var basicRefinement = new List<string>(Database.Techs.TECH_GROUPING["BasicRefinement"]) { RockPolisherConfig.Id};
                Database.Techs.TECH_GROUPING["BasicRefinement"] = basicRefinement.ToArray();
            }
        }
    }
}
