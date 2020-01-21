using PeterHan.PLib;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
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
			titleKey = "Test this is a title key";
		}

		public bool IsStorage {
			get {
				return (UnityEngine.Object)this.storage != (UnityEngine.Object)null;
			}
		}

		protected override void OnPrefabInit() {
			base.OnPrefabInit();
			Console.WriteLine("creating prefab for filterable side screen");
			if (control == null)
			{
				Console.WriteLine("actually making it");
				control = new UncategorizedFilterableControl();
				ContentContainer = control.RootPanel.AddTo(gameObject, 0);
			}
			Console.WriteLine("sucessfully set control to not null!");
		}

		public void Initialize(UncategorizedFilterable target) {
			if (target == null) {
				Debug.LogError((object)"UNCATEGORIZED SELECT CONTROL: provided was null.");
			}

			Debug.Log("UNCATEGORIZED SELECT CONTROL: Initialized");
			this.targetFilterable = target;
			this.gameObject.SetActive(true);
		}

		private void OnRefreshData(object obj) {
			this.SetTarget(this.targetFilterable.gameObject);
			Debug.Log("UNCATEGORIZED SELECT CONTROL: Onrefreshdata");
		}

		public override bool IsValidForTarget(GameObject target) {
			return target.GetComponent<UncategorizedFilterable>() != null;
		}

		public override void SetTarget(GameObject target) {
			this.target = target;
			if ((UnityEngine.Object)target == (UnityEngine.Object)null) {
				Debug.LogError((object)"The target object provided was null");
			}
			else {
				this.targetFilterable = target.GetComponent<UncategorizedFilterable>();
				if ((UnityEngine.Object)this.targetFilterable == (UnityEngine.Object)null)
					Debug.LogError((object)"The target provided does not have a Uncategorized Filterable component");
				else if (!this.targetFilterable.showUserMenu)
					DetailsScreen.Instance.DeactivateSideContent();
				else if (this.IsStorage && !this.storage.showInUI) {
					DetailsScreen.Instance.DeactivateSideContent();
				}
				else {
					this.storage = this.targetFilterable.GetComponent<Storage>();
					Console.WriteLine("target setting");
					if (control == null)
					{
						Console.WriteLine("actually making it");
						control = new UncategorizedFilterableControl();
						ContentContainer = control.RootPanel.AddTo(gameObject, 0);
					}
					Console.WriteLine("done target setting");
					this.control.Update(target);
				}
			}
		}
	}
}
