using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                      ElementLoader.FindElementByHash(SimHashes.Lime).tag,
                      outputAmount)
            };
            string str1 = ComplexRecipeManager.MakeRecipeID(Id, 
                ingredients1, results1);
            string desc1 = "Turn corundum into lime.";
            new ComplexRecipe(str1, ingredients1, results1)
            {
                time = BUILDINGS.FABRICATION_TIME_SECONDS.SHORT,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                description = desc1
            }.fabricators = new List<Tag>()
            {
              TagManager.Create(Id)
            };
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_1);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_1);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_1);
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGetDef<PoweredActiveController.Def>();
            SymbolOverrideControllerUtil.AddToPrefab(go);
        }
    }
}
