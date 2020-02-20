using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSerialization;
using UnityEngine;
using Klei.AI;

namespace ArtifactCabinet
{
    public class ArtifactCabinet : KMonoBehaviour, IUserControlledCapacity, IEffectDescriptor, IGameObjectEffectDescriptor
    {
        public static List<Tag> ArtifactsFilterTagList = new List<Tag>()
        {
            "artifact_sandstone".ToTag(),
            "artifact_sink".ToTag(),
            "artifact_rubikscube".ToTag(),
            "artifact_officemug".ToTag(),
            "artifact_obelisk".ToTag(),
            "artifact_okayxray".ToTag(),
            "artifact_blender".ToTag(),
            "artifact_moldavite".ToTag(),
            "artifact_vhs".ToTag(),
            "artifact_saxophone".ToTag(),
            "artifact_modernart".ToTag(),
            "artifact_ameliaswatch".ToTag(),
            "artifact_teapot".ToTag(),
            "artifact_brickphone".ToTag(),
            "artifact_robotarm".ToTag(),
            "artifact_shieldgenerator".ToTag(),
            "artifact_bioluminescentrock".ToTag(),
            "artifact_stethoscope".ToTag(),
            "artifact_eggrock".ToTag(),
            "artifact_hatchfossil".ToTag(),
            "artifact_rocktornado".ToTag(),
            "artifact_pacupercolator".ToTag(),
            "artifact_magmalamp".ToTag(),
            "artifact_dnamodel".ToTag(),
            "artifact_rainboweggrock".ToTag(),
            "artifact_plasmalamp".ToTag(),
            "artifact_solarsystem".ToTag(),
            "artifact_moonmoonmoon".ToTag()
        };

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

            public static void UpdateLogicCircuitWrapper(ArtifactCabinet component, object data)
            {
                component.UpdateLogicCircuit(data);
            }

            public static void OnStorageChangedWrapper(ArtifactCabinet component, object data)
            {
                component.OnStorageChanged(data);
            }
        }

        private static readonly EventSystem.IntraObjectHandler<ArtifactCabinet> OnOperationalChangedDelegate
            = new EventSystem.IntraObjectHandler<ArtifactCabinet>(StaticDelegateWrappers.OnOperationalChangedWrapper);
        private static readonly EventSystem.IntraObjectHandler<ArtifactCabinet> OnCopySettingsDelegate
            = new EventSystem.IntraObjectHandler<ArtifactCabinet>(StaticDelegateWrappers.OnCopySettingsWrapper);
        private static readonly EventSystem.IntraObjectHandler<ArtifactCabinet> UpdateLogicCircuitDelegate 
            = new EventSystem.IntraObjectHandler<ArtifactCabinet>(StaticDelegateWrappers.UpdateLogicCircuitWrapper);
        private static readonly EventSystem.IntraObjectHandler<ArtifactCabinet> OnStorageChangedDelegate
            = new EventSystem.IntraObjectHandler<ArtifactCabinet>(StaticDelegateWrappers.OnStorageChangedWrapper);

        [Serialize]
        private float userMaxCapacity = float.PositiveInfinity;
        [MyCmpGet]
        private Storage storage;
        [MyCmpGet]
        private Operational operational;
        [MyCmpGet]
        private LogicPorts ports;
        [MyCmpReq]
        private DecorProvider decorProvider;
        protected UncategorizedFilteredStorage filteredStorage;
        private KBatchedAnimController anim;
        private Dictionary<Tag, AttributeModifier> decorModifier = new Dictionary<Tag, AttributeModifier>();

        private const float MINIMUM_DECOR_PER_ITEM = 5f; // minimum 5 decor for each stored item
        private const float STORED_DECOR_MODIFIER = 0.5f; // artifact cabinet halves the decor or the items put into it
        private const int ARTIFACT_RADIUS = 5; // all artifacts have radius forced to 5

        protected override void OnPrefabInit()
        {
            filteredStorage = new UncategorizedFilteredStorage(this, null, null, this, true, 
                Db.Get().ChoreTypes.StorageFetch);
        }

