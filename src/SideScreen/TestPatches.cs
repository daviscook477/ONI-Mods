using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;
using PeterHan.PLib;
using PeterHan.PLib.UI;

namespace SideScreen {
	public class TestPatches {
		public static void OnLoad() {
			PeterHan.PLib.PUtil.InitLibrary();
		}

		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public class DetailsScreen_OnPrefabInit_Patch {

			public static void Postfix() {
				List<DetailsScreen.SideScreenRef> sideScreens = Traverse.Create(DetailsScreen.Instance).Field("sideScreens").GetValue<List<DetailsScreen.SideScreenRef>>();
				GameObject sideScreenContentBody = Traverse.Create(DetailsScreen.Instance).Field("sideScreenContentBody").GetValue<GameObject>();
				TestControl controller = new TestControl();
				TestSideScreen screen = controller.RootPanel.AddComponent<TestSideScreen>();
				screen.gameObject.transform.parent = sideScreenContentBody.transform;
				DetailsScreen.SideScreenRef myRef = new DetailsScreen.SideScreenRef {
					name = "TestSideScreen",
					screenPrefab = screen,
					offset = new Vector2(0f, 0f),
					screenInstance = screen
				};
				sideScreens.Add(myRef);
			}
		}
	}
}
