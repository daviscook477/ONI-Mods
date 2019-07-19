using System.Reflection;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace TeleStorage
{
    public class StoredItem
    {
        [JsonProperty]
        public float mass = 0.0f;

        [JsonProperty]
        public float temperature = 273.15f;

        [JsonProperty]
        public byte diseaseIdx = 0;

        [JsonProperty]
        public int diseaseCount = 0;
    }

    public class TeleStorageData
    {
        private static TeleStorageData instance = null;
        private static readonly object _lock = new object();

        [JsonProperty]
        public Dictionary<SimHashes, StoredItem> storedElementsMap = new Dictionary<SimHashes, StoredItem>();

        [JsonIgnore]
        public List<TeleStorage> storageContainers = new List<TeleStorage>();

        public void FireRefresh(SimHashes element, StoredItem item)
        {
            foreach (TeleStorage storageContainer in storageContainers)
            {
                storageContainer.RefreshStorage(element, item);
            }
        }

        TeleStorageData() { }

        public static TeleStorageData Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        DebugUtil.LogArgs("Attempting to access unitialized TeleStorageData. Using default values instead.");
                        instance = new TeleStorageData();
                    }
                    return instance;
                }
            }
        }

        public static void Load(string filename)
        {
            instance = ConfigManager.LoadConfig<TeleStorageData>(Assembly.GetExecutingAssembly().Location, Path.GetFileName(filename));
            if (instance == null)
            {
                instance = new TeleStorageData();
            }
        }

        public static void Save(string filename)
        {
            ConfigManager.SaveConfig(Assembly.GetExecutingAssembly().Location, instance, Path.GetFileName(filename));
        }

    }
}
