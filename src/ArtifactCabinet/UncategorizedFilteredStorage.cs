using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ArtifactCabinet
{
    public class UncategorizedFilteredStorage
    {
        public static readonly HashedString FULL_PORT_ID = "FULL";
        public static readonly Color32 FILTER_TINT = Color.white;
        public static readonly Color32 NO_FILTER_TINT = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);
        public Color32 filterTint = FILTER_TINT;
        public Color32 noFilterTint = NO_FILTER_TINT;
        private bool hasMeter = true;
        private KMonoBehaviour root;
        private FetchList2 fetchList;
        private IUserControlledCapacity capacityControl;
        private UncategorizedFilterable filterable;
        private Storage storage;
        private MeterController meter;
        private MeterController logicMeter;
        private Tag[] requiredTags;
        private Tag[] forbiddenTags;
        private bool useLogicMeter;
        private static StatusItem capacityStatusItem;
        private static StatusItem noFilterStatusItem;
        private ChoreType choreType;

        public UncategorizedFilteredStorage(
    KMonoBehaviour root,
    Tag[] required_tags,
    Tag[] forbidden_tags,
    IUserControlledCapacity capacity_control,
    bool use_logic_meter,
    ChoreType fetch_chore_type)
        {
            this.root = root;
            this.requiredTags = required_tags;
            this.forbiddenTags = forbidden_tags;
            this.capacityControl = capacity_control;
            this.useLogicMeter = use_logic_meter;
            this.choreType = fetch_chore_type;
            root.Subscribe(-1697596308, new Action<object>(OnStorageChanged));
            root.Subscribe(-543130682, new Action<object>(OnUserSettingsChanged));
            this.filterable = root.FindOrAdd<UncategorizedFilterable>();
            filterable.OnFilterChanged += new Action<Tag[]>(OnFilterChanged);
            this.storage = root.GetComponent<Storage>();
            storage.Subscribe(644822890, new Action<object>(OnOnlyFetchMarkedItemsSettingChanged));
            if (capacityStatusItem == null)
            {
                capacityStatusItem = new StatusItem("StorageLocker", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
                capacityStatusItem.resolveStringCallback = (str, data) =>
                {
                    UncategorizedFilteredStorage filteredStorage = (UncategorizedFilteredStorage)data;
                    float amountStored = filteredStorage.GetAmountStored();
                    float b = filteredStorage.storage.capacityKg;
                    string newValue1 = Util.FormatWholeNumber(amountStored <= b - filteredStorage.storage.storageFullMargin || amountStored >= b ? Mathf.Floor(amountStored) : b);
                    IUserControlledCapacity component = filteredStorage.root.GetComponent<IUserControlledCapacity>();
                    if (component != null)
                        b = Mathf.Min(component.UserMaxCapacity, b);
                    string newValue2 = Util.FormatWholeNumber(b);
                    str = str.Replace("{Stored}", newValue1);
                    str = str.Replace("{Capacity}", newValue2);
                    str = component == null ? str.Replace("{Units}", (string)GameUtil.GetCurrentMassUnit(false)) : str.Replace("{Units}", (string)component.CapacityUnits);
                    return str;
                };
                noFilterStatusItem = new StatusItem("NoStorageFilterSet", "BUILDING", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
            }
            root.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, capacityStatusItem, this);
        }

        public void SetHasMeter(bool has_meter)
        {
            hasMeter = has_meter;
        }

        private void OnOnlyFetchMarkedItemsSettingChanged(object data)
        {
            OnFilterChanged(filterable.GetTags());
        }

        private void CreateMeter()
        {
            if (!hasMeter)
                return;
            meter = new MeterController(root.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[2]
            {
              "meter_frame",
              "meter_level"
            });
        }

        private void CreateLogicMeter()
        {
            if (!hasMeter)
                return;
            logicMeter = new MeterController(root.GetComponent<KBatchedAnimController>(), "logicmeter_target", "logicmeter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[0]);
        }

        public void CleanUp()
        {
            if (filterable != null)
                filterable.OnFilterChanged -= new Action<Tag[]>(OnFilterChanged);
            if (fetchList == null)
                return;
            fetchList.Cancel("Parent destroyed");
        }

        public void FilterChanged()
        {
            if (hasMeter)
            {
                if (meter == null)
                    CreateMeter();
                if (logicMeter == null && useLogicMeter)
                    CreateLogicMeter();
            }
            OnFilterChanged(filterable.GetTags());
            UpdateMeter();
        }

        private void OnUserSettingsChanged(object data)
        {
            OnFilterChanged(filterable.GetTags());
            UpdateMeter();
        }

        private void OnStorageChanged(object data)
        {
            if (fetchList == null)
                OnFilterChanged(filterable.GetTags());
            UpdateMeter();
        }

        private void UpdateMeter()
        {
            float percent_full = Mathf.Clamp01(GetAmountStored() / GetMaxCapacityMinusStorageMargin());
            if (meter == null)
                return;
            meter.SetPositionPercent(percent_full);
        }

        public bool IsFull()
        {
            float percent_full = Mathf.Clamp01(GetAmountStored() / GetMaxCapacityMinusStorageMargin());
            if (meter != null)
                meter.SetPositionPercent(percent_full);
            return percent_full >= 1.0;
        }

        private void OnFetchComplete()
        {
            OnFilterChanged(filterable.GetTags());
        }

        private float GetMaxCapacity()
        {
            float a = storage.capacityKg;
            if (capacityControl != null)
                a = Mathf.Min(a, capacityControl.UserMaxCapacity);
            return a;
        }

        private float GetMaxCapacityMinusStorageMargin()
        {
            return GetMaxCapacity() - storage.storageFullMargin;
        }

        private float GetAmountStored()
        {
            float num = storage.MassStored();
            if (capacityControl != null)
                num = capacityControl.AmountStored;
            return num;
        }

        private void OnFilterChanged(Tag[] tags)
        {
            KBatchedAnimController component = root.GetComponent<KBatchedAnimController>();
            bool flag = tags != null && tags.Length != 0;
            component.TintColour = !flag ? noFilterTint : filterTint;
            if (fetchList != null)
            {
                fetchList.Cancel(string.Empty);
                fetchList = null;
            }
            float minusStorageMargin = GetMaxCapacityMinusStorageMargin();
            float amountStored = GetAmountStored();
            if (Mathf.Max(0.0f, minusStorageMargin - amountStored) > 0.0 && flag)
            {
                float amount = Mathf.Max(0.0f, GetMaxCapacity() - amountStored);
                fetchList = new FetchList2(storage, choreType);
                fetchList.ShowStatusItem = false;
                fetchList.Add(tags, requiredTags, forbiddenTags, amount, FetchOrder2.OperationalRequirement.Functional);
                fetchList.Submit(new System.Action(OnFetchComplete), false);
            }
            root.GetComponent<KSelectable>().ToggleStatusItem(noFilterStatusItem, !flag, this);
        }

        public void SetLogicMeter(bool on)
        {
            if (logicMeter == null)
                return;
            logicMeter.SetPositionPercent(!on ? 0.0f : 1f);
        }
    }
}
