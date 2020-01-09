using System.Collections.Generic;
using System.Linq;
using Harmony;
using UnityEngine;

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

        [HarmonyPatch(typeof(Immigration))]
        [HarmonyPatch("ConfigureCarePackages")]
        public static class Immigration_ConfigureCarePackages_Patch
        {
            public static void Postfix(ref Immigration __instance)
            {
                var field = Traverse.Create(__instance).Field("carePackages");
                var list = field.GetValue<CarePackageInfo[]>().ToList();

                list.Add(new CarePackageInfo(BabyRollerSnakeConfig.Id, 1f, () => true));
                list.Add(new CarePackageInfo(RollerSnakeConfig.EggId, 3f, () => true));

                field.SetValue(list.ToArray());
            }
        }

        [HarmonyPatch(typeof(CreatureFeederConfig))]
        [HarmonyPatch(nameof(CreatureFeederConfig.ConfigurePost))]
        public static class CreatureFeederConfig_ConfigurePost_Patch
        {
            public static void Postfix(BuildingDef def)
            {
                List<Tag> tagList = def.BuildingComplete.GetComponent<Storage>().storageFilters;
                Tag[] target_species = new Tag[1]
                {
                    BaseRollerSnakeConfig.SpeciesId
                };
                foreach (KeyValuePair<Tag, Diet> collectDiet in DietManager.CollectDiets(target_species))
                    tagList.Add(collectDiet.Key);
                def.BuildingComplete.GetComponent<Storage>().storageFilters = tagList;
            }
        }

        [HarmonyPatch(typeof(CodexEntryGenerator), "GenerateCreatureEntries")]
        public class CodexEntryGenerator_GenerateCreatureEntries_Patch
        {
            public static void Postfix(Dictionary<string, CodexEntry> __result)
            {
                Strings.Add($"STRINGS.CREATURES.FAMILY.{BaseRollerSnakeConfig.SpeciesId.ToUpperInvariant()}", RollerSnakeConfig.Name);
                Strings.Add($"STRINGS.CREATURES.FAMILY_PLURAL.{BaseRollerSnakeConfig.SpeciesId.ToUpperInvariant()}", RollerSnakeConfig.PluralName);
                Action(BaseRollerSnakeConfig.SpeciesId, RollerSnakeConfig.PluralName, __result);
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

                Strings.Add($"STRINGS.CODEX.ROLLERSNAKESTEEL.TITLE", "Tough Roller Snake");
                Strings.Add($"STRINGS.CODEX.ROLLERSNAKESTEEL.SUBTITLE", "Critter Morph");
                Strings.Add($"STRINGS.CODEX.ROLLERSNAKESTEEL.BODY.CONTAINER1", "<smallcaps>Pictured: \"Tough\" Roller Snake variant</smallcaps>");
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
