using Harmony;

using Klei.AI;

namespace CrystalBiome
{
    public class CrystalBiomePatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{GemTileConfig.Id.ToUpperInvariant()}.NAME", GemTileConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{GemTileConfig.Id.ToUpperInvariant()}.DESC", GemTileConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{GemTileConfig.Id.ToUpperInvariant()}.EFFECT", GemTileConfig.Effect);
                ModUtil.AddBuildingToPlanScreen("Plumbing", GemTileConfig.Id);
            }
        }
    }
}
