using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace Automation
{
    public class DenseLogicGatePatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            private static void Prefix()
            {
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{DenseLogicGateOrConfig.Id.ToUpperInvariant()}.NAME", "Dense " + STRINGS.BUILDINGS.PREFABS.LOGICGATEOR.NAME);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{DenseLogicGateOrConfig.Id.ToUpperInvariant()}.DESC", STRINGS.BUILDINGS.PREFABS.LOGICGATEOR.DESC);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{DenseLogicGateOrConfig.Id.ToUpperInvariant()}.EFFECT", STRINGS.BUILDINGS.PREFABS.LOGICGATEOR.EFFECT);

                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{DenseLogicGateAndConfig.Id.ToUpperInvariant()}.NAME", "Dense " + STRINGS.BUILDINGS.PREFABS.LOGICGATEAND.NAME);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{DenseLogicGateAndConfig.Id.ToUpperInvariant()}.DESC", STRINGS.BUILDINGS.PREFABS.LOGICGATEAND.DESC);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{DenseLogicGateAndConfig.Id.ToUpperInvariant()}.EFFECT", STRINGS.BUILDINGS.PREFABS.LOGICGATEAND.EFFECT);

                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{DenseLogicGateNotConfig.Id.ToUpperInvariant()}.NAME", "Dense " + STRINGS.BUILDINGS.PREFABS.LOGICGATENOT.NAME);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{DenseLogicGateNotConfig.Id.ToUpperInvariant()}.DESC", STRINGS.BUILDINGS.PREFABS.LOGICGATENOT.DESC);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{DenseLogicGateNotConfig.Id.ToUpperInvariant()}.EFFECT", STRINGS.BUILDINGS.PREFABS.LOGICGATENOT.EFFECT);

                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{DenseLogicGateXorConfig.Id.ToUpperInvariant()}.NAME", "Dense " + STRINGS.BUILDINGS.PREFABS.LOGICGATEXOR.NAME);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{DenseLogicGateXorConfig.Id.ToUpperInvariant()}.DESC", STRINGS.BUILDINGS.PREFABS.LOGICGATEXOR.DESC);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{DenseLogicGateXorConfig.Id.ToUpperInvariant()}.EFFECT", STRINGS.BUILDINGS.PREFABS.LOGICGATEXOR.EFFECT);

                ModUtil.AddBuildingToPlanScreen("Refining", DenseLogicGateOrConfig.Id);
                ModUtil.AddBuildingToPlanScreen("Refining", DenseLogicGateAndConfig.Id);
                ModUtil.AddBuildingToPlanScreen("Refining", DenseLogicGateNotConfig.Id);
                ModUtil.AddBuildingToPlanScreen("Refining", DenseLogicGateXorConfig.Id);
            }
        }

        /*[HarmonyPatch(typeof(BuildToolHoverTextCard), nameof(BuildToolHoverTextCard.UpdateHoverElements))]
        public class BuildToolHoverTextCard_UpdateHoverElements_Patch
        {
            private static void Postfix
        }*/
    }
}
