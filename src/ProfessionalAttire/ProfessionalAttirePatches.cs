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
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{BuildingAttireConfig.Id.ToUpperInvariant()}.NAME", BuildingAttireConfig.DisplayName);
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{BuildingAttireConfig.Id.ToUpperInvariant()}.GENERICNAME", BuildingAttireConfig.GenericName);
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{FarmingAttireConfig.Id.ToUpperInvariant()}.NAME", FarmingAttireConfig.DisplayName);
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{FarmingAttireConfig.Id.ToUpperInvariant()}.GENERICNAME", FarmingAttireConfig.GenericName);
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{RanchingAttireConfig.Id.ToUpperInvariant()}.NAME", RanchingAttireConfig.DisplayName);
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{RanchingAttireConfig.Id.ToUpperInvariant()}.GENERICNAME", RanchingAttireConfig.GenericName);
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{ArtistAttireConfig.Id.ToUpperInvariant()}.NAME", ArtistAttireConfig.DisplayName);
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{ArtistAttireConfig.Id.ToUpperInvariant()}.GENERICNAME", ArtistAttireConfig.GenericName);
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{CookAttireConfig.Id.ToUpperInvariant()}.NAME", CookAttireConfig.DisplayName);
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{CookAttireConfig.Id.ToUpperInvariant()}.GENERICNAME", CookAttireConfig.GenericName);
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{ScientistAttireConfig.Id.ToUpperInvariant()}.NAME", ScientistAttireConfig.DisplayName);
                Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{ScientistAttireConfig.Id.ToUpperInvariant()}.GENERICNAME", ScientistAttireConfig.GenericName);
            }

            private static void Postfix()
            {
                EquipmentConfigManager.Instance.RegisterEquipment(new DiggingAttireConfig());
                EquipmentConfigManager.Instance.RegisterEquipment(new BuildingAttireConfig());
                EquipmentConfigManager.Instance.RegisterEquipment(new FarmingAttireConfig());
                EquipmentConfigManager.Instance.RegisterEquipment(new RanchingAttireConfig());
                EquipmentConfigManager.Instance.RegisterEquipment(new ArtistAttireConfig());
                EquipmentConfigManager.Instance.RegisterEquipment(new CookAttireConfig());
                EquipmentConfigManager.Instance.RegisterEquipment(new ScientistAttireConfig());
            }
        }



        [HarmonyPatch(typeof(ClothingFabricatorConfig))]
        [HarmonyPatch("ConfigureRecipes")]
        public class ClothingFabricatorConfig_ConfigureRecipes_Patch
        {
            private static void Postfix()
            {
                DiggingAttireConfig.ConfigureRecipe();
                BuildingAttireConfig.ConfigureRecipe();
                FarmingAttireConfig.ConfigureRecipe();
                RanchingAttireConfig.ConfigureRecipe();
                ArtistAttireConfig.ConfigureRecipe();
                CookAttireConfig.ConfigureRecipe();
                ScientistAttireConfig.ConfigureRecipe();
            }
        }

    }
}
