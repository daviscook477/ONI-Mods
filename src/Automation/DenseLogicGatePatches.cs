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
                ModUtil.AddBuildingToPlanScreen("Refining", DenseLogicGateAndConfig.Id);
            }
        }
    }
}
