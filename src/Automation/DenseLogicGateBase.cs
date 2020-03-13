using UnityEngine;

namespace Automation
{
    public class DenseLogicGateBase : KMonoBehaviour
    {
        public static LogicModeUI uiSrcData;
        [SerializeField]
        public LogicGateBase.Op op;
        public CellOffset[] inputPortOffsets;
        public CellOffset[] outputPortOffsets;
        private int GetActualCell(CellOffset offset)
        {
            Rotatable component = GetComponent<Rotatable>();
            if (component != null)
                offset = component.GetRotatedCellOffset(offset);
            return Grid.OffsetCell(Grid.PosToCell(transform.GetPosition()), offset);
        }

        public int InputCellOne
        {
            get
            {
                return GetActualCell(inputPortOffsets[0]);
            }
        }

        public int InputCellTwo
        {
            get
            {
                return GetActualCell(inputPortOffsets[1]);
            }
        }

        public int OutputCellOne
        {
            get
            {
                return GetActualCell(outputPortOffsets[0]);
            }
        }

        public int PortCell(LogicGateBase.PortId port)
        {
            switch (port)
            {
                case LogicGateBase.PortId.InputOne:
                    return InputCellOne;
                case LogicGateBase.PortId.InputTwo:
                    return InputCellTwo;
                case LogicGateBase.PortId.OutputOne:
                    return OutputCellOne;
                default:
                    return OutputCellOne;
            }
        }

        public bool TryGetPortAtCell(int cell, out LogicGateBase.PortId port)
        {
            if (cell == InputCellOne)
            {
                port = LogicGateBase.PortId.InputOne;
                return true;
            }
            if (RequiresTwoInputs && cell == InputCellTwo)
            {
                port = LogicGateBase.PortId.InputTwo;
                return true;
            }
            if (cell == OutputCellOne)
            {
                port = LogicGateBase.PortId.OutputOne;
                return true;
            }
            port = LogicGateBase.PortId.InputOne;
            return false;
        }

        public bool RequiresTwoInputs
        {
            get
            {
                return OpRequiresTwoInputs(op);
            }
        }

        public static bool OpRequiresTwoInputs(LogicGateBase.Op op)
        {
            switch (op)
            {
                case LogicGateBase.Op.Not:
                case LogicGateBase.Op.CustomSingle:
                    return false;
                default:
                    return true;
            }
        }
    }
}
