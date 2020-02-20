using System.Collections.Generic;
using UnityEngine;

namespace Automation
{
    [SkipSaveFileSerialization]
    public class DenseLogicGateVisualizer : DenseLogicGateBase
    {
        private List<IOVisualizer> visChildren = new List<IOVisualizer>();

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Register();
        }

        protected override void OnCleanUp()
        {
            base.OnCleanUp();
            Unregister();
        }

        private void Register()
        {
            Unregister();
            visChildren.Add(new IOVisualizer(OutputCellOne, false, true));
            visChildren.Add(new IOVisualizer(InputCellOne, true, true));
            if (RequiresTwoInputs)
                visChildren.Add(new IOVisualizer(InputCellTwo, true, true));
            LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
            foreach (IOVisualizer visChild in visChildren)
                logicCircuitManager.AddVisElem(visChild);
        }

        private void Unregister()
        {
            LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
            foreach (IOVisualizer visChild in visChildren)
                logicCircuitManager.RemoveVisElem(visChild);
            visChildren.Clear();
        }

        private class IOVisualizer : ILogicUIElement, IUniformGridObject
        {
            private int cell;
            private bool input;
            private bool ribbon;

            public IOVisualizer(int cell, bool input, bool ribbon)
            {
                this.cell = cell;
                this.input = input;
                this.ribbon = ribbon;
            }

            public int GetLogicUICell()
            {
                return cell;
            }

            public LogicPortSpriteType GetLogicPortSpriteType()
            {
                return input ? (ribbon ? LogicPortSpriteType.RibbonInput : LogicPortSpriteType.Input) : 
                    (ribbon ? LogicPortSpriteType.RibbonOutput : LogicPortSpriteType.Output);
            }

            public Vector2 PosMin()
            {
                return Grid.CellToPos2D(cell);
            }

            public Vector2 PosMax()
            {
                return PosMin();
            }
        }
    }
}
