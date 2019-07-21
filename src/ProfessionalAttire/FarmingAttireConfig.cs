using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

namespace ProfessionalAttire
{
    public class FarmingAttireConfig : IEquipmentConfig
    {
        public const string Id = "FarmingAttire";
        public const string DisplayName = "Farmers's Outfit";
        public const string GenericName = "Clothing";
        public static string RecipeDescription = $"Tending to plants in a {DisplayName} helps a duplicant to work more effectively.";
        public static string Description = "Improves the farming capabilities of one duplicant.";

        public static int DecorModifier = ClothingWearer.ClothingInfo.BASIC_CLOTHING.decorMod;
        public static float ConductivityModifier = ClothingWearer.ClothingInfo.BASIC_CLOTHING.conductivityMod;
        public static float HomeostasisEfficiencyModifier = ClothingWearer.ClothingInfo.BASIC_CLOTHING.homeostasisEfficiencyMultiplier;
        public const float AttributeIncrease = ProfessionalAttirePatches.BasicAttributeIncrease;
        public const float VestClothMass = 6.0f;

        public static readonly ClothingWearer.ClothingInfo NEW_CLOTHING =
            new ClothingWearer.ClothingInfo(DisplayName, DecorModifier, ConductivityModifier, HomeostasisEfficiencyModifier);

        public static readonly ComplexRecipe.RecipeElement[] ingredients = new ComplexRecipe.RecipeElement[]
        {
            new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), VestClothMass),
        };
        public static readonly ComplexRecipe.RecipeElement[] results = new ComplexRecipe.RecipeElement[]
        {
            new ComplexRecipe.RecipeElement(Id.ToTag(), 1f)
        };

        public static void ConfigureRecipe()
        {
            new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ClothingFabricator",
                ingredients, results), ingredients, results)
            {
                time = TUNING.EQUIPMENT.VESTS.FUNKY_VEST_FABTIME,
                description = RecipeDescription,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                fabricators = new List<Tag>() { "ClothingFabricator" },
                sortOrder = 1
            };
        }

        public EquipmentDef CreateEquipmentDef()
        {
            ClothingWearer.ClothingInfo clothingInfo = NEW_CLOTHING;
            List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();
            attributeModifiers.Add(new AttributeModifier(Db.Get().Attributes.Botanist.Id, AttributeIncrease, DisplayName, false, false, true));
            EquipmentDef equipment = EquipmentTemplates.CreateEquipmentDef(
                Id: Id,
                Slot: TUNING.EQUIPMENT.CLOTHING.SLOT,
                OutputElement: SimHashes.Carbon,
                Mass: TUNING.EQUIPMENT.VESTS.FUNKY_VEST_MASS,
                Anim: "farmershirt",
                SnapOn: TUNING.EQUIPMENT.VESTS.SNAPON0,
                BuildOverride: "farmerbodyshirt",
                BuildOverridePriority: 4,
                AttributeModifiers: attributeModifiers,
                SnapOn1: TUNING.EQUIPMENT.VESTS.SNAPON1,
                IsBody: true,
                CollisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.75f,
                height: 0.4f,
                additional_tags: null,
                RecipeTechUnlock: null);
            string thermalConductivityDescriptor = string.Format("{0}: {1}",
                    DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME,
                    GameUtil.GetFormattedDistance(clothingInfo.conductivityMod));
            equipment.additionalDescriptors.Add(new Descriptor(
                thermalConductivityDescriptor, thermalConductivityDescriptor,
                Descriptor.DescriptorType.Effect, false));
            string decorDescriptor = string.Format("{0}: {1}",
                DUPLICANTS.ATTRIBUTES.DECOR.NAME,
                clothingInfo.decorMod);
            equipment.additionalDescriptors.Add(
                new Descriptor(decorDescriptor, decorDescriptor,
                Descriptor.DescriptorType.Effect, false));
            equipment.OnEquipCallBack = eq => CoolVestConfig.OnEquipVest(eq, clothingInfo);
            equipment.OnUnequipCallBack = eq => CoolVestConfig.OnUnequipVest(eq);
            equipment.RecipeDescription = RecipeDescription;
            return equipment;
        }

        public void DoPostConfigure(GameObject go)
        {
            go.GetComponent<KPrefabID>().AddTag(GameTags.Clothes, false);
            Equippable equippable = go.AddOrGet<Equippable>();
            equippable.SetQuality(QualityLevel.Poor);
            go.GetComponent<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.BuildingBack;
            go.GetComponent<KPrefabID>().AddTag(GameTags.PedestalDisplayable, false);
        }
    }

}
