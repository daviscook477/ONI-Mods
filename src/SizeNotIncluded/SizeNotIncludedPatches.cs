using Harmony;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace SizeNotIncluded
{
    public class SizeNotIncludedPatches
    {
        public static float xscale = 2.0f;
        public static float yscale = 2.0f;
        public static float maxDensity = 2.5f;

        [HarmonyPatch(typeof(ProcGen.Worlds), "UpdateWorldCache")]
        public static class Worlds_UpdateWorldCache_Patch
        {
            private static void Postfix(ProcGen.Worlds __instance)
            {
                foreach(ProcGen.World world in __instance.worldCache.Values)
                {
                    Traverse.Create(world).Property("worldsize").SetValue(
                        new Vector2I((int) (256 / xscale), (int) (384 / yscale)));
                }
            }
        }

        [HarmonyPatch(typeof(ProcGen.WorldGenSettings), nameof(ProcGen.WorldGenSettings.GetFloatSetting))]
        public static class WorldGenSettings_GetFloatSetting_Patch
        {
            private static bool Prefix(string target, ref float __result)
            {
                var densityModifier = xscale * yscale;
                if (densityModifier > maxDensity)
                {
                    densityModifier = maxDensity;
                }
                if (target.Equals("OverworldDensityMin"))
                {
                    __result = 600.0f / densityModifier;
                    return false;
                }
                else if (target.Equals("OverworldDensityMax"))
                {
                    __result = 700.0f / densityModifier;
                    return false;
                }
                else if (target.Equals("OverworldAvoidRadius"))
                {
                    __result = 72.0f / densityModifier;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ProcGenGame.Border), "ConvertToMap")]
        public static class Border_ConvertToMap_Patch
        {
            private static void Prefix(ProcGenGame.Border __instance)
            {
                __instance.width = 0.5f;
            }
        }

        [HarmonyPatch(typeof(ProcGen.MobSettings), "GetMob")]
        public static class MobSettings_GetMob_Patch
        {
            private static HashSet<string> patched = new HashSet<string>();

            private static List<string> critters = new List<string>()
            {
                "Hexaped",
                "Puft",
                "PuftBleachstone",
                "LightBug",
                "OilEater",
                "OilFloater",
                "Hatch",
                "Crab",
                "Drecko",
                "Mole",
                "Pacu",
                "Glom",
                "Squirrel",
                "ColdBreather",
                "ColdBreatherSeed",
            };

            private static void Postfix(string id, ref ProcGen.Mob __result)
            {
                if (__result != null)
                {
                    var name = __result.name;
                    var prefabName = __result.prefabName;
                    if (prefabName == null)
                    {
                        prefabName = name;
                    }
                    if (name != null && !patched.Contains(name) && critters.Contains(prefabName))
                    {
                        var densityModifier = xscale * yscale;
                        if (name.Equals("Mole") || prefabName.Equals("Mole"))
                        {
                            densityModifier *= 4.0f;
                        }
                        patched.Add(name);
                        Console.WriteLine("patching density of " + name);
                        Console.WriteLine("pre-patch density: " + __result.density.min + ", " + __result.density.max);
                        typeof(ProcGen.SampleDescriber).GetProperty("density", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                            .SetValue(__result, new ProcGen.MinMax(__result.density.min * densityModifier, __result.density.max * densityModifier), null);
                        Console.WriteLine("pst-patch density: " + __result.density.min + ", " + __result.density.max);
                    }
                }
            }
        }
    }
}
