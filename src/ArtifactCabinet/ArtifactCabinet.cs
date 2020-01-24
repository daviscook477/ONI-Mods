using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSerialization;
using System.Collections.Generic;
using UnityEngine;

namespace ArtifactCabinet
{
    public class ArtifactCabinet : KMonoBehaviour, IUserControlledCapacity, IEffectDescriptor, IGameObjectEffectDescriptor
    {
        private class StaticDelegateWrappers
        {
            public static void OnOperationalChangedWrapper(ArtifactCabinet component, object data)
            {
                component.OnOperationalChanged(data);
            }

            public static void OnCopySettingsWrapper(ArtifactCabinet component, object data)
            {
                component.OnCopySettings(data);
            }

            public static void UpdateLogicCircuitCBWrapper(ArtifactCabinet component, object data)
            {
                component.UpdateLogicCircuitCB(data);
            }
        }

        private static readonly EventSystem.IntraObjectHandler<ArtifactCabinet> OnOperationalChangedDelegate
            = new EventSystem.IntraObjectHandler<ArtifactCabinet>(StaticDelegateWrappers.OnOperationalChangedWrapper);
        private static readonly EventSystem.IntraObjectHandler<ArtifactCabinet> OnCopySettingsDelegate
            = new EventSystem.IntraObjectHandler<ArtifactCabinet>(StaticDelegateWrappers.OnCopySettingsWrapper);
        private static readonly EventSystem.IntraObjectHandler<ArtifactCabinet> UpdateLogicCircuitCBDelegate 
            = new EventSystem.IntraObjectHandler<ArtifactCabinet>(StaticDelegateWrappers.UpdateLogicCircuitCBWrapper);

        [Serialize]
        private float userMaxCapacity = float.PositiveInfinity;
        [MyCmpGet]
        private Storage storage;
        [MyCmpGet]
        private Operational operational;
        [MyCmpGet]
        private LogicPorts ports;
        protected UncategorizedFilteredStorage filteredStorage;

        protected override void OnPrefabInit()
        {
            this.filteredStorage = new UncategorizedFilteredStorage((KMonoBehaviour)this, null, null, (IUserControlledCapacity)this, true, 
                Db.Get().ChoreTypes.StorageFetch);
        }

        protected override void OnSpawn()
        {
            this.operational.SetActive(this.operational.IsOperational, false);
            this.GetComponent<KAnimControllerBase>().Play((HashedString)"off", KAnim.PlayMode.Once, 1f, 0.0f);
            this.filteredStorage.FilterChanged();
            this.UpdateLogicCircuit();
            this.Subscribe<ArtifactCabinet>(-592767678, ArtifactCabinet.OnOperationalChangedDelegate);
            this.Subscribe<ArtifactCabinet>(-905833192, ArtifactCabinet.OnCopySettingsDelegate);
            this.Subscribe<ArtifactCabinet>(-1697596308, ArtifactCabinet.UpdateLogicCircuitCBDelegate);
            this.Subscribe<ArtifactCabinet>(-592767678, ArtifactCabinet.UpdateLogicCircuitCBDelegate);
        }

        protected override void OnCleanUp()
        {
            this.filteredStorage.CleanUp();
        }

        private void OnOperationalChanged(object data)
        {
            this.operational.SetActive(this.operational.IsOperational, false);
        }

        public bool IsActive()
        {
            return this.operational.IsActive;
        }

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
                return;
            ArtifactCabinet component = gameObject.GetComponent<ArtifactCabinet>();
            if ((UnityEngine.Object)component == (UnityEngine.Object)null)
                return;
            this.UserMaxCapacity = component.UserMaxCapacity;
        }

        public List<Descriptor> GetDescriptors(BuildingDef def)
        {
            return this.GetDescriptors(def.BuildingComplete);
        }

        public List<Descriptor> GetDescriptors(GameObject go)
        {
            return new List<Descriptor>() { };
        }

        public float UserMaxCapacity
        {
            get
            {
                return Mathf.Min(this.userMaxCapacity, this.storage.capacityKg);
            }
            set
            {
                this.userMaxCapacity = value;
                this.filteredStorage.FilterChanged();
                this.UpdateLogicCircuit();
            }
        }

        public float AmountStored
        {
            get
            {
                return this.storage.MassStored();
            }
        }

        public float MinCapacity
        {
            get
            {
                return 0.0f;
            }
        }

        public float MaxCapacity
        {
            get
            {
                return this.storage.capacityKg;
            }
        }

        public bool WholeValues
        {
            get
            {
                return false;
            }
        }

        public LocString CapacityUnits
        {
            get
            {
                return GameUtil.GetCurrentMassUnit(false);
            }
        }

        private void UpdateLogicCircuitCB(object data)
        {
            this.UpdateLogicCircuit();
        }

        private void UpdateLogicCircuit()
        {
            bool flag = this.filteredStorage.IsFull();
            bool isOperational = this.operational.IsOperational;
            bool on = flag && isOperational;
            this.ports.SendSignal(UncategorizedFilteredStorage.FULL_PORT_ID, !on ? 0 : 1);
            this.filteredStorage.SetLogicMeter(on);
        }
    }
}
