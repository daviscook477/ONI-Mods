using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace Champagne
{
    public class ChampagneFillerPatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ChampagneFillerConfig.Id.ToUpperInvariant()}.NAME", ChampagneFillerConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ChampagneFillerConfig.Id.ToUpperInvariant()}.DESC", ChampagneFillerConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ChampagneFillerConfig.Id.ToUpperInvariant()}.EFFECT", ChampagneFillerConfig.Effect);

                ModUtil.AddBuildingToPlanScreen("Furniture", ChampagneFillerConfig.Id);
            }
        }

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            private static void Prefix()
            {
                var precisionPlumbing = new List<string>(Database.Techs.TECH_GROUPING["PrecisionPlumbing"]) { ChampagneFillerConfig.Id };
                Database.Techs.TECH_GROUPING["PrecisionPlumbing"] = precisionPlumbing.ToArray();
            }
        }

        public const string INTERACTION_ANIM_PREFIX = "anim_interacts";
        public static HashedString INTERACTION_ANIM_GROUP = new HashedString(296858093);

        [HarmonyPatch(typeof(ModUtil), "AddKAnimMod")]
        public class ModUtil_AddKAnimMod_Patch
        {
            public static bool Prefix(string name, KAnimFile.Mod anim_mod)
            {
                if (!name.StartsWith(INTERACTION_ANIM_PREFIX))
                {
                    return true;
                }
                KAnimFile instance = ScriptableObject.CreateInstance<KAnimFile>();
                instance.mod = anim_mod;
                instance.name = name;
                KAnimGroupFile groupFile = KAnimGroupFile.GetGroupFile();
                HashedString groupId = INTERACTION_ANIM_GROUP;
                List<KAnimGroupFile.Group> groups = Traverse.Create(groupFile).Field("groups").GetValue<List<KAnimGroupFile.Group>>();
                var group = groups.Find(t => t.id == groupId);
                if (group == null)
                {
                    Debug.LogError($"Could not find animation group {INTERACTION_ANIM_GROUP.HashValue} when adding animation {name}. Loading animation as normal and not as interaction. Perhaps Klei changed the name of the interaction group?");
                    return true;
                }
                group.files.Add(instance);
                Assets.ModLoadedKAnims.Add(instance);
                return false;
            }
        }
    }
}
