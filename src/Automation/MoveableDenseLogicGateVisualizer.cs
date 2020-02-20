using System.Collections.Generic;
using UnityEngine;

namespace Automation
{
    public class MoveableDenseLogicGateVisualizer : DenseLogicGateBase
    {
        private class StaticDelegateWrappers
        {
            public static void OnRotatedWrapper(MoveableDenseLogicGateVisualizer component, object data)
            {
                component.OnRotated(data);
            }
        }

        private static readonly EventSystem.IntraObjectHandler<MoveableDenseLogicGateVisualizer> OnRotatedDelegate = 
            new EventSystem.IntraObjectHandler<MoveableDenseLogicGateVisualizer>(StaticDelegateWrappers.OnRotatedWrapper);
        protected List<GameObject> visChildren = new List<GameObject>();
        private int cell;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            cell = -1;
            OverlayScreen.Instance.OnOverlayChanged += new System.Action<HashedString>(OnOverlayChanged);
            OnOverlayChanged(OverlayScreen.Instance.mode);
            Subscribe(-1643076535, OnRotatedDelegate);
        }

        protected override void OnCleanUp()
        {
            OverlayScreen.Instance.OnOverlayChanged -= new System.Action<HashedString>(OnOverlayChanged);
            Unregister();
            base.OnCleanUp();
        }

        private void OnOverlayChanged(HashedString mode)
        {
            if (mode == OverlayModes.Logic.ID)
                Register();
            else
                Unregister();
        }

        private void OnRotated(object data)
        {
            Unregister();
            OnOverlayChanged(OverlayScreen.Instance.mode);
        }

        private void Update()
        {
            if (visChildren.Count <= 0)
                return;
            int cell = Grid.PosToCell(transform.GetPosition());
            if (cell == this.cell)
                return;
            this.cell = cell;
            Unregister();
            Register();
        }

        private GameObject CreateUIElem(int cell, bool is_input)
        {
            GameObject gameObject = Util.KInstantiate(is_input ? uiSrcData.ribbonInputPrefab : uiSrcData.ribbonOutputPrefab, 
                Grid.CellToPosCCC(cell, Grid.SceneLayer.Front), Quaternion.identity, GameScreenManager.Instance.worldSpaceCanvas, null, true, 0);
            return gameObject;
        }

        private void Register()
        {
            if (visChildren.Count > 0)
                return;
            enabled = true;
            visChildren.Add(CreateUIElem(OutputCellOne, false));
            visChildren.Add(CreateUIElem(InputCellOne, true));
            if (RequiresTwoInputs)
                visChildren.Add(CreateUIElem(InputCellTwo, true));
        }

        private void Unregister()
        {
            if (visChildren.Count <= 0)
                return;
            enabled = false;
            cell = -1;
            foreach (GameObject visChild in visChildren)
                Util.KDestroyGameObject(visChild);
            visChildren.Clear();
        }
    }
}
