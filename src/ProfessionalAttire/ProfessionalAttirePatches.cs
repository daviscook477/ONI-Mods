using Harmony;

namespace ProfessionalAttire
{
    public class ProfessionalAttirePatches
    {

        public const float BasicAttributeIncrease = 2.0f;

        [HarmonyPatch(typeof(GeneratedEquipment))]
        [HarmonyPatch("LoadGeneratedEquipment")]
        public class GeneratedEquipment_LoadGeneratedEquipment_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{DiggingAttireConfig.Id.ToUpperInvariant()}.NAME", DiggingAttireConfig.DisplayName);
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{DiggingAttireConfig.Id.ToUpperInvariant()}.GENERICNAME", DiggingAttireConfig.GenericName);
            }

            private static void Postfix()
            {
                EquipmentConfigManager.Instance.RegisterEquipment(new DiggingAttireConfig());
            }
        }



        [HarmonyPatch(typeof(ClothingFabricatorConfig))]
        [HarmonyPatch("ConfigureRecipes")]
        public class ClothingFabricatorConfig_ConfigureRecipes_Patch
        {
            private static void Postfix()
            {
                DiggingAttireConfig.ConfigureRecipe();
            }
        }

    }
}
