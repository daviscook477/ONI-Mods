using PeterHan.PLib;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mineralizer {
	public class GridFilterableSideScreen : SideScreenContent {
		public GameObject target;
		public Storage storage;

		private GridFilterable targetFilterable;

		public GridFilterableSideScreen() {
			activateOnSpawn = true;
			ConsumeMouseScroll = true;
			titleKey = "Test this is a title key";
		}

		public bool IsStorage {
			get {
				return (UnityEngine.Object)this.storage != (UnityEngine.Object)null;
			}
		}

		protected override void OnPrefabInit() {
			base.OnPrefabInit();
		}

		public void Initialize(GridFilterable target) {
			if (target == null) {
				Debug.LogError((object)"GRID SELECT CONTROL: provided was null.");
			}

			Debug.Log("GRID SELECT CONTROL: Initialized");
			this.targetFilterable = target;
			this.gameObject.SetActive(true);
		}

		private void OnRefreshData(object obj) {
			this.SetTarget(this.targetFilterable.gameObject);
			Debug.Log("GRID SELECT CONTROL: Onrefreshdata");
		}

		public override bool IsValidForTarget(GameObject target) {
			return target.GetComponent<GridFilterable>() != null;
		}

		public override void SetTarget(GameObject target) {
			this.target = target;
			if ((UnityEngine.Object)target == (UnityEngine.Object)null) {
				Debug.LogError((object)"The target object provided was null");
			}
			else {
				this.targetFilterable = target.GetComponent<GridFilterable>();
				if ((UnityEngine.Object)this.targetFilterable == (UnityEngine.Object)null)
					Debug.LogError((object)"The target provided does not have a Grid Filterable component");
				else if (!this.targetFilterable.showUserMenu)
					DetailsScreen.Instance.DeactivateSideContent();
				else if (this.IsStorage && !this.storage.showInUI) {
					DetailsScreen.Instance.DeactivateSideContent();
				}
				else {
					this.storage = this.targetFilterable.GetComponent<Storage>();
					GridFilterableControl.Instance.Update(this);
				}
			}
		}
	}
}
