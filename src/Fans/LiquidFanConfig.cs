using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using Harmony;

namespace Fans
{
    public class LiquidFanConfig : IBuildingConfig
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
                var liquidPiping = new List<string>(Database.Techs.TECH_GROUPING["LiquidPiping"]) { Id };
                Database.Techs.TECH_GROUPING["LiquidPiping"] = liquidPiping.ToArray();
            }
        }

        public const string Id = "TurbineBlock";
        public static string DisplayName = UI.FormatAsLink("Turbine Block", Id.ToUpper());
        public static string Description = $"Moves {UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID")} from one side to the other.";
        public static string Effect = $"Pumps around {UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID")}.";

        private const float SuckRate = 5.0f;
        private const ConduitType Type = ConduitType.Liquid;
        private const float OverPressureThreshold = 1000.0f;

        public override BuildingDef CreateBuildingDef()
        {
            return BaseFanConfig.CreateBuildingDef(Id, "liquidfan_kanim");
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BaseFanConfig.ConfigureBuildingTemplate(go, prefab_tag);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            BaseFanConfig.DoPostConfigureComplete(go, SuckRate, Type, OverPressureThreshold);
        }
    }

}
