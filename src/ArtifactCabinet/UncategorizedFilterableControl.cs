using PeterHan.PLib;
using PeterHan.PLib.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using STRINGS;

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
		/// 1px dark border everywhere except the top.
		/// </summary>
		private static readonly RectOffset BORDER_MARGIN = new RectOffset(1, 1, 0, 1);

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
		/// Map of tag to entities. Rather than reusing UI elements in a pool, UI elements will simply be reused
		/// per item tag. These entities will simply be added or removed from the UI structure based on the target
		/// of the side screen.
		/// </summary>
		private Dictionary<Tag, UncategorizedFilterableEntity> entities;

		/// <summary>
		/// The child rows.
		/// </summary>
		private List<UncategorizedFilterableRow> rows;

		public UncategorizedFilterableControl()
		{
			// Select/deselect all types
			var allCheckBox = new PCheckBox("SelectAll")
			{
				Text = UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ALLBUTTON,
				CheckSize = ALL_CHECK_SIZE,
				InitialState = PCheckBox.STATE_CHECKED,
				OnChecked = OnCheck,
				TextStyle = PUITuning.Fonts.TextDarkStyle,
				Margin = ELEMENT_MARGIN
			};
			allCheckBox.OnRealize += (obj) => 
			{
				allItems = obj;
				allItems.AddComponent<ToolTip>().SetSimpleTooltip("Allow storage of all resource choices in this container");
			};
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
				Direction = PanelDirection.Vertical,
				Margin = BORDER_MARGIN,
				Alignment = TextAnchor.UpperLeft,
				Spacing = 0,
				BackColor = new Color(0, 0, 0, 255)
			}.AddChild(new PPanel("Content")
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
			}));
			rows = new List<UncategorizedFilterableRow>(4);
			entities = new Dictionary<Tag, UncategorizedFilterableEntity>(16);
		}

		/// <summary>
		/// Updates the list of available elements.
		/// </summary>
		public void Update(GameObject target)
		{
			Target = target;
			Storage storage = Target.GetComponent<Storage>();
			if (storage.storageFilters == null || storage.storageFilters.Count < 1)
			{
				PUtil.LogError("If you're filtering, your storage filter should have the filters set on it");
				return;
			}

			HashSet<Tag> containedTags = ContainedTags();
			HashSet<Tag> goalTags = new HashSet<Tag>(storage.storageFilters.Where(tag => WorldInventory.Instance.IsDiscovered(tag)));
			// if this is not supposed to display the exact same UI elements rebuild the entire UI
			// not the *most* performant way to handle things but trying to perform multiple insertions/deletions into this
			// row system is probably *less* performant
			if (!containedTags.SetEquals(goalTags))
			{
				// clear the UI
				foreach (var row in rows)
				{
					// null the parent of the entity and disable it
					foreach (var entity in row.entities)
					{
						PUIElements.SetParent(entity.CheckBox, null);
						entity.CheckBox.SetActive(false);
						entity.Parent = null;
					}
					// clear all the entities from this row
					row.entities.Clear();
					// do not null the parent of the row since it will be reused in same spot but do disable it
					row.ChildPanel.SetActive(false);
				}

				// build the UI with tags in alphabetic order
				List<Tag> goalList = goalTags.ToList();
				goalList.Sort(TagAlphabetComparer.INSTANCE);
				int rowIndex = 0;
				foreach (Tag tag in goalList)
				{
					// wrap around when rows are full
					if (rowIndex < rows.Count && rows[rowIndex].RowSize >= PER_ROW)
					{
						rowIndex++;
					}
					// build new rows as needed
					if (rows.Count <= rowIndex)
					{
						UncategorizedFilterableRow newRow = new UncategorizedFilterableRow(this);
						rows.Add(newRow);
						PUIElements.SetParent(newRow.ChildPanel, childPanel);
						PUIElements.SetAnchors(newRow.ChildPanel, PUIAnchoring.Stretch, PUIAnchoring.Stretch);
					}
					var row = rows[rowIndex];
					row.ChildPanel.SetActive(true);
					// build new entity for tag when it is first encountered
					if (!entities.ContainsKey(tag))
					{
						UncategorizedFilterableEntity newEntity = new UncategorizedFilterableEntity(null, tag, tag.ProperName());
						if (PCheckBox.GetCheckState(newEntity.CheckBox) == PCheckBox.STATE_CHECKED)
							// Set to checked
							PCheckBox.SetCheckState(newEntity.CheckBox, PCheckBox.STATE_CHECKED);
						entities[tag] = newEntity;
					}
					var entity = entities[tag];
					row.entities.Add(entity);
					PUIElements.SetParent(entity.CheckBox, row.ChildPanel);
					entity.CheckBox.SetActive(true);
					entity.Parent = row;
				}
			}

			// with the right elements in the UI it is now necessary to set the properties for each entity correctly based on
			// if they are checked already and if they are present in the world
			UncategorizedFilterable filterable = Target.GetComponent<UncategorizedFilterable>();
			foreach (var row in rows)
			{
				foreach (var entity in row.entities)
				{
					// set checkbox state
					if (filterable.AcceptedTags.Contains(entity.ElementTag))
					{
						PCheckBox.SetCheckState(entity.CheckBox, PCheckBox.STATE_CHECKED);
					}
					else
					{
						PCheckBox.SetCheckState(entity.CheckBox, PCheckBox.STATE_UNCHECKED);
					}
					// set active state
					var button = entity.CheckBox.GetComponentInChildren<KButton>();
					button.isInteractable = WorldInventory.Instance.GetTotalAmount(entity.ElementTag) > 0.0;
				}
			}
			UpdateFromChildren();
		}

		/// <summary>
		/// Gets all tags contained in the UI currently
		/// </summary>
		/// <returns>A set of tags in the UI</returns>
		public HashSet<Tag> ContainedTags()
		{
			HashSet<Tag> tags = new HashSet<Tag>();
			foreach (var row in rows)
			{
				foreach (var entity in row.entities)
				{
					tags.Add(entity.ElementTag);
				}
			}
			return tags;
		}

		/// <summary>
		/// Selects all items.
		/// </summary>
		public void CheckAll()
		{
			PCheckBox.SetCheckState(allItems, PCheckBox.STATE_CHECKED);
			foreach (var row in rows)
			{
				foreach (var entity in row.entities)
				{
					PCheckBox.SetCheckState(entity.CheckBox, PCheckBox.STATE_CHECKED);
					Target.GetComponent<UncategorizedFilterable>().AddTagToFilter(entity.ElementTag);
				}
			}
		}

		/// <summary>
		/// Deselects all items.
		/// </summary>
		public void ClearAll()
		{
			PCheckBox.SetCheckState(allItems, PCheckBox.STATE_UNCHECKED);
			foreach (var row in rows)
			{
				foreach (var entity in row.entities)
				{
					PCheckBox.SetCheckState(entity.CheckBox, PCheckBox.STATE_UNCHECKED);
					Target.GetComponent<UncategorizedFilterable>().RemoveTagFromFilter(entity.ElementTag);
				}
			}
		}

		/// <summary>
		/// Handle checking for the all checkbox
		/// </summary>
		/// <param name="source">source of the event</param>
		/// <param name="state">the new state of the checkbox</param>
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
			foreach (UncategorizedFilterableRow row in rows)
			{
				entities.AddRange(row.entities);
			}
			UpdateAllItems(allItems, entities);
		}
	}
}
