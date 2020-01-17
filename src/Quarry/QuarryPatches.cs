using System.Collections.Generic;
using Harmony;

namespace Quarry
{
    public class QuarryPatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{QuarryConfig.Id.ToUpperInvariant()}.NAME", QuarryConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{QuarryConfig.Id.ToUpperInvariant()}.DESC", QuarryConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{QuarryConfig.Id.ToUpperInvariant()}.EFFECT", QuarryConfig.Effect);

                ModUtil.AddBuildingToPlanScreen("Utilities", QuarryConfig.Id);
            }
        }

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            private static void Prefix()
            {
                var valveMiniaturization = new List<string>(Database.Techs.TECH_GROUPING["ValveMiniaturization"]) { QuarryConfig.Id };
                Database.Techs.TECH_GROUPING["ValveMiniaturization"] = valveMiniaturization.ToArray();
            }
        }
    }
}
