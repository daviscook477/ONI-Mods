using System.Reflection;
using System.IO;

using Harmony;
using UnityEngine;

namespace CrystalBiome.AssetLoading
{
    public class Patches
    {
        [HarmonyPatch(typeof(KMod.Manager), "Load")]
        public static class Manager_Load_Patch
        {
            private static bool Loaded = false;

            private static void Prefix()
            {
                if (Loaded)
                {
                    return;
                }

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
                            AssetLoader.Instance.TextureTable[textureName] = texture;
                        }
                    }
                }

                Loaded = true;
            }
        }
    }
}
