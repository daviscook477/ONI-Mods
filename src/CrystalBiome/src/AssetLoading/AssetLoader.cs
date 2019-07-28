using System.Collections.Generic;
using System.Reflection;
using System.IO;

using UnityEngine;

namespace CrystalBiome.AssetLoading
{
    public class AssetLoader
    {
        public static void OnLoad()
        {
            string executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
            string executingAsemblyDirectory = Path.GetDirectoryName(executingAssemblyPath);
            string textureDirectory = Path.Combine(executingAsemblyDirectory, "textures");

            foreach (string texturePath in Directory.GetFiles(textureDirectory))
            {
                string textureName = Path.GetFileName(texturePath);
                foreach (Object asset in AssetBundle.LoadFromFile(texturePath).LoadAllAssets())
                {
                    Texture2D texture = asset as Texture2D;
                    if (texture != null)
                    {
                        Instance.TextureTable[textureName] = texture;
                    }
                }
            }
        }

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
