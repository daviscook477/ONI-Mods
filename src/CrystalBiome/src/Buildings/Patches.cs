using System;
using System.Collections.Generic;

using Harmony;
using UnityEngine;

using STRINGS;

namespace CrystalBiome.Buildings
{
    public class Patches
    {

        /*[HarmonyPatch(typeof(HashedString), MethodType.Constructor, new Type[] { typeof(string) })]
        public class AAABBB
        {
            private static void Postfix(string name)
            {
                Console.WriteLine(name);
            }
        }*/
        
        [HarmonyPatch(typeof(LoopingSounds), nameof(LoopingSounds.StartSound), new Type[] { typeof(string) })]
        public class AAAABBBB
        {
            private static void Prefix(string asset)
            {
                Console.WriteLine(asset);
            }
        }

        [HarmonyPatch(typeof(LoopingSounds), nameof(LoopingSounds.StartSound), new Type[] { typeof(string), typeof(AnimEventManager.EventPlayerData), typeof(EffectorValues), typeof(bool), typeof(bool) })]
        public class AAAABBBBB
        {
            private static void Prefix(string asset)
            {
                Console.WriteLine(asset);
            }
        }

        [HarmonyPatch(typeof(LoopingSounds), nameof(LoopingSounds.StartSound), new Type[] { typeof(string), typeof(bool), typeof(bool), typeof(bool) })]
        public class AAAABBBBBB
        {
            private static void Prefix(string asset)
            {
                Console.WriteLine(asset);
            }
        }


        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{RockPolisherConfig.Id.ToUpperInvariant()}.NAME", RockPolisherConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{RockPolisherConfig.Id.ToUpperInvariant()}.DESC", RockPolisherConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{RockPolisherConfig.Id.ToUpperInvariant()}.EFFECT", RockPolisherConfig.Effect);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{MineralizerConfig.Id.ToUpperInvariant()}.NAME", MineralizerConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{MineralizerConfig.Id.ToUpperInvariant()}.DESC", MineralizerConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{MineralizerConfig.Id.ToUpperInvariant()}.EFFECT", MineralizerConfig.Effect);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{GemstoneSculptureConfig.Id.ToUpperInvariant()}.NAME", GemstoneSculptureConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{GemstoneSculptureConfig.Id.ToUpperInvariant()}.DESC", GemstoneSculptureConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{GemstoneSculptureConfig.Id.ToUpperInvariant()}.EFFECT", GemstoneSculptureConfig.Effect);

                ModUtil.AddBuildingToPlanScreen("Refining", RockPolisherConfig.Id);
                ModUtil.AddBuildingToPlanScreen("Refining", MineralizerConfig.Id);
                ModUtil.AddBuildingToPlanScreen("Furniture", GemstoneSculptureConfig.Id);
            }
        }

        [HarmonyPatch(typeof(Building), "OnSpawn")]
        public class Building_OnSpawn_Patch
        {
            private static void Prefix(Building __instance)
            {
                if (__instance.Def == null) return;
                if (!__instance.Def.name.Equals(GemstoneSculptureConfig.Id)) return;
                var primaryElement = __instance.gameObject.GetComponent<PrimaryElement>();
                if (primaryElement == null || primaryElement.Element == null)
                {
                    DebugUtil.LogWarningArgs(GemstoneSculptureConfig.Id + " building did not have a primary element! Unable to properly apply texture override.");
                    return;
                }
                if (primaryElement.Element.id == Elements.PolishedCorundumElement.SimHash)
                {
                    __instance.gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim("gem_sculpture"), "pink_", null, 0);
                }
                else if (primaryElement.Element.id == Elements.PolishedKyaniteElement.SimHash)
                {
                    __instance.gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim("gem_sculpture"), "cyan_", null, 0);
                }
            }
        }

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            private static void Prefix()
            {
                var basicRefinement = new List<string>(Database.Techs.TECH_GROUPING["BasicRefinement"]) { RockPolisherConfig.Id, MineralizerConfig.Id };
                Database.Techs.TECH_GROUPING["BasicRefinement"] = basicRefinement.ToArray();
                var refractiveDecor = new List<string>(Database.Techs.TECH_GROUPING["RefractiveDecor"]) { GemstoneSculptureConfig.Id };
                Database.Techs.TECH_GROUPING["RefractiveDecor"] = refractiveDecor.ToArray();
            }
        }

        public static void OnLoad()
        {
            BUILDINGS.PREFABS.DESALINATOR.DESC += " Aluminum salt can be refined into aluminum metal.";
            BUILDINGS.PREFABS.DESALINATOR.EFFECT += $" Additionally removes {UI.FormatAsLink("Aluminum Salt", Elements.AluminumSaltElement.Id)} from {UI.FormatAsLink("Mineral Water", Elements.MineralWaterElement.Id)}.";
        }

        [HarmonyPatch(typeof(DesalinatorConfig), nameof(DesalinatorConfig.ConfigureBuildingTemplate))]
        public static class DesalinatorConfig_ConfigureBuildingTemplate
        {
            private static void Postfix(GameObject go)
            {
                ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
                var filter = new List<SimHashes>(conduitDispenser.elementFilter) { Elements.MineralWaterElement.SimHash } ;
                conduitDispenser.elementFilter = filter.ToArray();

                ElementConverter elementConverter = go.AddComponent<ElementConverter>();
                elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
                {
                    new ElementConverter.ConsumedElement(ElementLoader.FindElementByHash(Elements.MineralWaterElement.SimHash).tag, 5f)
                };
                elementConverter.outputElements = new ElementConverter.OutputElement[2]
                {
                    new ElementConverter.OutputElement(3.5f, SimHashes.Water, 0.0f, false, true, 0.0f, 0.5f, 0.75f, byte.MaxValue, 0),
                    new ElementConverter.OutputElement(1.5f, Elements.AluminumSaltElement.SimHash, 0.0f, false, true, 0.0f, 0.5f, 0.25f, byte.MaxValue, 0)
                };
            }
        }


        [HarmonyPatch(typeof(MetalRefineryConfig), nameof(MetalRefineryConfig.ConfigureBuildingTemplate))]
        public static class MetalRefineryConfig_ConfigureBuildingTemplate_Patch
        {
            private static void Postfix()
            {
                ComplexRecipe.RecipeElement[] ingredients1 = new ComplexRecipe.RecipeElement[1]
                {
                      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(Elements.AluminumSaltElement.SimHash).tag, 200f)
                };
                ComplexRecipe.RecipeElement[] results1 = new ComplexRecipe.RecipeElement[1]
                {
                    new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Aluminum).tag, 100f)
                };
                string str1 = ComplexRecipeManager.MakeRecipeID("MetalRefinery", ingredients1, results1);
                new ComplexRecipe(str1, ingredients1, results1)
                {
                    time = 40f,
                    nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
                    description = string.Format(BUILDINGS.PREFABS.METALREFINERY.RECIPE_DESCRIPTION, ElementLoader.FindElementByHash(SimHashes.Aluminum).name, ElementLoader.FindElementByHash(Elements.AluminumSaltElement.SimHash).name)
                }.fabricators = new List<Tag>()
                {
                  TagManager.Create("MetalRefinery")
                };
            }
        }

    }
}
