using Klei;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Rendering;

namespace CrystalBiome
{
    public class BlockTileDecorInfoHandler
    {
        private static BlockTileDecorInfoHandler instance = null;
        private static readonly object _lock = new object();
        BlockTileDecorInfoHandler() { }

        public static BlockTileDecorInfoHandler Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new BlockTileDecorInfoHandler();
                    }
                    return instance;
                }
            }
        }

        private bool loaded = false;

        public BlockTileDecorInfo GetBlockTileDecorInfo(string name)
        {
            if (!loaded)
            {
                string executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
                string executingAssemblyDirectory = Path.GetDirectoryName(executingAssemblyPath);
                Assets.BlockTileDecorInfos.AddRange(loadDirectory(FileSystem.Normalize(Path.Combine(executingAssemblyDirectory, "decor"))));
                loaded = true;
            }

            return Assets.GetBlockTileDecorInfo(name);
        }

        private List<BlockTileDecorInfo> loadDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return new List<BlockTileDecorInfo>();
            }

            List<BlockTileDecorInfo> blockTileDecorInfoList = new List<BlockTileDecorInfo>();
            foreach (FileInfo file in new DirectoryInfo(directoryPath).GetFiles())
            {
                TextAsset text = null;
                TextAsset atlasText = null;
                Texture2D texture = null;
                foreach (UnityEngine.Object asset in AssetBundle.LoadFromFile(file.FullName).LoadAllAssets())
                {
                    if (asset is Texture2D)
                    {
                        texture = asset as Texture2D;
                    }
                    else if (asset.name.EndsWith("_info"))
                    {
                        text = asset as TextAsset;
                    }
                    else if (asset.name.EndsWith("_atlas"))
                    {
                        atlasText = asset as TextAsset;
                    }
                }
                if (text == null)
                {
                    DebugUtil.LogWarningArgs(string.Format("could not load decor file, skipping {0}", file.Name));
                }
                if (atlasText == null)
                {
                    DebugUtil.LogWarningArgs(string.Format("could not load atlas file, skipping {0}", file.Name));
                }
                if (texture == null)
                {
                    DebugUtil.LogWarningArgs(string.Format("could not load texture file, skipping {0}", file.Name));
                }
                BlockTileDecorInfo blockTileDecorInfo = makeBlockTileDecorInfo(text, atlasText, texture);
                blockTileDecorInfo.name = file.Name;
                blockTileDecorInfoList.Add(blockTileDecorInfo);
            }
            return blockTileDecorInfoList;
        }

        private BlockTileDecorInfo makeBlockTileDecorInfo(TextAsset jsonData, TextAsset atlasData, Texture2D texture)
        {
            BlockTileDecorInfo blockTileDecorInfo = BlockTileDecorInfo.CreateInstance<BlockTileDecorInfo>();
            JObject parsed = JObject.Parse(jsonData.text);
            blockTileDecorInfo.sortOrder = parsed["sortOrder"].Value<int>();
            List<BlockTileDecorInfo.Decor> decorList = new List<BlockTileDecorInfo.Decor>();
            foreach (JObject jItem in parsed["decor"]["Array"])
            {
                BlockTileDecorInfo.Decor decor = new BlockTileDecorInfo.Decor();
                decor.name = jItem["name"].Value<string>();
                decor.requiredConnections = (BlockTileRenderer.Bits)jItem["requiredConnections"].Value<int>();
                decor.forbiddenConnections = (BlockTileRenderer.Bits)jItem["forbiddenConnections"].Value<int>();
                decor.probabilityCutoff = jItem["probabilityCutoff"].Value<float>();
                decor.sortOrder = jItem["sortOrder"].Value<int>();
                List<BlockTileDecorInfo.ImageInfo> imageInfoList = new List<BlockTileDecorInfo.ImageInfo>();
                foreach (JObject jImageInfoItem in jItem["variants"]["Array"])
                {
                    BlockTileDecorInfo.ImageInfo imageInfo = new BlockTileDecorInfo.ImageInfo();
                    imageInfo.name = jImageInfoItem["name"].Value<string>();
                    imageInfo.offset = new Vector3(jImageInfoItem["offset"]["x"].Value<float>(),
                        jImageInfoItem["offset"]["y"].Value<float>());
                    imageInfoList.Add(imageInfo);
                }
                decor.variants = imageInfoList.ToArray();
                decorList.Add(decor);
            }
            blockTileDecorInfo.decor = decorList.ToArray();
            TextureAtlas atlas = TextureAtlasHandler.Instance.makeTextureAtlas(atlasData, texture);
            blockTileDecorInfo.atlas = atlas;
            return blockTileDecorInfo;
        }

    }
}
