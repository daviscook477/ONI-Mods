using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using Harmony;

namespace Fans
{
    public class CompressorLiquidFanConfig : IBuildingConfig
    {
        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{Id.ToUpperInvariant()}.NAME", DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{Id.ToUpperInvariant()}.DESC", Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{Id.ToUpperInvariant()}.EFFECT", Effect);

                ModUtil.AddBuildingToPlanScreen("Plumbing", Id);
            }
        }

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            private static void Prefix()
            {
                var liquidTemperature = new List<string>(Database.Techs.TECH_GROUPING["LiquidTemperature"]) { Id };
                Database.Techs.TECH_GROUPING["LiquidTemperature"] = liquidTemperature.ToArray();
            }
        }

        public const string Id = "CompressorTurbineBlock";
        public static string DisplayName = UI.FormatAsLink("Compressor Turbine Block", Id.ToUpper());
        public static string Description = $"Compresses {UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID")} from one side to the other.";
        public static string Effect = $"Compresses {UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID")}.";

        private const float SuckRate = 5.0f;
        private const ConduitType Type = ConduitType.Liquid;
        private const float OverPressureThreshold = -1.0f;

        public override BuildingDef CreateBuildingDef()
        {
            return BaseFanConfig.CreateBuildingDef(Id, "liquidfan_kanim", false);
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BaseFanConfig.ConfigureBuildingTemplate(go, prefab_tag, false);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            BaseFanConfig.DoPostConfigureComplete(go, SuckRate, Type, OverPressureThreshold);
        }
    }
}
