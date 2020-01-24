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
		public static void OnLoad()
		{
			PUtil.InitLibrary();
		}

		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public class DetailsScreen_OnPrefabInit_Patch
		{
			public static void Postfix()
			{
				Strings.Add("STRINGS.UI.UISIDESCREENS.UNCATEGORIZED_FILTERABLE_SIDE_SCREEN.TITLE", "Automated Storage Capacity");
				PUIUtils.AddSideScreenContent<UncategorizedFilterableSideScreen>(inOrder: true, insertionName: "Capacity Control Side Screen");
			}
		}
	}
}
