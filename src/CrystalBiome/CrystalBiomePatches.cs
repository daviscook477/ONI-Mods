using System;
using System.Collections.Generic;
using Harmony;
using System.Reflection;
using UnityEngine;
using TUNING;
using KMod;
using Klei;
using System.Drawing;
using System.IO;

namespace CrystalBiome
{
    public class CrystalBiomePatches
    {

        public static List<TextureAtlas> modAtlasList = new List<TextureAtlas>();
        public static bool loadedAtlas = false;
        public static Texture2D specialTex = null;

        public static List<byte[]> loadedBinaryAssets = new List<byte[]>();

        [HarmonyPatch(typeof(KGlobalAnimParser))]
        [HarmonyPatch("ParseAnimData")]
        public static class KGlobalAnimParser_ParseAnimData_Patch
        {
            private static void Prefix(ref FastReader reader)
            {
                char[] prefix = reader.ReadChars("BINPLACEHOLDER".Length);
                Console.WriteLine(prefix);
                for (int i = 0; i < prefix.Length; i++)
                {
                    if (prefix[i] != "BINPLACEHOLDER"[i])
                    {
                        reader.Position = 0;
                        return;
                    }
                }

                reader = new FastReader(loadedBinaryAssets[int.Parse(System.Text.Encoding.ASCII.GetString(new[] { reader.ReadByte() }))]);
            }
        }

        [HarmonyPatch(typeof(KGlobalAnimParser))]
        [HarmonyPatch("ParseBuildData")]
        public static class KGlobalAnimParser_ParseBuildData_Patch
        {
            private static void Prefix(ref FastReader reader)
            {
                char[] prefix = reader.ReadChars("BINPLACEHOLDER".Length);
                Console.WriteLine(prefix);
                for (int i = 0; i < prefix.Length; i++)
                {
                    if (prefix[i] != "BINPLACEHOLDER"[i])
                    {
                        reader.Position = 0;
                        return;
                    }
                }

                reader = new FastReader(loadedBinaryAssets[int.Parse(System.Text.Encoding.ASCII.GetString(new[] { reader.ReadByte() }))]);
            }
        }


        [HarmonyPatch(typeof(Mod))]
        [HarmonyPatch("Load")]
        public static class Mod_Load_Patch
        {
            private static bool Loaded = false;

            private static Texture2D LoadTextureFromPath(string texturePath)
            {
                Texture2D loadedTexture = null;
                if (!File.Exists(texturePath))
                {
                    DebugUtil.LogWarningArgs(string.Format("Could not load texture from path {0}", texturePath));
                    return loadedTexture;
                }

                Bitmap image = new Bitmap(texturePath);
                loadedTexture = new Texture2D(image.Width, image.Height, TextureFormat.ARGB32, false);
                using (MemoryStream stream = new MemoryStream())
                {
                    for (int j = image.Height - 1; j >= 0; j--)
                    {
                        for (int i = 0; i < image.Width; i++)
                        {
                            byte[] arr = BitConverter.GetBytes(image.GetPixel(i, j).ToArgb());
                            Array.Reverse(arr);
                            stream.Write(arr, 0, 4);
                        }
                    }
                    loadedTexture.LoadRawTextureData(stream.ToArray());
                    loadedTexture.Apply();
                }
                return loadedTexture;
            }

