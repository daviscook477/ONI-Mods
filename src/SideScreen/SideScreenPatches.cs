using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;
using PeterHan.PLib;
using PeterHan.PLib.UI;

namespace Mineralizer {
	/*public class SideScreenPatches {

		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public class DetailsScreen_OnPrefabInit_Patch {

			public static void Postfix() {
				List<DetailsScreen.SideScreenRef> sideScreens = Traverse.Create(DetailsScreen.Instance).Field("sideScreens").GetValue<List<DetailsScreen.SideScreenRef>>();
				GameObject gameObject = Make();
				ArtifactSideScreen prefab = gameObject.AddOrGet<ArtifactSideScreen>();
				DetailsScreen.SideScreenRef myRef = new DetailsScreen.SideScreenRef {
					name = "bob",
					screenPrefab = prefab,
					offset = new Vector2(0f, 0f),
					screenInstance = null
				};
				sideScreens.Add(myRef);
			}

			/// <summary>
			/// The margin around the scrollable area to avoid stomping on the scrollbar.
			/// </summary>
			private static readonly RectOffset ELEMENT_MARGIN = new RectOffset(2, 2, 2, 2);

			/// <summary>
			/// The indent of the categories, and the items in each category.
			/// </summary>
			internal const int INDENT = 24;

			/// <summary>
			/// The size of checkboxes and images in this control.
			/// </summary>
			internal static readonly Vector2 PANEL_SIZE = new Vector2(260.0f, 320.0f);

			/// <summary>
			/// The margin between the scroll pane and the window.
			/// </summary>
			private static readonly RectOffset OUTER_MARGIN = new RectOffset(6, 10, 6, 14);

			/// <summary>
			/// The size of checkboxes and images in this control.
			/// </summary>
			internal static readonly Vector2 ROW_SIZE = new Vector2(24.0f, 24.0f);

			/// <summary>
			/// The spacing between each row.
			/// </summary>
			internal const int ROW_SPACING = 2;

			public static GameObject Make() {
				GameObject RootPanel = new PPanel("Border") {
					// 1px dark border for contrast
					Margin = new RectOffset(1, 1, 1, 1),
					Direction = PanelDirection.Vertical,
					Alignment = TextAnchor.MiddleCenter,
					Spacing = 1
				}.AddChild(new PLabel("Title") {
					// Title bar
					TextAlignment = TextAnchor.MiddleCenter,
					Text = "STRINGS.UI.BUILDINGS.MINERALIZER.TITLE",
					FlexSize = new Vector2(1.0f, 0.0f),
					DynamicSize = true,
					Margin = new RectOffset(1, 1, 1, 1)
				}.SetKleiPinkColor()).AddChild(new PPanel("TypeSelectControl") {
					// White background for scroll bar
					Direction = PanelDirection.Vertical,
					Margin = OUTER_MARGIN,
					Alignment = TextAnchor.MiddleCenter,
					Spacing = 0,
					BackColor = PUITuning.Colors.BackgroundLight,
					FlexSize = Vector2.one
				}.AddChild(new PScrollPane("Scroll") {
					// Scroll to select elements
					Child = new PPanel("SelectType") {
						Direction = PanelDirection.Vertical,
						Margin = ELEMENT_MARGIN,
						FlexSize = new Vector2(1.0f, 0.0f),
						Alignment = TextAnchor.UpperLeft
					},
					ScrollHorizontal = false,
					ScrollVertical = true,
					AlwaysShowVertical = true,
					TrackSize = 8.0f,
					FlexSize = Vector2.one,
					BackColor = PUITuning.Colors.BackgroundLight,
				}).AddChild(new PLabel("Will this please appear") {

				})).SetKleiBlueColor().BuildWithFixedSize(PANEL_SIZE);
				return RootPanel;
			}


			private void OnCheck(GameObject source, int state) {
				switch (state) {
					case PCheckBox.STATE_UNCHECKED:
						// Clicked when unchecked, check all
						//CheckAll();
						break;
					default:
						// Clicked when checked or partial, clear all
						//ClearAll();
						break;
				}
			}

		}
	}

	/// <summary>
	/// Handles localization by registering for translation.
	/// </summary>
	[HarmonyPatch(typeof(Db), "Initialize")]
	public static class Db_Initialize_Patch {
		public static void Prefix() {
			Strings.Add("STRINGS.UI.BUILDINGS.MINERALIZER.TITLE", "Side Screen for Mineralizer");

		}

	}*/
	public class SideScreenPatches {

		public static void OnLoad() {
			PeterHan.PLib.PUtil.InitLibrary();
		}

		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public class DetailsScreen_OnPrefabInit_Patch {

			public static void Postfix() {
				List<DetailsScreen.SideScreenRef> sideScreens = Traverse.Create(DetailsScreen.Instance).Field("sideScreens").GetValue<List<DetailsScreen.SideScreenRef>>();
				GameObject sideScreenContentBody = Traverse.Create(DetailsScreen.Instance).Field("sideScreenContentBody").GetValue<GameObject>();
				GridFilterableControl gridSelectControl = new GridFilterableControl();
				GridFilterableSideScreen screen = gridSelectControl.RootPanel.AddComponent<GridFilterableSideScreen>();
				screen.gameObject.transform.parent = sideScreenContentBody.transform;
				PUIUtils.DebugObjectTree(sideScreenContentBody);
				DetailsScreen.SideScreenRef myRef = new DetailsScreen.SideScreenRef {
					name = "bob",
					screenPrefab = screen,
					offset = new Vector2(0f, 0f),
					screenInstance = screen
				};
				sideScreens.Add(myRef);
				Console.WriteLine("Postfix patch was called and added in the side screen");
			}
		}

		[HarmonyPatch(typeof(DetailsScreen), "Refresh")]
		public class DetailsScreen_Refresh_Patch {
			public static void Postfix() {
				SideScreenContent scn = Traverse.Create(DetailsScreen.Instance).Field("currentSideScreen").GetValue<SideScreenContent>();
				if (scn != null) {
					PUIUtils.DebugObjectTree(scn.gameObject);
				}
			}
		}
	}

}
