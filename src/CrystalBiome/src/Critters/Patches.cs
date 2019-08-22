using System;
using System.Collections.Generic;

using Harmony;
using UnityEngine;

namespace CrystalBiome.Critters
{
    public class Patches
    {

        public static void OnLoad()
        {
            TUNING.CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(
                Traverse.Create(typeof(TUNING.CREATURES.EGG_CHANCE_MODIFIERS)).Method("CreateDietaryModifier", new[] { typeof(string), typeof(Tag), typeof(Tag), typeof(float) })
                .GetValue<System.Action>(
                    HatchMutedConfig.Id, 
                    HatchMutedConfig.EggId.ToTag(), 
                    SimHashes.Diamond.CreateTag(),
                    0.05f / HatchTuning.STANDARD_CALORIES_PER_CYCLE));

            /*Strings.Add($"STRINGS.CODEX.MUTED.TITLE", HatchMutedConfig.Name);
            Strings.Add($"STRINGS.CODEX.MUTED.SUBTITLE", "Critter Morph");
            Strings.Add($"STRINGS.CODEX.MUTED.BODY.CONTAINER1", "<smallcaps>Pictured: \"Muted\" Hatch variant</smallcaps>");
            Strings.Add($"STRINGS.CODEX.MUTED.BODY.CONTAINER2", "However does the hatch obtain its calories is an often asked question. The answer is unknown as the duplicants are yet to understand the alien biology.");
            */
        }

        [HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendEntityToFertileCreature))]
        public class EntityTemplates_ExtendEntityToFertileCreature_Patch
        {
            private static void Prefix(string eggId, List<FertilityMonitor.BreedingChance> egg_chances)
            {
                if (eggId.Equals("HatchEgg"))
                {
                    egg_chances.Add(new FertilityMonitor.BreedingChance()
                    {
                        egg = HatchMutedConfig.EggId.ToTag(),
                        weight = 0.02f
                    });
                }
            }
        }

        [HarmonyPatch(typeof(GasAndLiquidConsumerMonitor.Instance), "OnMassConsumed")]
        public class GasAndLiquidConsumerMonitorInstance_OnMassConsumed_Patch
        {
            private static void Prefix(GasAndLiquidConsumerMonitor.Instance __instance, Sim.MassConsumedCallback mcd)
            {
                LivingCrystal livingCrystal = __instance.GetComponent<LivingCrystal>();
                if (livingCrystal == null)
                {
                    return;
                }

                if (mcd.mass > 0.0f)
                {
                    livingCrystal.AccumulateMass(mcd.mass, mcd.temperature);
                }
            }
        }

        [HarmonyPatch(typeof(CreatureCalorieMonitor.Stomach), "Poop")]
        public class Stomach_Poop_Patch
        {
            private static float temperature = 100.0f;

            private static void Prefix(GameObject ___owner)
            {
                temperature = ___owner.GetComponent<PrimaryElement>().Temperature;
                LivingCrystal livingCrystal = ___owner.GetComponent<LivingCrystal>();
                if (livingCrystal == null)
                {
                    return;
                }
                if (livingCrystal.CanConsumeTemperature())
                {
                    float decreasedTemperature = Math.Max(livingCrystal.ConsumeTemperature() + LivingCrystalConfig.OutputTemperatureDelta, LivingCrystalConfig.LethalLowTemperature);
                    ___owner.GetComponent<PrimaryElement>().Temperature = decreasedTemperature;
                }
            }

            private static void Postfix(GameObject ___owner)
            {
                LivingCrystal livingCrystal = ___owner.GetComponent<LivingCrystal>();
                if (livingCrystal == null)
                {
                    return;
                }

                ___owner.GetComponent<PrimaryElement>().Temperature = temperature;
            }
        }

        [HarmonyPatch(typeof(CodexEntryGenerator), "GenerateCreatureEntries")]
        public class CodexEntryGenerator_GenerateCreatureEntries_Patch
        {
            private static void Postfix(Dictionary<string, CodexEntry> __result)
            {
                Strings.Add($"STRINGS.CREATURES.FAMILY.{LivingCrystalConfig.Id.ToUpperInvariant()}", LivingCrystalConfig.Name);
                Strings.Add($"STRINGS.CREATURES.FAMILY_PLURAL.{LivingCrystalConfig.Id.ToUpperInvariant()}", LivingCrystalConfig.PluralName);
                Action(LivingCrystalConfig.Id, LivingCrystalConfig.Name, __result);
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
