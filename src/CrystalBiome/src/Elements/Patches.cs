using System;
using System.Collections;
using System.Collections.Generic;

using Harmony;
using UnityEngine;

using Klei;
using Klei.AI;

namespace CrystalBiome.Elements
{
    public class Patches
    {
        public static void OnLoad()
        {
            Strings.Add("STRINGS.MISC.TAGS.GEMSTONE", "Gemstones");
        }

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public static class Db_Initialize_Patch
        {
            private static void ApplyDecorModifierToElement(SimHashes hash, float amount, bool isMultiplier)
            {
                ApplyAttributeToElement(hash, new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, amount, null, isMultiplier, false, true));
            }

            private static void ApplyOverheatTemperatureModifierToElement(SimHashes hash, float amount, bool isMultiplier)
            {
                ApplyAttributeToElement(hash, new AttributeModifier(Db.Get().BuildingAttributes.OverheatTemperature.Id, amount, null, isMultiplier, false, true));

            }

            private static void ApplyAttributeToElement(SimHashes hash, AttributeModifier attr)
            {
                ElementLoader.FindElementByHash(hash).attributeModifiers.Add(attr);
            }

            private static void Postfix()
            {
                ApplyDecorModifierToElement(CorundumElement.SimHash, 0.1f, true);
                ApplyOverheatTemperatureModifierToElement(CorundumElement.SimHash, 20.0f, false);

                ApplyDecorModifierToElement(KyaniteElement.SimHash, 0.3f, true);
                ApplyOverheatTemperatureModifierToElement(KyaniteElement.SimHash, -20.0f, false);

                ApplyDecorModifierToElement(SodaliteElement.SimHash, 0.2f, true);
            }
        }

        public static Dictionary<SimHashes, string> SimHashTable = new Dictionary<SimHashes, string>();
        public static Dictionary<string, object> SimHashReverseTable = new Dictionary<string, object>();

        private static void AddHashToTable(SimHashes hash, string id)
        {
            SimHashTable.Add(hash, id);
            SimHashReverseTable.Add(id, hash);
        }

        static Patches()
        {
            AddHashToTable(CorundumElement.SimHash, CorundumElement.Id);
            AddHashToTable(KyaniteElement.SimHash, KyaniteElement.Id);
            AddHashToTable(SodaliteElement.SimHash, SodaliteElement.Id);
            AddHashToTable(PolishedCorundumElement.SimHash, PolishedCorundumElement.Id);
            AddHashToTable(PolishedKyaniteElement.SimHash, PolishedKyaniteElement.Id);
            AddHashToTable(PolishedSodaliteElement.SimHash, PolishedSodaliteElement.Id);
            AddHashToTable(AluminumSaltElement.SimHash, AluminumSaltElement.Id);
            AddHashToTable(MineralWaterElement.SimHash, MineralWaterElement.Id);
            AddHashToTable(MineralIceElement.SimHash, MineralIceElement.Id);
            AddHashToTable(CrystalElement.SimHash, CrystalElement.Id);

        }

        [HarmonyPatch(typeof(Enum), nameof(Enum.ToString), new Type[] { })]
        public static class SimHashes_ToString_Patch
        {
            private static bool Prefix(ref Enum __instance, ref string __result)
            {
                if (!(__instance is SimHashes)) return true;
                return !SimHashTable.TryGetValue((SimHashes)__instance, out __result);
            }
        }

        [HarmonyPatch(typeof(Enum), nameof(Enum.Parse), new Type[] { typeof(Type), typeof(string), typeof(bool) })]
        public static class SimHashes_Parse_Patch
        {
            private static bool Prefix(Type enumType, string value, ref object __result)
            {
                if (!enumType.Equals(typeof(SimHashes))) return true;
                return !SimHashReverseTable.TryGetValue(value, out __result);
            }
        }

