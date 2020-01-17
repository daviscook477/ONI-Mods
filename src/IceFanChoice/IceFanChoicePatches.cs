using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace IceFanChoice
{
    public class IceFanChoicePatches
    {


        [HarmonyPatch(typeof(IceCooledFanConfig), "ConfigureBuildingTemplate")]
        public class IceCooledFanConfig_ConfigureBuildingTemplate_Patch
        {
            public static void Postfix(GameObject go, Tag prefab_tag)
            {
                IceCooledFan fan = go.GetComponent<IceCooledFan>();
                fan.iceStorage.storageFilters = new List<Tag>()
                {
                    GameTags.Liquifiable
                };
                fan.iceStorage.showInUI = true;
                go.AddComponent<TreeFilterable>();
                go.AddComponent<IceFanChoice>();
            }
        }
    }
}
