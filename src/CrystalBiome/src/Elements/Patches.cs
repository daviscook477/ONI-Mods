using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Harmony;
using UnityEngine;

using Klei;

namespace CrystalBiome.Elements
{
    public class Patches
    {
        [HarmonyPatch(typeof(Enum), "ToString", new Type[] { })]
        public static class SimHashes_ToString_Patch
        {
            public static Dictionary<SimHashes, string> SimHashTable = new Dictionary<SimHashes, string>
            {
                { SodaliteElement.SimHash, SodaliteElement.Id },
                { CorundumElement.SimHash, CorundumElement.Id },
                { KyaniteElement.SimHash, KyaniteElement.Id },
                { AluminumSaltElement.SimHash, AluminumSaltElement.Id },
                { MineralWaterElement.SimHash, MineralWaterElement.Id },
                { MineralIceElement.SimHash, MineralIceElement.Id }
            };

            private static bool Prefix(ref Enum __instance, ref string __result)
            {
                if (!(__instance is SimHashes)) return true;
                return !SimHashTable.TryGetValue((SimHashes)__instance, out __result);
            }
        }

        [HarmonyPatch(typeof(ElementLoader), "CollectElementsFromYAML")]
        public static class ElementLoader_CollectElementsFromYAML_Patch
        {
            private static void Postfix(ref List<ElementLoader.ElementEntry> __result)
            {
                Strings.Add($"STRINGS.ELEMENTS.{SodaliteElement.Id.ToUpper()}.NAME", SodaliteElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{SodaliteElement.Id.ToUpper()}.DESC", SodaliteElement.Description);
                Strings.Add($"STRINGS.ELEMENTS.{CorundumElement.Id.ToUpper()}.NAME", CorundumElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{CorundumElement.Id.ToUpper()}.DESC", CorundumElement.Description);
                Strings.Add($"STRINGS.ELEMENTS.{KyaniteElement.Id.ToUpper()}.NAME", KyaniteElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{KyaniteElement.Id.ToUpper()}.DESC", KyaniteElement.Description);
                Strings.Add($"STRINGS.ELEMENTS.{AluminumSaltElement.Id.ToUpper()}.NAME", AluminumSaltElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{AluminumSaltElement.Id.ToUpper()}.DESC", AluminumSaltElement.Description);
                Strings.Add($"STRINGS.ELEMENTS.{MineralWaterElement.Id.ToUpper()}.NAME", MineralWaterElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{MineralWaterElement.Id.ToUpper()}.DESC", MineralWaterElement.Description);
                Strings.Add($"STRINGS.ELEMENTS.{MineralIceElement.Id.ToUpper()}.NAME", MineralIceElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{MineralIceElement.Id.ToUpper()}.DESC", MineralIceElement.Description);

                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(SodaliteElement.Data, null).elements);
                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(CorundumElement.Data, null).elements);
                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(KyaniteElement.Data, null).elements);
                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(AluminumSaltElement.Data, null).elements);
                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(MineralWaterElement.Data, null).elements);
                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(MineralIceElement.Data, null).elements);
            }
        }

        [HarmonyPatch(typeof(ElementLoader), "Load")]
        public static class ElementLoader_Load_Patch
        {
            private static void Prefix(ref Hashtable substanceList, SubstanceTable substanceTable)
            {
                Traverse.Create(typeof(Assets)).Field("AnimTable").GetValue<Dictionary<HashedString, KAnimFile>>().Clear();
                foreach (KAnimFile anim in Assets.Anims)
                {
                    if ((UnityEngine.Object)anim != (UnityEngine.Object)null)
                    {
                        HashedString name = (HashedString)anim.name;
                        Traverse.Create(typeof(Assets)).Field("AnimTable").GetValue<Dictionary<HashedString, KAnimFile>>()[name] = anim;
                    }
                }

                var solid = substanceTable.GetSubstance(SimHashes.SandStone);
                var liquid = substanceTable.GetSubstance(SimHashes.Water);
                substanceList[SodaliteElement.SimHash] = BaseElement.CreateSolidSubstance(SodaliteElement.Id, solid.material, "sodalite", AssetLoading.AssetLoader.Instance.TextureTable["sodalite_mat"]);
                substanceList[CorundumElement.SimHash] = BaseElement.CreateSolidSubstance(CorundumElement.Id, solid.material, "corundum", AssetLoading.AssetLoader.Instance.TextureTable["corundum_mat"]);
                substanceList[KyaniteElement.SimHash] = BaseElement.CreateSolidSubstance(KyaniteElement.Id, solid.material, "kyanite", AssetLoading.AssetLoader.Instance.TextureTable["kyanite_mat"]);
                substanceList[AluminumSaltElement.SimHash] = BaseElement.CreateSolidSubstance(AluminumSaltElement.Id, solid.material, "aluminum_salt", AssetLoading.AssetLoader.Instance.TextureTable["aluminum_salt_mat"]);
                substanceList[MineralWaterElement.SimHash] = BaseElement.CreateLiquidSubstance(MineralWaterElement.Id, liquid, new Color32(255, 204, 230, 255));
                substanceList[MineralIceElement.SimHash] = BaseElement.CreateSolidSubstance(MineralIceElement.Id, solid.material, "mineral_ice", AssetLoading.AssetLoader.Instance.TextureTable["mineral_ice_mat"]);
            }
        }
    }
}