        [HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.CollectElementsFromYAML))]
        public static class ElementLoader_CollectElementsFromYAML_Patch
        {
            private static void Postfix(ref List<ElementLoader.ElementEntry> __result)
            {
                Strings.Add($"STRINGS.ELEMENTS.{CorundumElement.Id.ToUpper()}.NAME", CorundumElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{CorundumElement.Id.ToUpper()}.DESC", CorundumElement.Description);
                Strings.Add($"STRINGS.ELEMENTS.{KyaniteElement.Id.ToUpper()}.NAME", KyaniteElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{KyaniteElement.Id.ToUpper()}.DESC", KyaniteElement.Description);
                Strings.Add($"STRINGS.ELEMENTS.{SodaliteElement.Id.ToUpper()}.NAME", SodaliteElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{SodaliteElement.Id.ToUpper()}.DESC", SodaliteElement.Description);
                Strings.Add($"STRINGS.ELEMENTS.{PolishedCorundumElement.Id.ToUpper()}.NAME", PolishedCorundumElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{PolishedCorundumElement.Id.ToUpper()}.DESC", PolishedCorundumElement.Description);
                Strings.Add($"STRINGS.ELEMENTS.{PolishedKyaniteElement.Id.ToUpper()}.NAME", PolishedKyaniteElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{PolishedKyaniteElement.Id.ToUpper()}.DESC", PolishedKyaniteElement.Description);
                Strings.Add($"STRINGS.ELEMENTS.{PolishedSodaliteElement.Id.ToUpper()}.NAME", PolishedSodaliteElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{PolishedSodaliteElement.Id.ToUpper()}.DESC", PolishedSodaliteElement.Description);
                Strings.Add($"STRINGS.ELEMENTS.{AluminumSaltElement.Id.ToUpper()}.NAME", AluminumSaltElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{AluminumSaltElement.Id.ToUpper()}.DESC", AluminumSaltElement.Description);
                Strings.Add($"STRINGS.ELEMENTS.{MineralWaterElement.Id.ToUpper()}.NAME", MineralWaterElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{MineralWaterElement.Id.ToUpper()}.DESC", MineralWaterElement.Description);
                Strings.Add($"STRINGS.ELEMENTS.{MineralIceElement.Id.ToUpper()}.NAME", MineralIceElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{MineralIceElement.Id.ToUpper()}.DESC", MineralIceElement.Description);
                Strings.Add($"STRINGS.ELEMENTS.{CrystalElement.Id.ToUpper()}.NAME", CrystalElement.Name);
                Strings.Add($"STRINGS.ELEMENTS.{CrystalElement.Id.ToUpper()}.DESC", CrystalElement.Description);

                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(CorundumElement.Data, null).elements);
                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(KyaniteElement.Data, null).elements);
                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(SodaliteElement.Data, null).elements);
                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(PolishedCorundumElement.Data, null).elements);
                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(PolishedKyaniteElement.Data, null).elements);
                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(PolishedSodaliteElement.Data, null).elements);
                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(AluminumSaltElement.Data, null).elements);
                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(MineralWaterElement.Data, null).elements);
                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(MineralIceElement.Data, null).elements);
                __result.AddRange(YamlIO.Parse<ElementLoader.ElementEntryCollection>(CrystalElement.Data, null).elements);
            }
        }

        [HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.Load))]
        public static class ElementLoader_Load_Patch
        {
            private static void Prefix(ref Hashtable substanceList, SubstanceTable substanceTable)
            {
                Traverse.Create(typeof(Assets)).Field("AnimTable").GetValue<Dictionary<HashedString, KAnimFile>>().Clear();
                foreach (KAnimFile anim in Assets.Anims)
                {
                    if (anim != null)
                    {
                        HashedString name = anim.name;
                        Traverse.Create(typeof(Assets)).Field("AnimTable").GetValue<Dictionary<HashedString, KAnimFile>>()[name] = anim;
                    }
                }

                var solid = substanceTable.GetSubstance(SimHashes.SandStone);
                var liquid = substanceTable.GetSubstance(SimHashes.Water);
                substanceList[CorundumElement.SimHash] = BaseElement.CreateSolidSubstance(CorundumElement.Id, solid.material, "corundum", AssetLoading.AssetLoader.Instance.TextureTable["corundum_mat"]);
                substanceList[KyaniteElement.SimHash] = BaseElement.CreateSolidSubstance(KyaniteElement.Id, solid.material, "kyanite", AssetLoading.AssetLoader.Instance.TextureTable["kyanite_mat"]);
                substanceList[SodaliteElement.SimHash] = BaseElement.CreateSolidSubstance(SodaliteElement.Id, solid.material, "sodalite", AssetLoading.AssetLoader.Instance.TextureTable["sodalite_mat"]);
                substanceList[PolishedCorundumElement.SimHash] = BaseElement.CreateSolidSubstance(PolishedCorundumElement.Id, solid.material, "corundum_item", AssetLoading.AssetLoader.Instance.TextureTable["aluminum_salt_mat"]);
                substanceList[PolishedKyaniteElement.SimHash] = BaseElement.CreateSolidSubstance(PolishedKyaniteElement.Id, solid.material, "kyanite_item", AssetLoading.AssetLoader.Instance.TextureTable["aluminum_salt_mat"]);
                substanceList[PolishedSodaliteElement.SimHash] = BaseElement.CreateSolidSubstance(PolishedSodaliteElement.Id, solid.material, "sodalite_item", AssetLoading.AssetLoader.Instance.TextureTable["aluminum_salt_mat"]);
                substanceList[AluminumSaltElement.SimHash] = BaseElement.CreateSolidSubstance(AluminumSaltElement.Id, solid.material, "aluminum_salt", AssetLoading.AssetLoader.Instance.TextureTable["aluminum_salt_mat"]);
                substanceList[MineralWaterElement.SimHash] = BaseElement.CreateLiquidSubstance(MineralWaterElement.Id, liquid, new Color32(255, 204, 230, 255));
                substanceList[MineralIceElement.SimHash] = BaseElement.CreateSolidSubstance(MineralIceElement.Id, solid.material, "mineral_ice", AssetLoading.AssetLoader.Instance.TextureTable["mineral_ice_mat"]);
                substanceList[CrystalElement.SimHash] = BaseElement.CreateSolidSubstance(CrystalElement.Id, solid.material, "crystal_item", AssetLoading.AssetLoader.Instance.TextureTable["aluminum_salt_mat"]);
            }
        }
    }
}
