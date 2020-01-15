using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace PipIceSculpture {
	public class PipIceSculpturePatches {

        [HarmonyPatch(typeof(IceSculptureConfig), "DoPostConfigureComplete")]
        private static class Patch_MarbleSculptureConfig_DoPostConfigureComplete {
            public static void Postfix(GameObject go) {
                Artable artable = go.AddOrGet<Sculpture>();
                artable.stages.Add(new Artable.Stage(
                    "Average2",
                    STRINGS.BUILDINGS.PREFABS.ICESCULPTURE.AVERAGEQUALITYNAME,
                    "idle2",
                    10,
                    true,
                    Artable.Status.Okay));
            }
        }

    }
}
