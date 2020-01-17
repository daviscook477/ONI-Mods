using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace Quarry
{
    public class IronPilePatches
    {
        [HarmonyPatch(typeof(EntityConfigManager), "LoadGeneratedEntities")]
        public class EntityConfigManager_LoadGeneratedEntities_Patch
        {
            public static void Prefix()
            {
                // iron pile
                Strings.Add($"STRINGS.CREATURES.SPECIES.IRON_PILE.NAME", IronPileConfig.Name);
                Strings.Add($"STRINGS.CREATURES.SPECIES.IRON_PILE.DESC", IronPileConfig.Description);
            }
        }
    }
}
