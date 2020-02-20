using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace ArtifactCabinet
{
    public class ArtifactCabinetPatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ArtifactCabinetConfig.Id.ToUpperInvariant()}.NAME", ArtifactCabinetConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ArtifactCabinetConfig.Id.ToUpperInvariant()}.DESC", ArtifactCabinetConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ArtifactCabinetConfig.Id.ToUpperInvariant()}.EFFECT", ArtifactCabinetConfig.Effect);

                ModUtil.AddBuildingToPlanScreen("Base", ArtifactCabinetConfig.Id);
            }
        }

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            private static void Prefix()
            {
                var basicRocketry = new List<string>(Database.Techs.TECH_GROUPING["BasicRocketry"]) { ArtifactCabinetConfig.Id };
                Database.Techs.TECH_GROUPING["BasicRocketry"] = basicRocketry.ToArray();
            }
        }
    }
}
