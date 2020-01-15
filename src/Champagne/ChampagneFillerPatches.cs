using System.Collections.Generic;
using Harmony;
using System;
using UnityEngine;

namespace Champagne
{
    public class ChampagneFillerPatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ChampagneFillerConfig.Id.ToUpperInvariant()}.NAME", ChampagneFillerConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ChampagneFillerConfig.Id.ToUpperInvariant()}.DESC", ChampagneFillerConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ChampagneFillerConfig.Id.ToUpperInvariant()}.EFFECT", ChampagneFillerConfig.Effect);

                ModUtil.AddBuildingToPlanScreen("Refining", ChampagneFillerConfig.Id);
            }
        }

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            private static void Prefix()
            {
                var basicRefinement = new List<string>(Database.Techs.TECH_GROUPING["LiquidFiltering"]) { ChampagneFillerConfig.Id };
                Database.Techs.TECH_GROUPING["LiquidFiltering"] = basicRefinement.ToArray();
            }
        }

        [HarmonyPatch(typeof(Worker), "AttachOverrideAnims")]
        public class AAAA
        {
            private static void Prefix(Worker __instance)
            {
                Workable.AnimInfo animInfo = Traverse.Create(__instance).Field("animInfo").GetValue<Workable.AnimInfo>();
                if (animInfo.overrideAnims == null || animInfo.overrideAnims.Length <= 0)
                    Console.WriteLine("Tried to AttachOverrideAnims but was null");
                for (int index = 0; index < animInfo.overrideAnims.Length; ++index)
                    Console.WriteLine($"Attaching override {animInfo.overrideAnims[index].name}");
            }
        }

        [HarmonyPatch(typeof(KAnimControllerBase), "AddAnimOverrides")]
        public class BBBB
        {
            private static void Postfix(KAnimControllerBase __instance, KAnimFile kanim_file)
            {
                if (kanim_file.GetData().build != null && kanim_file.GetData().build.symbols.Length > 0)
                {
                    Console.WriteLine($"kanim_file has build data and requires a build override for {kanim_file.name}");
                }
                var overrideAnimFiles = Traverse.Create(__instance).Field("overrideAnimFiles").GetValue<List<KAnimControllerBase.OverrideAnimFileData>>();
                Console.WriteLine("After adding override has list:");
                foreach (var animFile in overrideAnimFiles)
                {
                    Console.WriteLine($"{animFile.file.name} @ priority {animFile.priority}");
                }
            }
        }

        [HarmonyPatch(typeof(KAnimControllerBase), "RebuildOverrides")]
        public class CCCC
        {
            private static void Prefix(KAnimControllerBase __instance, KAnimFile kanim_file)
            {
                if (__instance == null)
                {
                    Console.WriteLine("RebuildOverrides on null instance... returning");
                    return;
                }
                bool flag = false;
                var overrideAnims = Traverse.Create(__instance).Field("overrideAnims").GetValue<Dictionary<HashedString, KAnimControllerBase.AnimLookupData>>();
                var overrideAnimFiles = Traverse.Create(__instance).Field("overrideAnimFiles").GetValue<List<KAnimControllerBase.OverrideAnimFileData>>();
                for (int index1 = 0; index1 < overrideAnimFiles.Count; ++index1)
                {
                    KAnimControllerBase.OverrideAnimFileData overrideAnimFile = overrideAnimFiles[index1];
                    KAnimFileData data = overrideAnimFile.file.GetData();
                    for (int index2 = 0; index2 < data.animCount; ++index2)
                    {
                        KAnim.Anim anim = data.GetAnim(index2);
                        if (anim.animFile.hashName != data.hashName)
                            Console.WriteLine((object)string.Format("How did we get an anim from another file? [{0}] != [{1}] for anim [{2}]", (object)data.name, (object)anim.animFile.name, (object)index2));
                        KAnimControllerBase.AnimLookupData animLookupData = new KAnimControllerBase.AnimLookupData();
                        animLookupData.animIndex = anim.index;
                        HashedString key = new HashedString(anim.name);
                        if (!overrideAnims.ContainsKey(key))
                        {
                            Console.WriteLine($"The list of override anims did not contain {anim.name} so we are adding it with the lookup data of index {animLookupData.animIndex}");
                            overrideAnims[key] = animLookupData;
                            Console.WriteLine($"But it turns out that the animation at index {animLookupData.animIndex} is actually {__instance.GetAnim(animLookupData.animIndex).name} for {__instance.GetAnim(animLookupData.animIndex).animFile.name}");
                        }
                        var curAnim2 = Traverse.Create(__instance).Field("curAnim").GetValue<KAnim.Anim>();
                        if (curAnim2 != null && curAnim2.hash == key && (UnityEngine.Object)overrideAnimFile.file == (UnityEngine.Object)kanim_file)
                            flag = true;
                    }
                }
                var curAnim = Traverse.Create(__instance).Field("curAnim").GetValue<KAnim.Anim>();
                if (!flag)
                {
                    Console.WriteLine($"Does not need to restart the anim {curAnim?.name}");
                    return;
                }
                Console.WriteLine($"Restarting {curAnim?.name} in mode {__instance.GetMode()}");
            }
        }

        [HarmonyPatch(typeof(AnimCommandFile), "GetGroupName")]
        public class DDDD
        {
            public static void Postfix(KAnimFile kaf, string __result)
            {
                Console.WriteLine($"group for {kaf.name} is {__result}");
            }
        }

        [HarmonyPatch(typeof(KGlobalAnimParser), "ParseBuildData")]
        public class EEEE
        {
            public static void Prefix(KBatchGroupData data,
    KAnimHashedString fileNameHash)
            {
                
                KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(data.groupID);
                Console.WriteLine($"For {fileNameHash.ToString()} the group information is id: {data.groupID.HashValue} and directory: {group.commandDirectory}");
            }
        }

        [HarmonyPatch(typeof(ModUtil), "AddKAnimMod")]
        public class FFFF
        {
            public static bool Prefix(string name, KAnimFile.Mod anim_mod)
            {
                if (!name.Equals("anim_interacts_champagnefiller_kanim"))
                {
                    return true;
                }
                Console.WriteLine($"Manually adding {name} to the interactions group.");
                KAnimFile instance = ScriptableObject.CreateInstance<KAnimFile>();
                instance.mod = anim_mod;
                instance.name = name;
                KAnimGroupFile groupFile = KAnimGroupFile.GetGroupFile();
                HashedString groupId = new HashedString(296858093);
                List<KAnimGroupFile.Group> groups = Traverse.Create(groupFile).Field("groups").GetValue<List<KAnimGroupFile.Group>>();
                int num = groups.FindIndex(t => t.id == groupId);
                groups[num].files.Add(instance);
                Assets.ModLoadedKAnims.Add(instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(KAnimGroupFile), "LoadAll")]
        public class GGGG
        {
            public static void Prefix()
            {
                Console.WriteLine("Loading all right now");
            }
        }

        [HarmonyPatch(typeof(KGlobalAnimParser), "ParseAnimData")]
        public class HHH
        {
            public static void Prefix(KBatchGroupData data,
    HashedString fileNameHash,
    FastReader reader,
    KAnimFileData animFile)
            {
                Console.WriteLine($"writing to {data.groupID.HashValue} the anim named {animFile.name.ToString()}");
            }
        }
    }
}
