using System;
using Harmony;
using UnityEngine;
using System.Runtime.Serialization;
using STRINGS;
using KSerialization;

namespace InfiniteSourceSink
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class InfiniteSource : KMonoBehaviour, ISaveLoadable, ISingleSliderControl
	{
		private static StatusItem filterStatusItem = (StatusItem)null;

		public const int MinAllowedTemperature = 1;
		public const int MaxAllowedTemperature = 7500;

		[SerializeField]
		public ConduitType Type;

		[Serialize]
		public float Flow = 10000f;

		[Serialize]
		public float Temp = 300f;

		[Serialize]
		public Tag FilteredTag;

		private Filterable filterable = null;
		private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;
		private int outputCell = -1;

		public SimHashes FilteredElement { get; private set; } = SimHashes.Void;

		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			filterable = GetComponent<Filterable>();
			accumulator = Game.Instance.accumulators.Add("Source", (KMonoBehaviour)this);
			this.InitializeStatusItems();
		}

		protected override void OnSpawn()
		{
			base.OnSpawn();

			var building = GetComponent<Building>();
			outputCell = building.GetUtilityOutputCell();

			Conduit.GetFlowManager(Type).AddConduitUpdater(ConduitUpdate);

			OnFilterChanged(ElementLoader.FindElementByHash(FilteredElement).tag);
			this.filterable.onFilterChanged += new System.Action<Tag>(this.OnFilterChanged);
			this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, InfiniteSource.filterStatusItem, (object)this);
		}

		protected override void OnCleanUp()
		{
			Conduit.GetFlowManager(Type).RemoveConduitUpdater(ConduitUpdate);
			Game.Instance.accumulators.Remove(accumulator);
			base.OnCleanUp();
		}

		private Boolean IsOperational
		{
			get
			{
				return (FilteredTag != null) && (FilteredElement != SimHashes.Void)
					&& (FilteredElement != SimHashes.Vacuum);
			}
		}

		public string SliderTitleKey
		{
			get
			{
				switch (Type)
				{
					case ConduitType.Gas:
						return "STRINGS.UI.UISIDESCREENS.GASSOURCE.TITLE";
					case ConduitType.Liquid:
						return "STRINGS.UI.UISIDESCREENS.LIQUIDSOURCE.TITLE";
					default:
						throw new Exception("Invalid ConduitType provided to InfiniteSource: " + Type.ToString());
				}
			}
		}

		public string SliderUnits
		{
			get
			{
				return UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
			}
		}

		private Boolean inUpdate = false;

		private void OnFilterChanged(Tag tag)
		{
			this.FilteredTag = tag;
			Element element = ElementLoader.GetElement(this.FilteredTag);
			if (element != null)
			{
				this.FilteredElement = element.id;
			}
			this.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, !IsOperational, (object)null);
			Temp = Math.Max(Temp, element.lowTemp);
			Temp = Math.Min(Temp, element.highTemp);
			Temp = Math.Max(Temp, MinAllowedTemperature);
			Temp = Math.Min(Temp, MaxAllowedTemperature);
			SetSliderValue(Temp, -1);
			if (DetailsScreen.Instance != null && !inUpdate)
			{
				inUpdate = true;
				try
				{
					DetailsScreen.Instance.Refresh(gameObject);
				}
				catch (Exception) { }
				inUpdate = false;
			}
		}

		[OnDeserialized]
		private void OnDeserialized()
		{
			if (ElementLoader.GetElement(FilteredTag) == null)
				return;
			this.filterable.SelectedTag = FilteredTag;
			OnFilterChanged(FilteredTag);
		}

		private void InitializeStatusItems()
		{
			if (InfiniteSource.filterStatusItem != null)
				return;
			InfiniteSource.filterStatusItem = new StatusItem("Filter", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.LiquidConduits.ID, true, 129022);
			InfiniteSource.filterStatusItem.resolveStringCallback = (Func<string, object, string>)((str, data) =>
			{
				InfiniteSource infiniteSource = (InfiniteSource)data;
				if (infiniteSource.FilteredElement == SimHashes.Void)
				{
					str = string.Format((string)BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, (object)BUILDINGS.PREFABS.GASFILTER.ELEMENT_NOT_SPECIFIED);
				}
				else
				{
					Element elementByHash = ElementLoader.FindElementByHash(infiniteSource.FilteredElement);
					str = string.Format((string)BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, (object)elementByHash.name);
				}
				return str;
			});
			InfiniteSource.filterStatusItem.conditionalOverlayCallback = new Func<HashedString, object, bool>(this.ShowInUtilityOverlay);
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
			if (flowManager == null || !flowManager.HasConduit(outputCell) || !IsOperational)
			{
				return;
			}

			var delta = flowManager.AddElement(outputCell, FilteredElement, Flow / InfiniteSourceFlowControl.GramsPerKilogram, Temp, 0, 0);
			Game.Instance.accumulators.Accumulate(accumulator, delta);
		}

		public int SliderDecimalPlaces(int index)
		{
			return 1;
		}

		public float GetSliderMin(int index)
		{
			Element element = ElementLoader.GetElement(FilteredTag);
			if (element == null)
			{
				return 0.0f;
			}
			return Math.Max(element.lowTemp, MinAllowedTemperature);
		}

		public float GetSliderMax(int index)
		{
			Element element = ElementLoader.GetElement(FilteredTag);
			if (element == null)
			{
				return 100.0f;
			}
			return Math.Min(element.highTemp, MaxAllowedTemperature);
		}

		public float GetSliderValue(int index)
		{
			return Temp;
		}

		public void SetSliderValue(float percent, int index)
		{
			Temp = percent;
		}

		public string GetSliderTooltipKey(int index)
		{
			switch (Type)
			{
				case ConduitType.Gas:
					return "STRINGS.UI.UISIDESCREENS.GASSOURCE.TOOLTIP";
				case ConduitType.Liquid:
					return "STRINGS.UI.UISIDESCREENS.LIQUIDSOURCE.TOOLTIP";
				default:
					throw new Exception("Invalid ConduitType provided to InfiniteSource: " + Type.ToString());
			}
		}
	}
}
