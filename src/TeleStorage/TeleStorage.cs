using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

namespace TeleStorage
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class TeleStorage : KMonoBehaviour, ISaveLoadable
    {
        private static StatusItem filterStatusItem = null;

        [SerializeField]
        public ConduitType Type;

        [Serialize]
        public float Flow = 10000f;

        [Serialize]
        public Tag FilteredTag;

        private Filterable filterable = null;
        private int inputCell = -1;
        private int outputCell = -1;
        private Dictionary<SimHashes, SubstanceChunk> chunks = new Dictionary<SimHashes, SubstanceChunk>();

        public SimHashes FilteredElement { get; private set; } = SimHashes.Void;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            filterable = GetComponent<Filterable>();
            InitializeStatusItems();
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();

            var building = GetComponent<Building>();
            inputCell = building.GetUtilityInputCell();
            outputCell = building.GetUtilityOutputCell();

            Conduit.GetFlowManager(Type).AddConduitUpdater(ConduitUpdate);

            OnFilterChanged(ElementLoader.FindElementByHash(FilteredElement).tag);
            filterable.onFilterChanged += new Action<Tag>(OnFilterChanged);
            GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, filterStatusItem, this);

            TeleStorageData.Instance.storageContainers.Add(this);
        }

        protected override void OnCleanUp()
        {
            Conduit.GetFlowManager(Type).RemoveConduitUpdater(ConduitUpdate);
            TeleStorageData.Instance.storageContainers.Remove(this);
            base.OnCleanUp();
        }

        private bool IsValidFilter
        {
            get
            {
                return (FilteredTag != null) && (FilteredElement != SimHashes.Void)
                    && (FilteredElement != SimHashes.Vacuum);
            }

        }

        private bool IsOperational
        {
            get
            {
                return IsValidFilter && GetComponent<Operational>().IsOperational;
            }
        }

        public void FireRefresh()
        {
            try
            {
                Trigger(-1697596308);
            }
            catch (Exception)
            {

            }
        }

        private void OnFilterChanged(Tag tag)
        {
            FilteredTag = tag;
            Element element = ElementLoader.GetElement(FilteredTag);
            if (element != null)
            {
                FilteredElement = element.id;
            }
            GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, !IsValidFilter, null);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (ElementLoader.GetElement(FilteredTag) == null)
                return;
            filterable.SelectedTag = FilteredTag;
            OnFilterChanged(FilteredTag);
        }

        private void InitializeStatusItems()
        {
            if (filterStatusItem != null)
                return;
            filterStatusItem = new StatusItem("Filter", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.LiquidConduits.ID, true, 129022);
            filterStatusItem.resolveStringCallback = (str, data) =>
            {
                TeleStorage infiniteSource = (TeleStorage)data;
                if (infiniteSource.FilteredElement == SimHashes.Void)
                {
                    str = string.Format(BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, BUILDINGS.PREFABS.GASFILTER.ELEMENT_NOT_SPECIFIED);
                }
                else
                {
                    Element elementByHash = ElementLoader.FindElementByHash(infiniteSource.FilteredElement);
                    str = string.Format(BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, elementByHash.name);
                }
                return str;
            };
            filterStatusItem.conditionalOverlayCallback = new Func<HashedString, object, bool>(ShowInUtilityOverlay);
        }

        private bool ShowInUtilityOverlay(HashedString mode, object data)
        {
            bool flag = false;
            switch (Type)
            {
                case ConduitType.Gas:
                    flag = mode == OverlayModes.GasConduits.ID;
                    break;
                case ConduitType.Liquid:
                    flag = mode == OverlayModes.LiquidConduits.ID;
                    break;
            }
            return flag;
        }

        private void ConduitUpdate(float dt)
        {
            var flowManager = Conduit.GetFlowManager(Type);
            if (flowManager == null)
            {
                return;
            }

            var inputContents = flowManager.GetContents(inputCell);
            if (!TeleStorageData.Instance.storedElementsMap.ContainsKey(inputContents.element))
            {
                TeleStorageData.Instance.storedElementsMap[inputContents.element] = new StoredItem();
            }
            StoredItem inputStored = TeleStorageData.Instance.storedElementsMap[inputContents.element];
            if (inputContents.mass > 0.0f && !float.IsNaN(inputStored.temperature) && !float.IsNaN(inputContents.temperature))
            {
                inputStored.temperature = GameUtil.GetFinalTemperature(inputStored.temperature, inputStored.mass, inputContents.temperature, inputContents.mass);
                inputStored.mass += inputContents.mass;
                SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(inputContents.diseaseIdx, inputContents.diseaseCount, inputStored.diseaseIdx, inputStored.diseaseCount);
                inputStored.diseaseIdx = diseaseInfo.idx;
                inputStored.diseaseCount = diseaseInfo.count;
                TeleStorageData.Instance.FireRefresh();
                flowManager.RemoveElement(inputCell, inputContents.mass);
            }

            if (!IsOperational || !TeleStorageData.Instance.storedElementsMap.ContainsKey(FilteredElement))
            {
                return;
            }
            StoredItem outputStored = TeleStorageData.Instance.storedElementsMap[FilteredElement];
            var possibleOutput = Math.Min(outputStored.mass, Flow / TeleStorageFlowControl.GramsPerKilogram);
            if (possibleOutput > 0.0f)
            {
                var delta = flowManager.AddElement(outputCell, FilteredElement, possibleOutput, outputStored.temperature, 0, 0);
                outputStored.mass -= delta;
                TeleStorageData.Instance.FireRefresh();
            }
        }

    }
}
