using System.Linq;
using System.Collections.Generic;
using Harmony;
using ProcGen;
using STRINGS;

namespace EthanolGeyser
{
    public class EthanolGeyserPatches
    {

        public const string Id = "chilled_ethanol";
        public static string Name = UI.FormatAsLink("Chilled Ethanol Geyser", $"GeyserGeneric_{Id.ToUpper()}");
        public static string Description = $"A highly pressurized geyser that periodically erupts with {UI.FormatAsLink("Chilled Ethanol", "ETHANOL")}.";

        public const string SubworldPrefix = "subworlds/rust/";
        public const string RequiredGeyserList = "geysers_a";
        public const string EthanolGeyserPOI = "poi_rust_geyser_ethanol";

        [HarmonyPatch(typeof(SettingsCache))]
        [HarmonyPatch("LoadSubworlds")]
        public class SettingsCache_LoadSubworlds_Patch
        {
            private static void Postfix(List<WeightedName> subworlds)
            {
                if (subworlds == null)
                {
                    return;
                }

                foreach (WeightedName subworld in subworlds)
                {
                    string key = subworld.name;
                    if (!key.StartsWith(SubworldPrefix))
                    {
                        continue;
                    }

                    if (SettingsCache.subworlds[key].pointsOfInterest == null)
                    {
                        Traverse.Create(SettingsCache.subworlds[key]).Property("pointsOfInterest").SetValue(new Dictionary<string, string[]>());
                    }
                    if (!SettingsCache.subworlds[key].pointsOfInterest.ContainsKey(RequiredGeyserList))
                    {
                        SettingsCache.subworlds[key].pointsOfInterest[RequiredGeyserList] = new string[] { };
                    }
                    foreach (string poiKey in SettingsCache.subworlds[key].pointsOfInterest.Keys.ToList())
                    {
                        if (!poiKey.StartsWith("geysers_"))
                        {
                            continue;
                        }

                        var geysers = new List<string>(SettingsCache.subworlds[key].pointsOfInterest[poiKey]) { EthanolGeyserPOI };
                        SettingsCache.subworlds[key].pointsOfInterest[poiKey] = geysers.ToArray();
                    }
                }
            }
        }

        [HarmonyPatch(typeof(GeyserGenericConfig))]
        [HarmonyPatch("GenerateConfigs")]
        public class GeyserGenericConfig_GenerateConfigs_Patch
        {
            private static void Postfix(List<GeyserGenericConfig.GeyserPrefabParams> __result)
            {
                Strings.Add($"STRINGS.CREATURES.SPECIES.GEYSER.{Id.ToUpper()}.NAME", Name);
                Strings.Add($"STRINGS.CREATURES.SPECIES.GEYSER.{Id.ToUpper()}.DESC", Description);

                __result.Add(new GeyserGenericConfig.GeyserPrefabParams("geyser_liquid_ethanol_chilled", 4, 2, new GeyserConfigurator.GeyserType(Id, SimHashes.Ethanol, 263.15f, 1000f, 2000f, 500f, 60f, 1140f, 0.1f, 0.9f, 15000f, 135000f, 0.4f, 0.8f)));
            }
        }

    }
}
