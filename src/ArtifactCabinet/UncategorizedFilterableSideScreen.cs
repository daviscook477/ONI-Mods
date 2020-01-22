using System;
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
				Debug.LogError("UNCATEGORIZED SELECT CONTROL: provided was null.");
			}

			Debug.Log("UNCATEGORIZED SELECT CONTROL: Initialized");
			targetFilterable = target;
			gameObject.SetActive(true);
		}

		public override bool IsValidForTarget(GameObject target) {
			return target.GetComponent<UncategorizedFilterable>() != null;
		}

		public override void SetTarget(GameObject target) {
			this.target = target;
			if (target == null) {
				Debug.LogError("The target object provided was null");
			}
			else {
				targetFilterable = target.GetComponent<UncategorizedFilterable>();
				if (targetFilterable == null)
					Debug.LogError("The target provided does not have a Uncategorized Filterable component");
				else if (!targetFilterable.showUserMenu)
					DetailsScreen.Instance.DeactivateSideContent();
				else if (IsStorage && !storage.showInUI) {
					DetailsScreen.Instance.DeactivateSideContent();
				}
				else {
					storage = targetFilterable.GetComponent<Storage>();
					if (control == null)
						InitSideScreen();
					control.Update(target);
				}
			}
		}
	}
}
