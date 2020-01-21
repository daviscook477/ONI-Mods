using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Harmony;

namespace SideScreen {
	public class TestSideScreen : SideScreenContent {
		public GameObject target;
		public Test targetTest;

		public TestSideScreen() {
			activateOnSpawn = true;
			ConsumeMouseScroll = true;
			titleKey = "Test this is a title key";
		}

		public override bool IsValidForTarget(GameObject target) {
			return false;
		}

		public override void SetTarget(GameObject target) {
			this.target = target;
			if (target == null) {
				PeterHan.PLib.PUtil.LogError("The target object provided was null");
				return;
			}

			targetTest = target.GetComponent<Test>();
			if (targetTest == null) {
				PeterHan.PLib.PUtil.LogError("The target provided does not have a Test");
			}
		}
	}
}
