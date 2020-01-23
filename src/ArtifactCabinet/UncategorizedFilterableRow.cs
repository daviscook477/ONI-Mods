using System;
using System.Collections.Generic;
using UnityEngine;
using PeterHan.PLib;
using PeterHan.PLib.UI;

namespace ArtifactCabinet
{
    /// <summary>
    /// An individual row used to organize type selection
    /// </summary>
    public sealed class UncategorizedFilterableRow
    {
		/// <summary>
		/// The spacing between each card.
		/// </summary>
		internal const int CARD_SPACING = 10;

		/// <summary>
		/// The panel holding all children.
		/// </summary>
		public GameObject ChildPanel { get; }

		/// <summary>
		/// The parent side screen.
		/// </summary>
		public UncategorizedFilterableControl Parent;

		/// <summary>
		/// The child elements.
		/// </summary>
		public List<UncategorizedFilterableEntity> entities;

		public int RowSize
		{
			get
			{
				return entities.Count;
			}
		}

		public UncategorizedFilterableRow(UncategorizedFilterableControl control)
		{
			Parent = control ?? throw new ArgumentNullException("parent");
			entities = new List<UncategorizedFilterableEntity>(16);
			ChildPanel = new PPanel("Children")
			{
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
		internal void UpdateFromChildren()
		{
			Parent.UpdateFromChildren();
		}
	}
}
