using System;
using System.Collections.Generic;
using Harmony;

namespace InfiniteSourceSink
{
    public class InfiniteSourceSinkPatches
    {

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{InfiniteLiquidSinkConfig.Id.ToUpperInvariant()}.NAME", InfiniteLiquidSinkConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{InfiniteLiquidSinkConfig.Id.ToUpperInvariant()}.DESC", InfiniteLiquidSinkConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{InfiniteLiquidSinkConfig.Id.ToUpperInvariant()}.EFFECT", InfiniteLiquidSinkConfig.Effect);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{InfiniteLiquidSourceConfig.Id.ToUpperInvariant()}.NAME", InfiniteLiquidSourceConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{InfiniteLiquidSourceConfig.Id.ToUpperInvariant()}.DESC", InfiniteLiquidSourceConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{InfiniteLiquidSourceConfig.Id.ToUpperInvariant()}.EFFECT", InfiniteLiquidSourceConfig.Effect);

                Strings.Add($"STRINGS.UI.UISIDESCREENS.{InfiniteLiquidSourceConfig.Id.ToUpperInvariant()}.TITLE", InfiniteLiquidSourceConfig.TemperatureSliderTitle);
                Strings.Add($"STRINGS.UI.UISIDESCREENS.{InfiniteLiquidSourceConfig.Id.ToUpperInvariant()}.TOOLTIP", InfiniteLiquidSourceConfig.TemperatureSliderTooltip);

                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{InfiniteGasSinkConfig.Id.ToUpperInvariant()}.NAME", InfiniteGasSinkConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{InfiniteGasSinkConfig.Id.ToUpperInvariant()}.DESC", InfiniteGasSinkConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{InfiniteGasSinkConfig.Id.ToUpperInvariant()}.EFFECT", InfiniteGasSinkConfig.Effect);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{InfiniteGasSourceConfig.Id.ToUpperInvariant()}.NAME", InfiniteGasSourceConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{InfiniteGasSourceConfig.Id.ToUpperInvariant()}.DESC", InfiniteGasSourceConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{InfiniteGasSourceConfig.Id.ToUpperInvariant()}.EFFECT", InfiniteGasSourceConfig.Effect);

                Strings.Add($"STRINGS.UI.UISIDESCREENS.{InfiniteGasSourceConfig.Id.ToUpperInvariant()}.TITLE", InfiniteGasSourceConfig.TemperatureSliderTitle);
                Strings.Add($"STRINGS.UI.UISIDESCREENS.{InfiniteGasSourceConfig.Id.ToUpperInvariant()}.TOOLTIP", InfiniteGasSourceConfig.TemperatureSliderTooltip);


                Strings.Add($"STRINGS.UI.UISIDESCREENS.INFINITESOURCE.FLOW.TITLE", InfiniteSourceFlowControl.FlowTitle);
                Strings.Add($"STRINGS.UI.UISIDESCREENS.INFINITESOURCE.FLOW.TOOLTIP", InfiniteSourceFlowControl.FlowTooltip);

                ModUtil.AddBuildingToPlanScreen("Plumbing", InfiniteLiquidSinkConfig.Id);
                ModUtil.AddBuildingToPlanScreen("Plumbing", InfiniteLiquidSourceConfig.Id);
                ModUtil.AddBuildingToPlanScreen("HVAC", InfiniteGasSinkConfig.Id);
                ModUtil.AddBuildingToPlanScreen("HVAC", InfiniteGasSourceConfig.Id);
            }
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            private static void Prefix()
            {
                var liquid = new List<string>(Database.Techs.TECH_GROUPING["LiquidPiping"]) { InfiniteLiquidSinkConfig.Id, InfiniteLiquidSourceConfig.Id };
                Database.Techs.TECH_GROUPING["LiquidPiping"] = liquid.ToArray();

                var gas = new List<string>(Database.Techs.TECH_GROUPING["GasPiping"]) { InfiniteGasSinkConfig.Id, InfiniteGasSourceConfig.Id };
                Database.Techs.TECH_GROUPING["GasPiping"] = gas.ToArray();
            }
        }

        [HarmonyPatch(typeof(KSerialization.Manager))]
        [HarmonyPatch("GetType")]
        [HarmonyPatch(new[] { typeof(string) })]
        public static class InfiniteSourceSinkSerializationPatch
        {
            public static void Postfix(string type_name, ref Type __result)
            {
                if (type_name == "InfiniteSourceSink.InfiniteSink")
                {
                    __result = typeof(InfiniteSink);
                }
                else if (type_name == "InfiniteSourceSink.InfiniteSource")
                {
                    __result = typeof(InfiniteSource);
                }
            }
        }
    }

}
