using Harmony;

namespace RpgClothes
{
    public class InsulatedJacketPatches
    {

        [HarmonyPatch(typeof(GeneratedEquipment))]
        [HarmonyPatch("LoadGeneratedEquipment")]
        public class GeneratedEquipment_LoadGeneratedEquipment_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{InsulatedJacketConfig.Id.ToUpperInvariant()}.NAME", InsulatedJacketConfig.DisplayName);
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{InsulatedJacketConfig.Id.ToUpperInvariant()}.GENERICNAME", InsulatedJacketConfig.GenericName);
            }

            private static void Postfix()
            {
                EquipmentConfigManager.Instance.RegisterEquipment(new InsulatedJacketConfig());
            }
        }



        [HarmonyPatch(typeof(ClothingFabricatorConfig))]
        [HarmonyPatch("ConfigureRecipes")]
        public class ClothingFabricatorConfig_ConfigureRecipes_Patch
        {
            private static void Postfix()
            {
                InsulatedJacketConfig.ConfigureRecipe();
            }
        }


    }
}
