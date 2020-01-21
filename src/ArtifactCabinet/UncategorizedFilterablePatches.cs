using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;
using PeterHan.PLib;

namespace ArtifactCabinet
{
    public class UncategorizedFilterablePatches
    {
		/// <summary>
		/// maps the artifact id/tag to the ui anim
		/// </summary>
		public static Dictionary<string, string> ArtifactMap = new Dictionary<string, string>();

		public static void OnLoad()
		{
			PUtil.InitLibrary();
		}

		[HarmonyPatch(typeof(ArtifactConfig), "CreateArtifact")]
		public class ArtifactConfig_CreateArtifact_Patch
		{
			public static void Prefix(string id, string name, string desc, string initial_anim, string ui_anim)
			{
				ArtifactMap.Add("artifact_" + id.ToLower(), ui_anim);
			}
		}

		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public class DetailsScreen_OnPrefabInit_Patch
		{
			public static void Postfix()
			{
				List<DetailsScreen.SideScreenRef> sideScreens = Traverse.Create(DetailsScreen.Instance).Field("sideScreens").GetValue<List<DetailsScreen.SideScreenRef>>();
				GameObject sideScreenContentBody = Traverse.Create(DetailsScreen.Instance).Field("sideScreenContentBody").GetValue<GameObject>();
				UncategorizedFilterableControl uncategorizedSelectControl = new UncategorizedFilterableControl();
				UncategorizedFilterableSideScreen screen = uncategorizedSelectControl.RootPanel.AddComponent<UncategorizedFilterableSideScreen>();
				screen.gameObject.transform.parent = sideScreenContentBody.transform;
				DetailsScreen.SideScreenRef myRef = new DetailsScreen.SideScreenRef
				{
					name = "ArtifactSideScreen",
					screenPrefab = screen,
					offset = new Vector2(0f, 0f),
					screenInstance = screen
				};
				sideScreens.Add(myRef);
			}
		}
	}
}
