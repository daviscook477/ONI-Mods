using PeterHan.PLib;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SideScreen {
	public class TestControl {
		/// <summary>
		/// The margin around the scrollable area to avoid stomping on the scrollbar.
		/// </summary>
		private static readonly RectOffset ELEMENT_MARGIN = new RectOffset(2, 2, 2, 2);

		/// <summary>
		/// The margin between the scroll pane and the window.
		/// </summary>
		private static readonly RectOffset OUTER_MARGIN = new RectOffset(6, 10, 6, 14);

		/// <summary>
		/// The size of checkboxes and images in this control.
		/// </summary>
		internal static readonly Vector2 PANEL_SIZE = new Vector2(240.0f, 360.0f);

		/// <summary>
		/// The size of checkboxes and images in this control.
		/// </summary>
		internal static readonly Vector2 ROW_SIZE = new Vector2(48, 48);

		/// <summary>
		/// The spacing between each row.
		/// </summary>
		internal const int ROW_SPACING = 2;

		/// <summary>
		/// The root panel of the whole control.
		/// </summary>
		public GameObject RootPanel { get; }

		/// <summary>
		/// The child panel where all rows are added.
		/// </summary>
		private GameObject childPanel;

		public TestControl(bool disableIcons = false) {
			var cp = new PPanel("Categories") {
				Direction = PanelDirection.Vertical,
				Alignment = TextAnchor.UpperLeft,
				Spacing = ROW_SPACING
			};
			cp.AddChild(new PTextField() {
				FlexSize = Vector2.one,
			});
			cp.AddChild(new PTextField() {

			});
			cp.AddChild(new PTextField() {

			});
			cp.AddChild(new PTextField() {

			});
			cp.AddChild(new PTextField() {

			});
			cp.OnRealize += (obj) => { childPanel = obj; };
			RootPanel = new PPanel("GridFilterableSideScreen") {
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
				}.AddChild(cp),
				ScrollHorizontal = false,
				ScrollVertical = true,
				AlwaysShowVertical = true,
				TrackSize = 8.0f,
				FlexSize = Vector2.one,
				BackColor = PUITuning.Colors.BackgroundLight,
			}).SetKleiBlueColor().Build();
		}
	}
}
