using Harmony;

using Klei.AI;

namespace CrystalBiome
{
    public class CrystalBiomePatches
    {
        [HarmonyPatch(typeof(Db), "Initialize")]
        public static class Db_Initialize_Patch
        {
            private static void Postfix(Db __instance)
            {
                ElementLoader.FindElementByHash(Elements.SodaliteElement.SimHash).attributeModifiers.Add(new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, 1000.0f, "words words words", false, false, true));
            }
        }

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
