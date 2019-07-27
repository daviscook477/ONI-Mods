using System.Collections.Generic;
using System.Reflection;
using System.IO;

using Harmony;
using UnityEngine;

namespace CrystalBiome.AssetLoading
{
    public class AssetLoader
    {

        private static AssetLoader instance = null;
        private static readonly object _lock = new object();

        AssetLoader() { }

        public static AssetLoader Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new AssetLoader();
                    }
                    return instance;
                }
            }
        }

        public Dictionary<string, Texture2D> TextureTable = new Dictionary<string, Texture2D>();
    }
}