            private static void Postfix(Mod __instance)
            {
                if (Loaded)
                {
                    return;
                }
                Loaded = true;

                string executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
                string executingAssemblyDirectoryPath = Path.GetDirectoryName(executingAssemblyPath);
                string assetsDirectoryPath = Path.Combine(executingAssemblyDirectoryPath, "assets");
                if (!Directory.Exists(assetsDirectoryPath))
                {
                    DebugUtil.LogWarningArgs(string.Format("Mod installed at {0} did not have an assets folder located at {1}. Skipping.", executingAssemblyPath, assetsDirectoryPath));
                    return;
                }


                DirectoryInfo assetsDirectoryInfo = new DirectoryInfo(assetsDirectoryPath);
                foreach (DirectoryInfo individualAssetDirectory in assetsDirectoryInfo.GetDirectories())
                {
                    string assetName = individualAssetDirectory.Name;
                    DebugUtil.LogWarningArgs(string.Format("Loading asset named {0} for mod {1}", assetName, executingAssemblyPath));

                    Texture2D animationTexture = null;
                    TextAsset animationBuild = null;
                    TextAsset animationAnimation = null;
                    foreach (FileInfo file in individualAssetDirectory.GetFiles())
                    {
                        DebugUtil.LogWarningArgs(string.Format("Loading file named {0} for asset {1}", file.Name, assetName));
                        if (file.Name.EndsWith(".png"))
                        {
                            animationTexture = LoadTextureFromPath(file.FullName);
                        }
                        else if (file.Name.EndsWith("_build.bytes"))
                        {
                            byte[] bytes = File.ReadAllBytes(file.FullName);
                            int index = loadedBinaryAssets.Count;
                            string identifier = "BINPLACEHOLDER" + index;
                            loadedBinaryAssets.Add(bytes);
                            animationBuild = new TextAsset(identifier);
                        }
                        else if (file.Name.EndsWith("_anim.bytes"))
                        {
                            byte[] bytes = File.ReadAllBytes(file.FullName);
                            int index = loadedBinaryAssets.Count;
                            string identifier = "BINPLACEHOLDER" + index;
                            loadedBinaryAssets.Add(bytes);
                            animationAnimation = new TextAsset(identifier);
                        }
                    }

                    ModUtil.AddKAnim(assetName, animationAnimation, animationBuild, animationTexture);
                }

                modAtlasList.AddRange(TextureAtlasBuilder.loadDirectory(FileSystem.Normalize(Path.Combine(__instance.label.install_path, "atlas"))));
            }
        }

        [HarmonyPatch(typeof(KSerialization.Manager))]
        [HarmonyPatch("GetType")]
        [HarmonyPatch(new[] { typeof(string) })]
        public static class CrystalBiomeSerializationPatch
        {
            public static void Postfix(string type_name, ref Type __result)
            {
                if (type_name == "CrystalBiome.LivingCrystal")
                {
                    __result = typeof(LivingCrystal);
                }
            }
        }

        public const float CyclesForGrowth = 0.1f;

        [HarmonyPatch(typeof(EntityConfigManager))]
        [HarmonyPatch("LoadGeneratedEntities")]
        public class EntityConfigManager_LoadGeneratedEntities_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{CrystalPlantConfig.Id.ToUpperInvariant()}.NAME", CrystalPlantConfig.SeedName);
                Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{CrystalPlantConfig.Id.ToUpperInvariant()}.DESC", CrystalPlantConfig.SeedDescription);

                Strings.Add($"STRINGS.CREATURES.SPECIES.{CrystalPlantConfig.Id.ToUpperInvariant()}.NAME", CrystalPlantConfig.Name);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{CrystalPlantConfig.Id.ToUpperInvariant()}.DESC", CrystalPlantConfig.Description);
                Strings.Add($"STRINGS.CREATURES.SPECIES.{CrystalPlantConfig.Id.ToUpperInvariant()}.DOMESTICATEDDESC", CrystalPlantConfig.DomesticatedDescription);

                CROPS.CROP_TYPES.Add(new Crop.CropVal(CrystalPlantConfig.SeedId, CyclesForGrowth * 600.0f, 12));
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            private static void Prefix()
            {
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{GemTileConfig.Id.ToUpperInvariant()}.NAME", GemTileConfig.DisplayName);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{GemTileConfig.Id.ToUpperInvariant()}.DESC", GemTileConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{GemTileConfig.Id.ToUpperInvariant()}.EFFECT", GemTileConfig.Effect);
                ModUtil.AddBuildingToPlanScreen("Plumbing", GemTileConfig.Id);
            }
        }

