using System;
using Harmony;
using System.Collections.Generic;

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

                Strings.Add($"STRINGS.UI.UISIDESCREENS.TELESTORAGE.FLOW.TITLE", TeleStorageFlowControl.FlowTitle);
                Strings.Add($"STRINGS.UI.UISIDESCREENS.TELESTORAGE.FLOW.TOOLTIP", TeleStorageFlowControl.FlowTooltip);

                ModUtil.AddBuildingToPlanScreen("Base", TeleStorageLiquidConfig.Id);
            }
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            private static void Prefix()
            {
                var liquid = new List<string>(Database.Techs.TECH_GROUPING["LiquidPiping"]) { TeleStorageLiquidConfig.Id };
                Database.Techs.TECH_GROUPING["LiquidPiping"] = liquid.ToArray();

                //var gas = new List<string>(Database.Techs.TECH_GROUPING["GasPiping"]) { InfiniteGasSinkConfig.Id, InfiniteGasSourceConfig.Id };
                //Database.Techs.TECH_GROUPING["GasPiping"] = gas.ToArray();
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
        [HarmonyPatch(new [] { typeof(string) })]
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
        [HarmonyPatch(new [] { typeof(string), typeof(bool), typeof(bool) })]
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
