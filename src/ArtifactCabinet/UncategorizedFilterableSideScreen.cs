using System;
using PeterHan.PLib;
using PeterHan.PLib.UI;
using UnityEngine;

namespace ArtifactCabinet
{
	public class UncategorizedFilterableSideScreen : SideScreenContent {
		public GameObject target;
		public Storage storage;
		public UncategorizedFilterableControl control;

		private UncategorizedFilterable targetFilterable;

		public UncategorizedFilterableSideScreen() {
			activateOnSpawn = true;
			ConsumeMouseScroll = true;
			titleKey = "STRINGS.UI.UISIDESCREENS.UNCATEGORIZED_FILTERABLE_SIDE_SCREEN.TITLE";
		}

		public bool IsStorage {
			get {
				return storage != null;
			}
		}

		private void InitSideScreen()
		{
			control = new UncategorizedFilterableControl();
			ContentContainer = control.RootPanel.AddTo(gameObject, 0);
			gameObject.SetMinUISize(UncategorizedFilterableControl.PANEL_SIZE);
		}

		protected override void OnPrefabInit() {
			base.OnPrefabInit();
			if (control == null)
				InitSideScreen();
		}

		public void Initialize(UncategorizedFilterable target) {
			if (target == null) {
				PUtil.LogError("UNCATEGORIZED SELECT CONTROL: provided was null.");
			}

			PUtil.LogDebug("UNCATEGORIZED SELECT CONTROL: Initialized");
			targetFilterable = target;
			gameObject.SetActive(true);
		}

		public override bool IsValidForTarget(GameObject target) {
			return target.GetComponent<UncategorizedFilterable>() != null;
		}

		public override void SetTarget(GameObject target) {
			this.target = target;
			if (target == null) {
				PUtil.LogError("The target object provided was null");
				return;
			}

			targetFilterable = target.GetComponent<UncategorizedFilterable>();
			if (targetFilterable == null)
			{
				PUtil.LogError("The target provided does not have a Uncategorized Filterable component");
				return;
			}

			if (!targetFilterable.showUserMenu || (IsStorage && !storage.showInUI))
				DetailsScreen.Instance.DeactivateSideContent();
			else {
				storage = targetFilterable.GetComponent<Storage>();
				if (control == null)
					InitSideScreen();
				control.Update(target);
			}
		}
	}
}
