using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace Mineralizer
{
    public class MineralizerPatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{MineralizerConfig.Id.ToUpperInvariant()}.NAME", MineralizerConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{MineralizerConfig.Id.ToUpperInvariant()}.DESC", MineralizerConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{MineralizerConfig.Id.ToUpperInvariant()}.EFFECT", MineralizerConfig.Effect);

                ModUtil.AddBuildingToPlanScreen("Refining", MineralizerConfig.Id);
            }
        }

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            //private static void Prefix()
            //{
            //    var liquidFiltering = new List<string>(Database.Techs.TECH_GROUPING["LiquidFiltering"]) { MineralizerConfig.Id };
            //    Database.Techs.TECH_GROUPING["LiquidFiltering"] = liquidFiltering.ToArray();
            //}
            private static void Postfix()
            {
                Db.Get().Techs.Get("LiquidFiltering").unlockedItemIDs.Add(MineralizerConfig.Id);
            }
        }
    }
}
