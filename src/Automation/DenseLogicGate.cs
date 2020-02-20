using KSerialization;
using STRINGS;
using System;
using Harmony;

namespace Automation
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class DenseLogicGate : DenseLogicGateBase, ILogicEventSender, ILogicNetworkConnection
    {
        private static readonly DenseLogicGateDescriptions.Description INPUT_ONE_MULTI_DESCRIPTION = new DenseLogicGateDescriptions.Description()
        {
            name = UI.LOGIC_PORTS.GATE_MULTI_INPUT_ONE_NAME,
            active = UI.LOGIC_PORTS.GATE_MULTI_INPUT_ONE_ACTIVE,
            inactive = UI.LOGIC_PORTS.GATE_MULTI_INPUT_ONE_INACTIVE
        };
        private static readonly DenseLogicGateDescriptions.Description INPUT_TWO_MULTI_DESCRIPTION  = new DenseLogicGateDescriptions.Description()
        {
            name = UI.LOGIC_PORTS.GATE_MULTI_INPUT_TWO_NAME,
            active = UI.LOGIC_PORTS.GATE_MULTI_INPUT_TWO_ACTIVE,
            inactive = UI.LOGIC_PORTS.GATE_MULTI_INPUT_TWO_INACTIVE
        };
        private static readonly DenseLogicGateDescriptions.Description OUTPUT_ONE_MULTI_DESCRIPTION = new DenseLogicGateDescriptions.Description()
        {
            name = UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_ONE_NAME,
            active = UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_ONE_ACTIVE,
            inactive = UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_ONE_INACTIVE
        };

        private class StaticDelegateWrappers
        {
            public static void OnBuildingBrokenWrapper(DenseLogicGate component, object data)
            {
                component.OnBuildingBroken(data);
            }

            public static void OnBuildingFullyRepairedWrapper(DenseLogicGate component, object data)
            {
                component.OnBuildingFullyRepaired(data);
            }
        }

        private static readonly EventSystem.IntraObjectHandler<DenseLogicGate> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<DenseLogicGate>(StaticDelegateWrappers.OnBuildingBrokenWrapper);
        private static readonly EventSystem.IntraObjectHandler<DenseLogicGate> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<DenseLogicGate>(StaticDelegateWrappers.OnBuildingFullyRepairedWrapper);

        private bool connected = false;
        protected bool cleaningUp = false;
        private int lastAnimState = 0;
        private DenseLogicGateDescriptions descriptions;
        private const bool IS_CIRCUIT_ENDPOINT = true;
        [Serialize]
        protected int outputValueOne;

        private object inputOne;
        private object inputTwo;
        private LogicPortVisualizer outputOne;

        private static object CreateLogicEventHandler(int cell, Action<int> on_value_changed, Action<int, bool> on_connection_changed, LogicPortSpriteType sprite_type)
        {
            Type logicEventHandlerType = typeof(LogicGate).Assembly.GetType("LogicEventHandler");
            return Activator.CreateInstance(logicEventHandlerType, cell, on_value_changed, on_connection_changed, sprite_type);
        }

        private static int GetValueOfLogicEventHandler(object event_handler)
        {
            return Traverse.Create(event_handler).Property("Value").GetValue<int>();
        }

        protected override void OnSpawn()
        {
            inputOne = CreateLogicEventHandler(InputCellOne, new Action<int>(UpdateState), null, LogicPortSpriteType.Input);
            if (RequiresTwoInputs)
                inputTwo = CreateLogicEventHandler(InputCellTwo, new Action<int>(UpdateState), null, LogicPortSpriteType.Input);
            Subscribe(774203113, OnBuildingBrokenDelegate);
            Subscribe(-1735440190, OnBuildingFullyRepairedDelegate);
            BuildingHP component = GetComponent<BuildingHP>();
            if (component != null && component.IsBroken)
                return;
            Connect();
        }

        protected override void OnCleanUp()
        {
            cleaningUp = true;
            Disconnect();
            Unsubscribe(774203113, OnBuildingBrokenDelegate, false);
            Unsubscribe(-1735440190, OnBuildingFullyRepairedDelegate, false);
            base.OnCleanUp();
        }

        private void OnBuildingBroken(object data)
        {
            Disconnect();
        }

        private void OnBuildingFullyRepaired(object data)
        {
            Connect();
        }

        private void Connect()
        {
            if (connected)
                return;
            LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
            UtilityNetworkManager<LogicCircuitNetwork, LogicWire> logicCircuitSystem = Game.Instance.logicCircuitSystem;
            connected = true;
            int outputCellOne = OutputCellOne;
            logicCircuitSystem.AddToNetworks(outputCellOne, this, true);
            outputOne = new LogicPortVisualizer(outputCellOne, LogicPortSpriteType.Output);
            logicCircuitManager.AddVisElem(outputOne);
            int inputCellOne = InputCellOne;
            logicCircuitSystem.AddToNetworks(inputCellOne, inputOne, true);
            logicCircuitManager.AddVisElem((ILogicUIElement)inputOne);
            if (RequiresTwoInputs)
            {
                int inputCellTwo = InputCellTwo;
                logicCircuitSystem.AddToNetworks(inputCellTwo, inputTwo, true);
                logicCircuitManager.AddVisElem((ILogicUIElement)inputTwo);
            }
            this.RefreshAnimation();
        }

        private void Disconnect()
        {
            if (!connected)
                return;
            LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
            UtilityNetworkManager<LogicCircuitNetwork, LogicWire> logicCircuitSystem = Game.Instance.logicCircuitSystem;
            connected = false;
            int outputCellOne = OutputCellOne;
            logicCircuitSystem.RemoveFromNetworks(outputCellOne, this, true);
            logicCircuitManager.RemoveVisElem(outputOne);
            outputOne = null;
            int inputCellOne = InputCellOne;
            logicCircuitSystem.RemoveFromNetworks(inputCellOne, inputOne, true);
            logicCircuitManager.RemoveVisElem((ILogicUIElement)inputOne);
            inputOne = null;
            if (RequiresTwoInputs)
            {
                int inputCellTwo = InputCellTwo;
                logicCircuitSystem.RemoveFromNetworks(inputCellTwo, inputTwo, true);
                logicCircuitManager.RemoveVisElem((ILogicUIElement)inputTwo);
                inputTwo = null;
            }
            RefreshAnimation();
        }

        private void UpdateState(int new_value)
        {
            if (cleaningUp)
                return;
            int val1 = GetValueOfLogicEventHandler(inputOne);
            int val2 = inputTwo != null ? GetValueOfLogicEventHandler(inputTwo) : 0;
            switch (op)
            {
                case LogicGateBase.Op.And:
                    outputValueOne = val1 & val2;
                    break;
                case LogicGateBase.Op.Or:
                    outputValueOne = val1 | val2;
                    break;
                case LogicGateBase.Op.Not:
                    outputValueOne = ~val1;
                    break;
                case LogicGateBase.Op.Xor:
                    outputValueOne = val1 ^ val2;
                    break;
                case LogicGateBase.Op.CustomSingle:
                    outputValueOne = GetCustomValue(val1, val2);
                    break;
            }
            RefreshAnimation();
        }

        public virtual void LogicTick()
        {
        }

        protected virtual int GetCustomValue(int val1, int val2)
        {
            return val1;
        }

        public int GetPortValue(LogicGateBase.PortId port)
        {
            switch (port)
            {
                case LogicGateBase.PortId.InputOne:
                    return GetValueOfLogicEventHandler(inputOne);
                case LogicGateBase.PortId.InputTwo:
                    if (RequiresTwoInputs)
                        return GetValueOfLogicEventHandler(inputTwo);
                    return 0;
                case LogicGateBase.PortId.OutputOne:
                    return outputValueOne;
                default:
                    return outputValueOne;
            }
        }

        public bool GetPortConnected(LogicGateBase.PortId port)
        {
            if (port == LogicGateBase.PortId.InputTwo && !RequiresTwoInputs)
                return false;
            return Game.Instance.logicCircuitManager.GetNetworkForCell(PortCell(port)) != null;
        }

        public void SetPortDescriptions(DenseLogicGateDescriptions descriptions)
        {
            this.descriptions = descriptions;
        }

        public DenseLogicGateDescriptions.Description GetPortDescription(LogicGateBase.PortId port)
        {
            switch (port)
            {
                case LogicGateBase.PortId.InputOne:
                    return descriptions.inputOne != null ? descriptions.inputOne : (RequiresTwoInputs ? INPUT_ONE_MULTI_DESCRIPTION : INPUT_ONE_MULTI_DESCRIPTION);
                case LogicGateBase.PortId.InputTwo:
                    return descriptions.inputTwo != null ? descriptions.inputTwo : INPUT_TWO_MULTI_DESCRIPTION;
                case LogicGateBase.PortId.OutputOne:
                    return descriptions.inputOne != null ? descriptions.inputOne : OUTPUT_ONE_MULTI_DESCRIPTION;
                default:
                    return descriptions.outputOne;
            }
        }

        public int GetLogicValue()
        {
            return outputValueOne;
        }

        public int GetLogicCell()
        {
            return GetLogicUICell();
        }

        public int GetLogicUICell()
        {
            return OutputCellOne;
        }

        public bool IsLogicInput()
        {
            return false;
        }

        private static int GetLightBulbStateForWireState(int wire_state)
        {
            if (wire_state == 0)
            {
                return 0;
            }
            else if (wire_state == 0b1111)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        protected void RefreshAnimation()
        {
            if (cleaningUp)
                return;
            KBatchedAnimController component = GetComponent<KBatchedAnimController>();
            if (!(Game.Instance.logicCircuitSystem.GetNetworkForCell(OutputCellOne) is LogicCircuitNetwork))
                component.Play("off", KAnim.PlayMode.Once, 1f, 0.0f);
            else if (RequiresTwoInputs)
            {
                int num = GetLightBulbStateForWireState(GetValueOfLogicEventHandler(inputOne)) +
                    GetLightBulbStateForWireState(GetValueOfLogicEventHandler(inputTwo)) * 3 + 
                    outputValueOne * 9;
                component.Play("on_" + num.ToString(), KAnim.PlayMode.Once, 1f, 0.0f);
            }
            else
            {
                int num = GetLightBulbStateForWireState(GetValueOfLogicEventHandler(inputOne)) + 
                    outputValueOne * 3;
                component.Play("on_" + num.ToString(), KAnim.PlayMode.Once, 1f, 0.0f);
            }
        }

        public void OnLogicNetworkConnectionChanged(bool connected)
        {
        }

        public class DenseLogicGateDescriptions
        {
            public Description inputOne;
            public Description inputTwo;
            public Description outputOne;

            public class Description
            {
                public string name;
                public string active;
                public string inactive;
            }
        }
    }
}
