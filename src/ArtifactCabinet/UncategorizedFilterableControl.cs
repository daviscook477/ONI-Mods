using PeterHan.PLib;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Harmony;


namespace ArtifactCabinet
{
	public class UncategorizedFilterableControl
	{
		/// <summary>
		/// The margin around each checkbox
		/// </summary>
		private static readonly RectOffset ELEMENT_MARGIN = new RectOffset(2, 2, 2, 2);

		/// <summary>
		/// The size of the panel. Used as a minimum size to allow for scroll bar to work properly
		/// </summary>
		internal static readonly Vector2 PANEL_SIZE = new Vector2(372.0f, 480.0f);

		/// <summary>
		/// The size of the all checkbox in this control.
		/// </summary>
		internal static readonly Vector2 ALL_CHECK_SIZE = new Vector2(24.0f, 24.0f);

		/// <summary>
		/// The margin between the scroll pane and the window.
		/// </summary>
		private static readonly RectOffset OUTER_MARGIN = new RectOffset(6, 10, 6, 14);

		/// <summary>
		/// The margin around the outside of the cards.
		/// </summary>
		private static readonly RectOffset CARD_MARGIN = new RectOffset(0, 0, 0, 0);

		/// <summary>
		/// The number of elements to show in each row.
		/// </summary>
		internal const int PER_ROW = 4;

		/// <summary>
		/// Updates the all check box state from the children.
		/// </summary>
		/// <param name="allItems">The "all" or "none" check box.</param>
		/// <param name="children">The child check boxes.</param>
		internal static void UpdateAllItems<T>(GameObject allItems,
				IEnumerable<T> children) where T : IHasCheckBox
		{
			if (allItems != null)
			{
				bool all = true, none = true;
				foreach (var child in children)
					switch (PCheckBox.GetCheckState(child.CheckBox))
					{
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
		public bool IsAllSelected
		{
			get
			{
				return PCheckBox.GetCheckState(allItems) == PCheckBox.STATE_CHECKED;
			}
		}

		/// <summary>
		/// The root panel of the whole control.
		/// </summary>
		public PPanel RootPanel { get; }

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

		public UncategorizedFilterableControl()
		{
			// Select/deselect all types
			var allCheckBox = new PCheckBox("SelectAll")
			{
				Text = STRINGS.UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ALLBUTTON,
				CheckSize = ALL_CHECK_SIZE,
				InitialState = PCheckBox.STATE_CHECKED,
				OnChecked = OnCheck,
				TextStyle = PUITuning.Fonts.TextDarkStyle,
				Margin = ELEMENT_MARGIN
			};
			allCheckBox.OnRealize += (obj) => { allItems = obj; };
			var cp = new PPanel("Categories")
			{
				Direction = PanelDirection.Vertical,
				Alignment = TextAnchor.UpperLeft,
				Spacing = UncategorizedFilterableRow.CARD_SPACING,
				Margin = CARD_MARGIN
			};
			cp.OnRealize += (obj) => { childPanel = obj; };
			RootPanel = new PPanel("UncategorizedFilterableSideScreen")
			{
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
			});
			children = new List<UncategorizedFilterableRow>(16);
		}

		/// <summary>
		/// Updates the list of available elements.
		/// </summary>
		public void Update(GameObject target)
		{
			Target = target;
			Storage storage = Target.GetComponent<Storage>();
			UncategorizedFilterable filterable = Target.GetComponent<UncategorizedFilterable>();
			if (storage.storageFilters != null && storage.storageFilters.Count >= 1)
			{
				// check for which ones aren't added already and add them
				foreach (Tag tag in storage.storageFilters)
				{
					if (!HasElement(tag))
					{
						if (children.Count <= 0)
						{
							UncategorizedFilterableRow firstRow = new UncategorizedFilterableRow(this);
							children.Add(firstRow);
							PUIElements.SetParent(firstRow.ChildPanel, childPanel);
							PUIElements.SetAnchors(firstRow.ChildPanel, PUIAnchoring.Stretch, PUIAnchoring.Stretch);
						}
						UncategorizedFilterableRow lastRow = children[children.Count - 1];
						if (lastRow.RowSize >= PER_ROW)
						{
							lastRow = new UncategorizedFilterableRow(this);
							PUIElements.SetParent(lastRow.ChildPanel, childPanel);
							PUIElements.SetAnchors(lastRow.ChildPanel, PUIAnchoring.Stretch, PUIAnchoring.Stretch);
							children.Add(lastRow);
						}
						UncategorizedFilterableEntity entity = new UncategorizedFilterableEntity(lastRow, tag);
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
				Debug.LogError("If you're filtering, your storage filter should have the filters set on it");
		}

		/// <summary>
		/// Checks if an element is already a part of the UI.
		/// </summary>
		/// <param name="tag">The tag to check.</param>
		/// <returns>If or if not the element is already added.</returns>
		public bool HasElement(Tag tag)
		{
			foreach (var child in children)
			{
				foreach (var entity in child.Children)
				{
					if (entity.ElementTag.Equals(tag))
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Selects all items.
		/// </summary>
		public void CheckAll()
		{
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
		public void ClearAll()
		{
			PCheckBox.SetCheckState(allItems, PCheckBox.STATE_UNCHECKED);
			foreach (var child in children)
				foreach (var entity in child.Children)
				{
					PCheckBox.SetCheckState(entity.CheckBox, PCheckBox.STATE_UNCHECKED);
					Target.GetComponent<UncategorizedFilterable>().RemoveTagFromFilter(entity.ElementTag);
				}
		}

		private void OnCheck(GameObject source, int state)
		{
			switch (state)
			{
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
		internal void UpdateFromChildren()
		{
			List<UncategorizedFilterableEntity> entities = new List<UncategorizedFilterableEntity>();
			foreach (UncategorizedFilterableRow row in children)
			{
				entities.AddRange(row.Children);
			}
			UpdateAllItems(allItems, entities);
		}
	}
}
