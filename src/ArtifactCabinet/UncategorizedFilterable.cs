using System.Collections.Generic;
using System.Linq;
using System;
using KSerialization;

using UnityEngine;


namespace ArtifactCabinet
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class UncategorizedFilterable : KMonoBehaviour, ISaveLoadable
    {
        private static readonly EventSystem.IntraObjectHandler<UncategorizedFilterable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<UncategorizedFilterable>(OnCopySettings);
        public bool showUserMenu = true;
        [SerializeField]
        [Serialize]
        private HashSet<Tag> acceptedTags = new HashSet<Tag>();
        [MyCmpReq]
        private Storage storage;
        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;
        public Action<Tag[]> OnFilterChanged;

        public HashSet<Tag> AcceptedTags
        {
            get
            {
                return acceptedTags;
            }
        }

        private void OnDiscover(Tag category_tag, Tag tag)
        {
            if (!storage.storageFilters.Contains(tag))
                return;
            if (acceptedTags.Contains(tag))
                return;
            // when a tag is discovered we only add it to the filter if the filter is behaving as if all are selected
            HashSet<Tag> storageSet = new HashSet<Tag>(storage.storageFilters.Where(storedTag => WorldInventory.Instance.IsDiscovered(storedTag)));
            storageSet.Remove(tag);
            if (storageSet.SetEquals(acceptedTags))
            {
                acceptedTags.Add(tag);
                OnFilterChanged?.Invoke(acceptedTags.ToArray());
            }
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe(-905833192, OnCopySettingsDelegate);
        }

        protected override void OnSpawn()
        {
            WorldInventory.Instance.OnDiscover += new Action<Tag, Tag>(OnDiscover);
            if (storage != null)
            {
                List<Tag> source = new List<Tag>();
                source.AddRange(acceptedTags);
                source.AddRange(storage.GetAllTagsInStorage());
                UpdateFilters(source.Distinct().ToList());
            }
            OnFilterChanged?.Invoke(acceptedTags.ToArray());
            RemoveIncorrectAcceptedTags();
        }

        private void RemoveIncorrectAcceptedTags()
        {
            List<Tag> tagList = acceptedTags.Where(tag => !WorldInventory.Instance.IsDiscovered(tag)).ToList();
            foreach (Tag t in tagList)
                RemoveTagFromFilter(t);
        }

        protected override void OnCleanUp()
        {
            WorldInventory.Instance.OnDiscover -= new Action<Tag, Tag>(OnDiscover);
            base.OnCleanUp();
        }

        private static void OnCopySettings(UncategorizedFilterable filterable, object data)
        {
            UncategorizedFilterable component = ((GameObject)data).GetComponent<UncategorizedFilterable>();
            if (component == null)
                return;
            filterable.UpdateFilters(component.GetTags());
        }

        public Tag[] GetTags()
        {
            return acceptedTags.ToArray();
        }

        public bool ContainsTag(Tag t)
        {
            return acceptedTags.Contains(t);
        }

        public void AddTagToFilter(Tag t)
        {
            if (ContainsTag(t))
                return;
            UpdateFilters(new List<Tag>(acceptedTags)
            {
                t
            });
        }

        public void RemoveTagFromFilter(Tag t)
        {
            if (!ContainsTag(t))
                return;
            List<Tag> tagList = new List<Tag>(acceptedTags);
            tagList.Remove(t);
            UpdateFilters(tagList);
        }

        public void UpdateFilters(IList<Tag> filters)
        {
            acceptedTags.Clear();
            acceptedTags.UnionWith(filters);
            OnFilterChanged?.Invoke(acceptedTags.ToArray());
            if (storage == null || storage.items == null)
                return;
            List<GameObject> gameObjectList = new List<GameObject>();
            foreach (GameObject gameObject in storage.items)
            {
                if (gameObject != null)
                {
                    KPrefabID component = gameObject.GetComponent<KPrefabID>();
                    bool flag = false;
                    foreach (Tag acceptedTag in acceptedTags)
                    {
                        if (component.Tags.Contains(acceptedTag))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                        gameObjectList.Add(gameObject);
                }
            }
            foreach (GameObject go in gameObjectList)
                storage.Drop(go, true);
        }

        public string GetTagsAsStatus(int maxDisplays = 6)
        {
            string str = "Tags:\n";
            List<Tag> first = new List<Tag>(acceptedTags);
            first.Intersect(storage.storageFilters);
            for (int index = 0; index < Mathf.Min(first.Count, maxDisplays); ++index)
            {
                str += first[index].ProperName();
                if (index < Mathf.Min(first.Count, maxDisplays) - 1)
                    str += "\n";
                if (index == maxDisplays - 1 && first.Count > maxDisplays)
                {
                    str += "\n...";
                    break;
                }
            }
            if (tag.Length == 0)
                str = "No tags selected";
            return str;
        }
    }
}