        protected override void OnSpawn()
        {
            operational.SetActive(operational.IsOperational, false);
            GetComponent<KAnimControllerBase>().Play("off", KAnim.PlayMode.Once, 1f, 0.0f);
            anim = GetComponent<KBatchedAnimController>();
            filteredStorage.FilterChanged();
            UpdateLogicCircuit(null);
            OnStorageChanged(null);
            Subscribe(-592767678, OnOperationalChangedDelegate);
            Subscribe(-905833192, OnCopySettingsDelegate);
            Subscribe(-1697596308, UpdateLogicCircuitDelegate);
            Subscribe(-592767678, UpdateLogicCircuitDelegate);
            Subscribe(-1697596308, OnStorageChangedDelegate);
        }

        protected override void OnCleanUp()
        {
            filteredStorage.CleanUp();
        }

        private void OnOperationalChanged(object data)
        {
            operational.SetActive(operational.IsOperational, false);
        }

        public bool IsActive()
        {
            return operational.IsActive;
        }

        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
                return;
            ArtifactCabinet component = gameObject.GetComponent<ArtifactCabinet>();
            if ((UnityEngine.Object)component == (UnityEngine.Object)null)
                return;
            UserMaxCapacity = component.UserMaxCapacity;
        }

        private void OnStorageChanged(object data)
        {
            // inefficient - set the status of every symbol based on its presence
            foreach (Tag tag in ArtifactsFilterTagList)
            {
                anim.SetSymbolVisiblity(tag.ToString(), false);
            }
            foreach (Tag tag in storage.GetAllTagsInStorage())
            {
                anim.SetSymbolVisiblity(tag.ToString(), true);
            }
            // determine appropriate decor amount
            Attributes attributes = this.GetAttributes();
            if (decorModifier.Count > 0)
            {
                foreach (AttributeModifier attr in decorModifier.Values)
                {
                    attributes.Remove(attr);
                }
                decorModifier.Clear();
            }
            // probably need a hashmap from the tag of the artifact to the decor modifier and decor radius modifier for it so I can properly remove
            // and add the components
            foreach (GameObject go in storage.items)
            {
                if (go.GetComponent<DecorProvider>() != null)
                {
                    float decorValue = go.GetComponent<PrimaryElement>().Units * Mathf.Max(Db.Get().BuildingAttributes.Decor.Lookup(go).GetTotalValue() * STORED_DECOR_MODIFIER, MINIMUM_DECOR_PER_ITEM);
                    string description = string.Format(STRINGS.BUILDINGS.PREFABS.ITEMPEDESTAL.DISPLAYED_ITEM_FMT, go.GetComponent<KPrefabID>().PrefabTag.ProperName());
                    Tag prefabTag = go.GetComponent<KPrefabID>().PrefabTag;
                    if (decorModifier.ContainsKey(prefabTag))
                    {
                        decorModifier[prefabTag].SetValue(decorModifier[prefabTag].Value + decorValue);
                    }
                    else
                    {
                        decorModifier[prefabTag] = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, decorValue, description, false, false, true);
                    }
                }
            }
            foreach (AttributeModifier attr in decorModifier.Values)
            {
                attributes.Add(attr);
            }
        }

        public List<Descriptor> GetDescriptors(BuildingDef def)
        {
            return GetDescriptors(def.BuildingComplete);
        }

        public List<Descriptor> GetDescriptors(GameObject go)
        {
            return new List<Descriptor>() { };
        }

        public float UserMaxCapacity
        {
            get
            {
                return Mathf.Min(userMaxCapacity, storage.capacityKg);
            }
            set
            {
                userMaxCapacity = value;
                filteredStorage.FilterChanged();
                UpdateLogicCircuit(null);
            }
        }

        public float AmountStored
        {
            get
            {
                return storage.MassStored();
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
                return storage.capacityKg;
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

        private void UpdateLogicCircuit(object data)
        {
            bool flag = filteredStorage.IsFull();
            bool isOperational = operational.IsOperational;
            bool on = flag && isOperational;
            ports.SendSignal(UncategorizedFilteredStorage.FULL_PORT_ID, !on ? 0 : 1);
            filteredStorage.SetLogicMeter(on);
        }
    }
}
