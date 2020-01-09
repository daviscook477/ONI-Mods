using System;
using System.Collections.Generic;
using System.IO;
using Harmony;
using Klei;

namespace Codex
{
    public class CodexPatches
    {
        [HarmonyPatch(typeof(CodexCache), nameof(CodexCache.CollectEntries))]
        public class CodexCache_CollectEntries_Patch
        {
            private static void YamlParseErrorCB(YamlIO.Error error, bool force_log_as_warning)
            {
                throw new Exception(string.Format("{0} parse error in {1}\n{2}", error.severity, error.file.full_path, error.message), error.inner_exception);
            }

            public static void Postfix(string folder, List<CodexEntry> __result)
            {
                // check to see if we are loading the "Creatures" directory for the codex
                string baseEntryPath = Traverse.Create(typeof(CodexCache)).Field("baseEntryPath").GetValue<string>();
                string path = !string.IsNullOrEmpty(folder) ? Path.Combine(baseEntryPath, folder) : baseEntryPath;
                if (!path.EndsWith("Creatures") && !path.EndsWith("Plants"))
                {
                    return;
                }
                string pathEnd = "/codex/Creatures";
                if (path.EndsWith("Plants"))
                {
                    pathEnd = "/codex/Plants";
                }
                string assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                // remove "file:\
                assemblyPath = assemblyPath.Substring("file:\\".Length);
                string[] strArray = new string[0];
                try
                {
                    strArray = Directory.GetFiles(assemblyPath + pathEnd, "*.yaml");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.LogWarning(ex);
                }
                string upper = folder.ToUpper();
                foreach (string str in strArray)
                {
                    try
                    {
                        string filename = str;
                        YamlIO.ErrorHandler fMgCache0 = new YamlIO.ErrorHandler(YamlParseErrorCB);
                        List<Tuple<string, Type>> widgetTagMappings = Traverse.Create(typeof(CodexCache)).Field("widgetTagMappings").GetValue<List<Tuple<string, Type>>>();
                        CodexEntry codexEntry = YamlIO.LoadFile<CodexEntry>(filename, fMgCache0, widgetTagMappings);
                        if (codexEntry != null)
                        {
                            codexEntry.category = upper;
                            __result.Add(codexEntry);
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugUtil.DevLogErrorFormat("CodexCache.CollectEntries failed to load [{0}]: {1}", str, ex.ToString());
                    }
                }
                foreach (CodexEntry codexEntry in __result)
                {
                    if (string.IsNullOrEmpty(codexEntry.sortString))
                        codexEntry.sortString = Strings.Get(codexEntry.title);
                }
                __result.Sort((x, y) => x.sortString.CompareTo(y.sortString));
            }
        }

        [HarmonyPatch(typeof(CodexCache), nameof(CodexCache.CollectSubEntries))]
        public class CodexCache_CollectSubEntries_Patch
        {
            private static void YamlParseErrorCB(YamlIO.Error error, bool force_log_as_warning)
            {
                throw new Exception(string.Format("{0} parse error in {1}\n{2}", error.severity, error.file.full_path, error.message), error.inner_exception);
            }

            public static void Postfix(string folder, List<SubEntry> __result)
            {
                string assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                // remove "file:\
                assemblyPath = assemblyPath.Substring("file:\\".Length);
                string[] strArray = new string[0];
                try
                {
                    strArray = Directory.GetFiles(assemblyPath + "/codex", "*.yaml", SearchOption.AllDirectories);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.LogWarning(ex);
                }
                foreach (string str in strArray)
                {
                    try
                    {
                        string filename = str;
                        YamlIO.ErrorHandler fMgCache1 = new YamlIO.ErrorHandler(YamlParseErrorCB);
                        List<Tuple<string, Type>> widgetTagMappings = Traverse.Create(typeof(CodexCache)).Field("widgetTagMappings").GetValue<List<Tuple<string, Type>>>();
                        SubEntry subEntry = YamlIO.LoadFile<SubEntry>(filename, fMgCache1, widgetTagMappings);
                        if (subEntry != null)
                            __result.Add(subEntry);
                    }
                    catch (Exception ex)
                    {
                        DebugUtil.DevLogErrorFormat("CodexCache.CollectSubEntries failed to load [{0}]: {1}", str, ex.ToString());
                    }
                }
                __result.Sort((x, y) => x.title.CompareTo(y.title));
            }
        }
    }
}
