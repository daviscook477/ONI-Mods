using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace RollerSnake
{
    public class RollerSnakePatches
    {
        [HarmonyPatch(typeof(CodexEntryGenerator), "GenerateCreatureEntries")]
        public class CodexEntryGenerator_GenerateCreatureEntries_Patch
        {
            private static void Postfix(Dictionary<string, CodexEntry> __result)
            {
                Strings.Add($"STRINGS.CREATURES.FAMILY.{RollerSnakeConfig.Id.ToUpperInvariant()}", RollerSnakeConfig.Name);
                Strings.Add($"STRINGS.CREATURES.FAMILY_PLURAL.{RollerSnakeConfig.Id.ToUpperInvariant()}", RollerSnakeConfig.PluralName);
                Action(RollerSnakeConfig.Id, RollerSnakeConfig.Name, __result);
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
                CreatureBrain component = gameObject.GetComponent<CreatureBrain>();
                if (component.species == speciesTag)
                {
                    List<ContentContainer> contentContainerList = new List<ContentContainer>();
                    string symbolPrefix = component.symbolPrefix;
                    Sprite first = Def.GetUISprite(gameObject, symbolPrefix + "ui", false).first;
                    contentContainerList.Add(new ContentContainer(new List<ICodexWidget>()
                    {
                      new CodexImage(128, 128, first)
                    }, ContentContainer.ContentLayout.Vertical));
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
