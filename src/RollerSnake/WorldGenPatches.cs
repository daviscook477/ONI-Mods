using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace WorldGen
{
    public class WorldGenPatches
    {

        [HarmonyPatch(typeof(Db), "Initialize")]
        public class Db_Initialize_Patch
        {
            public static LocString Name = "Tetrament";
            public static LocString Description = "A location with moderately hot temperatures, Tetrament is home to large expanses of hot, dry desert in addition to its lush forests, swamps, and jungles.\n\n<smallcaps>Tetrament is an ideal location for Duplicant life. Its environment is quite different from other locations but is not any more challenging.</smallcaps>\n\n";

            public static void Prefix()
            {
                AddWorldYaml(Name, Description, "Asteroid_desert_oasis", typeof(Db_Initialize_Patch));
            }
        }

        /* see https://github.com/Pholith/ONI-Mods/blob/master/src/Pholib/Utilities.cs 
         * used with permission from @Pholith */
        private static List<Type> alreadyLoaded = new List<Type>();
        /// <summary>
        /// Add strings and icon for a world
        /// Don't call this method OnLoad ! 
        /// To call at Db.Initialize
        /// </summary>
        /// <param name="NAME"> Name of the world </param>
        /// <param name="DESCRIPTION"> Description of the world </param>
        /// <param name="iconName"> DDS icon name (incorporated ressources only) </param>
        /// <param name="className"> Class containing the locstrings </param>
        public static void AddWorldYaml(string NAME, string DESCRIPTION, string iconName, Type className)
        {
            // Add strings used in ****.yaml
            Strings.Add($"STRINGS.WORLDS." + NAME.ToUpper() + ".NAME", NAME);
            Strings.Add($"STRINGS.WORLDS." + NAME.ToUpper() + ".DESCRIPTION", DESCRIPTION);

            // Generate a translation .pot
            if (!alreadyLoaded.Contains(className))
            {
                ModUtil.RegisterForTranslation(className);
                alreadyLoaded.Add(className);
            }

            if (!iconName.IsNullOrWhiteSpace())
            {
                //Load the sprite from Asteroid_****.dds (converted online from png) and add it to the project and set build action to embedded resource
                string resourceName = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith(iconName + ".dds"));
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    throw new ArgumentException($"Could not load the sprite at {resourceName}.");
                }
                Sprite sprite = CreateSpriteDXT5(stream, 512, 512);
                Assets.Sprites.Add(iconName, sprite);
            }
        }

        // Load a incorporated sprite
        public static Sprite CreateSpriteDXT5(Stream inputStream, int width, int height)
        {
            byte[] array = new byte[inputStream.Length - 128L];
            inputStream.Seek(128L, SeekOrigin.Current);
            inputStream.Read(array, 0, array.Length);
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.DXT5, false);
            texture2D.LoadRawTextureData(array);
            texture2D.Apply(false, false);
            // this isn't an efficient way to flip the loaded texture but it only runs once so the performance impact can't be that terrible
            Texture2D texture2DFlipped = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int i = 0; i < texture2D.width; i++)
            {
                for (int j = 0; j < texture2D.height; j++)
                {
                    texture2DFlipped.SetPixel(i, j, texture2D.GetPixel(i, height - j - 1));
                }
            }
            texture2DFlipped.Apply(false, true);
            Sprite sprite = Sprite.Create(texture2DFlipped, new Rect(0f, 0f, (float)width, (float)height), new Vector2((float)(width / 2), (float)(height / 2)));
            return sprite;
        }
    }
}
