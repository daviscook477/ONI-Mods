using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace TeleStorage
{
    public class TeleStoragePatches
    {

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{TeleStorageLiquidConfig.Id.ToUpperInvariant()}.NAME", TeleStorageLiquidConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{TeleStorageLiquidConfig.Id.ToUpperInvariant()}.DESC", TeleStorageLiquidConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{TeleStorageLiquidConfig.Id.ToUpperInvariant()}.EFFECT", TeleStorageLiquidConfig.Effect);

                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{TeleStorageGasConfig.Id.ToUpperInvariant()}.NAME", TeleStorageGasConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{TeleStorageGasConfig.Id.ToUpperInvariant()}.DESC", TeleStorageGasConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{TeleStorageGasConfig.Id.ToUpperInvariant()}.EFFECT", TeleStorageGasConfig.Effect);

                Strings.Add($"STRINGS.UI.UISIDESCREENS.TELESTORAGE.FLOW.TITLE", TeleStorageFlowControl.FlowTitle);
                Strings.Add($"STRINGS.UI.UISIDESCREENS.TELESTORAGE.FLOW.TOOLTIP", TeleStorageFlowControl.FlowTooltip);

                ModUtil.AddBuildingToPlanScreen("Base", TeleStorageLiquidConfig.Id);
                ModUtil.AddBuildingToPlanScreen("Base", TeleStorageGasConfig.Id);
            }
        }

        [HarmonyPatch(typeof(SimpleInfoScreen))]
        [HarmonyPatch("RefreshStorage")]
        public class SimpleInfoScreen_RefreshStorage_Patch
        {
            private static void Postfix(
                SimpleInfoScreen __instance,
                GameObject ___selectedTarget,
                ref Dictionary<string, GameObject> ___storageLabels)
            {
                if (___selectedTarget == null || ___selectedTarget.GetComponent<TeleStorage>() == null)
                {
                    return;
                }
                ConduitType type = ___selectedTarget.GetComponent<TeleStorage>().Type;
                __instance.StoragePanel.gameObject.SetActive(true);
                __instance.StoragePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = (!(___selectedTarget.GetComponent<MinionIdentity>() != null) ? STRINGS.UI.DETAILTABS.DETAILS.GROUPNAME_CONTENTS : STRINGS.UI.DETAILTABS.DETAILS.GROUPNAME_MINION_CONTENTS);
                if (___storageLabels == null)
                {
                    ___storageLabels = new Dictionary<string, GameObject>();
                }
                foreach (KeyValuePair<string, GameObject> storageLabel in ___storageLabels)
                    storageLabel.Value.SetActive(false);
                int num = 0;
                foreach (SimHashes element in TeleStorageData.Instance.storedElementsMap.Keys)
                {
                    StoredItem item = TeleStorageData.Instance.storedElementsMap[element];
                    if (item.mass > 0.0f)
                    {
                        Element elementObj = ElementLoader.FindElementByHash(element);
                        if (elementObj.IsLiquid && !type.Equals(ConduitType.Liquid))
                        {
                            continue;
                        }
                        if (elementObj.IsGas && !type.Equals(ConduitType.Gas))
                        {
                            continue;
                        }
                        GameObject storageLabel = Traverse.Create(__instance).Method("AddOrGetStorageLabel", new Type[] { typeof(Dictionary<string, GameObject>), typeof(GameObject), typeof(string) }).GetValue<GameObject>(new object[] { ___storageLabels, __instance.StoragePanel, "storage_" + num.ToString() });
                        ++num;
                        storageLabel.GetComponentInChildren<ToolTip>().ClearMultiStringTooltip();
                        string formattedName = elementObj.name;
                        string str1 = string.Format(STRINGS.UI.DETAILTABS.DETAILS.CONTENTS_MASS, formattedName, GameUtil.GetFormattedMass(item.mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
                        string str2 = string.Format(STRINGS.UI.DETAILTABS.DETAILS.CONTENTS_TEMPERATURE, str1, GameUtil.GetFormattedTemperature(item.temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
                        if (item.diseaseIdx != byte.MaxValue)
                        {
                            str2 += string.Format(STRINGS.UI.DETAILTABS.DETAILS.CONTENTS_DISEASED, GameUtil.GetFormattedDisease(item.diseaseIdx, item.diseaseCount, false));
                            string formattedDisease = GameUtil.GetFormattedDisease(item.diseaseIdx, item.diseaseCount, true);
                            storageLabel.GetComponentInChildren<ToolTip>().AddMultiStringTooltip(formattedDisease, PluginAssets.Instance.defaultTextStyleSetting);
                        }
                        storageLabel.GetComponentInChildren<LocText>().text = str2;
                    }
                }
                if (num == 0)
                {
                    Traverse.Create(__instance).Method("AddOrGetStorageLabel", new Type[] { typeof(Dictionary<string, GameObject>), typeof(GameObject), typeof(string) }).GetValue<GameObject>(___storageLabels, __instance.StoragePanel, "empty").GetComponentInChildren<LocText>().text = STRINGS.UI.DETAILTABS.DETAILS.STORAGE_EMPTY;

                }
                Traverse.Create(__instance.StoragePanel.GetComponent<CollapsibleDetailContentPanel>().scalerMask).Method("Update").GetValue();
            }
        }

        [HarmonyPatch(typeof(FilterSideScreen), nameof(FilterSideScreen.IsValidForTarget))]
        public class FilterSideScreen_IsValidForTarget
        {
            private static bool Prefix(GameObject target, FilterSideScreen __instance, ref bool __result)
            {
                if (target.GetComponent<TeleStorageFlowControl>() != null)
                {
                    __result = !__instance.isLogicFilter;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            private static void Postfix()
            {
                Db.Get().Techs.Get("Catalytics").unlockedItemIDs.Add(TeleStorageLiquidConfig.Id);
                Db.Get().Techs.Get("Catalytics").unlockedItemIDs.Add(TeleStorageGasConfig.Id);
            }
        }

        [HarmonyPatch(typeof(KSerialization.Manager))]
        [HarmonyPatch("GetType")]
        [HarmonyPatch(new[] { typeof(string) })]
        public static class TeleStorageSerializationPatch
        {
            public static void Postfix(string type_name, ref Type __result)
            {
                if (type_name == "TeleStorage.TeleStorage")
                {
                    __result = typeof(TeleStorage);
                }
            }
        }

        [HarmonyPatch(typeof(SaveLoader))]
        [HarmonyPatch("Load")]
        [HarmonyPatch(new[] { typeof(string) })]
        public static class SaveLoader_Load_Patch
        {
            public static void Postfix(string filename)
            {
                Console.WriteLine("Consumed load event");
                TeleStorageData.Load(filename);
            }
        }

        [HarmonyPatch(typeof(SaveLoader))]
        [HarmonyPatch("Save")]
        [HarmonyPatch(new[] { typeof(string), typeof(bool), typeof(bool) })]
        public static class SaveLoader_Save_Patch
        {
            public static void Postfix(string filename)
            {
                Console.WriteLine("Consumed save event");
                TeleStorageData.Save(filename);
            }
        }

    }
}