        [HarmonyPatch(typeof(GasAndLiquidConsumerMonitor.Instance))]
        [HarmonyPatch("OnMassConsumed")]
        public class GasAndLiquidConsumerMonitorInstance_OnMassConsumed_Patch
        {
            private static void Prefix(GasAndLiquidConsumerMonitor.Instance __instance, Sim.MassConsumedCallback mcd)
            {
                LivingCrystal livingCrystal = __instance.GetComponent<LivingCrystal>();
                if (livingCrystal == null)
                {
                    return;
                }

                if (mcd.mass > 0.0f)
                {
                    livingCrystal.AccumulateMass(mcd.mass, mcd.temperature);
                }
            }
        }

        [HarmonyPatch(typeof(CreatureCalorieMonitor.Stomach))]
        [HarmonyPatch("Poop")]
        public class Stomach_Poop_Patch
        {
            private static float temperature = 100.0f;

            private static void Prefix(GameObject ___owner)
            {
                temperature = ___owner.GetComponent<PrimaryElement>().Temperature;
                LivingCrystal livingCrystal = ___owner.GetComponent<LivingCrystal>();
                if (livingCrystal == null)
                {
                    return;
                }
                if (livingCrystal.CanConsumeTemperature())
                {
                    float decreasedTemperature = Math.Max(livingCrystal.ConsumeTemperature() + LivingCrystalConfig.OutputTemperatureDelta, LivingCrystalConfig.LethalLowTemperature);
                    ___owner.GetComponent<PrimaryElement>().Temperature = decreasedTemperature;
                }
            }

            private static void Postfix(GameObject ___owner)
            {
                LivingCrystal livingCrystal = ___owner.GetComponent<LivingCrystal>();
                if (livingCrystal == null)
                {
                    return;
                }

                ___owner.GetComponent<PrimaryElement>().Temperature = temperature;
            }
        }

        [HarmonyPatch(typeof(CodexEntryGenerator))]
        [HarmonyPatch("GenerateCreatureEntries")]
        public class CodexEntryGenerator_GenerateCreatureEntries_Patch
        {
            private static void Postfix(Dictionary<string, CodexEntry> __result)
            {
                Strings.Add($"STRINGS.CREATURES.FAMILY.{LivingCrystalConfig.Id.ToUpperInvariant()}", LivingCrystalConfig.Name);
                Strings.Add($"STRINGS.CREATURES.FAMILY_PLURAL.{LivingCrystalConfig.Id.ToUpperInvariant()}", LivingCrystalConfig.PluralName);
                Action(LivingCrystalConfig.Id, LivingCrystalConfig.Name, __result);
            }
        }

        private static void Action(Tag speciesTag, string name, Dictionary<string, CodexEntry> results)
        {
            List<GameObject> brains = Assets.GetPrefabsWithComponent<CreatureBrain>();
            CodexEntry entry = new CodexEntry("CREATURES", new List<ContentContainer>()
            {
                new ContentContainer(new List<ICodexWidget>()
                {
                    new CodexSpacer(),
                    new CodexSpacer()
                }, ContentContainer.ContentLayout.Vertical)
            }, name);
            entry.parentId = "CREATURES";
            CodexCache.AddEntry(speciesTag.ToString(), entry, null);
            results.Add(speciesTag.ToString(), entry);
            foreach (GameObject gameObject in brains)
            {
                CreatureBrain component = gameObject.GetComponent<CreatureBrain>();
                if (component.species == speciesTag)
                {
                    List<ContentContainer> contentContainerList = new List<ContentContainer>();
                    string symbolPrefix = component.symbolPrefix;
                    Sprite first = Def.GetUISprite(gameObject, symbolPrefix + "ui", false).first;
                    contentContainerList.Add(new ContentContainer(new List<ICodexWidget>()
                    {
                      new CodexImage(128, 128, first)
                    }, ContentContainer.ContentLayout.Vertical));
                    Traverse.Create(typeof(CodexEntryGenerator)).Method("GenerateCreatureDescriptionContainers", new[] { typeof(GameObject), typeof(List<ContentContainer>)}).GetValue(gameObject, contentContainerList);
                    entry.subEntries.Add(new SubEntry(component.PrefabID().ToString(), speciesTag.ToString(), contentContainerList, component.GetProperName())
                    {
                        icon = first,
                        iconColor = UnityEngine.Color.white
                    });
                }
            }
        }

    }
}
