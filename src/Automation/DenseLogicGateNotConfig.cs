using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STRINGS;

namespace Automation
{
    public class DenseLogicGateNotConfig : DenseLogicGateBaseConfig
    {
        public const string Id = "DenseLogicGateNOT";

        protected override LogicGateBase.Op GetLogicOp()
        {
            return LogicGateBase.Op.Not;
        }

        protected override CellOffset[] InputPortOffsets
        {
            get
            {
                return new CellOffset[1]
                {
                    CellOffset.none
                };
            }
        }

        protected override CellOffset[] OutputPortOffsets
        {
            get
            {
                return new CellOffset[1] { new CellOffset(1, 0) };
            }
        }

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = CreateBuildingDef(Id, "not_dense_kanim", 2, 1);
            buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
            {
                LogicPorts.Port.RibbonInputPort(LogicRibbonReader.INPUT_PORT_ID, new CellOffset(0, 0),
                    UI.LOGIC_PORTS.GATE_MULTI_INPUT_ONE_NAME,
                    UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Provides a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " to its corresponding bit in the AND gate.",
                    UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Provides a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " to its corresponding bit in the AND gate."),
            };
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
            {
                LogicPorts.Port.OutputPort(LogicRibbonReader.OUTPUT_PORT_ID, new CellOffset(1, 0),
                    BUILDINGS.PREFABS.LOGICGATENOT.OUTPUT_NAME,
                    BUILDINGS.PREFABS.LOGICGATENOT.OUTPUT_ACTIVE,
                    BUILDINGS.PREFABS.LOGICGATENOT.OUTPUT_INACTIVE, true, false)
            };
            return buildingDef;
        }
    }
}
