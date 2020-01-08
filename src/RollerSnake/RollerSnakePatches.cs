using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Harmony;
using UnityEngine;
using Klei;

namespace RollerSnake
{
    public class RollerSnakePatches
    {

        public static void OnLoad()
        {
            TUNING.CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                Traverse.Create(typeof(TUNING.CREATURES.EGG_CHANCE_MODIFIERS)).Method("CreateDietaryModifier", new[] { typeof(string), typeof(Tag), typeof(Tag), typeof(float) })
                .GetValue<System.Action>(
                    SteelRollerSnakeConfig.Id,
                    SteelRollerSnakeConfig.EggId.ToTag(),
                    SimHashes.Obsidian.CreateTag(),
                    0.05f / RollerSnakeTuning.STANDARD_CALORIES_PER_CYCLE));
        }

        [HarmonyPatch(typeof(CodexEntryGenerator), "GenerateCreatureEntries")]
        public class CodexEntryGenerator_GenerateCreatureEntries_Patch
        {
            private static void Postfix(Dictionary<string, CodexEntry> __result)
            {
                Strings.Add($"STRINGS.CREATURES.FAMILY.{BaseRollerSnakeConfig.SpeciesId.ToUpperInvariant()}", RollerSnakeConfig.Name);
                Strings.Add($"STRINGS.CREATURES.FAMILY_PLURAL.{BaseRollerSnakeConfig.SpeciesId.ToUpperInvariant()}", RollerSnakeConfig.PluralName);
                Action(BaseRollerSnakeConfig.SpeciesId, RollerSnakeConfig.PluralName, __result);
            }
        }

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
                if (!path.EndsWith("Creatures"))
                {
                    return;
                }
                string assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                // remove "file:\
                assemblyPath = assemblyPath.Substring("file:\\".Length);
                string[] strArray = new string[0];
                try
                {
                    strArray = Directory.GetFiles(assemblyPath + "/codex/Creatures", "*.yaml");
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

        [HarmonyPatch(typeof(EntityConfigManager))]
        [HarmonyPatch(nameof(EntityConfigManager.LoadGeneratedEntities))]
        public class EntityConfigManager_LoadGeneratedEntities_Patch
        {
            public static void Prefix()
            {
                Strings.Add($"STRINGS.CODEX.ROLLERSNAKE.SPECIES_TITLE", "Roller Snakes");
                Strings.Add($"STRINGS.CODEX.ROLLERSNAKE.SPECIES_SUBTITLE", "Critter Species");
                Strings.Add($"STRINGS.CODEX.ROLLERSNAKE.TITLE", "Roller Snake");
                Strings.Add($"STRINGS.CODEX.ROLLERSNAKE.SUBTITLE", "Domesticable Critter");
                Strings.Add($"STRINGS.CODEX.ROLLERSNAKE.BODY.CONTAINER1", "The reptilian species of Roller Snakes moves in the most peculiar way. Coiling up upon itself and rolling along in a loop is its preferred means of transportation.");
                Strings.Add($"STRINGS.CODEX.ROLLERSNAKE.BODY.CONTAINER2", "The rattle on the tail of the Roller Snake, while effective at warning and deterring predators in the wild, is entirely decorative in captivity and can be shorn off without any damage to the animal.\n\nBecause the metal in a Roller Snake's diet is accumulated in its rattle, their rattles are highly sought after for metallurgy.");

                Strings.Add($"STRINGS.CODEX.ROLLERSNAKESTEEL.TITLE", "Hardened Roller Snake");
                Strings.Add($"STRINGS.CODEX.ROLLERSNAKESTEEL.SUBTITLE", "Critter Morph");
                Strings.Add($"STRINGS.CODEX.ROLLERSNAKESTEEL.BODY.CONTAINER1", "<smallcaps>Pictured: \"Hardened\" Roller Snake variant</smallcaps>");
                Strings.Add($"STRINGS.CODEX.ROLLERSNAKESTEEL.BODY.CONTAINER2", "Roller Snakes are known to be quite kind to their caretakers and do not bite or otherwise retaliate in response to grooming or shearing.");
            }
        }

        private static void Action(Tag speciesTag, string name, Dictionary<string, CodexEntry> results)
        {
            List<GameObject> brains = Assets.GetPrefabsWithComponent<CreatureBrain>();
            CodexEntry entry = new CodexEntry("CREATURES", new List<ContentContainer>()
            {
                new ContentContainer(new List<ICodexWidget>()
                {
                    new CodexSpacer(),
                    new CodexSpacer()
                }, ContentContainer.ContentLayout.Vertical)
            }, name);
            entry.parentId = "CREATURES";
            CodexCache.AddEntry(speciesTag.ToString(), entry, null);
            results.Add(speciesTag.ToString(), entry);
            foreach (GameObject gameObject in brains)
            {
                if (gameObject.GetDef<BabyMonitor.Def>() == null)
                {
                    Sprite sprite = null;
                    GameObject prefab = Assets.TryGetPrefab((gameObject.PrefabID().ToString() + "Baby"));
                    if (prefab != null)
                        sprite = Def.GetUISprite(prefab, "ui", false).first;
                    CreatureBrain component = gameObject.GetComponent<CreatureBrain>();
                    if (component.species == speciesTag)
                    {
                        List<ContentContainer> contentContainerList = new List<ContentContainer>();
                        string symbolPrefix = component.symbolPrefix;
                        Sprite first = Def.GetUISprite(gameObject, symbolPrefix + "ui", false).first;
                        if ((bool)((UnityEngine.Object)sprite))
                        {
                            Traverse.Create(typeof(CodexEntryGenerator)).Method("GenerateImageContainers", new[] { typeof(Sprite[]), typeof(List<ContentContainer>), typeof(ContentContainer.ContentLayout) })
                                .GetValue(new Sprite[2]
                                {
                                    first,
                                    sprite
                                }, contentContainerList, ContentContainer.ContentLayout.Horizontal);
                        }
                        else
                        {
                            contentContainerList.Add(new ContentContainer(new List<ICodexWidget>()
                            {
                              new CodexImage(128, 128, first)
                            }, ContentContainer.ContentLayout.Vertical));
                        }

                        Traverse.Create(typeof(CodexEntryGenerator)).Method("GenerateCreatureDescriptionContainers", new[] { typeof(GameObject), typeof(List<ContentContainer>) }).GetValue(gameObject, contentContainerList);
                        entry.subEntries.Add(new SubEntry(component.PrefabID().ToString(), speciesTag.ToString(), contentContainerList, component.GetProperName())
                        {
                            icon = first,
                            iconColor = UnityEngine.Color.white
                        });
                    }
                }
            }
        }
    }
}
