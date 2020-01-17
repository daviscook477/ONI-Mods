using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace IceFanChoice
{
    public class IceFanChoice : KMonoBehaviour
    {
        private const float ICE_CAPACITY = 50f;

        private ManualDeliveryKG manualDeliveryKGIce, manualDeliveryKGDirtyIce, manualDeliveryKGBrineIce;

        private void OnFilterChanged(Tag[] tags)
        {
            Console.WriteLine("Filter was changed");
            bool ice = true;
            bool dirtyIce = true;
            bool brineIce = true;
            foreach (var tag in tags)
            {
                Console.WriteLine($"It contains tag: {tag.ToString()}");
                if (tag.Equals(SimHashes.Ice.CreateTag()))
                {
                    ice = false;
                }
                if (tag.Equals(SimHashes.DirtyIce.CreateTag()))
                {
                    dirtyIce = false;
                }
                if (tag.Equals(SimHashes.BrineIce.CreateTag()))
                {
                    brineIce = false;
                }
            }
            manualDeliveryKGIce.Pause(ice, "Filter changed");
            manualDeliveryKGDirtyIce.Pause(dirtyIce, "Filter changed");
            manualDeliveryKGBrineIce.Pause(brineIce, "Filter changed");
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();

            IceCooledFan fan = GetComponent<IceCooledFan>();

            manualDeliveryKGIce = GetComponent<ManualDeliveryKG>();
            manualDeliveryKGIce.requestedItemTag = SimHashes.Ice.CreateTag();
            manualDeliveryKGIce.Pause(true, "Filter empty");

            manualDeliveryKGDirtyIce = gameObject.AddComponent<ManualDeliveryKG>();
            manualDeliveryKGDirtyIce.SetStorage(fan.iceStorage);
            manualDeliveryKGDirtyIce.Pause(true, "Filter empty");
            manualDeliveryKGDirtyIce.requestedItemTag = SimHashes.DirtyIce.CreateTag();
            manualDeliveryKGDirtyIce.capacity = ICE_CAPACITY;
            manualDeliveryKGDirtyIce.refillMass = ICE_CAPACITY * 0.2f;
            manualDeliveryKGDirtyIce.minimumMass = 10f;
            manualDeliveryKGDirtyIce.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;

            manualDeliveryKGBrineIce = gameObject.AddComponent<ManualDeliveryKG>();
            manualDeliveryKGBrineIce.SetStorage(fan.iceStorage);
            manualDeliveryKGBrineIce.Pause(true, "Filter empty");
            manualDeliveryKGBrineIce.requestedItemTag = SimHashes.BrineIce.CreateTag();
            manualDeliveryKGBrineIce.capacity = ICE_CAPACITY;
            manualDeliveryKGBrineIce.refillMass = ICE_CAPACITY * 0.2f;
            manualDeliveryKGBrineIce.minimumMass = 10f;
            manualDeliveryKGBrineIce.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;

            GetComponent<TreeFilterable>().OnFilterChanged += OnFilterChanged;
        }

        protected override void OnCleanUp()
        {
            base.OnCleanUp();
            GetComponent<TreeFilterable>().OnFilterChanged -= OnFilterChanged;
        }

    }
}
