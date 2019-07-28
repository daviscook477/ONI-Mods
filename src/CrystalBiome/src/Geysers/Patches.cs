using System.Linq;
using System.Collections.Generic;

using Harmony;

using ProcGen;

namespace CrystalBiome.Geysers
{
    public class Patches
    {
        private class GeyserData
        {
            public string GeyserList;
            public string GeyserPOI;

            public GeyserData(string GeyserList, string GeyserPOI)
            {
                this.GeyserList = GeyserList;
                this.GeyserPOI = GeyserPOI;
            }
        }

        public const string SubworldPrefix = "subworlds/crystal/";
        private static List<GeyserData> geyserOptions = new List<GeyserData>
        {
            new GeyserData("geysers_a", "poi_crystal_geyser_mineral_water_a"),
            new GeyserData("geysers_b", "poi_crystal_geyser_mineral_water_b"),
            new GeyserData("geysers_c", "poi_crystal_geyser_mineral_water_c")
        };

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
                    foreach (GeyserData geyserData in geyserOptions)
                    {
                        if (!SettingsCache.subworlds[key].pointsOfInterest.ContainsKey(geyserData.GeyserList))
                        {
                            SettingsCache.subworlds[key].pointsOfInterest[geyserData.GeyserList] = new string[] { };
                        }
                    }
                    foreach (string poiKey in SettingsCache.subworlds[key].pointsOfInterest.Keys.ToList())
                    {
                        foreach (GeyserData geyserData in geyserOptions)
                        {
                            if (poiKey.Equals(geyserData.GeyserList))
                            {
                                var geysers = new List<string>(SettingsCache.subworlds[key].pointsOfInterest[poiKey]) { geyserData.GeyserPOI };
                                SettingsCache.subworlds[key].pointsOfInterest[poiKey] = geysers.ToArray();
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(GeyserGenericConfig), "GenerateConfigs")]
        public class GeyserGenericConfig_GenerateConfigs_Patch
        {
            private static void Postfix(List<GeyserGenericConfig.GeyserPrefabParams> __result)
            {
                Strings.Add($"STRINGS.CREATURES.SPECIES.GEYSER.{MineralWaterGeyser.Id.ToUpper()}.NAME", MineralWaterGeyser.Name);
                Strings.Add($"STRINGS.CREATURES.SPECIES.GEYSER.{MineralWaterGeyser.Id.ToUpper()}.DESC", MineralWaterGeyser.Description);

                __result.Add(new GeyserGenericConfig.GeyserPrefabParams("geyser_mineral_water", 4, 2, 
                    new GeyserConfigurator.GeyserType(id: MineralWaterGeyser.Id, 
                        element: Elements.MineralWaterElement.SimHash,
                        temperature: 368.15f, 
                        minRatePerCycle: 2000f,
                        maxRatePerCycle: 4000f, 
                        maxPressure: 500f)));
            }
        }

    }
}
