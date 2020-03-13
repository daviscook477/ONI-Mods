using System.Collections.Generic;

using TUNING;
using UnityEngine;

namespace CrystalBiome.Buildings
{
    public class RockPolisherConfig : IBuildingConfig
    {
        public const string Id = "RockPolisher";
        public const string DisplayName = "Rock Polisher";
        public const string Description = "Polishes some raw minerals into pretty gemstones.";
        public const string Effect = "Uses brute force to turn dull minerals in shiny gems.";

        public override BuildingDef CreateBuildingDef()
        {

            var buildingDef = BuildingTemplates.CreateBuildingDef(
                id: Id,
                width: 3,
                height: 2,
                anim: "rock_polisher",
                hitpoints: BUILDINGS.HITPOINTS.TIER2,
                construction_time: BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER2,
                construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER3,
                construction_materials: MATERIALS.ALL_METALS,
                melting_point: BUILDINGS.MELTING_POINT_KELVIN.TIER0,
                build_location_rule: BuildLocationRule.OnFloor,
                decor: BUILDINGS.DECOR.PENALTY.TIER1,
                noise: NOISE_POLLUTION.NOISY.TIER5,
                0.2f
                );
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 120f;
            buildingDef.ExhaustKilowattsWhenActive = 3f;
            buildingDef.SelfHeatKilowattsWhenActive = 2f;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
            go.AddOrGet<DropAllWorkable>();
            go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
            ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
            fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
            fabricator.duplicantOperated = false;
            go.AddOrGet<FabricatorIngredientStatusManager>();
            go.AddOrGet<CopyBuildingSettings>();
            BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
            ConfigureRecipes();
            Prioritizable.AddRef(go);
        }

        private void ConfigureRecipes()
        {
            float inputAmount = 200.0f;
            float outputAmount = 100.0f;
            ComplexRecipe.RecipeElement[] ingredients1 = new ComplexRecipe.RecipeElement[1]
            {
                new ComplexRecipe.RecipeElement(
                    ElementLoader.FindElementByHash(Elements.CorundumElement.SimHash).tag,
                    inputAmount)
            };
            ComplexRecipe.RecipeElement[] results1 = new ComplexRecipe.RecipeElement[1]
            {
                  new ComplexRecipe.RecipeElement(
                      ElementLoader.FindElementByHash(Elements.PolishedCorundumElement.SimHash).tag,
                      outputAmount)
            };
            string str1 = ComplexRecipeManager.MakeRecipeID(Id, 
                ingredients1, results1);
            string desc1 = $"Polishes {STRINGS.UI.FormatAsLink(Elements.CorundumElement.Name, Elements.CorundumElement.Id)} into a shiny gemstone.";
            new ComplexRecipe(str1, ingredients1, results1)
            {
                time = BUILDINGS.FABRICATION_TIME_SECONDS.SHORT,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                description = desc1
            }.fabricators = new List<Tag>()
            {
              TagManager.Create(Id)
            };
            ComplexRecipe.RecipeElement[] ingredients2 = new ComplexRecipe.RecipeElement[1]
            {
                new ComplexRecipe.RecipeElement(
                    ElementLoader.FindElementByHash(Elements.KyaniteElement.SimHash).tag,
                    inputAmount)
            };
            ComplexRecipe.RecipeElement[] results2 = new ComplexRecipe.RecipeElement[1]
            {
                  new ComplexRecipe.RecipeElement(
                      ElementLoader.FindElementByHash(Elements.PolishedKyaniteElement.SimHash).tag,
                      outputAmount)
            };
            string str2 = ComplexRecipeManager.MakeRecipeID(Id,
                ingredients2, results2);
            string desc2 = $"Polishes {STRINGS.UI.FormatAsLink(Elements.KyaniteElement.Name, Elements.KyaniteElement.Id)} into a shiny gemstone.";
            new ComplexRecipe(str2, ingredients2, results2)
            {
                time = BUILDINGS.FABRICATION_TIME_SECONDS.SHORT,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                description = desc2
            }.fabricators = new List<Tag>()
            {
              TagManager.Create(Id)
            };
            ComplexRecipe.RecipeElement[] ingredients3 = new ComplexRecipe.RecipeElement[1]
            {
                new ComplexRecipe.RecipeElement(
                    ElementLoader.FindElementByHash(Elements.SodaliteElement.SimHash).tag,
                    inputAmount)
            };
            ComplexRecipe.RecipeElement[] results3 = new ComplexRecipe.RecipeElement[1]
            {
                  new ComplexRecipe.RecipeElement(
                      ElementLoader.FindElementByHash(Elements.PolishedSodaliteElement.SimHash).tag,
                      outputAmount)
            };
            string str3 = ComplexRecipeManager.MakeRecipeID(Id,
                ingredients3, results3);
            string desc3 = $"Polishes {STRINGS.UI.FormatAsLink(Elements.SodaliteElement.Name, Elements.SodaliteElement.Id)} into a shiny gemstone.";
            new ComplexRecipe(str3, ingredients3, results3)
            {
                time = BUILDINGS.FABRICATION_TIME_SECONDS.SHORT,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                description = desc3
            }.fabricators = new List<Tag>()
            {
              TagManager.Create(Id)
            };
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGetDef<PoweredActiveController.Def>();
            SymbolOverrideControllerUtil.AddToPrefab(go);
        }
    }
}
