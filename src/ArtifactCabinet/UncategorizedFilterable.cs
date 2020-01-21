using System.Collections.Generic;
using System.Linq;
using System;
using KSerialization;

using UnityEngine;


namespace ArtifactCabinet
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class UncategorizedFilterable : KMonoBehaviour, ISaveLoadable {
        private static readonly EventSystem.IntraObjectHandler<UncategorizedFilterable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<UncategorizedFilterable>(OnCopySettings);
        public bool showUserMenu = true;
        [SerializeField]
        [Serialize]
        private List<Tag> acceptedTags = new List<Tag>();
        [MyCmpReq]
        private Storage storage;
        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;
        public System.Action<Tag[]> OnFilterChanged;

        public List<Tag> AcceptedTags {
            get {
                return this.acceptedTags;
            }
        }

        private void OnDiscover(Tag category_tag, Tag tag) {
            // not really sure what to do when discovery occurs?
            // right now we kinda show everything regardless of if the player has discovered it
            // probably need to make a list of what is discovered and what is not
            return;
        }

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            this.Subscribe<UncategorizedFilterable>(-905833192, UncategorizedFilterable.OnCopySettingsDelegate);
        }

        protected override void OnSpawn() {
            WorldInventory.Instance.OnDiscover += new System.Action<Tag, Tag>(this.OnDiscover);
            if ((UnityEngine.Object)this.storage != (UnityEngine.Object)null) {
                List<Tag> source = new List<Tag>();
                source.AddRange((IEnumerable<Tag>)this.acceptedTags);
                source.AddRange((IEnumerable<Tag>)this.storage.GetAllTagsInStorage());
                this.UpdateFilters((IList<Tag>)source.Distinct<Tag>().ToList<Tag>());
            }
            if (this.OnFilterChanged != null)
                this.OnFilterChanged(this.acceptedTags.ToArray());
        }

        protected override void OnCleanUp() {
            WorldInventory.Instance.OnDiscover -= new System.Action<Tag, Tag>(this.OnDiscover);
            base.OnCleanUp();
        }

        private static void OnCopySettings(UncategorizedFilterable filterable, object data) {
            UncategorizedFilterable component = ((GameObject)data).GetComponent<UncategorizedFilterable>();
            if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
                return;
            filterable.UpdateFilters((IList<Tag>)component.GetTags());
        }

        public Tag[] GetTags() {
            return this.acceptedTags.ToArray();
        }

        public bool ContainsTag(Tag t) {
            return this.acceptedTags.Contains(t);
        }

        public void AddTagToFilter(Tag t) {
            if (this.ContainsTag(t))
                return;
            this.UpdateFilters((IList<Tag>)new List<Tag>((IEnumerable<Tag>)this.acceptedTags)
            {
              t
            });
        }

        public void RemoveTagFromFilter(Tag t)
        {
            if (!this.ContainsTag(t))
                return;
            List<Tag> tagList = new List<Tag>((IEnumerable<Tag>)this.acceptedTags);
            tagList.Remove(t);
            this.UpdateFilters((IList<Tag>)tagList);
        }

        public void UpdateFilters(IList<Tag> filters) {
            this.acceptedTags.Clear();
            this.acceptedTags.AddRange((IEnumerable<Tag>)filters);
            if (this.OnFilterChanged != null)
                this.OnFilterChanged(this.acceptedTags.ToArray());
            if (!((UnityEngine.Object)this.storage != (UnityEngine.Object)null) || this.storage.items == null)
                return;
            List<GameObject> gameObjectList = new List<GameObject>();
            foreach (GameObject gameObject in this.storage.items) {
                if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null)) {
                    KPrefabID component = gameObject.GetComponent<KPrefabID>();
                    bool flag = false;
                    foreach (Tag acceptedTag in this.acceptedTags) {
                        if (component.Tags.Contains(acceptedTag)) {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                        gameObjectList.Add(gameObject);
                }
            }
            foreach (GameObject go in gameObjectList)
                this.storage.Drop(go, true);
        }

        public string GetTagsAsStatus(int maxDisplays = 6) {
            string str = "Tags:\n";
            List<Tag> first = new List<Tag>((IEnumerable<Tag>)this.acceptedTags);
            first.Intersect<Tag>((IEnumerable<Tag>)this.storage.storageFilters);
            for (int index = 0; index < Mathf.Min(first.Count, maxDisplays); ++index) {
                str += first[index].ProperName();
                if (index < Mathf.Min(first.Count, maxDisplays) - 1)
                    str += "\n";
                if (index == maxDisplays - 1 && first.Count > maxDisplays) {
                    str += "\n...";
                    break;
                }
            }
            if (this.tag.Length == 0)
                str = "No tags selected";
            return str;
        }
    }
}
