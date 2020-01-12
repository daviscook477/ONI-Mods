using PeterHan.PLib;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mineralizer {
	public class GridFilterableControl {
		public static GridFilterableControl Instance = null;

		/// <summary>
		/// The margin around the scrollable area to avoid stomping on the scrollbar.
		/// </summary>
		private static readonly RectOffset ELEMENT_MARGIN = new RectOffset(2, 2, 2, 2);

		/// <summary>
		/// The size of checkboxes and images in this control.
		/// </summary>
		internal static readonly Vector2 PANEL_SIZE = new Vector2(240.0f, 360.0f);

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

		/// <summary>
		/// The number of elements to show in each row.
		/// </summary>
		internal const int PER_ROW = 2;

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
					// Gas bottles do not have a place sprite, silence the warning
					if (anim != null && anim.name != "gas_tank_kanim")
						result = Def.GetUISpriteFromMultiObjectAnim(anim);
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
		private readonly List<GridFilterableRow> children;

		public GridFilterableControl() {
			// Select/deselect all types
			var allCheckBox = new PCheckBox("SelectAll") {
				Text = STRINGS.UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ALLBUTTON,
				CheckSize = ROW_SIZE,
				InitialState = PCheckBox.STATE_CHECKED,
				OnChecked = OnCheck,
				TextStyle = PUITuning.Fonts.TextDarkStyle
			};
			allCheckBox.OnRealize += (obj) => { allItems = obj; };
			var cp = new PPanel("Categories") {
				Direction = PanelDirection.Vertical,
				Alignment = TextAnchor.UpperLeft,
				Spacing = ROW_SPACING
			};
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
				}.AddChild(allCheckBox).AddChild(cp),
				ScrollHorizontal = false,
				ScrollVertical = true,
				AlwaysShowVertical = true,
				TrackSize = 8.0f,
				FlexSize = Vector2.one,
				BackColor = PUITuning.Colors.BackgroundLight,
			}).SetKleiBlueColor().Build();
			children = new List<GridFilterableRow>(16);
			if (Instance != null) {
				Debug.LogError("ISSUE! created grid filterable control more than once");
			}
			Instance = this;
		}

		/// <summary>
		/// Updates the list of available elements.
		/// </summary>
		public void Update(GridFilterableSideScreen screen) {
			Console.WriteLine("updating with the storage");
			Storage storage = screen.storage;
			GameObject target = screen.target;
			if (storage.storageFilters != null && storage.storageFilters.Count >= 1) {
				bool flag = (UnityEngine.Object)target.GetComponent<CreatureDeliveryPoint>() != (UnityEngine.Object)null;
				// determine all the possible tags that go in this container based on its filter
				HashSet<Tag> tags = new HashSet<Tag>();
				foreach (Tag tag in storage.storageFilters) {
					if (flag || WorldInventory.Instance.IsDiscovered(tag)) {
						tags.UnionWith(WorldInventory.Instance.GetDiscoveredResourcesFromTag(tag));
					}
				}
				// check for which ones aren't added already and add them
				foreach (Tag tag in tags) {
					if (!HasElement(tag)) {
						Console.WriteLine($"Attempted to add {tag.ToString()} to the panel");
						if (children.Count <= 0) {
							GridFilterableRow firstRow = new GridFilterableRow(this);
							children.Add(firstRow);
							PUIElements.SetParent(firstRow.ChildPanel, childPanel);
							PUIElements.SetAnchors(firstRow.ChildPanel, PUIAnchoring.Stretch, PUIAnchoring.Stretch);
						}
						GridFilterableRow lastRow = children[children.Count - 1];
						if (lastRow.RowSize >= PER_ROW) {
							lastRow = new GridFilterableRow(this);
							PUIElements.SetParent(lastRow.ChildPanel, childPanel);
							PUIElements.SetAnchors(lastRow.ChildPanel, PUIAnchoring.Stretch, PUIAnchoring.Stretch);
							children.Add(lastRow);
						}
						GridFilterableSelectableEntity entity = new GridFilterableSelectableEntity(lastRow, tag);
						lastRow.Children.Add(entity);
						PUIElements.SetParent(entity.CheckBox, lastRow.ChildPanel);
						if (PCheckBox.GetCheckState(entity.CheckBox) == PCheckBox.STATE_CHECKED)
							// Set to checked
							PCheckBox.SetCheckState(entity.CheckBox, PCheckBox.STATE_CHECKED);
					}
				}
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
					PCheckBox.SetCheckState(entity.CheckBox, PCheckBox.STATE_CHECKED);
		}

		/// <summary>
		/// Deselects all items.
		/// </summary>
		public void ClearAll() {
			PCheckBox.SetCheckState(allItems, PCheckBox.STATE_UNCHECKED);
			foreach (var child in children)
				foreach (var entity in child.Children)
					PCheckBox.SetCheckState(entity.CheckBox, PCheckBox.STATE_UNCHECKED);
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
			List<GridFilterableSelectableEntity> entities = new List<GridFilterableSelectableEntity>();
			foreach (GridFilterableRow row in children) {
				entities.AddRange(row.Children);
			}
			UpdateAllItems(allItems, entities);
		}

		/// <summary>
		/// An individual row used to organize type selection
		/// </summary>
		public sealed class GridFilterableRow {
			/// <summary>
			/// The panel holding all children.
			/// </summary>
			public GameObject ChildPanel { get; }

			/// <summary>
			/// The parent side screen.
			/// </summary>
			public readonly GridFilterableControl Control;

			/// <summary>
			/// The child elements.
			/// </summary>
			public readonly List<GridFilterableSelectableEntity> Children;

			public int RowSize {
				get {
					return Children.Count;
				}
			}

			public GridFilterableRow(GridFilterableControl control) {
				Control = control ?? throw new ArgumentNullException("parent");
				Children = new List<GridFilterableSelectableEntity>(16);
				ChildPanel = new PPanel("Children") {
					Direction = PanelDirection.Horizontal,
					Alignment = TextAnchor.MiddleLeft,
					Spacing = ROW_SPACING,
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
		public sealed class GridFilterableSelectableEntity : IHasCheckBox {
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
			public readonly GridFilterableRow Parent;

			public GridFilterableSelectableEntity(GridFilterableRow parent, Tag elementTag) {
				this.Parent = parent ?? throw new ArgumentNullException("parent");
				ElementTag = elementTag;
				CheckBox = new PEntityToggle("Select") {
					//OnChecked = OnCheck,
					Text = ElementTag.ProperName(),
					//InitialState = PCheckBox.
					//STATE_CHECKED,
					Sprite = GetStorageObjectSprite(elementTag),
				}.Build();
			}

			private void OnCheck(GameObject source, int state) {
				switch (state) {
					case PCheckBox.STATE_UNCHECKED:
						// Clicked when unchecked, check and possibly check all
						PCheckBox.SetCheckState(CheckBox, PCheckBox.STATE_CHECKED);
						break;
					default:
						// Clicked when checked, clear and possibly uncheck
						PCheckBox.SetCheckState(CheckBox, PCheckBox.STATE_UNCHECKED);
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
