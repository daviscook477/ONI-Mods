using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;
using PeterHan.PLib;
using PeterHan.PLib.UI;

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
				PUIUtils.AddSideScreenContent<UncategorizedFilterableSideScreen>();
			}
		}
	}
}
