using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STRINGS;

namespace Automation
{
    public class DenseLogicGateOrConfig : DenseLogicGateBaseConfig
    {
        public const string Id = "DenseLogicGateOR";

        protected override LogicGateBase.Op GetLogicOp()
        {
            return LogicGateBase.Op.Or;
        }

        protected override CellOffset[] InputPortOffsets
        {
            get
            {
                return new CellOffset[2]
                {
                    CellOffset.none,
                    new CellOffset(0, 1)
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
            var buildingDef = CreateBuildingDef(Id, "or_dense_kanim", 2, 2);
            buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
            {
                LogicPorts.Port.RibbonInputPort(LogicRibbonReader.INPUT_PORT_ID, new CellOffset(0, 0),
                    UI.LOGIC_PORTS.GATE_MULTI_INPUT_ONE_NAME,
                    UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Provides a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " to its corresponding bit in the AND gate.",
                    UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Provides a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " to its corresponding bit in the AND gate."),
                LogicPorts.Port.RibbonInputPort(LogicRibbonReader.INPUT_PORT_ID, new CellOffset(0, 1),
                    UI.LOGIC_PORTS.GATE_MULTI_INPUT_ONE_NAME,
                    UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Provides a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " to its corresponding bit in the AND gate.",
                    UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Provides a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " to its corresponding bit in the AND gate."),
            };
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
            {
                LogicPorts.Port.OutputPort(LogicRibbonReader.OUTPUT_PORT_ID, new CellOffset(1, 0),
                    BUILDINGS.PREFABS.LOGICGATEOR.OUTPUT_NAME,
                    BUILDINGS.PREFABS.LOGICGATEOR.OUTPUT_ACTIVE,
                    BUILDINGS.PREFABS.LOGICGATEOR.OUTPUT_INACTIVE, true, false)
            };
            return buildingDef;
        }
    }
}
