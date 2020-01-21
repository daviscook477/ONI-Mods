using PeterHan.PLib;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArtifactCabinet
{
	public class UncategorizedFilterableControl {
		public static UncategorizedFilterableControl Instance = null;

		/// <summary>
		/// The margin around the scrollable area to avoid stomping on the scrollbar.
		/// </summary>
		private static readonly RectOffset ELEMENT_MARGIN = new RectOffset(2, 2, 2, 2);

		/// <summary>
		/// The size of checkboxes and images in this control.
		/// </summary>
		internal static readonly Vector2 PANEL_SIZE = new Vector2(360.0f, 480.0f);

		/// <summary>
		/// The margin between the scroll pane and the window.
		/// </summary>
		private static readonly RectOffset OUTER_MARGIN = new RectOffset(6, 10, 6, 14);

		/// <summary>
		/// The size of checkboxes in this control.
		/// </summary>
		internal static readonly Vector2 CHECK_SIZE = new Vector2(24.0f, 24.0f);

		/// <summary>
		/// The size of images in this control.
		/// </summary>
		internal static readonly Vector2 ICON_SIZE = new Vector2(64.0f, 64.0f); 

		/// <summary>
		/// The spacing between each card.
		/// </summary>
		internal const int CARD_SPACING = 2;

		/// <summary>
		/// The number of elements to show in each row.
		/// </summary>
		internal const int PER_ROW = 5;

		/// <summary>
		/// Gets the sprite for a particular element tag.
		/// </summary>
		/// <param name="elementTag">The tag of the element to look up.</param>
		/// <returns>The sprite to use for it.</returns>
		internal static Sprite GetStorageObjectSprite(Tag elementTag) {
			Sprite result = null;
			var prefab = Assets.GetPrefab(elementTag);
			if (prefab != null) {
				// Extract the UI preview image (sucks for bottles, but it is all we have)
				var component = prefab.GetComponent<KBatchedAnimController>();
				if (component != null) {
					var anim = component.AnimFiles[0];
					string animName = "ui";
					if (UncategorizedFilterablePatches.ArtifactMap.ContainsKey(elementTag.ToString()))
					{
						animName = UncategorizedFilterablePatches.ArtifactMap[elementTag.ToString()];
					}
					// Gas bottles do not have a place sprite, silence the warning
					if (anim != null) //&& anim.name != "gas_tank_kanim")
						result = Def.GetUISpriteFromMultiObjectAnim(anim, animName);
				}
			}
			return result;
		}


		/// <summary>
		/// Updates the all check box state from the children.
		/// </summary>
		/// <param name="allItems">The "all" or "none" check box.</param>
		/// <param name="children">The child check boxes.</param>
		internal static void UpdateAllItems<T>(GameObject allItems,
				IEnumerable<T> children) where T : IHasCheckBox {
			if (allItems != null) {
				bool all = true, none = true;
				foreach (var child in children)
					switch (PCheckBox.GetCheckState(child.CheckBox)) {
						case PCheckBox.STATE_CHECKED:
							none = false;
							break;
						default:
							// Partially checked or unchecked
							all = false;
							break;
					}
				PCheckBox.SetCheckState(allItems, none ? PCheckBox.STATE_UNCHECKED : (all ?
					PCheckBox.STATE_CHECKED : PCheckBox.STATE_PARTIAL));
			}
		}

		/// <summary>
		/// Returns true if all items are selected to sweep.
		/// </summary>
		public bool IsAllSelected {
			get {
				return PCheckBox.GetCheckState(allItems) == PCheckBox.STATE_CHECKED;
			}
		}

		/// <summary>
		/// The root panel of the whole control.
		/// </summary>
		public GameObject RootPanel { get; }

		/// <summary>
		/// The target of the uncategorized filterable control UI.
		/// </summary>
		public GameObject Target { get; set; }

		/// <summary>
		/// The "all items" checkbox.
		/// </summary>
		private GameObject allItems;

		/// <summary>
		/// The child panel where all rows are added.
		/// </summary>
		private GameObject childPanel;

		/// <summary>
		/// The child rows.
		/// </summary>
		private readonly List<UncategorizedFilterableRow> children;

		public UncategorizedFilterableControl() {
			// Select/deselect all types
			var allCheckBox = new PCheckBox("SelectAll") {
				Text = STRINGS.UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ALLBUTTON,
				CheckSize = CHECK_SIZE,
				InitialState = PCheckBox.STATE_CHECKED,
				OnChecked = OnCheck,
				TextStyle = PUITuning.Fonts.TextLightStyle
			};
			allCheckBox.OnRealize += (obj) => { allItems = obj; };
			var cp = new PPanel("Categories") {
				Direction = PanelDirection.Vertical,
				Alignment = TextAnchor.UpperLeft,
				Spacing = CARD_SPACING
			};
			cp.OnRealize += (obj) => { childPanel = obj; };
			RootPanel = new PPanel("UncategorizedFilterableSideScreen") {
				// White background for scroll bar
				Direction = PanelDirection.Vertical,
				Margin = OUTER_MARGIN,
				Alignment = TextAnchor.UpperLeft,
				Spacing = 0,
				BackColor = PUITuning.Colors.BackgroundLight,
				FlexSize = Vector2.one,
			}.AddChild(new PScrollPane("Scroll")
			{
				// Scroll to select elements
				Child = new PPanel("SelectType")
				{
					Direction = PanelDirection.Vertical,
					Margin = ELEMENT_MARGIN,
					FlexSize = new Vector2(1.0f, 0.0f),
					Alignment = TextAnchor.UpperLeft
				}.AddChild(allCheckBox).AddChild(cp),
				ScrollHorizontal = false,
				ScrollVertical = true,
				AlwaysShowVertical = true,
				TrackSize = 8.0f,
				FlexSize = Vector2.one,
				BackColor = PUITuning.Colors.BackgroundLight,
			}).SetKleiBlueColor().BuildWithFixedSize(PANEL_SIZE);
			children = new List<UncategorizedFilterableRow>(16);
			if (Instance != null) {
				Debug.LogError("ISSUE! created uncategorized filterable control more than once");
			}
			Instance = this;
		}

		/// <summary>
		/// Updates the list of available elements.
		/// </summary>
		public void Update(GameObject target) {
			Target = target;
			Storage storage = Target.GetComponent<Storage>();
			UncategorizedFilterable filterable = Target.GetComponent<UncategorizedFilterable>();
			if (storage.storageFilters != null && storage.storageFilters.Count >= 1) {
				// check for which ones aren't added already and add them
				foreach (Tag tag in storage.storageFilters) {
					if (!HasElement(tag)) {
						if (children.Count <= 0) {
							UncategorizedFilterableRow firstRow = new UncategorizedFilterableRow(this);
							children.Add(firstRow);
							PUIElements.SetParent(firstRow.ChildPanel, childPanel);
							PUIElements.SetAnchors(firstRow.ChildPanel, PUIAnchoring.Stretch, PUIAnchoring.Stretch);
						}
						UncategorizedFilterableRow lastRow = children[children.Count - 1];
						if (lastRow.RowSize >= PER_ROW) {
							lastRow = new UncategorizedFilterableRow(this);
							PUIElements.SetParent(lastRow.ChildPanel, childPanel);
							PUIElements.SetAnchors(lastRow.ChildPanel, PUIAnchoring.Stretch, PUIAnchoring.Stretch);
							children.Add(lastRow);
						}
						UncategorizedFilterableSelectableEntity entity = new UncategorizedFilterableSelectableEntity(lastRow, tag);
						lastRow.Children.Add(entity);
						PUIElements.SetParent(entity.CheckBox, lastRow.ChildPanel);
						if (PCheckBox.GetCheckState(entity.CheckBox) == PCheckBox.STATE_CHECKED)
							// Set to checked
							PCheckBox.SetCheckState(entity.CheckBox, PCheckBox.STATE_CHECKED);
					}
				}
				// update the state of each to what the filter actually says
				foreach (var child in children)
				{
					foreach (var entity in child.Children)
					{
						if (filterable.AcceptedTags.Contains(entity.ElementTag))
						{
							PCheckBox.SetCheckState(entity.CheckBox, PCheckBox.STATE_CHECKED);
						}
						else
						{
							PCheckBox.SetCheckState(entity.CheckBox, PCheckBox.STATE_UNCHECKED);
						}
					}
				}
				UpdateFromChildren();
			}
			else
				Debug.LogError((object)"If you're filtering, your storage filter should have the filters set on it");
		}

		/// <summary>
		/// Checks if an element is already a part of the UI.
		/// </summary>
		/// <param name="tag">The tag to check.</param>
		/// <returns>If or if not the element is already added.</returns>
		public bool HasElement(Tag tag) {
			foreach (var child in children) {
				foreach (var entity in child.Children) {
					if (entity.ElementTag.Equals(tag)) {
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Selects all items.
		/// </summary>
		public void CheckAll() {
			PCheckBox.SetCheckState(allItems, PCheckBox.STATE_CHECKED);
			foreach (var child in children)
				foreach (var entity in child.Children)
				{
					PCheckBox.SetCheckState(entity.CheckBox, PCheckBox.STATE_CHECKED);
					Target.GetComponent<UncategorizedFilterable>().AddTagToFilter(entity.ElementTag);
				}

		}

		/// <summary>
		/// Deselects all items.
		/// </summary>
		public void ClearAll() {
			PCheckBox.SetCheckState(allItems, PCheckBox.STATE_UNCHECKED);
			foreach (var child in children)
				foreach (var entity in child.Children)
				{
					PCheckBox.SetCheckState(entity.CheckBox, PCheckBox.STATE_UNCHECKED);
					Target.GetComponent<UncategorizedFilterable>().RemoveTagFromFilter(entity.ElementTag);
				}
		}

		private void OnCheck(GameObject source, int state) {
			switch (state) {
				case PCheckBox.STATE_UNCHECKED:
					// Clicked when unchecked, check all
					CheckAll();
					break;
				default:
					// Clicked when checked or partial, clear all
					ClearAll();
					break;
			}
		}

		/// <summary>
		/// Updates the parent check box state from the children.
		/// </summary>
		internal void UpdateFromChildren() {
			List<UncategorizedFilterableSelectableEntity> entities = new List<UncategorizedFilterableSelectableEntity>();
			foreach (UncategorizedFilterableRow row in children) {
				entities.AddRange(row.Children);
			}
			UpdateAllItems(allItems, entities);
		}

		/// <summary>
		/// An individual row used to organize type selection
		/// </summary>
		public sealed class UncategorizedFilterableRow {
			/// <summary>
			/// The panel holding all children.
			/// </summary>
			public GameObject ChildPanel { get; }

			/// <summary>
			/// The parent side screen.
			/// </summary>
			public readonly UncategorizedFilterableControl Control;

			/// <summary>
			/// The child elements.
			/// </summary>
			public readonly List<UncategorizedFilterableSelectableEntity> Children;

			public int RowSize {
				get {
					return Children.Count;
				}
			}

			public UncategorizedFilterableRow(UncategorizedFilterableControl control) {
				Control = control ?? throw new ArgumentNullException("parent");
				Children = new List<UncategorizedFilterableSelectableEntity>(16);
				ChildPanel = new PPanel("Children") {
					Direction = PanelDirection.Horizontal,
					Alignment = TextAnchor.MiddleLeft,
					Spacing = CARD_SPACING,
					Margin = new RectOffset(0, 0, 0, 0)
				}.Build();
				//ChildPanel.transform.localScale = Vector3.zero;
			}

			/// <summary>
			/// Updates the parent check box state from the children.
			/// </summary>
			internal void UpdateFromChildren() {
				Control.UpdateFromChildren();
			}
		}

		/// <summary>
		/// An individual element choice used in type select controls.
		/// </summary>
		public sealed class UncategorizedFilterableSelectableEntity : IHasCheckBox {
			/// <summary>
			/// The selection checkbox.
			/// </summary>
			public GameObject CheckBox { get; }

			/// <summary>
			/// The tag for this element.
			/// </summary>
			public Tag ElementTag { get; }

			/// <summary>
			/// The parent row
			/// </summary>
			public readonly UncategorizedFilterableRow Parent;

			public UncategorizedFilterableSelectableEntity(UncategorizedFilterableRow parent, Tag elementTag) {
				this.Parent = parent ?? throw new ArgumentNullException("parent");
				ElementTag = elementTag;

				CheckBox = new PPanel("Border")
				{
					// 1px dark border for contrast
					Margin = new RectOffset(1, 1, 1, 1),
					Direction = PanelDirection.Vertical,
					Alignment = TextAnchor.MiddleCenter,
					Spacing = 1,
					BackColor = new Color(0, 0, 0, 255)
				}.AddChild(new PPanel("Background")
				{
					Direction = PanelDirection.Vertical,
					Alignment = TextAnchor.MiddleCenter
				}.SetKleiBlueColor().AddChild(new PEntityToggle("Select")
				{
					OnChecked = OnCheck,
					InitialState = PCheckBox.STATE_CHECKED,
					Sprite = GetStorageObjectSprite(elementTag),
					TextAlignment = TextAnchor.UpperCenter,
					CheckSize = CHECK_SIZE,
					SpriteSize = ICON_SIZE,
				})).Build();
			}

			private void OnCheck(GameObject source, int state) {
				UncategorizedFilterable uncategorizedFilterable = UncategorizedFilterableControl.Instance.Target.GetComponent<UncategorizedFilterable>();
				switch (state) {
					case PCheckBox.STATE_UNCHECKED:
						// Clicked when unchecked, check and possibly check all
						PCheckBox.SetCheckState(CheckBox, PCheckBox.STATE_CHECKED);
						uncategorizedFilterable.AddTagToFilter(ElementTag);
						break;
					default:
						// Clicked when checked, clear and possibly uncheck
						PCheckBox.SetCheckState(CheckBox, PCheckBox.STATE_UNCHECKED);
						uncategorizedFilterable.RemoveTagFromFilter(ElementTag);
						break;
				}
				Parent.UpdateFromChildren();
			}

			public override string ToString() {
				return "TypeSelectElement[Tag={0},State={1}]".F(ElementTag.ToString(),
					PCheckBox.GetCheckState(CheckBox));
			}
		}

		/// <summary>
		/// Applied to categories and elements with a single summary checkbox.
		/// </summary>
		internal interface IHasCheckBox {
			/// <summary>
			/// Checkbox!
			/// </summary>
			GameObject CheckBox { get; }
		}
	}
}
