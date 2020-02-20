using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using UnityEngine;

namespace Automation
{
    public abstract class DenseLogicGateBaseConfig : IBuildingConfig
    {
        protected BuildingDef CreateBuildingDef(
            string ID,
            string anim,
            int width = 2,
            int height = 2)
        {
            string id = ID;
            int width1 = width;
            int height1 = height;
            string anim1 = anim;
            float[] tieR0_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            EffectorValues none = NOISE_POLLUTION.NONE;
            EffectorValues tieR0_2 = BUILDINGS.DECOR.PENALTY.TIER0;
            EffectorValues noise = none;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width1, height1, anim1, 10, 3f, tieR0_1, refinedMetals, 1600f, BuildLocationRule.Anywhere, tieR0_2, noise, 0.2f);
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.ObjectLayer = ObjectLayer.LogicGates;
            buildingDef.SceneLayer = Grid.SceneLayer.LogicGates;
            buildingDef.ThermalConductivity = 0.05f;
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.Entombable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.DragBuild = true;
            LogicGateBase.uiSrcData = Assets.instance.logicModeUIData;
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
            return buildingDef;
        }

        protected abstract CellOffset[] InputPortOffsets { get; }

        protected abstract CellOffset[] OutputPortOffsets { get; }

        protected abstract LogicGateBase.Op GetLogicOp();

        protected abstract DenseLogicGate.DenseLogicGateDescriptions GetDescriptions();

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
            MoveableLogicGateVisualizer logicGateVisualizer = go.AddComponent<MoveableLogicGateVisualizer>();
            logicGateVisualizer.op = GetLogicOp();
            logicGateVisualizer.inputPortOffsets = InputPortOffsets;
            logicGateVisualizer.outputPortOffsets = OutputPortOffsets;
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            LogicGateVisualizer logicGateVisualizer = go.AddComponent<LogicGateVisualizer>();
            logicGateVisualizer.op = GetLogicOp();
            logicGateVisualizer.inputPortOffsets = InputPortOffsets;
            logicGateVisualizer.outputPortOffsets = OutputPortOffsets;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            DenseLogicGate logicGate = go.AddComponent<DenseLogicGate>();
            logicGate.op = GetLogicOp();
            logicGate.inputPortOffsets = InputPortOffsets;
            logicGate.outputPortOffsets = OutputPortOffsets;
            go.GetComponent<KPrefabID>().prefabInitFn += game_object => game_object.GetComponent<DenseLogicGate>().SetPortDescriptions(GetDescriptions());
        }
    }
}
