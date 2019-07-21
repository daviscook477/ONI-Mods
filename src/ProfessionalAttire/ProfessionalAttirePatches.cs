using System;
using Harmony;

namespace ProfessionalAttire
{
    public class ProfessionalAttirePatches
    {

        public const float BasicAttributeIncrease = 2.0f;

        private static void RegisterTypeStrings(Type type)
        {
            string Id = (string)type.GetField("Id").GetValue(null);
            string DisplayName = (string)type.GetField("DisplayName").GetValue(null);
            string GenericName = (string)type.GetField("GenericName").GetValue(null);
            string Description = (string)type.GetField("Description").GetValue(null);
            Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{Id.ToUpperInvariant()}.NAME", DisplayName);
            Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{Id.ToUpperInvariant()}.GENERICNAME", GenericName);
            Strings.Add($"STRINGS.EQUIPMENT.PREFABS.{Id.ToUpperInvariant()}.DESC", Description);
        }

        [HarmonyPatch(typeof(GeneratedEquipment))]
        [HarmonyPatch("LoadGeneratedEquipment")]
        public class GeneratedEquipment_LoadGeneratedEquipment_Patch
        {
            private static void Prefix()
            {
                RegisterTypeStrings(typeof(ArtistAttireConfig));
                RegisterTypeStrings(typeof(BuildingAttireConfig));
                RegisterTypeStrings(typeof(CookAttireConfig));
                RegisterTypeStrings(typeof(DiggingAttireConfig));
                RegisterTypeStrings(typeof(DoctorAttireConfig));
                RegisterTypeStrings(typeof(FarmingAttireConfig));
                RegisterTypeStrings(typeof(RanchingAttireConfig));
                RegisterTypeStrings(typeof(ScientistAttireConfig));
                RegisterTypeStrings(typeof(StrongAttireConfig));
                RegisterTypeStrings(typeof(TinkerAttireConfig));
            }

            private static void Postfix()
            {
                EquipmentConfigManager.Instance.RegisterEquipment(new ArtistAttireConfig());
                EquipmentConfigManager.Instance.RegisterEquipment(new BuildingAttireConfig());
                EquipmentConfigManager.Instance.RegisterEquipment(new CookAttireConfig());
                EquipmentConfigManager.Instance.RegisterEquipment(new DiggingAttireConfig());
                EquipmentConfigManager.Instance.RegisterEquipment(new DoctorAttireConfig());
                EquipmentConfigManager.Instance.RegisterEquipment(new FarmingAttireConfig());
                EquipmentConfigManager.Instance.RegisterEquipment(new RanchingAttireConfig());
                EquipmentConfigManager.Instance.RegisterEquipment(new ScientistAttireConfig());
                EquipmentConfigManager.Instance.RegisterEquipment(new StrongAttireConfig());
                EquipmentConfigManager.Instance.RegisterEquipment(new TinkerAttireConfig());
            }
        }



        [HarmonyPatch(typeof(ClothingFabricatorConfig))]
        [HarmonyPatch("ConfigureRecipes")]
        public class ClothingFabricatorConfig_ConfigureRecipes_Patch
        {
            private static void Postfix()
            {
                ArtistAttireConfig.ConfigureRecipe();
                BuildingAttireConfig.ConfigureRecipe();
                CookAttireConfig.ConfigureRecipe();
                DiggingAttireConfig.ConfigureRecipe();
                DoctorAttireConfig.ConfigureRecipe();
                FarmingAttireConfig.ConfigureRecipe();
                RanchingAttireConfig.ConfigureRecipe();
                ScientistAttireConfig.ConfigureRecipe();
                StrongAttireConfig.ConfigureRecipe();
                TinkerAttireConfig.ConfigureRecipe();
            }
        }

    }
}
