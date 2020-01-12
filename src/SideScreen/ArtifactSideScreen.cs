using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSerialization;

namespace EthanolGeyser {
    public class ArtifactSideScreen : SideScreenContent {
        [Header("Recipe List")]
        private Mineralizer.Mineralizer target;
        protected override void OnPrefabInit() {
            Debug.Log("ARTIFACT CABINET: OnPrefabInit");
            base.OnPrefabInit();
        }


        public void Initialize(Mineralizer.Mineralizer target) {
            if (target == null) {
                Debug.LogError((object)"ArtfiactCabinet provided was null.");
            }

            Debug.Log("ARTIFACT CABINET: Initialized");
            this.target = target;
            this.gameObject.SetActive(true);

        }

        private void OnRefreshData(object obj) {
            this.SetTarget(this.target.gameObject);
            Debug.Log("ARTIFACT CABINET: Onrefreshdata");
        }

        public override bool IsValidForTarget(GameObject target) {
            return target.GetComponent<Mineralizer.Mineralizer>() != null;
        }

        public override void SetTarget(GameObject target) {
            if (target == null) {
                Debug.LogError("The target object provided was null");
            }
            else {
                Initialize(target.GetComponent<Mineralizer.Mineralizer>());
            }
        }
    }
}