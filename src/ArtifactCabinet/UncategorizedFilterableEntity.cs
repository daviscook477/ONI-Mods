using PeterHan.PLib;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Harmony;

namespace ArtifactCabinet
{
	/// <summary>
	/// An individual entity for a single tag
	/// </summary>
    public sealed class UncategorizedFilterableEntity : IHasCheckBox
    {
		internal static ButtonSoundPlayer ButtonSounds { get; }

		static UncategorizedFilterableEntity()
		{
			ButtonSounds = new ButtonSoundPlayer()
			{
				Enabled = true
			};
		}

		/// <summary>
		/// Gets the sprite for a particular element tag.
		/// </summary>
		/// <param name="elementTag">The tag of the element to look up.</param>
		/// <param name="tint">The tint which will be used for the image.</param>
		/// <returns>The sprite to use for it.</returns>
		internal static Sprite GetStorageObjectSprite(Tag elementTag, out Color tint)
		{
			Sprite result = null;
			var prefab = Assets.GetPrefab(elementTag);
			tint = Color.white;
			if (prefab != null)
			{
				// Extract the UI preview image (sucks for bottles, but it is all we have)
				var sprite = Def.GetUISprite(prefab);
				if (sprite != null)
				{
					tint = sprite.second;
					result = sprite.first;
				}
			}
			return result;
		}

		/// <summary>
		/// 1px border for contrast
		/// </summary>
		private static readonly RectOffset BORDER_MARGIN = new RectOffset(1, 1, 1, 1);

		/// <summary>
		/// The margin around each checkbox
		/// </summary>
		private static readonly RectOffset ELEMENT_MARGIN = new RectOffset(2, 2, 2, 2);

		/// <summary>
		/// The size of checkboxes in this control.
		/// </summary>
		internal static readonly Vector2 CHECK_SIZE = new Vector2(24.0f, 24.0f);

		/// <summary>
		/// The size of images in this control.
		/// </summary>
		internal static readonly Vector2 ICON_SIZE = new Vector2(72.0f, 72.0f);

		/// <summary>
		/// The selection checkbox.
		/// </summary>
		public GameObject CheckBox { get; }

		/// <summary>
		/// The tag for this element.
		/// </summary>
		public Tag ElementTag { get; }

		/// <summary>
		/// The name of this element.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The parent row
		/// </summary>
		public UncategorizedFilterableRow Parent;

		public UncategorizedFilterableEntity(UncategorizedFilterableRow parent, Tag elementTag, string name)
		{
			Parent = parent;
			ElementTag = elementTag;
			Name = name;

			var tint = Color.white;
			var sprite = GetStorageObjectSprite(elementTag, out tint);

			var background = new PPanel("Background")
			{
				Direction = PanelDirection.Vertical,
				Alignment = TextAnchor.MiddleCenter
			}.AddChild(new PEntityToggle("Select")
			{
				OnChecked = OnCheck,
				InitialState = PCheckBox.STATE_CHECKED,
				Sprite = sprite,
				SpriteTint = tint,
				Margin = ELEMENT_MARGIN,
				TextAlignment = TextAnchor.UpperCenter,
				CheckSize = CHECK_SIZE,
				SpriteSize = ICON_SIZE,
			});
			// Background
			background.OnRealize += (obj) =>
			{
				var kImage = obj.AddComponent<KImage>();
				var ButtonBlueStyle = ScriptableObject.CreateInstance<ColorStyleSetting>();
				ButtonBlueStyle.activeColor = new Color(0.5033521f, 0.5444419f, 0.6985294f);
				ButtonBlueStyle.inactiveColor = new Color(0.2431373f, 0.2627451f, 0.3411765f);
				ButtonBlueStyle.disabledColor = new Color(0.4156863f, 0.4117647f, 0.4f);
				ButtonBlueStyle.disabledActiveColor = new Color(0.625f, 0.6158088f, 0.5882353f);
				ButtonBlueStyle.hoverColor = new Color(0.3461289f, 0.3739619f, 0.4852941f);
				ButtonBlueStyle.disabledhoverColor = new Color(0.5f, 0.4898898f, 0.4595588f);
				kImage.colorStyleSetting = ButtonBlueStyle;
				kImage.color = ButtonBlueStyle.inactiveColor;

				var kButton = obj.AddComponent<KButton>();
				kButton.additionalKImages = new KImage[0];
				kButton.soundPlayer = ButtonSounds;
				kButton.bgImage = kImage;
				kButton.colorStyleSetting = ButtonBlueStyle;
			};
			CheckBox = new PPanel("Border")
			{
				Margin = BORDER_MARGIN,
				Direction = PanelDirection.Vertical,
				Alignment = TextAnchor.MiddleCenter,
				Spacing = 1,
				BackColor = new Color(0, 0, 0, 255)
			}.AddChild(background).Build();

			var tooltip = CheckBox.AddComponent<ToolTip>();
			tooltip.SetSimpleTooltip(name);
			tooltip.tooltipPivot = new Vector2(0.5f, 1f);
			tooltip.tooltipPositionOffset = new Vector2(0.0f, -60f);
			tooltip.parentPositionAnchor = new Vector2(0.5f, 0.5f);
		}

		private void OnCheck(GameObject source, int state)
		{
			UncategorizedFilterable uncategorizedFilterable = Parent.Parent.Target.GetComponent<UncategorizedFilterable>();
			switch (state)
			{
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

		public override string ToString()
		{
			return $"UncategorizedFilterableEntity[Tag={ElementTag.ToString()},State={PCheckBox.GetCheckState(CheckBox)}]";
		}
	}
}
